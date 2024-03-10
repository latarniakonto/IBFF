using IBFF.Dev.Operations;
using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Tools.Validators.Operations;

public static class DepositOperationValidator
{
    public static bool IsOperationMergable(Operation operation, in Portfolio portfolio)
    {
        return true;
    }
}
