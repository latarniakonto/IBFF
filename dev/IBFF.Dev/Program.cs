
using IBFF.Dev.Portfolios;
using IBFF.Dev.Operations.Factories;

SessionManager sessionManager = new SessionManager();
Portfolio portfolio = new Portfolio();
sessionManager.Attach(portfolio);

OperationFactory operationFactory = new DepositOperationFactory();
sessionManager.OperationFactory = operationFactory;
operationFactory.Create();



Console.WriteLine(portfolio.ToString());
