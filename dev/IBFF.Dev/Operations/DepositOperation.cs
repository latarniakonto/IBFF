using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public class DepositOperation : Operation
{
    private float _deposit;

    public DepositOperation(DateTime date, float deposit) : base(date)
    {
        _type = OperationType.Deposit;
        _deposit = deposit;
    }

    public override void Apply(Portfolio porftolio)
    {
        porftolio.Deposit += _deposit;
    }

    public override void Revert(Portfolio portfolio)
    {
        portfolio.Deposit -= _deposit;
    }
}
