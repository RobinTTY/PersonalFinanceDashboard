"use strict";(self.webpackChunkpersonal_finance_dashboard_docs=self.webpackChunkpersonal_finance_dashboard_docs||[]).push([[134],{3905:(e,t,a)=>{a.d(t,{Zo:()=>u,kt:()=>d});var n=a(7294);function r(e,t,a){return t in e?Object.defineProperty(e,t,{value:a,enumerable:!0,configurable:!0,writable:!0}):e[t]=a,e}function o(e,t){var a=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),a.push.apply(a,n)}return a}function i(e){for(var t=1;t<arguments.length;t++){var a=null!=arguments[t]?arguments[t]:{};t%2?o(Object(a),!0).forEach((function(t){r(e,t,a[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(a)):o(Object(a)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(a,t))}))}return e}function l(e,t){if(null==e)return{};var a,n,r=function(e,t){if(null==e)return{};var a,n,r={},o=Object.keys(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||(r[a]=e[a]);return r}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||Object.prototype.propertyIsEnumerable.call(e,a)&&(r[a]=e[a])}return r}var s=n.createContext({}),c=function(e){var t=n.useContext(s),a=t;return e&&(a="function"==typeof e?e(t):i(i({},t),e)),a},u=function(e){var t=c(e.components);return n.createElement(s.Provider,{value:t},e.children)},p={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},m=n.forwardRef((function(e,t){var a=e.components,r=e.mdxType,o=e.originalType,s=e.parentName,u=l(e,["components","mdxType","originalType","parentName"]),m=c(a),d=r,f=m["".concat(s,".").concat(d)]||m[d]||p[d]||o;return a?n.createElement(f,i(i({ref:t},u),{},{components:a})):n.createElement(f,i({ref:t},u))}));function d(e,t){var a=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var o=a.length,i=new Array(o);i[0]=m;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l.mdxType="string"==typeof e?e:r,i[1]=l;for(var c=2;c<o;c++)i[c]=a[c];return n.createElement.apply(null,i)}return n.createElement.apply(null,a)}m.displayName="MDXCreateElement"},2333:(e,t,a)=>{a.r(t),a.d(t,{assets:()=>s,contentTitle:()=>i,default:()=>p,frontMatter:()=>o,metadata:()=>l,toc:()=>c});var n=a(7462),r=(a(7294),a(3905));const o={id:"development-essential-features",title:"Essential Features",sidebar_position:2},i=void 0,l={unversionedId:"Development/development-essential-features",id:"Development/development-essential-features",title:"Essential Features",description:"The features I want to implement for version 1.0 are as follows:",source:"@site/docs/Development/Essential Features.md",sourceDirName:"Development",slug:"/Development/development-essential-features",permalink:"/PersonalFinanceDashboard/docs/Development/development-essential-features",draft:!1,tags:[],version:"current",sidebarPosition:2,frontMatter:{id:"development-essential-features",title:"Essential Features",sidebar_position:2},sidebar:"tutorialSidebar",previous:{title:"Purpose",permalink:"/PersonalFinanceDashboard/docs/Development/development-purpose"},next:{title:"Banking APIs",permalink:"/PersonalFinanceDashboard/docs/Development/development-banking-apis"}},s={},c=[],u={toc:c};function p(e){let{components:t,...a}=e;return(0,r.kt)("wrapper",(0,n.Z)({},u,a,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("p",null,"The features I want to implement for version 1.0 are as follows:"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"Account Management",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Add accounts like bank, credit card, investment account, etc. to manage inflows/outflows of money"),(0,r.kt)("li",{parentName:"ul"},"Display current balance of each account"),(0,r.kt)("li",{parentName:"ul"},"Automatic import of transactions through Banking APIs"),(0,r.kt)("li",{parentName:"ul"},"Each account should have a graph to visualize the balance over time"),(0,r.kt)("li",{parentName:"ul"},"Another graph to visualize the outgoing money by category/tag would also be nice"),(0,r.kt)("li",{parentName:"ul"},"Transactions may have multiple categories associated with them (e.g. you go to the super market and buy electronics/food/etc. and want to split it into electronics/food category)"))),(0,r.kt)("li",{parentName:"ul"},"Transaction Management",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Add transactions which interact with accounts by adding/substracting money from them"),(0,r.kt)("li",{parentName:"ul"},"Transactions should have a date, amount, payee, category and optional tags/description"),(0,r.kt)("li",{parentName:"ul"},"Recurring transactions should be possible to setup (e.g. monthly payments)"),(0,r.kt)("li",{parentName:"ul"},"Have an overview screen that lists transactions from all accounts and detailed lists filtered by account, category, etc."))),(0,r.kt)("li",{parentName:"ul"},"Reports",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Net Worth => bar chart displaying the net worth/debt over time"),(0,r.kt)("li",{parentName:"ul"},"Outflows report => pie chart breakdown of outflows by category/tag/payee and a bar chart to show the trend for each category/tag/payee over time"),(0,r.kt)("li",{parentName:"ul"},"Cashflow report which lists all categories and their associated income/expenses grouped by month/year/customizable timeframe"),(0,r.kt)("li",{parentName:"ul"},"Simple inflow/outflow statement that gives the inflow and outflow of money for a month so you can easily see if you are saving money or are spending it"))),(0,r.kt)("li",{parentName:"ul"},"Budget",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Allocating money to categories and giving the information how much money is still available to budget",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Saving Goals",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"A user should be able to setup a saving goal with a specific target amount which can be worked towards"),(0,r.kt)("li",{parentName:"ul"},"Transactions directed towards the saving goals should decrease the remaining amount needed to achieve the goal"),(0,r.kt)("li",{parentName:"ul"},"Progress should be visualized and a time estimate to achieve the goal should be provided based on the average saving amount per month")))))))),(0,r.kt)("p",null,"These goals require considerable work and should maybe go into the next version:"),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"Investment Account Tracking",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Have specialized investment accounts which take into account current asset prizes (stocks) to give an accurate net worth overview (if stocks appreciate the net worth should do too)"),(0,r.kt)("li",{parentName:"ul"},"Provide charts for stock prices if possible"),(0,r.kt)("li",{parentName:"ul"},"Provide information about dividends if possible"),(0,r.kt)("li",{parentName:"ul"},"Import of statements from investment accounts (prototype for etoro.com) to integrate those transactions"),(0,r.kt)("li",{parentName:"ul"},"Provide risk scores for assets (There are established ways to calculate those based on volitility and other factors)")))))}p.isMDXComponent=!0}}]);