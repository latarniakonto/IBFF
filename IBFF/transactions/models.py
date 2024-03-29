import uuid
import datetime
from django.conf import settings
from django.db import models
from portfolios.models import Portfolio
from django.db.models import Q
from assets.models import Asset
from operations.models import Dividend
from core.utils import get_asset_to_date
from snapshots.models import Snapshot
from django.utils import timezone


class Types(models.IntegerChoices):
    BUY = 1, 'Buy'
    SELL = 2, 'Sell'


class Transaction(models.Model):
    uutid = models.UUIDField(db_index=True, default=uuid.uuid4, editable=False)
    slug = models.SlugField(max_length=63, unique=True)
    portfolio = models.ForeignKey(
        Portfolio, on_delete=models.CASCADE, related_name='transactions'
    )
    created_at = models.DateTimeField(auto_now_add=True)

    ticker = models.CharField(max_length=31)
    type = models.IntegerField(choices=Types.choices, verbose_name='transaction type')
    price = models.FloatField()
    amount = models.IntegerField()
    value = models.IntegerField()
    provision = models.FloatField()
    date = models.DateTimeField()

    def __str__(self):
        return self.ticker
    
    def save(self, *args, **kwargs):
        if 'normal_save' in kwargs:
            super().save(args, kwargs)
            return

        if (
            self.type == Types.BUY
            and self.portfolio.cash < self.value + self.provision
        ):
            raise Exception

        asset, created = Asset.objects.get_or_create(
            ticker=self.ticker, portfolio=self.portfolio
        )

        if created and self.type == Types.SELL and asset.total == 0:
            asset.delete()
            raise Exception

        elif self.type == Types.SELL and asset.total < self.amount:
            raise Exception

        to_date_asset = get_asset_to_date(asset, self.date)
        if self.type == Types.SELL and to_date_asset.total < self.amount:
            raise Exception

        update_asset_dividends(asset, self)

        super().save(*args, **kwargs)

        update_asset(asset, self)
        update_assets_weight()
        update_asset_portfolio(asset, self)

    def delete(self, *args, **kwargs):        
        asset = Asset.objects.get(ticker=self.ticker, portfolio=self.portfolio)

        transactions = Transaction.objects.all()
        if not transactions.filter(~Q(uutid=self.uutid), ticker=asset.ticker).exists():
            asset.delete()

        update_asset_dividends(asset, self, True)

        super().delete(*args, **kwargs)

        update_asset(asset, self)
        update_assets_weight()
        update_asset_portfolio(asset, self, True)


def get_asset_initial_weight(asset):
    portfolio_assets = Asset.objects.filter(
        ~Q(uuaid=asset.uuaid), portfolio=asset.portfolio
    )
    if len(portfolio_assets) == 0:
        return 1
    
    total_value = asset.initial_value
    for portfolio_asset in portfolio_assets:
        total_value += portfolio_asset.initial_value

    if total_value == 0:
        return 0

    return asset.initial_value / total_value


def get_asset_current_weight(asset):
    portfolio_assets = Asset.objects.filter(
        ~Q(uuaid=asset.uuaid), portfolio=asset.portfolio
    )
    if len(portfolio_assets) == 0:
        return 1

    total_value = asset.current_value
    for portfolio_asset in portfolio_assets:
        total_value += portfolio_asset.current_value
    
    if total_value == 0:
        return 0

    return asset.current_value / total_value


def update_assets_weight():
    assets = Asset.objects.all()
    for asset in assets:
        asset.initial_weight = get_asset_initial_weight(asset)
        asset.current_weight = get_asset_current_weight(asset)
        
        asset.save()


def update_asset_portfolio(asset, transaction, transaction_deleted=False):
    portfolio = asset.portfolio
    
    if transaction_deleted and transaction.type == Types.BUY:
        portfolio.cash += transaction.value

    elif transaction_deleted and transaction.type == Types.SELL:
        portfolio.cash -= transaction.value

    elif not transaction_deleted and transaction.type == Types.BUY:
        portfolio.cash -= transaction.value

    elif not transaction_deleted and transaction.type == Types.SELL:
        portfolio.cash += transaction.value

    if transaction_deleted:
        portfolio.transactions_cost -= transaction.provision
        portfolio.transactions_counter -= 1
        portfolio.cash += transaction.provision
    else:
        portfolio.transactions_cost += transaction.provision
        portfolio.transactions_counter += 1
        portfolio.cash -= transaction.provision

    p_assets = portfolio.assets.all()
    
    total_current_value = 0
    for p_asset in p_assets:
        total_current_value += p_asset.current_value

    portfolio.value = total_current_value + portfolio.cash
    today = datetime.date.today()
    try:
        snapshot = Snapshot.objects.get(portfolio=portfolio, date__year=today.year - 1)
        portfolio.annual_gain = portfolio.value - snapshot.value
        portfolio.annual_yield = portfolio.value / snapshot.value - 1

    except Snapshot.DoesNotExist:
        portfolio.annual_gain = portfolio.value - portfolio.deposit
        portfolio.annual_yield = portfolio.value / portfolio.deposit - 1

    portfolio.save()


def update_asset(asset, transaction):
    today = timezone.now()
    to_date_asset = get_asset_to_date(asset, today)
    asset.current_price = transaction.price
    asset.total = to_date_asset.total
    asset.initial_value = to_date_asset.initial_value

    if asset.total > 0:
        asset.initial_price = asset.initial_value / asset.total

    asset.current_value = asset.current_price * asset.total

    if asset.total == 0:
        asset.gain = 0

    else:
        asset.gain = asset.current_value / asset.initial_value - 1

    asset.save()


def update_asset_dividends(asset, transaction, transaction_deleted=False):
    dividends = Dividend.objects.filter(
        asset__ticker=transaction.ticker, date__gte=transaction.date,
        asset__portfolio=transaction.portfolio
    )
    portfolio = asset.portfolio
    total_sum_difference = 0
    if (transaction.type == Types.BUY and not transaction_deleted) or (
        transaction.type == Types.SELL and transaction_deleted
    ):
        for dividend in dividends:
            temp_asset = get_asset_to_date(dividend.asset, dividend.date)
            temp_asset.total += transaction.amount               
            new_dividend_value = temp_asset.total * dividend.per_share            
            assert new_dividend_value > dividend.value

            difference = new_dividend_value - dividend.value
            dividend.value += difference
            total_sum_difference += difference                

    else:
        for dividend in dividends:
            temp_asset = get_asset_to_date(dividend.asset, dividend.date)            
            temp_asset.total -= transaction.amount            
            new_dividend_value = temp_asset.total * dividend.per_share            

            assert new_dividend_value < dividend.value

            difference = new_dividend_value - dividend.value
            dividend.value += difference
            total_sum_difference += difference

        if portfolio.cash + total_sum_difference < 0:
            raise Exception
    
    for dividend in dividends:
        dividend.save(normal_save=True)

    portfolio.cash += total_sum_difference
    portfolio.value += total_sum_difference        
    portfolio.annual_dividends += total_sum_difference
    today = datetime.date.today()
    try:
        snapshot = Snapshot.objects.get(
            portfolio=portfolio, date__year=today.year - 1
        )
        portfolio.annual_gain = portfolio.value - snapshot.value
        portfolio.annual_yield = portfolio.value / snapshot.value - 1

    except Snapshot.DoesNotExist:
        portfolio.annual_gain = portfolio.value - portfolio.deposit
        portfolio.annual_yield = portfolio.value / portfolio.deposit - 1

    portfolio.save()
