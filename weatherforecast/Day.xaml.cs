using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace weatherforecast
{
    public partial class Day : Window
    {
        private int index; // індекс дня в списку
        private List<WeatherData> weatherList;

        public Day(int index, List<WeatherData> weatherList)
        {
            InitializeComponent();

            this.index = index;
            this.weatherList = weatherList;
            WeatherData item = weatherList[index];

            //Перетворення дати до dd.MM
            LabelDate.Content = DateTime.Parse(item.Date).ToString("dd.MM");

            // Відображення даних про погоду
            LabelTemp.Content = item.Temperature.ToString("F1") + " °C";
            Label1.Content = item.Humidity.ToString() + "%";
            Label2.Content = item.Pressure.ToString();
            Label3.Content = item.Wind + ", " + item.WindSpeed.ToString();
            Label4.Content = item.Precipitation + " " + item.PrecipitationVal.ToString() + " %";

            // Визначення шляху до зображення залежно від типу опадів
            string path = "unknown.png"; // дефолтне зображення

            switch (item.Precipitation)
            {
                case "дощ":
                    path = "rain.png";
                    break;
                case "сніг":
                    path = "snow.png";
                    break;
                case "град":
                    path = "hail.png";
                    break;
                case "хмарно":
                    path = "cloud.png";
                    break;
                case "ясно":
                    path = "sun.png";
                    break;
            }

            Image1.Source = new BitmapImage(new Uri($"pack://application:,,,/images/{path}"));

            // Заповнення кнопок для наступних днів
            if (index + 1 >= weatherList.Count)
            {
                Button1.IsEnabled = false; // Вимкнення кнопки, якщо немає наступного дня
            }
            else
            {
                FillButtonDay(Button1, weatherList[index + 1]);
            }
            if (index + 2 >= weatherList.Count)
            {
                Button2.IsEnabled = false; // Вимкнення кнопки, якщо немає наступного дня
            }
            else
            {
                FillButtonDay(Button2, weatherList[index + 2]);
            }
        }

        // Заповнення кнопки для наступного дня
        private void FillButtonDay(Button button, WeatherData item)
        {
            var grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // дата
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // температура
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });  // іконка

            var txtDate = new TextBlock
            {
                Text = DateTime.Parse(item.Date).ToString("dd.MM"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 16
            };
            Grid.SetColumn(txtDate, 0);

            var txtTemp = new TextBlock
            {
                Text = item.Temperature.ToString("F1") + " °C",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16
            };
            Grid.SetColumn(txtTemp, 1);

            string path = "unknown.png";
            switch (item.Precipitation)
            {
                case "дощ": path = "rain.png"; break;
                case "сніг": path = "snow.png"; break;
                case "град": path = "hail.png"; break;
                case "хмарно": path = "cloud.png"; break;
                case "ясно": path = "sun.png"; break;
            }

            var img = new Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{path}")),
                Width = 24,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(img, 2);

            grid.Children.Add(txtDate);
            grid.Children.Add(txtTemp);
            grid.Children.Add(img);

            button.Content = grid;
        }

        // Обробники подій для кнопок переходу до наступних днів
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (index + 1 < weatherList.Count)
            {
                Day nextDayWindow = new Day(index + 1, weatherList);
                nextDayWindow.Show();
                this.Close();
            }
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (index + 2 < weatherList.Count)
            {
                Day nextDayWindow = new Day(index + 2, weatherList);
                nextDayWindow.Show();
                this.Close();
            }
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
