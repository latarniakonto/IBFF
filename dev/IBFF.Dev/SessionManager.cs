using IBFF.Dev.Operations;
using IBFF.Dev.Portfolios;

public class SessionManager : IDisposable
{
    private List<Portfolio> _portfolios = new List<Portfolio>();

    private Portfolio? _selectedPortfolio;

    private readonly OperationFactory _operationFactory;

    public SessionManager(OperationFactory operationFactory)
    {
        _operationFactory = operationFactory;
        _operationFactory.OnOperationCreated += OnOperationCreated;
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

        operation.Apply(_selectedPortfolio);
    }

    public void Dispose()
    {
        _operationFactory.OnOperationCreated -= OnOperationCreated;
    }
}
