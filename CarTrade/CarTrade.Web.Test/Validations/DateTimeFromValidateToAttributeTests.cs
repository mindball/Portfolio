using CarTrade.Web.Models.Vignettes;
using CarTrade.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CarTrade.Web.Test.Validations
{
    public class DateTimeFromValidateToAttributeTests
    {
        [Theory]
        [InlineData("2021-01-01 00:00:00.0000000", "2022-01-01 00:00:00.0000000", true)]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000000", false)]
        [InlineData("2021-01-01 00:00:00.0000001", "2021-01-01 00:00:00.0000000", false)]
        [InlineData("2021-01-01 00:00:00.0000000", "2021-01-01 00:00:00.0000001", true)]
        [InlineData("1900-01-01 00:00:00.0000000", "9999-12-31 23:59:59.9999999", true)]       
        public void IsValidateWorkCorrectly(string dateTimeFrom, string dateTimeTo, bool expected)
        {   
            //Arrange
            var target = new  VignetteFormViewModel()
            {
                StartDate = DateTime.Parse(dateTimeFrom),
                EndDate = DateTime.Parse(dateTimeTo)
            };
            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            //Act
            var result = Validator.TryValidateObject(target, context, results, true);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
