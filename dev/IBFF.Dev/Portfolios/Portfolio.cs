using IBFF.Dev.Operations;
using IBFF.Dev.Tools.Validators;

namespace IBFF.Dev.Portfolios;

public class Portfolio
{
    private List<Operation> _operations = new List<Operation>();
    private float _deposit = 0f;
    private float _value = 0f;

    public float Deposit 
    {
        get { return _deposit; }
        set 
        {
            _deposit = value;
            _value = _deposit;
        }
    }

    public override string ToString()
    {
        return "Deposit: " + _deposit.ToString() + "\n" + "Value: " + _value.ToString();
    }

    public void AddOperation(Operation operation, OperationValidator operationValidator)
    {
        if (_operations == null) return;
        if (!operationValidator.IsOperationMergeable(operation, this))
        {
            throw new ArgumentException("Merge conflict: portfolio-operation");
        }

        operation.Apply(this);
        _operations.Add(operation);
    }

    public void DeleteOperation(Operation operation, OperationValidator operationValidator)
    {
        if (_operations == null || _operations.Count == 0) return;
        if (!_operations.Contains(operation)) return;
        if (!operationValidator.IsOperationMergeable(operation, this))
        {
            throw new ArgumentException("Merge conflict: portfolio-operation");
        }

        operation.Revert(this);
        _operations.Remove(operation);
    }
}
