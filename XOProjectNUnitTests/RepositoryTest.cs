using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using XOProject.Repository;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProjectNUnitTests.Helpers;

namespace XOProjectNUnitTests
{
    public class RepositoryTest
    {
        private ExchangeContext _context;
        private ShareRepository _shareRepository;

        [SetUp]
        public void Initialize()
        {
            _context = ContextFactory.CreateContext(true);
            _shareRepository = new ShareRepository(_context);
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
        public async Task GetAsync_ReturnItem_WhenItExists()
        {
            // Act
            var result = await _shareRepository.GetAsync(10);

            // Assert
            Assert.NotNull(result);
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
