
namespace IBFF.Dev.Operations.Factories;

public abstract class OperationFactory
{
    public delegate void OperationCreatedEventHandler(object sender, Operation operation);

    public event OperationCreatedEventHandler? OnOperationCreated;

    protected abstract Operation FactoryMethod();

    public Operation Create()
    {
        Operation operation = FactoryMethod();
        OnOperationCreated?.Invoke(this, operation);

        return operation;
    }
}
