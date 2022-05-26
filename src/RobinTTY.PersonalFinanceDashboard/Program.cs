using IEXSharp;
using IEXSharp.Helper;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

//var etoroImporter = new EtoroImporter();
//var accountStatement = etoroImporter.ImportAccountStatement(@"C:\Users\Robin\Desktop\eToro\etoro-account-statement-1-1-2022-3-6-2022.xlsx");

// database
var dbClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&appname=StockMarketDataArchiver");
var database = dbClient.GetDatabase("PersonalFinanceDashboard");
IMongoCollection<HistoricalPriceResponseDb> historicPriceDataCollection;


const bool useSandbox = false;
string publishableToken, secretToken;
if (useSandbox)
{
    publishableToken = "Tpk_b6f9f479945843be8748281acb10748b";
    secretToken = "Tsk_45b77e68db2a48c6a0c78112581f172d";
    historicPriceDataCollection = database.GetCollection<HistoricalPriceResponseDb>("HistoricalStockPriceData_test");
}
else
{
    publishableToken = "pk_c5769346a6604e1393b76324a71445f1";
    secretToken = "sk_47b333b3481d46959816516f10a0b97d";
    historicPriceDataCollection = database.GetCollection<HistoricalPriceResponseDb>("HistoricalStockPriceDataDaily");
}

var client = new IEXCloudClient(publishableToken, secretToken, false, useSandbox);
//var response = await client.StockPrices.HistoricalPriceAsync("TEAM", ChartRange.Date);

var symbols = new[] { "PEP", "PG", "BLK", "NKE", "TMUS", "LOGI", "DELL", "F", "VOO", "TEAM", "FB", "FFIV", "BA", "SQ", "SHOP", "PYPL", "AMZN", "DIS", "CRM", "INTC", "TSM", "TQQQ", "MSFT", "AAPL", "ADBE", "AMD", "GOOG", "JPM", "NVDA" };
// You may also see null data points when viewing intraday prices and this indicates that no trades took place within the Investor's Exchange during that minute. 
var currentDate = new DateTime(2022, 04, 15);
var endDate = new DateTime(2022, 04, 30);

foreach (var symbol in symbols)
{
    Console.WriteLine($"Downloading data for symbol {symbol}...");
    while (currentDate <= endDate)
    {
        if (currentDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            continue;

        var response = await client.StockPrices.HistoricalPriceByDateAsync(symbol, currentDate, false);
        foreach (var priceData in response.Data)
        {
            priceData.symbol = symbol;
        }
        var prices = response.Data.Select(resp => new HistoricalPriceResponseDb(resp)).ToList();

        if (prices.Count > 0)
            historicPriceDataCollection.InsertMany(prices);

        currentDate = currentDate.AddDays(1);
    }
}

Console.WriteLine("Operation finished.");


//var exchanges = await client.ReferenceData.ExchangeInternationalAsync();
//var symbols = await client.ReferenceData.SymbolsInternationalExchangeAsync("XFRA");

//var test = await client.ReferenceData.
//if (response.ErrorMessage != null)
//{
//    Console.WriteLine(response.ErrorMessage);
//}
//else
//{
//    historicPriceDataCollection.InsertMany(response.Data);
//}

public class HistoricalPriceResponseDb
{
    public ObjectId Id { get; set; }
    public string IexId { get; set; }
    public string date { get; set; }
    public string minute { get; set; }
    public decimal? close { get; set; }
    public decimal? high { get; set; }
    public decimal? low { get; set; }
    public decimal? open { get; set; }
    public string symbol { get; set; }
    public decimal? volume { get; set; }
    public string key { get; set; }
    public string subkey { get; set; }
    public decimal? updated { get; set; }
    public decimal? changeOverTime { get; set; }
    public decimal? marketChangeOverTime { get; set; }
    public decimal? uOpen { get; set; }
    public decimal? uClose { get; set; }
    public decimal? uHigh { get; set; }
    public decimal? uLow { get; set; }
    public decimal? uVolume { get; set; }
    public decimal? fOpen { get; set; }
    public decimal? fClose { get; set; }
    public decimal? fHigh { get; set; }
    public decimal? fLow { get; set; }
    public decimal? fVolume { get; set; }
    public string label { get; set; }
    public decimal? change { get; set; }
    public decimal? changePercent { get; set; }


    public HistoricalPriceResponseDb(HistoricalPriceResponse response)
    {
        IexId = response.id;
        date = response.date;
        minute = response.minute;
        close = response.close;
        high = response.high;
        low = response.low;
        open = response.open;
        symbol = response.symbol;
        volume = response.volume;
        key = response.key;
        subkey = response.subkey;
        updated = response.updated;
        changeOverTime = response.changeOverTime;
        marketChangeOverTime = response.marketChangeOverTime;
        uOpen = response.uOpen;
        uClose = response.uClose;
        uHigh = response.uHigh;
        uLow = response.uLow;
        uVolume = response.uVolume;
        fOpen = response.fOpen;
        fClose = response.fClose;
        fHigh = response.fHigh;
        fLow = response.fLow;
        fVolume = response.fVolume;
        label = response.label;
        change = response.change;
        changePercent = response.changePercent;
    }
}