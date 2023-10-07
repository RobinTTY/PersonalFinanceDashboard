using HotChocolate;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

public class Subscription
{
    // TODO: individual subscriptions for created/deleted/etc. or 1 combined subscription?
    [Subscribe]
    public Transaction TransactionCreated([EventMessage] Transaction transaction) => transaction;
}
