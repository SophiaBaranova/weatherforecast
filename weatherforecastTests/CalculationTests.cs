using Microsoft.VisualStudio.TestTools.UnitTesting;
using weatherforecast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace weatherforecast.Tests
{
    [TestClass()]
    public class CalculationTests
    {
        [TestMethod()]
        public void CalculationIntegrationTest()
        {
            // 1. Формування списку очікуваних результатів
            double expectedAverageTemp = 27.5;        // (20 + 40 + 22 + 28) / 4
            double expectedAverageHumidity = 43.0;    // (50 + 12 + 55 + 55) / 4
            double expectedAveragePressure = 1285.25; // (1012 + 2094 + 1020 + 1015) / 4
            double expectedRain = 21.0;               // (20 + 22) / 2
            double expectedSnow = 0.0;
            double expectedHail = 0.0;

            // 2. Ініціалізація списку даних
            List<WeatherData> weatherDataList = new List<WeatherData>
            {
                new WeatherData { 
                    Temperature = 20, 
                    Humidity = 50, 
                    Pressure = 1012, 
                    Precipitation = "дощ", 
                    WindSpeed = 5, 
                    Wind = "Пн", 
                    Country = "Україна", 
                    City = "Київ" },
                new WeatherData { 
                    Temperature = 40, 
                    Humidity = 12, 
                    Pressure = 2094, 
                    Precipitation = "ясно", 
                    WindSpeed = 5, 
                    Wind = "Сх", 
                    Country = "Україна", 
                    City = "Київ" },
                new WeatherData { 
                    Temperature = 22, 
                    Humidity = 55, 
                    Pressure = 1020, 
                    Precipitation = "дощ", 
                    WindSpeed = 3, 
                    Wind = "Зх", 
                    Country = "Україна", 
                    City = "Київ" },
                new WeatherData { 
                    Temperature = 28, 
                    Humidity = 55, 
                    Pressure = 1015, 
                    Precipitation = "ясно", 
                    WindSpeed = 3, 
                    Wind = "південний", 
                    Country = "Україна", 
                    City = "Київ" },
            };

            // 3. Створення форми калькулятора
            var window = new Calculation(weatherDataList);

            // 4. Отримання фактичних результатів
            double actualAverageTemp = window.AverageTemp;
            double actualAverageHumidity = window.AverageHumidity;
            double actualAveragePressure = window.AveragePressure;
            double actualRain = window.AverageRain;
            double actualSnow = window.AverageSnow;
            double actualHail = window.AverageHail;

            // 5. Перевірка відповідності очікуваних і фактичних значень
            Assert.AreEqual(expectedAverageTemp, actualAverageTemp, 0.1);
            Assert.AreEqual(expectedAverageHumidity, actualAverageHumidity, 0.1);
            Assert.AreEqual(expectedAveragePressure, actualAveragePressure, 0.1);
            Assert.AreEqual(expectedRain, actualRain, 0.1);
            Assert.AreEqual(expectedSnow, actualSnow, 0.1);
            Assert.AreEqual(expectedHail, actualHail, 0.1);
        }
    }
}