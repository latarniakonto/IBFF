using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public abstract class Operation
{
    protected DateTime _date;

    protected Operation()
    {
        _date = DateTime.Now;
    }

    protected Operation(DateTime date)
    {
        _date = date;
    }

    public abstract void Apply(Portfolio porftolio);

    public abstract void Revert(Portfolio portfolio);
}

public enum OperationType
{
    EmptyOperation = 0,
    Deposit,
    Withdraw,
}