using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace weatherforecast
{
    public partial class Filter : Window
    {
        // Список всіх даних
        private List<WeatherData> allData;
        // Список відфільтрованих даних
        public List<WeatherData> FilteredData { get; private set; }

        
        public Filter(List<WeatherData> allData)
        {
            InitializeComponent();
            this.allData = allData;
        }

        // Кнопка "Скасувати"
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Кнопка "OK"
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2 };
            TextBox[] textBoxes = { TextBox1, TextBox2 };

            // Перевірка заповнення полів
            for (int i = 0; i < textBoxes.Length; i++)
            {
                borders[i].BorderThickness = new Thickness(0);

                if (string.IsNullOrWhiteSpace(textBoxes[i].Text) || !double.TryParse(textBoxes[i].Text, out _))
                {
                    borders[i].BorderBrush = Brushes.Red;
                    borders[i].BorderThickness = new Thickness(4);
                    ValidationService.ShowMessage("Будь ласка, заповніть всі поля", "Помилка", MessageBoxImage.Error);
                    return;
                }
            }

            // Зчитування температури
            double tempFrom = double.Parse(TextBox1.Text);
            double tempTo = double.Parse(TextBox2.Text);

            // Зчитування типу опадів
            if (ComboBox1.SelectedItem == null)
            {
                Border3.BorderBrush = Brushes.Red;
                Border3.BorderThickness = new Thickness(4);
                ValidationService.ShowMessage("Будь ласка, оберіть тип опадів", "Помилка", MessageBoxImage.Error);
                return;
            }

            string selectedPrecipitation = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Фільтрація даних
            FilteredData = allData
                .Where(w => w.Temperature >= tempFrom &&
                            w.Temperature <= tempTo &&
                            w.Precipitation == selectedPrecipitation)
                .ToList();

            this.DialogResult = true;
            this.Close();
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
