using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public class WithdrawOperation : Operation
{
    private float _deposit;

    public WithdrawOperation(DateTime date, float deposit) : base(date)
    {
        _deposit = deposit;
    }

    public override void Apply(Portfolio porftolio)
    {
        porftolio.Deposit -= _deposit;
    }

    public override void Revert(Portfolio portfolio)
    {
        portfolio.Deposit += _deposit;
    }
}
