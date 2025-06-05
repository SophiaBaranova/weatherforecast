using System;
using System.Collections.Generic;
using System.Configuration;
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


namespace weatherforecast
{
    public partial class Table : Window
    {
        private string country, city;
        private List<WeatherData> weatherList;
        public Table(string country, string city)
        {
            InitializeComponent();

            this.country = country;
            this.city = city;

            // Завантаження даних з БД
            LoadWeatherData();
        }


        private void LoadWeatherData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;
            weatherList = new List<WeatherData>();

            // Перевірка з'єднання з БД
            if (!Registration.Connect(connectionString))
            {
                this.Close();
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Перевірка з'єднання
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    Registration.ShowMessage("Перевірте з'єднання", "Помилка", MessageBoxImage.Error);
                    this.Close();
                }

                // Перевірка наявності таблиці
                string checkQuery = "SHOW TABLES LIKE 'weatherdata';";
                using (MySqlCommand cmdCheck = new MySqlCommand(checkQuery, connection))
                using (MySqlDataReader readerCheck = cmdCheck.ExecuteReader())
                {
                    if (!readerCheck.Read())
                    {
                        Registration.ShowMessage("Таблиця БД не існує", "Помилка", MessageBoxImage.Error);
                        this.Close();
                    }
                }

                // Отримання записів за регіоном з таблиці
                string query = "SELECT * FROM weatherdata WHERE country = @country AND city = @city";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.ExecuteNonQuery();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Registration.ShowMessage("Таблиця БД існує, але не містить записів по обраному регіону", "Помилка", MessageBoxImage.Error);
                            this.Close();
                        }

                        while (reader.Read())
                        {
                            weatherList.Add(new WeatherData
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Country = reader["country"].ToString(),
                                City = reader["city"].ToString(),
                                Date = Convert.ToDateTime(reader["date"]).ToString("dd/MM/yyyy"),
                                Humidity = Convert.ToDouble(reader["humidity"].ToString()),
                                Precipitation = reader["precipitation"].ToString(),
                                PrecipitationVal = Convert.ToDouble(reader["precipitationVal"].ToString()),
                                Pressure = Convert.ToDouble(reader["pressure"].ToString()),
                                Temperature = Convert.ToDouble(reader["temperature"].ToString()),
                                Wind = reader["wind"].ToString(),
                                WindSpeed = Convert.ToDouble(reader["windSpeed"].ToString())
                            });
                        }
                    }
                }
                TableWeather.ItemsSource = weatherList;
            }
        }

        // Заповнення таблиці
        private void DG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            // Приховування стовпця із ID
            if (headername == "ID")
            {
                e.Cancel = true;
            }

            // Зміна назв стовпців
            else if (headername == "Country")
            {
                e.Column.Header = "Країна";
            }
            else if (headername == "City")
            {
                e.Column.Header = "Місто";
            }
            else if (headername == "Date")
            {
                e.Column.Header = "Дата";
            }
            else if (headername == "Humidity")
            {
                e.Column.Header = "Вологість";
            }
            else if (headername == "Precipitation")
            {
                e.Column.Header = "Опади";
            }
            else if (headername == "PrecipitationVal")
            {
                e.Column.Header = "Відсоток опадів";
            }
            else if (headername == "Pressure")
            {
                e.Column.Header = "Тиск";
            }
            else if (headername == "Temperature")
            {
                e.Column.Header = "Температура";
            }
            else if (headername == "Wind")
            {
                e.Column.Header = "Вітер";
            }
            else if (headername == "WindSpeed")
            {
                e.Column.Header = "Швидкість вітру";
            }
        }

        // Вибір місяця
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int month = ComboBox1.SelectedIndex + 1;
            var monthEntries = weatherList
                .Where(w =>
                {
                    if (DateTime.TryParseExact(w.Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                    {
                        return dt.Month == month;
                    }
                    return false;
                })
                .ToList();
            TableWeather.ItemsSource = monthEntries;
        }

        // Кнопка "Додати"
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            // Перехід на вікно додавання та передача туди обраного регіону
            Add addWindow = new Add(country, city);
            addWindow.ShowDialog();

            if (addWindow.DialogResult == true)
            {
                // Після додавання оновлення таблиці
                LoadWeatherData();
            }
        }

        // Кнопка "Редагувати"
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного запису
            WeatherData selectedItem = TableWeather.SelectedItem as WeatherData;

            if (selectedItem == null)
            {
                Registration.ShowMessage("Оберіть запис для редагування","Пояснення", MessageBoxImage.Warning);
                return;
            }

            // Перехід на вікно редагування та передача туди обраного об'єкта
            Edit editWindow = new Edit(selectedItem);
            editWindow.ShowDialog();

            if (editWindow.DialogResult == true)
            {
                // Після редагування оновлення таблиці
                LoadWeatherData();
            }
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class WeatherData
    {
        public int ID { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public double Humidity { get; set; }
        public string Precipitation { get; set; }
        public double PrecipitationVal { get; set; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public string Wind { get; set; }
        public double WindSpeed { get; set; }
    }
}
