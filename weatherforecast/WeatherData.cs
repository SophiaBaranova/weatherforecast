namespace weatherforecast
{
    public class WeatherData
    {
        public int ID { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public double Humidity { get; set; }
        public string Precipitation { get; set; }
        public double PrecipitationVal { get; set; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public string Wind { get; set; }
        public double WindSpeed { get; set; }
    }
}
