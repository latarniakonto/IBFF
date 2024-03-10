using IBFF.Dev.Operations;
using IBFF.Dev.Portfolios;
using IBFF.Dev.Tools.Validators.Operations;

namespace IBFF.Dev.Tools.Validators;

public class OperationValidator
{
    public bool IsOperationMergeable(Operation operation, in Portfolio portfolio)
    {
        switch (operation.Type)
        {
            case OperationType.EmptyOperation:
                return EmptyOperationValidator.IsOperationMergable(operation, portfolio);

            case OperationType.Deposit:
                return DepositOperationValidator.IsOperationMergable(operation, portfolio);;

            case OperationType.Withdraw:
                return WithdrawOperationValidator.IsOperationMergable(operation, portfolio);
            
            default:
                return false;
        }
    }
}
