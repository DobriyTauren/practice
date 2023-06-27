using System.ComponentModel.DataAnnotations;

namespace practice
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyScale { get; set; }
        public decimal? CurrencyOfficialRate { get; set; }

        public Actual Actual { get; set; }
    }
}
