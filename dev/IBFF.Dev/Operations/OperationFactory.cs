
namespace IBFF.Dev.Operations;

public class OperationFactory
{
    public delegate void OperationCreatedEventHandler(object sender, Operation operation);

    public event OperationCreatedEventHandler? OnOperationCreated;

    public Operation CreateOperation(OperationType type)
    {
        Operation operation;
        switch (type)
        {
            case OperationType.EmptyOperation:
                operation = new EmptyOperation();
                break;

            case OperationType.Deposit:
                operation = new DepositOperation(DateTime.Now, 1000f);
                break;

            case OperationType.Withdraw:
                operation = new WithdrawOperation(DateTime.Now, 1000f);
                break;
            
            default:
                throw new NotImplementedException("Invalid operation type");
        }

        OnOperationCreated?.Invoke(this, operation);
        return operation;
    }
}
