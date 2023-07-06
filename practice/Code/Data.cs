using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace practice
{
    public class Data
    {
        public static DateTime LastCheckDate { get; set; }

        public static List<Actual> Rates
        {
            get => _rates;
            set => _rates = value;
        }

        public static CurrencyDbContext DBContext
        {
            get => _dbContext;
            set => _dbContext = value;
        }

        private static CurrencyDbContext _dbContext = new CurrencyDbContext();

        private static List<Actual> _rates = new List<Actual>();


        public static void CheckRates(DateTime date)
        {
            LastCheckDate = date;
            bool isRewriteNeeded = false;

            if (date == DateTime.Today)
            {
                isRewriteNeeded = _dbContext.Actual.AsNoTracking().FirstOrDefault(e => e.Date.Date != date.Date) != null;
            }

            Rates = JsonConvert.DeserializeObject<List<Actual>>(RequestSender.GetRatesByDate(date));

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
                DBContext.Actual.AddRange(Rates);
                DBContext.SaveChanges();
            }
        }

        public static void LoadHistory(DateTime startDate, DateTime endDate)
        {
            // endDate - НЕ включительно

            TimeSpan duration = endDate - startDate;
            int totalDays = (int)duration.TotalDays;

            var rates = new List<Actual>();
            var history = new List<History>();

            for (int i = 0; i < totalDays; i++)
            {
                rates.AddRange(JsonConvert.DeserializeObject<List<Actual>>(RequestSender.GetRatesByDate(startDate.AddDays(i))));
            }

            for (int i = 0; i < rates.Count; i++)
            {
                history.Add(new History());
                history[i].CurrencyId = rates[i].CurrencyId;
                history[i].CurrencyOfficialRate = rates[i].CurrencyOfficialRate;
                history[i].CurrencyScale = rates[i].CurrencyScale;
                history[i].Date = rates[i].Date;
            }

            DBContext.History.AddRange(history);
            DBContext.SaveChanges();
        }

        public static ChartData[] GetChartRates(int? id, out string name)
        {
            List<History> history = DBContext.History.Where(h => h.CurrencyId == id).ToList();
            Actual actual = DBContext.Actual.FirstOrDefault(a => a.CurrencyId == id);

            name = DBContext.Actual.FirstOrDefault(a => a.CurrencyId == id).CurrencyName;

            string scale = DBContext.Actual.FirstOrDefault(a => a.CurrencyId == id).CurrencyScale.ToString();

            name = $"{scale} {name} к белорусским рублям";

            ChartData[] chart = new ChartData[history.Count + 1];

            for (int i = 0; i < chart.Length - 1; i++) 
            {
                chart[i] = new ChartData();
                chart[i].Name = history[i].Date.ToLongDateString();
                chart[i].Value = history[i].CurrencyOfficialRate;
            }

            int lastElem = chart.Length - 1;

            chart[lastElem] = new ChartData();
            chart[lastElem].Name = actual.Date.ToLongDateString();
            chart[lastElem].Value = actual.CurrencyOfficialRate;

            for (int i = 0; i < chart.Length; i++)
            {
                chart[i].Name = chart[i].Name.Replace("г.", "");
            }

            return chart;
        }
    }
}
