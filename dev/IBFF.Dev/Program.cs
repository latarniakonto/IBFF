
using IBFF.Dev.Portfolios;
using IBFF.Dev.Operations;

OperationFactory operationFactory = new OperationFactory();
SessionManager sessionManager = new SessionManager(operationFactory);
Portfolio portfolio = new Portfolio();
sessionManager.Attach(portfolio);

operationFactory.CreateOperation(OperationType.Deposit);

operationFactory.CreateOperation(OperationType.Withdraw);


Console.WriteLine(portfolio.ToString());
