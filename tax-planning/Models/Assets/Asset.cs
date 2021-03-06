﻿using System;
using System.Linq;
using System.Collections.Generic;
using tax_planning.Models.Tax_Calculation;

namespace tax_planning.Models
{
    public abstract class Asset
    {
        // Properties
        public string Name { get; set; }

        public string AssetType { get; set; }

        public decimal Value { get; set; }

        protected decimal _InterestRate = 0.06M;

        public decimal InterestRate
        {
            get
            {
                return _InterestRate * InterestRateMultiplier;
            }
            set
            {
                _InterestRate = value;
            }
        }

        public virtual decimal Additions { get; set; }

        public bool Preferred { get; set; }

        public List<int> Years
        {
            get
            {
                List<int> list = new List<int>();

                for (var i = 0; i < YearlyAmount.Count; i++)
                {
                    list.Add(DateTime.Today.Year + i);
                }

                return list;
            }
        }

        public List<decimal> YearlyAmount { get; set; }

        public virtual decimal Withdrawal { get; set; }

        public decimal AfterTaxWithdrawal { get; set; }

        public decimal TotalCashOut { get; set; }

        public decimal NetCashOut { get; set; }

        protected decimal InterestRateMultiplier { get; set; } = 1.0M;

        protected readonly int RetirementLength = Data.EndOfPlanDate - Data.RetirementDate;

        protected readonly int TimeToRetirement = Data.RetirementDate - DateTime.Today.Year;

        // Methods

        // Given desired additions, get withdrawal schedule
        // Not a constructor because Asset may be initialized with empty properties, in which case this would break
        public virtual void CalculateSchedule()
        {
            // Tax for dividends
            if (GetType() == typeof(BrokerageHolding))
            {
                if (Data.Income != 0)
                {
                    var liability = IncomeTaxCalculator.TotalIncomeTaxFor(Data.FilingStatus, Data.Income, Data.BasicAdjustment) / Data.Income;
                    InterestRateMultiplier = 1 - liability;
                } else
                {
                    InterestRateMultiplier = 1;
                }
            }

            Data.UpdateCapsFor(Data.CurrentAge);
            Data.ChildrensAges.RemoveAll(age => (age) >= 17);

            List<decimal> amounts = new List<decimal>() { Value + Additions - CalculateTaxOnAddition(Additions) };

            // Add additions up to retirement
            for (var i = 1; i < TimeToRetirement; i++)
            {
                // Tax for dividends
                if (GetType() == typeof(BrokerageHolding))
                {
                    if (Data.Income != 0)
                    {
                        var liability = IncomeTaxCalculator.TotalIncomeTaxFor(Data.FilingStatus, Data.Income, Data.BasicAdjustment) / Data.Income;
                        InterestRateMultiplier = 1 - liability;
                    }
                    else
                    {
                        InterestRateMultiplier = 1;
                    }
                }

                Data.UpdateCapsFor(Data.CurrentAge + i);
                Data.ChildrensAges.RemoveAll(age => (age + i) >= 17);
                amounts.Add(CalculateNextYearAmount(amounts[i - 1], Additions - CalculateTaxOnAddition(Additions)));
            }

            // Tax for dividends
            if (GetType() == typeof(BrokerageHolding))
            {
                if (Data.RetirementIncome != 0) {
                    var liability = IncomeTaxCalculator.TotalIncomeTaxFor(Data.FilingStatus, Data.RetirementIncome, Data.BasicAdjustment) / Data.RetirementIncome;
                    InterestRateMultiplier = 1 - liability;
                } else
                {
                    InterestRateMultiplier = 1;
                }
            }

            // Get the withdrawal
            var delta = GetWithdrawalFor(amounts[TimeToRetirement - 1], RetirementLength);

            // Populate the rest of the schedule
            for (var i = TimeToRetirement; i < TimeToRetirement + RetirementLength; i++)
            {
                amounts.Add(CalculateNextYearAmount(amounts[i - 1], -delta));
            }

            amounts = amounts.Select(x => Decimal.Round(x, 2)).ToList();

            Withdrawal = Decimal.Round(delta, 2);
            YearlyAmount = amounts;
        }

        public virtual void CalculateTaxInfo()
        {
            AfterTaxWithdrawal = Decimal.Round(Withdrawal - CalculateTaxOnWithdrawal(Withdrawal, Data.RetirementIncome), 2);
            TotalCashOut = Decimal.Round(AfterTaxWithdrawal * RetirementLength, 2);
            NetCashOut = Decimal.Round(TotalCashOut - (Additions * TimeToRetirement), 2);
        }

        protected decimal GetWithdrawalFor(decimal principal, int steps)
        {
            // Payment calculation
            return (InterestRate * principal) / (1 - (decimal)Math.Pow(1 + (double)InterestRate, -steps));
        }

        protected virtual decimal CalculateNextYearAmount(decimal previousYearAmount, decimal yearDelta)
        {
            return previousYearAmount * (InterestRate + 1.00M) + yearDelta;
        }

        // Abstract methods
        protected abstract decimal CalculateTaxOnAddition(decimal addition);
        protected abstract decimal CalculateTaxOnWithdrawal(decimal withdrawal, decimal income);
        public abstract void UpdateCapsFor(int age);
    }
}
