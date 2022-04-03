using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using XOProject.Api.Controller;
using XOProject.Services.Exchange;
using XOProject.Repository.Exchange;
using XOProject.Services.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XOProjectUnitTests.XOProject.Api.Tests
{
    [TestFixture]
    public class AnalyticsControllerTest
    {
        private Mock<IAnalyticsService> _analyticsServiceMock;
        private AnalyticsController _analyticsController;

        [SetUp]
        public void Setup()
        {
            _analyticsServiceMock = new Mock<IAnalyticsService>();
            _analyticsController = new AnalyticsController(_analyticsServiceMock.Object);
        }

        [Test]
        public async Task Daily_WrongSymbol_ShouldReturnNotFound()
        {
            var newRequest = await _analyticsController.Daily("ASA", 2001, 1, 1);

            var result = newRequest as NotFoundResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task Daily_DailyValue_ShouldReturnBadRequest()
        {
            var newRequest = await _analyticsController.Daily("CBI", 2000, 1, 54);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Daily_EmptySymbolValue_ShouldReturnBadRequest()
        {
            var newRequest = await _analyticsController.Daily("", 2000, 1, 31);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Daily_IncorrectYearValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Daily("CBI", 1, 1, 1);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Daily_MonthValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Daily("CBI", 2000, 19, 1);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Weekly_IncorrectWeekValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Weekly("REL", 2015, 55);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Weekly_IncorrectYearValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Weekly("ASD", 1, 1);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Weekly_SymbolValue_ShouldReturnNotFound()
        {
            var newRequest = await _analyticsController.Weekly("ABC", 2017, 38);

            var result = newRequest as NotFoundResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task Weekly_EmptySymbol_ShouldReturnBadRequest()
        {
            var newRequest = await _analyticsController.Weekly("", 2017, 38);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Monthly_IncorrectYearValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Monthly("CBI", -20, 1);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Monthly_IncorrectMonthValue_ShouldReturnBadrequest()
        {
            var newRequest = await _analyticsController.Monthly("CBI", 2015, 60);

            var result = newRequest as BadRequestResult;
            var response = newRequest as ObjectResult;

            Assert.IsNull(response);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task Monthly_Symbol_ShouldReturnNotFound()
        {
            var newRequest = await _analyticsController.Monthly("ASDF", 2017, 12);

            var result = newRequest as NotFoundResult;
            var response = newRequest as ObjectResult;

            Assert.That(response, Is.Null);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TearDown]
        public void CleanUp()
        {
            _analyticsServiceMock.Reset();
        }
    }
}

