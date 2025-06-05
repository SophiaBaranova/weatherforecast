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
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using static weatherforecast.Registration;
using System.Configuration;

namespace weatherforecast
{
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        // Кнопка "Увійти"
        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            Border[] borders = { Border1, Border2 };

            // Перевірка заповнення полів
            Registration regWindow = new Registration();
            if (!regWindow.CheckValid(borders))
            {
                Registration.ShowMessage("Будь ласка, заповніть всі поля", "Помилка", MessageBoxImage.Error);
                return;
            }

            // Зчитування даних
            User user = new User
            {
                Login = TextBox1.Text.Trim(),
                Password = TextBox2.Text.Trim()
            };

            string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;

            // Перевірка з'єднання з БД
            if (!Registration.Connect(connectionString))
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Перевірка наявності облікового запису
                string query = "SELECT * FROM authodata WHERE login = @Login AND password = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Login", user.Login);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Registration.ShowMessage("Вхід успішний!", "Успіх", MessageBoxImage.Information);

                            // Визначення ролі користувача
                            user.Admin = reader.GetBoolean("admin");

                            // Перехід на вікно вибору регіону
                            ChooseRegion regionWindow = new ChooseRegion(user);
                            regionWindow.Show();
                        }
                        else
                        {
                            Registration.ShowMessage("Обліковий запис не знайдено", "Помилка", MessageBoxImage.Error);
                            return;
                        }
                    }
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
