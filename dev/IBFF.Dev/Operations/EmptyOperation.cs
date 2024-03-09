using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Operations;

public class EmptyOperation : Operation
{
    public EmptyOperation() : base() {}

    public override void Apply(Portfolio porftolio)
    {
        return;
    }

    public override void Revert(Portfolio portfolio)
    {
        return;
    }
}
