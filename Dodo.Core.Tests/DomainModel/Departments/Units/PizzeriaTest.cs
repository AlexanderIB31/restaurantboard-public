﻿using System;
using Dodo.Core.DomainModel.Departments;
using Dodo.Core.DomainModel.Departments.Units;
using Xunit;

namespace Dodo.Core.Tests.DomainModel.Departments
{
    public class PizzeriaTest
    {
        [Fact]
        public void IsZeroYearsOld_WhenPizzeriaIsNotOpened()
        {
            Pizzeria pizzeria = CreatePizzeria()
                .ThatIsNotOpened()
                .Please();
            
            PizzeriaAssert.That(pizzeria).At(4.JulyOf(1856)).HasAgeOf(0, In.Years);
        }

        [Fact]
        public void IsOneYearOld_WhenOpenedOneYearFromViewDate()
        {
            Pizzeria pizzeria = CreatePizzeria()
                .ThatIsOpened(14.JulyOf(2019))
                .Please();
            
            PizzeriaAssert.That(pizzeria).At(14.JulyOf(2020)).HasAgeOf(1, In.Years);
        }
        
        [Fact]
        public void IsOneMonthsOld_WhenOpenedOneMonthFromViewDate()
        {
            Pizzeria pizzeria = CreatePizzeria()
                .ThatIsOpened(14.JuneOf(2018))
                .Please();
            
            PizzeriaAssert.That(pizzeria).At(14.JulyOf(2018)).HasAgeOf(1, In.Months);
        }   
        
        [Fact]
        public void IsZeroMonthsOld_WhenPizzeriaIsNotOpened()
        {
            Pizzeria pizzeria = CreatePizzeria()
                .ThatIsNotOpened()
                .Please();
            
            PizzeriaAssert.That(pizzeria).At(14.JulyOf(2018)).HasAgeOf(0, In.Months);
        }      
        
        [Fact]
        public void HasNoFormat_WhenPizzeriaIsCreatedWithoutFormat()
        {
            Pizzeria pizzeria = CreatePizzeria()
                .WithoutFormat()
                .Please();

            PizzeriaAssert.That(pizzeria).HasNoFormat();
        }

        private PizzeriaBuilder CreatePizzeria()
        {
            return new PizzeriaBuilder();
        }
    }

    public static class In
    {
        public static string Years => "Years";
        public static string Months => "Months";
    }

    public static class IntegerExtensions
    {
        public static DateTime JulyOf(this int day, int year)
        {
            return new DateTime(year, 7, day);
        }
        
        public static DateTime JuneOf(this int day, int year)
        {
            return new DateTime(year, 6, day);
        }
    }

    public class PizzeriaAssert
    {
        private Pizzeria _pizzeria;
        private DateTime? currentDateTime;

        private PizzeriaAssert(Pizzeria pizzeria)
        {
            _pizzeria = pizzeria;
        }

        public static PizzeriaAssert That(Pizzeria pizzeria)
        {
            return new PizzeriaAssert(pizzeria);
        }

        public void HasAgeOf(int expectedAge, string kind)
        {
            if (currentDateTime == null)
            {
                throw new Exception("Setup incomplete: set not-null currentDateTime");
            }
            var actualAge = kind == In.Years ? _pizzeria.GetYearsOld(currentDateTime.Value) : _pizzeria.GetMonthsOld(currentDateTime.Value);

            Assert.Equal(expectedAge, actualAge);
        }

        public PizzeriaAssert At(DateTime date)
        {
            currentDateTime = date;
            return this;
        }

        public void HasNoFormat()
        {
            Assert.Null(_pizzeria.Format);
        }
    }

    internal class PizzeriaBuilder
    {
        private DateTime? openingDate;
        private PizzeriaFormat pizzeriaFormat;

        public PizzeriaBuilder ThatIsNotOpened()
        {
            openingDate = null;
            return this;
        }
        
        public PizzeriaBuilder WithoutFormat()
        {
            pizzeriaFormat = null;
            return this;
        }

        public PizzeriaBuilder ThatIsOpened(DateTime date)
        {
            openingDate = date;
            return this;
        }

        public Pizzeria Please()
        {
            return new Pizzeria(0, null, "", null, "", UnitApprove.Approved, UnitState.Close, 0, null, 0, null, 0.0,
                openingDate,
                "", null, null, null, ClientTreatment.DefaultName, false, pizzeriaFormat);
        }
    }
}