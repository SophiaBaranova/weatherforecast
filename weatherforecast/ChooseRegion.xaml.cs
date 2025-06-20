using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace weatherforecast
{
    public partial class ChooseRegion : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;
        private User user; // Збережений користувач, передається з авторизації

        public ChooseRegion(User user)
        {
            InitializeComponent();
            this.user = user;

            // Завантаження збереженого регіону
            LoadSavedRegion();
        }

        // Метод для завантаження збереженого регіону
        private void LoadSavedRegion()
        {
            // Перевірка з'єднання з БД
            if (!ValidationService.Connect(connectionString))
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string getSavedRegionQuery = "SELECT savedCountry, savedCity FROM authodata WHERE login = @login";

                    using (MySqlCommand cmd = new MySqlCommand(getSavedRegionQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@login", user.Login);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string savedCountry = reader["savedCountry"]?.ToString();
                                string savedCity = reader["savedCity"]?.ToString();

                                TextBox1.Text = savedCountry;
                                TextBox2.Text = savedCity;

                                CheckBox1.IsChecked = !string.IsNullOrWhiteSpace(savedCountry) && !string.IsNullOrWhiteSpace(savedCity);
                            }
                        }
                    }
                }

                // Обробка помилок
                catch (Exception ex)
                {
                    ValidationService.ShowMessage("Помилка при завантаженні збереженого регіону: " + ex.Message, "Помилка", MessageBoxImage.Error);
                    return;
                }
            }
        }

        // Кнопка "OK"
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2 };

            if (!ValidationService.CheckNotEmpty(borders))
            {
                ValidationService.ShowMessage("Будь ласка, заповніть всі поля", "Помилка", MessageBoxImage.Error);
                return;
            }

            // Зчитування даних
            string country = TextBox1.Text.Trim();
            string city = TextBox2.Text.Trim();
            bool remember = CheckBox1.IsChecked == true;

            // Якщо натинуто "Запам'ятати регіон", збереження даних у БД
            if (remember)
            {
                // Перевірка з'єднання з БД
                if (!ValidationService.Connect(connectionString))
                {
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string updateQuery = "UPDATE authodata SET savedCountry = @country, savedCity = @city WHERE login = @login";
                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@country", country);
                            cmd.Parameters.AddWithValue("@city", city);
                            cmd.Parameters.AddWithValue("@login", user.Login);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Обробка помилок
                    catch (Exception ex)
                    {
                        ValidationService.ShowMessage("Помилка при збереженні регіону: " + ex.Message, "Помилка", MessageBoxImage.Error);
                        return;
                    }
                }
            }

            if (user.Admin == true) // Якщо користувач є адміністратором
            {
                // Перехід на вікно таблиці
                Table tableWindow = new Table(country, city);
                tableWindow.Show();
            }
            else // Якщо користувач є клієнтом
            {
                // Перехід на вікно вибору дат
                SearchParam searchWindow = new SearchParam();
                searchWindow.ShowDialog();

                // Якщо дати обрано, перехід на вікно календаря
                if (searchWindow.DialogResult == true)
                {
                    Calendar calendarWindow = new Calendar(searchWindow.StartDate.Value, searchWindow.EndDate.Value, country, city);
                    calendarWindow.Show();
                }
            }
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        // Методи для очищення текстових полів при двократному натисканні
        private void TextBox1_MouseDC(object sender, MouseButtonEventArgs e)
        {
            TextBox1.Text = "";
        }
        private void TextBox2_MouseDC(object sender, MouseButtonEventArgs e)
        {
            TextBox2.Text = "";
        }
    }
}
