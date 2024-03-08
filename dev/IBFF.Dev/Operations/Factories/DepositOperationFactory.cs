
namespace IBFF.Dev.Operations.Factories;

public class DepositOperationFactory : OperationFactory
{
    protected override Operation FactoryMethod()
    {
        return new DepositOperation(DateTime.Now, 1000f);
    }
}
