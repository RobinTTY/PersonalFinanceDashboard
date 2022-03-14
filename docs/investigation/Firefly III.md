# Firefly III

"Firefly III" is a (self-hosted) manager for ones personal finances.

## Features

- creating/editing transactions, accounts, giving them budgets, categories and tags
- automating recurring transactions and auto-budgets
- keeping track of liabilities
- import data from almost any bank (through Nordigen/Saltedge/CSV)
- offers API access to almost all functions ([API Docs](https://api-docs.firefly-iii.org/))
- Rule based transaction handling with the ability to create your own rules

### Bookkeeping System

- Double-entry

### Rule Engine

The rule engine can automatically update your transactions. It can do this when transactions are created or when they are changed. **It works by combining "triggers" with "actions".**

**This is especially useful when you're importing data and you wish all transactions to be updated at once.**

> Actually an interesting concept which I should think about

#### Rule groups

Rules are divided over rule groups. Each rule group has rules in a specific order. Rules can be set to be "strict" or not. If a rule is set to be strict, ALL triggers must match for the rule to fire. If a rule is not strict, ANY trigger is enough.

#### Triggers

- When a transaction is created
- When the description is something specific
- When the amount is more than X
- When the budget is X
- Account number / IBAN triggers
- Amount triggers
- Date triggers

#### Actions

- Change the budget, category, tag(s), description, amount
- Set a new description
- Change the source or destination account

### Deployment options

- Server
- Docker
- various other auto-installer options for servers

## Screenshots

### The Dashboard

![Dashboard1](resources/Dashboard1.png)
![Dashboard2](resources/Dashboard2.png)
![Dashboard3](resources/Dashboard3.png)

### Account Views

#### Account Overview

![AccountOverview](resources/AccountOverview.png)

#### Asset Account

![AssetAccount](resources/AssetAccount.png)

#### Expense Account

![ExpenseAccount](resources/ExpenseAccount.png)

### Transaction Management

![ff1](resources/ff1.png)

### Piggy Banks (Saving Goals)

![PiggyBank](resources/PiggyBank.png)

Interactive Help is offered:

![InteractiveHelp](resources/InteractiveHelp.png)

### Budgets, Categories and Tags

![ff2](resources/ff2.png)

### Rule Engine

![ff3](resources/ff3.png)
![rules-meta](resources/rules-meta.png)
![rules-triggers](resources/rules-triggers.png)
![rules-actions](resources/rules-actions.png)
![apply-rule-group](resources/apply-rule-group.png)

### Reports

#### Default financial report

General overview of your finances:

![reports-default-small](resources/reports-default-small.png)

#### Audit report

Exact overview of an asset account (or multiple):

![reports-audit-small](resources/reports-audit-small.png)

#### Expense/revenue report

Shows you an overview of an account such as for the tax department:

![reports-expense-small](resources/reports-expense-small.png)

#### Budget report

Detailed overview for the selected budgets and the selected account, allowing you to see how well you're doing for each selected budget. This is especially useful to see where you're spending your money, what the trend line is and if your budget limits are actually having any effect:

![reports-budget-small](resources/reports-budget-small.png)

#### Category report

Same as the budget report but focuses on your categories:

![reports-category-small](resources/reports-category-small.png)

#### Tag report

Same as the category report but focuses on your tags.

![reports-tag-small](resources/reports-tag-small.png)

### REST API

The REST API "allows you to tap into Firefly III's most important features":

![ff6](resources/ff6.png)
