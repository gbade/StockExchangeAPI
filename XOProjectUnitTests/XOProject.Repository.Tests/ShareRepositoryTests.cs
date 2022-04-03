using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XOProject.Repository;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProjectUnitTests.XOProject.Repository.Tests.Helpers;

namespace XOProjectUnitTests.XOProject.Repository.Tests
{
    public class ShareRepositoryTests
    {
        private ShareRepository _shareRepository;
        private ExchangeContext _context;

        [SetUp]
        public void Initialize()
        {
            _context = ContextFactory.CreateContext(true);
            _shareRepository = new ShareRepository(_context);
            SeedData();
        }

        public void SeedData()
        {
            var rates = new DataSeed();
            foreach (var share in rates.GetRates())
            {
                _context.Shares.Add(share);
            }

            foreach (var portfolio in new DataSeed().GetPortfolios())
            {
                _context.Portfolios.Add(portfolio);
            }

            foreach (var trade in new DataSeed().GetTrades())
            {
                _context.Trades.Add(trade);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
            _context = null;
            _shareRepository = null;
        }

        [Test]
        public async Task GetAsync_WhenExists_ReturnsHourlyShareRate()
        {
            // Arrange
            var expected = new HourlyShareRate
            { Id = 10, Symbol = "CBI", Rate = 96, TimeStamp = new DateTime(2018, 08, 13, 02, 00, 00) };

            // Act
            var result = await _shareRepository.GetAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.Symbol, result.Symbol);
            Assert.AreEqual(expected.Rate, result.Rate);
            Assert.AreEqual(expected.TimeStamp, result.TimeStamp);
        }

        [Test]
        public async Task GetAsync_WhenDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _shareRepository.GetAsync(99);

            // Assert
            Assert.IsNull(result);
        }
    }
}
