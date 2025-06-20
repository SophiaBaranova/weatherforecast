using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static weatherforecast.Registration;

namespace weatherforecast
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        // Кнопка "Зареєструватися"
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2};
            string errorMessage = "";
            bool isValid = true;

            // Перевірка заповнення полів
            if (ValidationService.CheckNotEmpty(borders))
            {
                errorMessage = "Будь ласка, заповніть всі поля";
                isValid = false;
            }
           
            // Перевірка логіна
            else if (!Regex.IsMatch(TextBox1.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Border1.BorderBrush = Brushes.Red;
                Border1.BorderThickness = new Thickness(4);
                errorMessage = "Логін - це Ваш email";
                isValid = false;
            }

            // Перевірка пароля
            else if (!Regex.IsMatch(TextBox2.Text, @"^(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;""'<>,.?~\\/-]).{6,}$"))
            {
                Border2.BorderBrush = Brushes.Red;
                Border2.BorderThickness = new Thickness(4);
                errorMessage = "Пароль має містити щонайменше 6 символів, включаючи малу літеру, цифру та спеціальний символ (!@#$%^&*()_+{}[]:;\"'<>,.?~\\/-)";
                isValid = false;
            }

            //Перевірка на співпадіння пароля
            else if (TextBox2.Text != TextBox3.Text)
            {
                Border3.BorderBrush = Brushes.Red;
                Border3.BorderThickness = new Thickness(4);
                errorMessage = "Паролі не співпадають. Повторіть ще раз";
                isValid = false;
            }

            if (!isValid)
            {
                ValidationService.ShowMessage(errorMessage, "Помилка", MessageBoxImage.Error);
                return;
            }

            // Зчитування даних
            User user = new User
            {
                Login = TextBox1.Text.Trim(),
                Password = TextBox2.Text.Trim(),
                Admin = false
            };

            string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;

            // Перевірка з'єднання з БД
            if (!ValidationService.Connect(connectionString))
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Перевірка наявності логіна
                string checkQuery = "SELECT COUNT(*) FROM authodata WHERE login = @Login";
                using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Login", user.Login);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        ValidationService.ShowMessage("Користувач з таким логіном вже існує", "Помилка", MessageBoxImage.Error);
                        return;
                    }
                }

                // Вставка нового користувача
                string insertQuery = "INSERT INTO authodata (login, password) VALUES (@Login, @Password)";
                using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Login", user.Login);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        ValidationService.ShowMessage("Реєстрація успішна!", "Успіх", MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }

                    // Обробка помилок
                    catch (Exception ex)
                    {
                        ValidationService.ShowMessage("Помилка при реєстрації: " + ex.Message, "Помилка", MessageBoxImage.Error);
                        return;
                    }
                }
            }
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
        private void TextBox3_MouseDC(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox3.Text = "";
        }

    }
}
