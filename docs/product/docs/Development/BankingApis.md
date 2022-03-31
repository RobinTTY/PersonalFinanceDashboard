---
id: development-banking-apis
title: Banking APIs
sidebar_position: 3
---

Since I want to be able to import transactions, balances and the like automatically from a bank account, I need to interface with banking APIs (PSD2 APIs).
A third party provider is necessary since direct interfacing with PSD2 APIs requires licensing, which is very much non trivial.
There are multiple options to interface with banking APIs through third parties, including [Nordigen](https://nordigen.com/en/), [Plaid](https://plaid.com/), [Truelayer](https://truelayer.com/), [Tink](https://tink.com/), and [Saltedge](https://www.saltedge.com/products/account_information/coverage).

## Comparison

|       Provider        |    [Nordigen](https://nordigen.com/en/)     |                   [Plaid](https://plaid.com/)                    | [Truelayer](https://truelayer.com/) | [Tink](https://tink.com/) | [Saltedge](https://www.saltedge.com/products/account_information/coverage) |
| :-------------------: | :-----------------------------------------: | :--------------------------------------------------------------: | :---------------------------------: | :-----------------------: | :------------------------------------------------------------------------: |
|  Free tier available  |                     ✅                      |                                ✅                                |                 ❌                  |            ❌             |                                     ❌                                     |
| Features in free tier |           Transactions, Balances            | Transactions, Balances, Investments, Liabilities, Authentication |                 \*                  |            \*             |                                     \*                                     |
|      Limitations      | Some rate limiting (no further information) |                     only testing envrionment                     |                 \*                  |            \*             |                                     \*                                     |
|       Coverage        |            [31 European countries](https://nordigen.com/en/coverage/)            |   [US, Canada, UK, France, Ireland, Netherlands, Spain, Germany](https://plaid.com/global/)   |                 \*                  |            \*             |                                     \*                                     |

\* not further investigated since no free tier is available.

## Decision

Nordigen will be used to interface with the PSD2 APIs.

### Rationale

Nordigen is the only provider that offers a free tier which can be used to provide real transaction data without cost.
For that reason there isn't much consideration needed in this case.
