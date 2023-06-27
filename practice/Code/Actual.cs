using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace practice
{
    public class Actual
    {
        [Key] 
        [JsonProperty("Cur_ID")]
        public int CurrencyId { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Cur_Abbreviation")]
        public string CurrencyAbbreviation { get; set; }

        [JsonProperty("Cur_Scale")]
        public int CurrencyScale { get; set; }

        [JsonProperty("Cur_Name")]
        public string CurrencyName { get; set; }

        [JsonProperty("Cur_OfficialRate")]
        public decimal? CurrencyOfficialRate { get; set; }

        public ICollection<History> Histories { get; set; }
    }
}
