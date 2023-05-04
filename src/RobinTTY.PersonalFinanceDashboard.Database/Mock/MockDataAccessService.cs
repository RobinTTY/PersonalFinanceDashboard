using Bogus;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Database.Mock;

public class MockDataAccessService
{
    public static IEnumerable<Transaction> GetTransactions(int amount)
    {
        return new Faker<Transaction>()
            .CustomInstantiator(f => new Transaction(
                valueDate: f.Date.Between(new DateTime(2018, 01, 01), DateTime.Today),
                payer: f.Person.FullName,
                payee: f.Company.CompanyName(),
                amount: f.Random.Decimal(10, 1000),
                currency: f.Finance.Currency().Code,
                category: f.Commerce.Categories(1).First(),
                tags: f.Lorem.Words().ToList(),
                notes: f.Lorem.Sentence()
            )).Generate(amount);
    }

    public static IEnumerable<Account> GetAccounts(int amount)
    {
        return new Faker<Account>()
            .CustomInstantiator(f => new Account(
                name: f.Person.FullName,
                description: f.Finance.AccountName(),
                balance: f.Finance.Amount(-10_000, 50_000),
                currency: f.Finance.Currency().Code,
                type: f.PickRandom<AccountType>(),
                transactions: GetTransactions(f.Random.Number(0, 100)).ToList()
            )).Generate(amount);
    }
}