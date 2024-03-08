using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public class EmptyOperation : Operation
{
    public EmptyOperation() : base() {}

    public EmptyOperation(DateTime date) : base(date) {}

    public override void Apply(Portfolio porftolio)
    {
        return;
    }
}
