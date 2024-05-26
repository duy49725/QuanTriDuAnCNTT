using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTDA.DataAccess.Data;
using QTDA.Models.ViewModels;
using QTDA.Utility;

namespace QTDAWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> MonthlyRevenueData()
        {
            var monthlyRevenue = await _context.OrderHeaders
                .GroupBy(oh => new { oh.OrderDate.Year, oh.OrderDate.Month })
                .Select(g => new MonthlyRevenueViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(oh => oh.OrderTotal)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return Json(monthlyRevenue);
        }
        public IActionResult MonthlyRevenueChart()
        {
            return View();
        }
    }
}
