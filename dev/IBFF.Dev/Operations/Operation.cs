using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public abstract class Operation
{
    protected DateTime _date;

    protected OperationType _type;

    protected Operation()
    {
        _date = DateTime.Now;
    }

    public DateTime Date
    {
        get { return _date; }
    }

    public OperationType Type
    {
        get { return _type; }
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