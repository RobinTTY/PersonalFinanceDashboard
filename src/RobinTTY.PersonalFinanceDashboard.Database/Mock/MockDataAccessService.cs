using Bogus;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Database.Mock;

public class MockDataAccessService
{
    public static IQueryable<Transaction> GetTransactions(int amount)
    {
        return new Faker<Transaction>()
            .CustomInstantiator(f => new Transaction(
                valueDate: f.Date.Between(new DateTime(2018, 01, 01), DateTime.Today),
                payer: f.Person.FullName,
                payee: f.Company.CompanyName(),
                amount: f.Random.Decimal(10, 1000),
                //currency: "f.Finance.Currency().Code",
                currency: "USD",
                category: f.Commerce.Categories(1).First(),
                tags: f.Lorem.Words().ToList(),
                notes: f.Lorem.Sentence()
            )).Generate(amount).AsQueryable();
    }

    public static IQueryable<Account> GetAccounts(int amount)
    {
        return new Faker<Account>()
            .CustomInstantiator(f => new Account(
                id: Guid.NewGuid().ToString(),
                name: f.Person.FullName,
                description: f.Finance.AccountName(),
                balance: f.Finance.Amount(0, 5_000),
                //currency: f.Finance.Currency().Code,
                currency: "USD",
                transactions: GetTransactions(f.Random.Number(0, 100)).ToList()
            )).Generate(amount).AsQueryable();
    }
}