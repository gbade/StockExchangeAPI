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
    public class TradeControllerTests
    {
        private Mock<ITradeService> _tradeServiceMock;
        private TradeController _tradeController;
        private List<Trade> _listOfTrades;

        [SetUp]
        public void Setup()
        {
            _tradeServiceMock = new Mock<ITradeService>();
            _tradeController = new TradeController(_tradeServiceMock.Object);
            _listOfTrades = new List<Trade>
            {
                new Trade
                {
                    Action = "Sell",
                    ContractPrice = 3.53m,
                    Id = 1,
                    PortfolioId = 2,
                    NoOfShares = 300,
                    Symbol = "REL"
                },
                new Trade
                {
                    Action = "Sell",
                    ContractPrice = 2.53m,
                    Id = 2,
                    PortfolioId = 1,
                    NoOfShares = 300,
                    Symbol = "REL"
                },
                new Trade
                {
                    Action = "Sell",
                    ContractPrice = 3.23m,
                    Id = 3,
                    PortfolioId = 4,
                    NoOfShares = 600,
                    Symbol = "CBL"
                }
            };
        }

        [Test]
        public async Task GetAllTradings_PortfolioIdIsEqualToZeroOrLessThanZero_ShouldReturnBadRequest()
        {
            var act = await _tradeController.GetAllTradings(-1);

            var result = act as BadRequestResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void GetAllTradings_NoPortfolioWithId_ShouldReturnNotfound()
        {
            _tradeServiceMock.Setup(e => e.GetByPortfolioId(1))
                .Returns(System.Threading.Tasks.Task.FromResult<IList<Trade>>(new List<Trade>()));

            var act = _tradeController.GetAllTradings(1);

            var result = act.Result as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Post_SupplyCorrectModelValue_ShouldReturnSuccess()
        {
            var portfolioModel = new TradeModel
            {
                Symbol = "DGA",
                NoOfShares = 2,
                PortfolioId = 1,
                Action = "Sell"
            };

            _tradeServiceMock
                .Setup(e => e.BuyOrSell(portfolioModel.PortfolioId, portfolioModel.Symbol, portfolioModel.NoOfShares,
                    portfolioModel.Action)).Returns(Task.FromResult(new Trade
                    {
                        Symbol = "DGA",
                        NoOfShares = 1,
                        PortfolioId = 2,
                        Action = "Sell"
                    }));

            var act = await _tradeController.Post(portfolioModel);

            var result = act as CreatedResult;

            Assert.IsNotNull(result);
            Assert.That(result.Location, Is.EqualTo("Trade/2"));
            Assert.That(result.StatusCode, Is.EqualTo(201));
        }


        [Test]
        public async Task Post_SupplyIncorrectModelValue_ShouldReturnBadRequest()
        {
            var portfolioModel = new TradeModel
            {
                Symbol = "DGA",
            };

            _tradeController.ModelState.AddModelError("error", "error");

            var act = await _tradeController.Post(portfolioModel);

            var result = act as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetAllTradings_PortfolioWithId_ShouldReturnOkWithPortfolioList()
        {
            _tradeServiceMock.Setup(e => e.GetByPortfolioId(1))
                .Returns(System.Threading.Tasks.Task.FromResult<IList<Trade>>(_listOfTrades));

            var act = await _tradeController.GetAllTradings(1);

            var result = act as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));

        }
    }
}
