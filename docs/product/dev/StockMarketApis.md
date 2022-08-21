---
id: stock-market-apis
title: Stock Market APIs
sidebar_position: 5
---

Gathering financial data is rather difficult at a reasonable prize. To offer live portfolio tracking I need this data. Here is an overview of available APIs I could gather:

- [Polygon](https://polygon.io/stocks)
  - Solid 29$/month tier: 15-minute delayed data, unlimited API calls, 5 years historical data, fundamental data
  - First party clients available for: Python, Go, Javascript, PHP, Kotlin
  - Third party C# client: [Github](https://github.com/brandonseydel/Polygon)
  - Good documentation
- [financialmodelingprep](https://site.financialmodelingprep.com/developer/docs/)
  - Reasonable pricing starting at 19$/month, 300 requests/min, 30 year historical data (10.50$/month yearly billing)
  - Huge array of fundamental data/economic data available, (earnings calendar, IPO calendar, stock news, etc)
  - 250 requests/day for free, should be enough for testing
  - Third party C# library available [Github](https://github.com/MatthiWare/FinancialModelingPrep.NET) - quality seems great
  - Good documentation
- [Marketstack](https://marketstack.com/)
  - Usable tier starting at 9.99$ with 10 000 requests/month (fundamentals included)
  - Third party(?) C# library available: [Github](https://github.com/orshe4/marketstack)
  - Company behind this data [seems to act questionable](https://github.com/public-apis/public-apis/issues/3104) to say the least, not an option for this reason already
  - Good documentation
- [Stockdata.org](https://www.stockdata.org/)
  - 9$ tier with 2500 requests/daily (~100/hour), 19$ tier with 10 000 requests/daily (~400/hour), some particular limitations
  - Fundamental data available
  - Good documentation
- [Alpaca](https://alpaca.markets/)
  - Only offers price data, no stock splits, dividends, etc. (200 API calls per minute free)
  - 99$/month for al US exchange data + unlimited websockets
- [Alpha Vantage](https://www.alphavantage.co/)
  - Free tier only includes 500 API calls a day, payed tiers start at 50$ a month => better options available
- [EODHD APIs](https://eodhistoricaldata.com/)
  - Data packet that includes fundamentals starts at 79.99€/month
- [Tiingo](https://api.tiingo.com):
  - Only offers price data, fundamental data is in beta and costs extra (50 API calls per hour free)
- Category "Way to expensive":
  - [Finnhub](https://finnhub.io/)
  - [Intrinio](https://intrinio.com/)

## Secondary APIs

There are some free APIs that can be used to gather secondary data:

- [EconDB](https://www.econdb.com/api/series/?page=1): Consumer Price index, Unemployment, Consumer Confidence, etc.
- [FiscalData.Treasury.gov](https://fiscaldata.treasury.gov/api-documentation/): Wide array of US fiscal data

## Decision

[financialmodelingprep](https://site.financialmodelingprep.com/developer/docs/) seems to offer the most complete dataset at the best price, so I will start testing that.
