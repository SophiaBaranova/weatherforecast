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

        // Метод для скидання червоних рамок
        private void ResetFieldBorders(params Border[] borders)
        {
            foreach (var border in borders)
                border.BorderThickness = new Thickness(0);
        }

        // Метод для перевірки коректності заповнення полів
        public bool CheckValid(out string errorMessage, Border[] borders)
        {
            bool isValid = true;
            errorMessage = "";
            ResetFieldBorders(borders);

            TextBox[] textBoxes = new TextBox[borders.Length - 2];
            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i] = (TextBox)borders[i].Child;
            }

            ComboBox[] comboBoxes = new ComboBox[2];
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                comboBoxes[i] = (ComboBox)borders[i + textBoxes.Length].Child;
            }

            // Перевірка TextBox
            for (int i = 0; i < textBoxes.Length; i++)
            {
                string text = textBoxes[i].Text.Trim();
                if (string.IsNullOrWhiteSpace(text) || !double.TryParse(text, out _))
                {
                    borders[i].BorderBrush = Brushes.Red;
                    borders[i].BorderThickness = new Thickness(4);
                    isValid = false;
                }
                else if (i == 1 || i == 2)
                {
                    if (double.Parse(text) < 0 || double.Parse(text) > 100)
                    {
                        borders[i].BorderBrush = Brushes.Red;
                        borders[i].BorderThickness = new Thickness(4);
                        isValid = false;
                    }
                }
            }

            // Перевірка ComboBox
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                if (comboBoxes[i].SelectedItem == null)
                {
                    borders[i + textBoxes.Length].BorderBrush = Brushes.Red;
                    borders[i + textBoxes.Length].BorderThickness = new Thickness(4);
                    isValid = false;
                }
            }

            if (!isValid)
                errorMessage = "Будь ласка, заповніть всі поля коректно";

            return isValid;
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
            if (!CheckValid(out string validationError, borders))
            {
                Registration.ShowMessage(validationError, "Помилка", MessageBoxImage.Error);
                return;
            }

            // Перевірка та конвертація дати
            if (!DateTime.TryParse(TextBox0.Text.Trim(), out DateTime parsedDate))
            {
                Registration.ShowMessage("Неправильний формат дати. Використовуйте формат РРРР-ММ-ДД", "Помилка", MessageBoxImage.Error);
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
                if (!Registration.Connect(connectionString))
                {
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Перевірка існування запису в БД за датою і регіоном
                    string checkQuery = "SELECT COUNT(*) FROM weatherdata WHERE date = @Date AND country = @Country AND city = @City";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Date", AddedData.Date);
                        checkCommand.Parameters.AddWithValue("@Country", AddedData.Country);
                        checkCommand.Parameters.AddWithValue("@City", AddedData.City);

                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

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

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Date", AddedData.Date);
                            command.Parameters.AddWithValue("@Country", AddedData.Country);
                            command.Parameters.AddWithValue("@City", AddedData.City);
                            command.Parameters.AddWithValue("@Temperature", AddedData.Temperature);
                            command.Parameters.AddWithValue("@PrecipitationVal", AddedData.PrecipitationVal);
                            command.Parameters.AddWithValue("@Humidity", AddedData.Humidity);
                            command.Parameters.AddWithValue("@Pressure", AddedData.Pressure);
                            command.Parameters.AddWithValue("@WindSpeed", AddedData.WindSpeed);
                            command.Parameters.AddWithValue("@Precipitation", AddedData.Precipitation);
                            command.Parameters.AddWithValue("@Wind", AddedData.Wind);
                            command.ExecuteNonQuery();
                            MessageBox.Show(message, "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                    }
                }
            }

            // Обробка помилок
            catch (Exception ex)
            {
                Registration.ShowMessage("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxImage.Error);
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
