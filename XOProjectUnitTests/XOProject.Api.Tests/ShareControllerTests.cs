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
using XOProject.Repository.Domain;
using XOProject.Api.Model;

namespace XOProjectUnitTests.XOProject.Api.Tests
{
    [TestFixture]
    public class ShareControllerTests
    {
        private readonly Mock<IShareService> _shareServiceMock = new Mock<IShareService>();

        private readonly ShareController _shareController;

        public ShareControllerTests()
        {
            _shareController = new ShareController(_shareServiceMock.Object);
        }

        [Test]
        public async Task GetLatestPrice_SupplySymbolInTheDatabase_ShouldReturnOkWithItemPrice()
        {
            _shareServiceMock.Setup(e => e.GetLastPriceAsync("REL")).Returns(Task.FromResult<HourlyShareRate>(new HourlyShareRate
            {
                Id = 1,
                Rate = 197.00m,
                Symbol = "REL"
            }));

            var act = await _shareController.GetLatestPrice("REL");

            var result = act as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(197.00));
        }

        [Test]
        public async Task GetLatestPrice_SupplySymbolNotInTheDatabase_ShouldReturnNotFound()
        {
            _shareServiceMock.Setup(e => e.GetLastPriceAsync("DER")).Returns(Task.FromResult<HourlyShareRate>(null));

            var act = await _shareController.GetLatestPrice("DER");

            var result = act as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Post_ShouldInsertHourlySharePrice()
        {
            // Arrange
            var hourRate = new HourlyShareRateModel
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task Post_SupplyIncorrectModel_ShouldReturnBadRequest()
        {
            var hourRate = new HourlyShareRateModel
            {
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            _shareController.ModelState.AddModelError("error", "Share symbol should be all capital letters with 3 characters");

            var act = await _shareController.Post(hourRate);

            var result = act as BadRequestObjectResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Get_SupplySymbol_ShouldReturnNotFound()
        {
            _shareServiceMock.Setup(e => e.GetBySymbolAsync("ED"))
                .Returns(Task.FromResult<IList<HourlyShareRate>>(new List<HourlyShareRate>()));

            var act = await _shareController.Get("ED");

            var result = act as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));

        }

        [Test]
        public async Task Get_SupplySymbol_ShouldReturnOkWithList()
        {
            _shareServiceMock.Setup(e => e.GetBySymbolAsync("AS"))
                .Returns(Task.FromResult<IList<HourlyShareRate>>(new List<HourlyShareRate>
                {
                    new HourlyShareRate
                    {
                        Id = 1,
                        Rate = 34.1m,
                        Symbol = "ADR",
                        TimeStamp = DateTime.Now.AddMinutes(30)
                    },

                    new HourlyShareRate
                    {
                        Id = 2,
                        Rate = 34.19m,
                        Symbol = "BAR",
                        TimeStamp = DateTime.Now.AddMinutes(50)


                    },

                    new HourlyShareRate
                    {
                        Id = 3,
                        Rate = 10.19m,
                        Symbol = "ADR",
                        TimeStamp = DateTime.Now.AddMinutes(210)


                    }
                }));


            var act = await _shareController.Get("AS");

            var result = act as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));

        }

        [Test]
        public async Task UpdateLastPrice_SymbolOrRateSupplied_ShouldReturnNotFound()
        {
            _shareServiceMock.Setup(e => e.UpdateLastPriceAsync("AS", 1.23m))
                .Returns(Task.FromResult<HourlyShareRate>(null));

            var act = await _shareController.UpdateLastPrice("AS", 2.45m);

            var result = act as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateLastPrice_SymbolOrRateSupplied_ShouldReturnOkWithHourlyRate()
        {
            _shareServiceMock.Setup(e => e.UpdateLastPriceAsync("AS", 1.23m))
                .Returns(Task.FromResult<HourlyShareRate>(new HourlyShareRate
                {
                    Id = 1,
                    Rate = 2.34m,
                    Symbol = "ADR",
                    TimeStamp = DateTime.Now
                }));

            var act = await _shareController.UpdateLastPrice("AS", 1.23m);

            var result = act as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [TearDown]
        public void Cleanup()
        {
            _shareServiceMock.Reset();
        }
    }
}
