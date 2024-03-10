
using IBFF.Dev.Portfolios;
using IBFF.Dev.Operations;
using IBFF.Dev.Tools;
using IBFF.Dev.Tools.Validators;

OperationValidator operationValidator = new OperationValidator();
OperationFactory operationFactory = new OperationFactory();
SessionManager sessionManager = new SessionManager(operationFactory, operationValidator);
Portfolio portfolio = new Portfolio();
sessionManager.Attach(portfolio);

operationFactory.CreateOperation(OperationType.Deposit);
operationFactory.CreateOperation(OperationType.Deposit);

operationFactory.CreateOperation(OperationType.Withdraw);


Console.WriteLine(portfolio.ToString());
