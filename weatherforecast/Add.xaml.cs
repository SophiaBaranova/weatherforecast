using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace weatherforecast
{
    public partial class Add : Window
    {
        public WeatherData AddedData { get; private set; }

        public Add(string country, string city)
        {
            InitializeComponent();
            AddedData = new WeatherData
            {
                Country = country,
                City = city
            };
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

        // Кнопка "ОК"
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2, Border3, Border4, Border5, Border6, Border7 };

            // Перевірка коректності заповнення полів
            if (!ValidationService.CheckValid(out string errorMessage, borders))
            {
                ValidationService.ShowMessage(errorMessage, "Помилка", MessageBoxImage.Error);
                return;
            }

            // Перевірка та конвертація дати
            if (!DateTime.TryParse(TextBox0.Text.Trim(), out DateTime parsedDate))
            {
                ValidationService.ShowMessage("Неправильний формат дати. Використовуйте формат РРРР-ММ-ДД", "Помилка", MessageBoxImage.Error);
                return;
            }
            string formattedDate = parsedDate.ToString("yyyy-MM-dd");

            try
            {
                // Заповнення об'єкта AddedData введеними даними
                AddedData.Date = formattedDate;
                AddedData.Temperature = double.Parse(TextBox1.Text);
                AddedData.PrecipitationVal = double.Parse(TextBox2.Text);
                AddedData.Humidity = double.Parse(TextBox3.Text);
                AddedData.Pressure = double.Parse(TextBox4.Text);
                AddedData.WindSpeed = double.Parse(TextBox5.Text);
                AddedData.Precipitation = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                AddedData.Wind = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;

                // Перевірка з'єднання з БД
                if (!ValidationService.Connect(connectionString))
                {
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    int count;

                    // Перевірка існування запису в БД за датою і регіоном
                    string checkQuery = "SELECT COUNT(*) FROM weatherdata WHERE date = @Date AND country = @Country AND city = @City";
                    using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Date", AddedData.Date);
                        cmd.Parameters.AddWithValue("@Country", AddedData.Country);
                        cmd.Parameters.AddWithValue("@City", AddedData.City);

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string query;
                    string message;

                    // Якщо запис існує
                    if (count > 0)
                    {
                        // Запит чи оновлювати дані
                        var result = MessageBox.Show("Дані за вказану дату вже існують. Замінити їх?", "Запис існує", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.No)
                        {
                            this.DialogResult = false;
                            this.Close();
                        }

                        // Оновлення запису
                        query = @"UPDATE weatherdata 
                              SET Temperature = @Temperature, PrecipitationVal = @PrecipitationVal, Humidity = @Humidity, 
                                  Pressure = @Pressure, WindSpeed = @WindSpeed, Precipitation = @Precipitation, Wind = @Wind 
                              WHERE date = @Date AND country = @Country AND city = @City";
                        message = "Дані успішно оновлено";
                    }

                    // Якщо запису не існує
                    else
                    {
                        // Додавання запису
                        query = @"INSERT INTO weatherdata 
                              (date, country, city, Temperature, PrecipitationVal, Humidity, Pressure, WindSpeed, Precipitation, Wind) 
                              VALUES (@Date, @Country, @City, @Temperature, @PrecipitationVal, @Humidity, @Pressure, @WindSpeed, @Precipitation, @Wind)";
                        message = "Дані успішно додано";
                    }

                    // Додавання або оновлення запису
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Date", AddedData.Date);
                        cmd.Parameters.AddWithValue("@Country", AddedData.Country);
                        cmd.Parameters.AddWithValue("@City", AddedData.City);
                        cmd.Parameters.AddWithValue("@Temperature", AddedData.Temperature);
                        cmd.Parameters.AddWithValue("@PrecipitationVal", AddedData.PrecipitationVal);
                        cmd.Parameters.AddWithValue("@Humidity", AddedData.Humidity);
                        cmd.Parameters.AddWithValue("@Pressure", AddedData.Pressure);
                        cmd.Parameters.AddWithValue("@WindSpeed", AddedData.WindSpeed);
                        cmd.Parameters.AddWithValue("@Precipitation", AddedData.Precipitation);
                        cmd.Parameters.AddWithValue("@Wind", AddedData.Wind);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(message, "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }

            // Обробка помилок
            catch (Exception ex)
            {
                ValidationService.ShowMessage("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxImage.Error);
                return;
            }
        }

        // Метод для очищення текстового поля при двократному натисканні
        private void TextBox1_MouseDC(object sender, MouseButtonEventArgs e)
        {
            TextBox1.Text = "";
        }
    }
}
