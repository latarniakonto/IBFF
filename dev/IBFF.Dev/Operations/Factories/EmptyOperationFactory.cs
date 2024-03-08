
namespace IBFF.Dev.Operations.Factories;

public class EmptyOperationFactory : OperationFactory
{
    protected override Operation FactoryMethod()
    {
        return new EmptyOperation();
    }
}
