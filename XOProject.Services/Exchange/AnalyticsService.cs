using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Domain;

namespace XOProject.Services.Exchange
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IShareRepository _shareRepository;

        public AnalyticsService(IShareRepository shareRepository)
        {
            _shareRepository = shareRepository;
        }

        public async Task<AnalyticsPrice> GetDailyAsync(string symbol, DateTime day)
        {
            var ratesPerHour = _shareRepository.Query()
                                                .Where(x => x.Symbol.Equals(symbol) && x.TimeStamp.Year == day.Year 
                                                        && x.TimeStamp.Month == day.Month && x.TimeStamp.Day == day.Day)
                                                .OrderBy(x => x.TimeStamp);

            var analyticsPrice = CalculatePrice(ratesPerHour);

            return await Task.FromResult(analyticsPrice);
        }

        public async Task<AnalyticsPrice> GetWeeklyAsync(string symbol, int year, int week)
        {
            var ratesPerHour = _shareRepository.Query()
                                                .Where(x => x.Symbol.Equals(symbol) && x.TimeStamp.Year == year 
                                                        && GetWeekNumber(x.TimeStamp) == week)
                                                .OrderBy(x => x.TimeStamp);

            var analyticsPrice = CalculatePrice(ratesPerHour);

            return await Task.FromResult(analyticsPrice);
        }

        public async Task<AnalyticsPrice> GetMonthlyAsync(string symbol, int year, int month)
        {
            var ratesPerHour = _shareRepository.Query()
                                               .Where(x => x.Symbol.Equals(symbol)&& x.TimeStamp.Year == year 
                                                        && x.TimeStamp.Month == month)
                                               .OrderBy(x => x.TimeStamp);

            var analyticsPrice = CalculatePrice(ratesPerHour);

            return await Task.FromResult(analyticsPrice);
        }

        private int GetWeekNumber(DateTime timestamp)
        {
            int weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(timestamp, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return weekNum;
        }

        private static AnalyticsPrice CalculatePrice(IOrderedQueryable<HourlyShareRate> ratesByTimestamp)
        {
            if (!ratesByTimestamp.Any())
                return null;

            var price = new AnalyticsPrice
            {
                High = ratesByTimestamp.Max(a => a.Rate),
                Low = ratesByTimestamp.Min(a => a.Rate),
                Open = ratesByTimestamp.First().Rate,
                Close = ratesByTimestamp.Last().Rate
            };

            return price;
        }
    }
}