namespace IBFF.Dev.Portfolios;

public class Portfolio
{
    private float _deposit;
    private float _value;

    public float Deposit 
    {
        get { return _deposit; }
        set 
        {
            _deposit = value;
            _value = _deposit;
        }
    }

    public Portfolio ()
    {
        _deposit = 0f;
        _value = 0f;
    }

    public override string ToString()
    {
        return "Deposit: " + _deposit.ToString() + "\n" + "Value: " + _value.ToString();
    }
}
