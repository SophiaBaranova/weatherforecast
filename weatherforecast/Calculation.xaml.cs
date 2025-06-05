using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace weatherforecast
{
    public partial class Calculation : Window
    {
        public Calculation(List<WeatherData> currentList)
        {
            InitializeComponent();
            // Перевірка наявності даних
            if (currentList == null || currentList.Count == 0)
            {
                Registration.ShowMessage("Список даних порожній", "Помилка", MessageBoxImage.Error);
                this.Close();
                return;
            }
            Calculate(currentList);
        }
        public double AverageTemp { get; private set; }
        public double AverageHumidity { get; private set; }
        public double AveragePressure { get; private set; }
        public double AverageRain { get; private set; }
        public double AverageSnow { get; private set; }
        public double AverageHail { get; private set; }
        public bool IsCalculated { get; set; } = false;


        public void Calculate(List<WeatherData> currentList)
        {
            // Обчислення середньої температури, вологості, тиску
            double averageTemp = currentList.Average(data => data.Temperature);
            double averageHumidity = currentList.Average(data => data.Humidity);
            double averagePressure = currentList.Average(data => data.Pressure);
            // Отримання списків даних за типами опадів
            List<WeatherData> rainData = currentList
                .Where(w => w.Precipitation == "дощ")
                .ToList();
            List<WeatherData> snowData = currentList
                .Where(w => w.Precipitation == "сніг")
                .ToList();
            List<WeatherData> hailData = currentList
               .Where(w => w.Precipitation == "град")
               .ToList();
            // Обчислення середнього значення відсотків опадів
            double averageRain = rainData.Count > 0 ? rainData.Average(data => data.Temperature) : 0;
            double averageSnow = snowData.Count > 0 ? snowData.Average(data => data.Temperature) : 0;
            double averageHail = hailData.Count > 0 ? hailData.Average(data => data.Temperature) : 0;

            // Виведення результатів у текстові поля
            TextBox1.Text = $"{averageTemp:F1}" + " °C";
            TextBox2.Text = $"{averageRain:F1}" + " %";
            TextBox3.Text = $"{averageSnow:F1}" + " %";
            TextBox4.Text = $"{averageHail:F1}" + " %";
            TextBox5.Text = $"{averageHumidity:F1}" + " %";
            TextBox6.Text = $"{averagePressure:F1}" + " мм.рт.ст.";

            AverageTemp = averageTemp;
            AverageHumidity = averageHumidity;
            AveragePressure = averagePressure;
            AverageRain = averageRain;
            AverageSnow = averageSnow;
            AverageHail = averageHail;
            IsCalculated = true;

        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
