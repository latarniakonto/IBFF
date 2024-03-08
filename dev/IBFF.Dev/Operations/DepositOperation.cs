using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public class DepositOperation : Operation
{
    private float _deposit;

    public DepositOperation() : base() {}

    public DepositOperation(DateTime date) : base(date) {}

    public DepositOperation(DateTime date, float deposit) : base(date)
    {
        _deposit = deposit;
    }

    public override void Apply(Portfolio porftolio)
    {
        porftolio.Deposit += _deposit;
    }
}
