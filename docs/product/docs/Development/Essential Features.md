---
id: development-essential-features
title: Essential Features
sidebar_position: 2
---

The features I want to implement for version 1.0 are as follows:

- Account Management
  - Add accounts like bank, credit card, investment account, etc. to manage inflows/outflows of money
  - Display current balance of each account
  - Automatic import of transactions through Banking APIs
  - Each account should have a graph to visualize the balance over time
  - Another graph to visualize the outgoing money by category/tag would also be nice
  - Transactions may have multiple categories associated with them (e.g. you go to the super market and buy electronics/food/etc. and want to split it into electronics/food category)
- Transaction Management
  - Add transactions which interact with accounts by adding/substracting money from them
  - Transactions should have a date, amount, payee, category and optional tags/description
  - Recurring transactions should be possible to setup (e.g. monthly payments)
  - Have an overview screen that lists transactions from all accounts and detailed lists filtered by account, category, etc.
- Reports
  - Net Worth => bar chart displaying the net worth/debt over time
  - Outflows report => pie chart breakdown of outflows by category/tag/payee and a bar chart to show the trend for each category/tag/payee over time
  - Cashflow report which lists all categories and their associated income/expenses grouped by month/year/customizable timeframe
  - Simple inflow/outflow statement that gives the inflow and outflow of money for a month so you can easily see if you are saving money or are spending it
- Budget
  - Allocating money to categories and giving the information how much money is still available to budget
    - Saving Goals
      - A user should be able to setup a saving goal with a specific target amount which can be worked towards
      - Transactions directed towards the saving goals should decrease the remaining amount needed to achieve the goal
      - Progress should be visualized and a time estimate to achieve the goal should be provided based on the average saving amount per month

These goals require considerable work and should maybe go into the next version:

- Investment Account Tracking
  - Have specialized investment accounts which take into account current asset prizes (stocks) to give an accurate net worth overview (if stocks appreciate the net worth should do too)
  - Provide charts for stock prices if possible
  - Provide information about dividends if possible
  - Import of statements from investment accounts (prototype for etoro.com) to integrate those transactions
  - Provide risk scores for assets (There are established ways to calculate those based on volitility and other factors)