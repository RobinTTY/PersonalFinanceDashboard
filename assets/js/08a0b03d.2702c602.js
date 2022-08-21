"use strict";(self.webpackChunkpersonal_finance_dashboard_docs=self.webpackChunkpersonal_finance_dashboard_docs||[]).push([[489],{3905:(e,t,a)=>{a.d(t,{Zo:()=>c,kt:()=>d});var r=a(7294);function n(e,t,a){return t in e?Object.defineProperty(e,t,{value:a,enumerable:!0,configurable:!0,writable:!0}):e[t]=a,e}function i(e,t){var a=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),a.push.apply(a,r)}return a}function l(e){for(var t=1;t<arguments.length;t++){var a=null!=arguments[t]?arguments[t]:{};t%2?i(Object(a),!0).forEach((function(t){n(e,t,a[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(a)):i(Object(a)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(a,t))}))}return e}function o(e,t){if(null==e)return{};var a,r,n=function(e,t){if(null==e)return{};var a,r,n={},i=Object.keys(e);for(r=0;r<i.length;r++)a=i[r],t.indexOf(a)>=0||(n[a]=e[a]);return n}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(r=0;r<i.length;r++)a=i[r],t.indexOf(a)>=0||Object.prototype.propertyIsEnumerable.call(e,a)&&(n[a]=e[a])}return n}var s=r.createContext({}),p=function(e){var t=r.useContext(s),a=t;return e&&(a="function"==typeof e?e(t):l(l({},t),e)),a},c=function(e){var t=p(e.components);return r.createElement(s.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},m=r.forwardRef((function(e,t){var a=e.components,n=e.mdxType,i=e.originalType,s=e.parentName,c=o(e,["components","mdxType","originalType","parentName"]),m=p(a),d=n,k=m["".concat(s,".").concat(d)]||m[d]||u[d]||i;return a?r.createElement(k,l(l({ref:t},c),{},{components:a})):r.createElement(k,l({ref:t},c))}));function d(e,t){var a=arguments,n=t&&t.mdxType;if("string"==typeof e||n){var i=a.length,l=new Array(i);l[0]=m;var o={};for(var s in t)hasOwnProperty.call(t,s)&&(o[s]=t[s]);o.originalType=e,o.mdxType="string"==typeof e?e:n,l[1]=o;for(var p=2;p<i;p++)l[p]=a[p];return r.createElement.apply(null,l)}return r.createElement.apply(null,a)}m.displayName="MDXCreateElement"},7305:(e,t,a)=>{a.r(t),a.d(t,{assets:()=>s,contentTitle:()=>l,default:()=>u,frontMatter:()=>i,metadata:()=>o,toc:()=>p});var r=a(7462),n=(a(7294),a(3905));const i={id:"stock-market-apis",title:"Stock Market APIs",sidebar_position:5},l=void 0,o={unversionedId:"stock-market-apis",id:"stock-market-apis",title:"Stock Market APIs",description:"Gathering financial data is rather difficult at a reasonable prize. To offer live portfolio tracking I need this data. Here is an overview of available APIs I could gather:",source:"@site/dev/StockMarketApis.md",sourceDirName:".",slug:"/stock-market-apis",permalink:"/PersonalFinanceDashboard/dev/stock-market-apis",draft:!1,tags:[],version:"current",sidebarPosition:5,frontMatter:{id:"stock-market-apis",title:"Stock Market APIs",sidebar_position:5},sidebar:"dev",previous:{title:"Banking APIs",permalink:"/PersonalFinanceDashboard/dev/banking-apis"}},s={},p=[{value:"Secondary APIs",id:"secondary-apis",level:2},{value:"Decision",id:"decision",level:2}],c={toc:p};function u(e){let{components:t,...a}=e;return(0,n.kt)("wrapper",(0,r.Z)({},c,a,{components:t,mdxType:"MDXLayout"}),(0,n.kt)("p",null,"Gathering financial data is rather difficult at a reasonable prize. To offer live portfolio tracking I need this data. Here is an overview of available APIs I could gather:"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://polygon.io/stocks"},"Polygon"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Solid 29$/month tier: 15-minute delayed data, unlimited API calls, 5 years historical data, fundamental data"),(0,n.kt)("li",{parentName:"ul"},"First party clients available for: Python, Go, Javascript, PHP, Kotlin"),(0,n.kt)("li",{parentName:"ul"},"Third party C# client: ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/brandonseydel/Polygon"},"Github")),(0,n.kt)("li",{parentName:"ul"},"Good documentation"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://site.financialmodelingprep.com/developer/docs/"},"financialmodelingprep"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Reasonable pricing starting at 19$/month, 300 requests/min, 30 year historical data (10.50$/month yearly billing)"),(0,n.kt)("li",{parentName:"ul"},"Huge array of fundamental data/economic data available, (earnings calendar, IPO calendar, stock news, etc)"),(0,n.kt)("li",{parentName:"ul"},"250 requests/day for free, should be enough for testing"),(0,n.kt)("li",{parentName:"ul"},"Third party C# library available ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/MatthiWare/FinancialModelingPrep.NET"},"Github")," - quality seems great"),(0,n.kt)("li",{parentName:"ul"},"Good documentation"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://marketstack.com/"},"Marketstack"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Usable tier starting at 9.99$ with 10 000 requests/month (fundamentals included)"),(0,n.kt)("li",{parentName:"ul"},"Third party(?) C# library available: ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/orshe4/marketstack"},"Github")),(0,n.kt)("li",{parentName:"ul"},"Company behind this data ",(0,n.kt)("a",{parentName:"li",href:"https://github.com/public-apis/public-apis/issues/3104"},"seems to act questionable")," to say the least, not an option for this reason already"),(0,n.kt)("li",{parentName:"ul"},"Good documentation"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://www.stockdata.org/"},"Stockdata.org"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"9$ tier with 2500 requests/daily (~100/hour), 19$ tier with 10 000 requests/daily (~400/hour), some particular limitations"),(0,n.kt)("li",{parentName:"ul"},"Fundamental data available"),(0,n.kt)("li",{parentName:"ul"},"Good documentation"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://alpaca.markets/"},"Alpaca"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Only offers price data, no stock splits, dividends, etc. (200 API calls per minute free)"),(0,n.kt)("li",{parentName:"ul"},"99$/month for al US exchange data + unlimited websockets"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://www.alphavantage.co/"},"Alpha Vantage"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Free tier only includes 500 API calls a day, payed tiers start at 50$ a month => better options available"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://eodhistoricaldata.com/"},"EODHD APIs"),(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Data packet that includes fundamentals starts at 79.99\u20ac/month"))),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://api.tiingo.com"},"Tiingo"),":",(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},"Only offers price data, fundamental data is in beta and costs extra (50 API calls per hour free)"))),(0,n.kt)("li",{parentName:"ul"},'Category "Way to expensive":',(0,n.kt)("ul",{parentName:"li"},(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://finnhub.io/"},"Finnhub")),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://intrinio.com/"},"Intrinio"))))),(0,n.kt)("h2",{id:"secondary-apis"},"Secondary APIs"),(0,n.kt)("p",null,"There are some free APIs that can be used to gather secondary data:"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://www.econdb.com/api/series/?page=1"},"EconDB"),": Consumer Price index, Unemployment, Consumer Confidence, etc."),(0,n.kt)("li",{parentName:"ul"},(0,n.kt)("a",{parentName:"li",href:"https://fiscaldata.treasury.gov/api-documentation/"},"FiscalData.Treasury.gov"),": Wide array of US fiscal data")),(0,n.kt)("h2",{id:"decision"},"Decision"),(0,n.kt)("p",null,(0,n.kt)("a",{parentName:"p",href:"https://site.financialmodelingprep.com/developer/docs/"},"financialmodelingprep")," seems to offer the most complete dataset at the best price, so I will start testing that."))}u.isMDXComponent=!0}}]);