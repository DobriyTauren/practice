using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using static System.Formats.Asn1.AsnWriter;

namespace practice.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string SearchText { get; set; }
        
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost(string searchText)
        {
            searchText = searchText.ToLower();

            var rates = Data.DBContext.Actual.Where(a => a.CurrencyName.ToLower().Contains(searchText) || a.CurrencyAbbreviation.ToLower().Contains(searchText)).ToList();

            Data.Rates = rates;
        }

        public void LoadTestData()
        {
            var rates = JsonConvert.DeserializeObject<List<Actual>>(RequestSender.GetRatesByDate(new DateTime(2023, 06, 23)));

            var oldRates = Data.DBContext.Actual.ToList();
            Data.DBContext.Actual.RemoveRange(oldRates);

            Data.DBContext.Actual.AddRange(rates);
            Data.DBContext.SaveChanges();
        }

        public async Task<IActionResult> OnGetTodayRefreshClick()
        {
            Data.CheckRates(DateTime.Today);

            return RedirectToPage();
        } 
        
        public async Task<FileResult> OnGetDownloadExcelClick()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Логика для создания Excel файла

            // Создание объекта Excel
            var excelPackage = new OfficeOpenXml.ExcelPackage();

            // Добавление данных в Excel файл
            var worksheet = excelPackage.Workbook.Worksheets.Add("Лист 1");

            worksheet.Cells[1, 1].Value = "Айди валюты";
            worksheet.Cells[1, 2].Value = "Дата";
            worksheet.Cells[1, 3].Value = "Аббревиатура";
            worksheet.Cells[1, 4].Value = "Количество";
            worksheet.Cells[1, 5].Value = "Наименование";
            worksheet.Cells[1, 6].Value = "Курс";

            // Запись данных из массива объектов
            for (int i = 0; i < Data.Rates.Count; i++)
            {
                int row = i + 2; // Смещение на 1 строку для учета заголовков

                worksheet.Cells[row, 1].Value = Data.Rates[i].CurrencyId;
                worksheet.Cells[row, 2].Value = Data.Rates[i].Date.ToShortDateString();
                worksheet.Cells[row, 3].Value = Data.Rates[i].CurrencyAbbreviation;
                worksheet.Cells[row, 4].Value = Data.Rates[i].CurrencyScale;
                worksheet.Cells[row, 5].Value = Data.Rates[i].CurrencyName;
                worksheet.Cells[row, 6].Value = Data.Rates[i].CurrencyOfficialRate;
            }

            // Авто-подгон размеров столбцов для лучшего отображения данных
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Сохранение Excel файла в поток
            var stream = new MemoryStream(excelPackage.GetAsByteArray());

            // Возвращение файла Excel
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Курсы валют на {Data.LastCheckDate.ToShortDateString()}.xlsx");
        }

        public async Task<IActionResult> OnGetYesterdayRefreshClick()
        {
            Data.CheckRates(DateTime.Today.AddDays(-1));

            return RedirectToPage();
        }
    }
}