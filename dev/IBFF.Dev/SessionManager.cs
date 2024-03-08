using IBFF.Dev.Operations;
using IBFF.Dev.Operations.Factories;
using IBFF.Dev.Portfolios;

public class SessionManager : IDisposable
{
    private List<Portfolio> _portfolios = new List<Portfolio>();

    private Portfolio? _selectedPortfolio;

    private OperationFactory _operationFactory;

    private Operation _currentOperation = new EmptyOperation();

    public SessionManager()
    {
        _operationFactory = new EmptyOperationFactory();
        _operationFactory.OnOperationCreated += OnOperationCreated;
    }

    public Operation CurrentOperation 
    { 
        get { return _currentOperation; }
        set { _currentOperation = value; } 
    }

    public OperationFactory OperationFactory
    {
        get { return _operationFactory; }
        set 
        { 
            _operationFactory = value;
            _operationFactory.OnOperationCreated += OnOperationCreated;
        }
    }

    public void Attach(Portfolio portfolio)
    {
        if (_portfolios == null) return;

        _portfolios.Add(portfolio);
        _selectedPortfolio = portfolio;
    }

    public void Detach(Portfolio portfolio)
    {
        if (_portfolios == null || _portfolios.Count == 0) return;
        if (!_portfolios.Contains(portfolio)) return;

        int portfolioIndex = _portfolios.IndexOf(portfolio);
        _portfolios.Remove(portfolio);

        _selectedPortfolio = _portfolios.Count < portfolioIndex ? _portfolios.Last() : _portfolios[portfolioIndex];
    }

    public void OnOperationCreated(object sender, Operation operation)
    {
        if (_selectedPortfolio == null) return;

        _currentOperation = operation;
        _currentOperation.Apply(_selectedPortfolio);

        _operationFactory.OnOperationCreated -= OnOperationCreated;
        _operationFactory = new EmptyOperationFactory();
        _operationFactory.OnOperationCreated += OnOperationCreated;
    }

    public void Dispose()
    {
        _operationFactory.OnOperationCreated -= OnOperationCreated;
    }
}
