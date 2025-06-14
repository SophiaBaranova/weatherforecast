using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace weatherforecast
{

    public static class ValidationService
    {

        // Метод для перевірки підключення до бази даних
        public static bool Connect(string connectionString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch (MySqlException ex)
            {
                ShowMessage("Помилка підключення до бази даних: " + ex.Message, "Помилка", MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                ShowMessage("Невідома помилка: " + ex.Message, "Помилка", MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для перевірки наявності таблиці в базі даних
        public static bool CheckTableExists(MySqlConnection connection, string tableName)
        {
            string checkQuery = "SHOW TABLES LIKE @Table;";
            using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Table", tableName);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowMessage("Таблиця БД не існує", "Помилка", MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            return true;
        }

        // Метод для перевірки заповненості полів
        public static bool CheckNotEmpty(Border[] borders)
        {
            bool isValid = true;

            foreach (var border in borders)
                border.BorderThickness = new Thickness(0);

            TextBox[] textBoxes = new TextBox[borders.Length];
            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i] = (TextBox)borders[i].Child;
            }

            for (int i = 0; i < textBoxes.Length; i++)
            {
                string text = textBoxes[i].Text.Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    borders[i].BorderBrush = Brushes.Red;
                    borders[i].BorderThickness = new Thickness(4);
                    isValid = false;
                }
            }

            return isValid;
        }

        // Метод для скидання червоних рамок
        public static void ResetFieldBorders(Border[] borders)
        {
            foreach (var border in borders)
                border.BorderThickness = new Thickness(0);
        }

        // Метод для перевірки коректності заповнення полів
        public static bool CheckValid(out string errorMessage, Border[] borders)
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

        // Метод для відображення повідомлення
        public static void ShowMessage(string msg, string title, MessageBoxImage icon)
        {
            MessageBox.Show(msg, title, MessageBoxButton.OK, icon);
            return;
        }

    }

}
