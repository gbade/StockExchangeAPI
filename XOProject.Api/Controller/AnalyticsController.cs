using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using XOProject.Api.Model.Analytics;
using XOProject.Services.Domain;
using XOProject.Services.Exchange;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XOProject.Api.Controller
{
    [Route("api")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("daily/{symbol}/{year}/{month}/{day}")]
        public async Task<IActionResult> Daily([FromRoute] string symbol, 
                                                [FromRoute] int year, [FromRoute] int month, 
                                                [FromRoute][Range(1, 31)] int day)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest();

            if (!IsDayValid(day))
                return BadRequest();

            if (!IsYearValid(year))
                return BadRequest();

            if (!IsMonthValid(month))
                return BadRequest();

            var timestamp = new DateTime(year, month, day);
            var dailyprice = await _analyticsService.GetDailyAsync(symbol, timestamp);

            if (dailyprice == null)
                return NotFound();

            var result = new DailyModel()
            {
                Symbol = symbol,
                Day = new DateTime(),
                Price = Map(dailyprice)
            };

            return Ok(result);
        }

        [HttpGet("weekly/{symbol}/{year}/{week}")]
        public async Task<IActionResult> Weekly([FromRoute] string symbol, 
                                                [FromRoute] int year, 
                                                [FromRoute][Range(1, 54)] int week)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest();

            if (!IsWeekOfYearValid(week))
                return BadRequest();

            if (!IsYearValid(year))
                return BadRequest();

            var weeklyprice = await _analyticsService.GetWeeklyAsync(symbol, year, week);

            if (weeklyprice == null)
                return NotFound();


            var result = new WeeklyModel()
            {
                Symbol = symbol,
                Year = year,
                Week = week,
                Price = Map(weeklyprice)
            };

            return Ok(result);
        }

        [HttpGet("monthly/{symbol}/{year}/{month}")]
        public async Task<IActionResult> Monthly([FromRoute] string symbol, 
                                                    [FromRoute] int year, 
                                                    [FromRoute, Range(1, 12)] int month)
        {
            if (string.IsNullOrEmpty(symbol))
                return BadRequest();

            if (!IsMonthValid(month))
                return BadRequest();

            if (!IsYearValid(year))
                return BadRequest();

            var monthlyprice = await _analyticsService.GetMonthlyAsync(symbol, year, month);

            if (monthlyprice == null)
                return NotFound();

            var result = new MonthlyModel()
            {
                Symbol = symbol,
                Year = year,
                Month = month,
                Price = Map(monthlyprice)
            };

            return Ok(result);
        }

        private PriceModel Map(AnalyticsPrice price)
        {
            return new PriceModel()
            {
                Open = price.Open,
                Close = price.Close,
                High = price.High,
                Low = price.Low
            };
        }

        private bool IsMonthValid (int month)
        {
            if (month < 1 || month > 12)
                return false;

            return true;
        }

        private bool IsDayValid(int day)
        {
            if (day < 1 || day > 31)
                return false;

            return true;
        }

        private bool IsWeekOfYearValid(int week)
        {
            if (week < 1 || week > 54)
                return false;

            return true;
        }

        private bool IsYearValid(int year)
        {
            if (year > 1 && year <= 9999)
                return true;

            return false;
        }
    }
}