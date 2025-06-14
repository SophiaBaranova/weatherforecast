using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Windows.Media;
using System.Configuration;

namespace weatherforecast
{
    public partial class Edit : Window
    {
        public WeatherData UpdatedData { get; private set; }

        private string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;
        private int entryId;

        public Edit(WeatherData entry)
        {
            InitializeComponent();

            entryId = entry.ID;

            // Відображення поточних значень показників у полях для введення
            TextBox1.Text = entry.Temperature.ToString();
            TextBox2.Text = entry.PrecipitationVal.ToString();
            TextBox3.Text = entry.Humidity.ToString();
            TextBox4.Text = entry.Pressure.ToString();
            TextBox5.Text = entry.WindSpeed.ToString();

            foreach (ComboBoxItem item in ComboBox1.Items)
            {
                if (item.Content.ToString().Equals(entry.Precipitation, StringComparison.OrdinalIgnoreCase))
                {
                    ComboBox1.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in ComboBox2.Items)
            {
                if (item.Content.ToString().Equals(entry.Wind, StringComparison.OrdinalIgnoreCase))
                {
                    ComboBox2.SelectedItem = item;
                    break;
                }
            }

            UpdatedData = new WeatherData
            {
                Country = entry.Country,
                City = entry.City
            };
        }

        // Кнопка "Видалити"
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            // Запит підтвердження видалення
            var result = MessageBox.Show("Ви впевнені, що хочете видалити запис?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // Перевірка з'єднання з БД
                if (!ValidationService.Connect(connectionString))
                {
                    return;
                }

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Видалення запису із БД
                        string deleteQuery = "DELETE FROM weatherdata WHERE id = @ID";
                        using (MySqlCommand cmd = new MySqlCommand(deleteQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@ID", entryId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    
                    ValidationService.ShowMessage("Запис успішно видалено", "Успіх", MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }

                // Обробка помилок
                catch (Exception ex)
                {
                    ValidationService.ShowMessage("Помилка при видаленні: " + ex.Message, "Помилка", MessageBoxImage.Error);
                    this.DialogResult = false;
                    this.Close();
                }
            }
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
            try
            {
                Border[] borders = { Border1, Border2, Border3, Border4, Border5, Border6, Border7 };

                // Перевірка коректності заповнення полів
                if (!ValidationService.CheckValid(out string errorMessage, borders))
                {
                    ValidationService.ShowMessage(errorMessage, "Помилка", MessageBoxImage.Error);
                    return;
                }

                // Перевірка з'єднання з БД
                if (!ValidationService.Connect(connectionString))
                {
                    return;
                }

                // Перевірка існування запису в БД
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM weatherdata WHERE id = @ID";
                    using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", entryId);
                        int recordCount = Convert.ToInt32(cmd.ExecuteScalar());

                        // Якщо запису не існує
                        if (recordCount == 0)
                        {
                            ValidationService.ShowMessage("Запис не знайдено", "Помилка", MessageBoxImage.Error);
                            this.DialogResult = false;
                            this.Close();
                        }
                    }
                }

                // Заповнення об'єкта UpdatedData введеними даними
                UpdatedData.ID = entryId;
                UpdatedData.Temperature = double.Parse(TextBox1.Text);
                UpdatedData.PrecipitationVal = double.Parse(TextBox2.Text);
                UpdatedData.Humidity = double.Parse(TextBox3.Text);
                UpdatedData.Pressure = double.Parse(TextBox4.Text);
                UpdatedData.WindSpeed = double.Parse(TextBox5.Text);
                UpdatedData.Precipitation = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                UpdatedData.Wind = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();

                // Оновлення запису
                string updateQuery = "UPDATE weatherdata SET Temperature = @Temperature, PrecipitationVal = @PrecipitationVal, Humidity = @Humidity, Pressure = @Pressure, WindSpeed = @WindSpeed, Precipitation = @Precipitation, Wind = @Wind WHERE id = @ID";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", UpdatedData.ID);
                        cmd.Parameters.AddWithValue("@Temperature", UpdatedData.Temperature);
                        cmd.Parameters.AddWithValue("@PrecipitationVal", UpdatedData.PrecipitationVal);
                        cmd.Parameters.AddWithValue("@Humidity", UpdatedData.Humidity);
                        cmd.Parameters.AddWithValue("@Pressure", UpdatedData.Pressure);
                        cmd.Parameters.AddWithValue("@WindSpeed", UpdatedData.WindSpeed);
                        cmd.Parameters.AddWithValue("@Precipitation", UpdatedData.Precipitation);
                        cmd.Parameters.AddWithValue("@Wind", UpdatedData.Wind);
                        cmd.ExecuteNonQuery();
                    }
                }

                ValidationService.ShowMessage("Дані успішно оновлено", "Успіх", MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }

            // Обробка помилок
            catch (Exception ex)
            {
                ValidationService.ShowMessage("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxImage.Error);
                return;
            }
        }
    }
}
