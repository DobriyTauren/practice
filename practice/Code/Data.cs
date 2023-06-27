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

        private static List<Actual> _rates = new List<Actual>();
    }
}
