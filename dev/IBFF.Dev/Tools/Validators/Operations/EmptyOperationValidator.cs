using IBFF.Dev.Operations;
using IBFF.Dev.Portfolios;

namespace IBFF.Dev.Tools.Validators.Operations;

public static class EmptyOperationValidator
{
    public static bool IsOperationMergable(Operation operation, in Portfolio portfolio)
    {
        return true;
    }
}
