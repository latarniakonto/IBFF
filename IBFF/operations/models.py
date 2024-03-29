import uuid
import datetime
from django.db import models
from assets.models import Asset
from core.utils import future_date
from core.utils import get_asset_to_date
from snapshots.models import Snapshot


class Types(models.IntegerChoices):
    DIVIDEND = 1, 'Dividend'

class Operation(models.Model):
    uuoid = models.UUIDField(db_index=True, default=uuid.uuid4, editable=False)
    slug = models.SlugField(max_length=63, unique=True)
    date = models.DateTimeField()
    type = models.IntegerField(choices=Types.choices, verbose_name='operation type')

    class Meta:
        abstract = True


class Dividend(Operation):
    asset = models.ForeignKey(
        Asset, on_delete=models.CASCADE, related_name='dividends'
    )
    per_share = models.FloatField()
    value = models.FloatField(default=0)

    def save(self, *args, **kwargs):
        if 'normal_save' in kwargs:
            super().save(args, kwargs)
            return

        if future_date(self.date):
            raise Exception
        
        to_date_asset = get_asset_to_date(self.asset, self.date)
        if to_date_asset.total == 0:
            raise Exception

        self.value = to_date_asset.total * self.per_share        
        
        portfolio = self.asset.portfolio
        portfolio.cash += self.value
        portfolio.value += self.value
        portfolio.annual_dividends += self.value
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
        super().save(*args, **kwargs)

    def delete(self, *args, **kwargs):
        if self.asset.portfolio.cash < self.value:
            raise Exception

        portfolio = self.asset.portfolio
        portfolio.cash -= self.value
        portfolio.value -= self.value
        portfolio.annual_dividends -= self.value
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
        super().delete(*args, **kwargs)
