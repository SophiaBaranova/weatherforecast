using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;

namespace weatherforecast
{
    public partial class SearchParam : Window
    {
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public SearchParam()
        {
            InitializeComponent();
        }

        // Кнопка "OK"
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2 };

            // Перевірка заповнення полів
            Registration regWindow = new Registration();
            if (!regWindow.CheckValid(borders))
            {
                Registration.ShowMessage("Будь ласка, заповніть всі поля", "Помилка", MessageBoxImage.Error);
                return;
            }

            string startText = TextBox1.Text.Trim();
            string endText = TextBox2.Text.Trim();

            DateTime startDate, endDate;
            string format = "yyyy-MM-dd";

            // Перевірка коректності дати
            if (!DateTime.TryParseExact(startText, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                Registration.ShowMessage("Невірний формат дати початку. Використовуйте формат РРРР-ММ-ДД", "Помилка", MessageBoxImage.Error);
                return;
            }
            if (!DateTime.TryParseExact(endText, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                Registration.ShowMessage("Невірний формат дати завершення. Використовуйте формат РРРР-ММ-ДД", "Помилка", MessageBoxImage.Error);
                return;
            }

            if (endDate < startDate)
            {
                Registration.ShowMessage("Дата завершення не може бути раніше дати початку", "Помилка", MessageBoxImage.Error);
                return;
            }

            // Перевірка на максимальний проміжок (42 дні)
            TimeSpan interval = endDate - startDate;
            if (interval.TotalDays > 42)
            {
                Registration.ShowMessage("Вибраний проміжок не може бути більше 42 днів", "Попередження", MessageBoxImage.Warning);
                return;
            }

            StartDate = startDate;
            EndDate = endDate;

            this.DialogResult = true;
            this.Close();
        }

        // Кнопка "Скасувати"
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Методи для очищення текстових полів при двократному натисканні
        private void TextBox1_MouseDC(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox1.Text = "";
        }
        private void TextBox2_MouseDC(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox2.Text = "";
        }
    }
}
