using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace practice.Pages
{
    public class IndexModel : PageModel
    {
        public CurrencyDbContext DBContext
        {
            get => _dbContext;
            set => _dbContext = value;
        }

        private CurrencyDbContext _dbContext = new CurrencyDbContext();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void LoadTestData()
        {
            var rates = JsonConvert.DeserializeObject<List<Actual>>(RequestSender.GetRatesByDate(new DateTime(2023, 06, 23)));

            var oldRates = DBContext.Actual.ToList();
            DBContext.Actual.RemoveRange(oldRates);

            DBContext.Actual.AddRange(rates);
            DBContext.SaveChanges();
        }

        public void CheckRates(DateTime date)
        {
            bool isRewriteNeeded = _dbContext.Actual.AsNoTracking().FirstOrDefault(e => e.Date.Date != date.Date) != null;
            
            Data.Rates = JsonConvert.DeserializeObject<List<Actual>>(RequestSender.GetRatesByDate(date));

            #region time format cringe
            //for (int i = 0;  i < rates.Count;  i++) 
            //{
            //    rates[i].Date = DateTime.SpecifyKind(rates[i].Date, DateTimeKind.Utc);
            //}
            #endregion


            if (DBContext.Actual.Count() != 0 && isRewriteNeeded)
            {
                var oldRates = DBContext.Actual.ToList();
                DBContext.Actual.RemoveRange(oldRates);
            }

            if (isRewriteNeeded)
            {
                DBContext.Actual.AddRange(Data.Rates);
                DBContext.SaveChanges();
            }
        }

        public async Task<IActionResult> OnGetTodayRefreshClick()
        {
            CheckRates(DateTime.Today);

            return RedirectToPage();
        } 
        
        public async Task<IActionResult> OnGetYesterdayRefreshClick()
        {
            CheckRates(DateTime.Today.AddDays(-1));

            return RedirectToPage();
        }
    }
}