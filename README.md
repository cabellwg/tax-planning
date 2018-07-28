# Tax Planning

> This web application compares retirement investment vehicles and determines what the optimal investment strategy given your tax information is. It was built for the [PIEtech, Inc](https://www.moneyguidepro.com/ifa/). internship in summer 2018, as the first of three projects. It was built by a team of five interns: two business/technical analysts, two front-end developers, and me, a back-end developer.

---

#### Built with:

* ASP.NET Core MVC Web API
* [Aurelia](https://aurelia.io/)

---

#### My contributions:
* Built entire back-end API with .NET Core
* Helped with front-end code, mostly UI design


#### Basic structure:

* Back-end code can be found in the `tax-planning` directory
* Front-end code can be found in the `web-app` directory

This code was never deployed; it was an intern project and the goal was to have a functional development build. The only way it has ever been run is a Webpack development server for the front-end and the Visual Studio built-in IIS Express server for the back-end. To run it on any other configuration, the fetch URL on the front-end will have to be changed, and, depending on the configuration, CORS may have to be reconfigured.

---

## Important Note

As with any project, there are things that I did that I could have done better. Upon completion of the project, we did a code review and professional developers were very helpful and pointed out several things that could have been improved. I have left it as-is in order to
1. Demonstrate progress in software development ability with the [second](https://gitlab.com/cabellwg/guaranteed-income) and [third](https://gitlab.com/cabellwg/monte-carlo) projects and
2. Accurately demonstrate my software development skills in a limited period of development time (one two-week sprint).

---

## More details

The application compares a Roth IRA with a traditional qualified IRA, a Roth 401k with a traditional qualified 401k, and each with a brokerage account. Employer matching options can be included for the 401k plans, with accurate tax calculations.

The tax calculations take into account filing status, income (or household income, depending on filing status), and the child tax credit to determine applicable tax rates. Virginia state tax is also calculated based on income and filing status. Taxes during retirement are calculated using the rates from preferred plans.

The optimal addition strategy, as determined by the business analysts on our team to keep the scope manageable for a two-week sprint, was to first add as much as possible to a 401k option, then put any leftovers in an IRA option, then in a brokerage account. The interest rates for each account are assumed fixed at 6%, since the purpose of the application is to compare strategies in terms of tax advantage.

---

#### Endnotes:
* I don't think the tests run
* Also there are only like two of them
* Don't look at me like that
* I feel guilty now
* Please stop
