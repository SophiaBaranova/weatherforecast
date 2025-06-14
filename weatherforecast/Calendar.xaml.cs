using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Word = Microsoft.Office.Interop.Word;

namespace weatherforecast
{
    public partial class Calendar : Window
    {
        private DateTime startDate;
        private DateTime endDate;
        private string country;
        private string city;
        private Calculation calcWindow;

        // Початковий список даних
        private List<WeatherData> initialList;
        // Поточний список даних, який відображається в календарі
        private List<WeatherData> currentList;

        private List<Button> dayButtons;

        public Calendar(DateTime startDate, DateTime endDate, string country, string city)
        {
            InitializeComponent();

            this.startDate = startDate;
            this.endDate = endDate;
            this.country = country;
            this.city = city;

            // Ініціалізація списку кнопок
            dayButtons = new List<Button>
            {
                DayBut1, DayBut2, DayBut3, DayBut4, DayBut5, DayBut6, DayBut7,
                DayBut8, DayBut9, DayBut10, DayBut11, DayBut12, DayBut13, DayBut14,
                DayBut15, DayBut16, DayBut17, DayBut18, DayBut19, DayBut20, DayBut21,
                DayBut22, DayBut23, DayBut24, DayBut25, DayBut26, DayBut27, DayBut28,
                DayBut29, DayBut30, DayBut31, DayBut32, DayBut33, DayBut34, DayBut35,
                DayBut36, DayBut37, DayBut38, DayBut39, DayBut40, DayBut41, DayBut42
            };

            // Завантаження даних з БД
            LoadWeatherData();
        }

        // Завантаження даних з БД
        private void LoadWeatherData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["WeatherDb"].ConnectionString;
            initialList = new List<WeatherData>();

            // Перевірка з'єднання з БД
            if (!ValidationService.Connect(connectionString))
            {
                this.Close();
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Перевірка наявності таблиці weatherdata
                if (!ValidationService.CheckTableExists(connection, "weatherdata"))
                {
                    this.Close();
                }

                // Отримання записів по обраному регіону за обраний період з таблиці
                string getDataQuery = "SELECT * FROM weatherdata WHERE country = @country AND city = @city AND date BETWEEN @startDate AND @endDate ORDER BY date ASC";
                using (MySqlCommand cmd = new MySqlCommand(getDataQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            ValidationService.ShowMessage("Записів по обраному регіону за обраний період не знайдено", "Помилка", MessageBoxImage.Error);
                            this.Close();
                        }

                        while (reader.Read())
                        {
                            // Додавання записів до списку
                            initialList.Add(new WeatherData
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Country = reader["country"].ToString(),
                                City = reader["city"].ToString(),
                                Date = Convert.ToDateTime(reader["date"]).ToString("dd/MM/yyyy"),
                                Humidity = Convert.ToDouble(reader["humidity"]),
                                Precipitation = reader["precipitation"].ToString(),
                                PrecipitationVal = Convert.ToDouble(reader["precipitationVal"]),
                                Pressure = Convert.ToDouble(reader["pressure"]),
                                Temperature = Convert.ToDouble(reader["temperature"]),
                                Wind = reader["wind"].ToString(),
                                WindSpeed = Convert.ToDouble(reader["windSpeed"])
                            });
                        }
                    }
                }
            }

            currentList = new List<WeatherData>(initialList);
            // Заповнення календаря даними
            FillCalendar();
        }

        // Метод для заповнення кнопок іконками із датою
        private void FillCalendar()
        {
            for (int i = 0; i < dayButtons.Count; i++)
            {
                if (i < currentList.Count)
                {
                    FillDay(dayButtons[i], currentList[i]);
                }
                else
                {
                    dayButtons[i].Content = "";
                }
            }
        }

        // Метод для заповнення кнопки даними про погоду
        private void FillDay(Button button, WeatherData item)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Визначення шляху до зображення залежно від типу опадів
            string path = "unknown.png"; // дефолтне зображення

            switch (item.Precipitation)
            {
                case "дощ":
                    path = "rain.png";
                    break;
                case "сніг":
                    path = "snow.png";
                    break;
                case "град":
                    path = "hail.png";
                    break;
                case "хмарно":
                    path = "cloud.png";
                    break;
                case "ясно":
                    path = "sun.png";
                    break;
            }

            // Створення картинки
            var img = new Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{path}")),
                Width = 24,
                Height = 24,
                Margin = new Thickness(0, 0, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Створення текстового блоку з датою у форматі dd.MM
            var txt = new TextBlock
            {
                Text = DateTime.Parse(item.Date).ToString("dd.MM"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16
            };

            stack.Children.Add(img);
            stack.Children.Add(txt);

            button.Content = stack;
        }

        // Кнопка "Фільтр"
        private void ButtonFilter_click(object sender, RoutedEventArgs e)
        {
            // Перехід на вікно фільтрації з початковим списком даних
            Filter filterWindow = new Filter(initialList);
            filterWindow.ShowDialog();
            // Якщо фільтр застосовано, оновлення календаря
            if (filterWindow.DialogResult == true)
            {
                currentList = filterWindow.FilteredData;
                FillCalendar();
                if (calcWindow != null && calcWindow.IsCalculated)
                {
                    calcWindow.IsCalculated = false; // Скидання статусу розрахунку, щоб при наступному відкритті калькулятора дані були актуальні
                }
            }
        }


        //Кнопка "Калькулятор"
        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Перехід на вікно фільтрації з початковим списком даних
            calcWindow = new Calculation(initialList);
            calcWindow.ShowDialog();
        }

        // Кнопка "Завантажити"
        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка наявності даних
            if (currentList == null || currentList.Count == 0)
            {
                ValidationService.ShowMessage("Немає даних для збереження", "Помилка", MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Створення нового документа Word
                var wordApp = new Word.Application();
                wordApp.Visible = false;
                var document = wordApp.Documents.Add();

                // Заголовок документа
                Word.Paragraph title = document.Content.Paragraphs.Add();
                title.Range.Text = "Погодні показники";
                title.Range.Font.Name = "Times New Roman";
                title.Range.Font.Bold = 1;
                title.Range.Font.Size = 16;
                title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.InsertParagraphAfter();

                // Інформація про локацію та період
                Word.Paragraph p1 = document.Content.Paragraphs.Add();
                p1.Range.Text = $"Країна: {country}\nМісто: {city}\nПеріод: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
                p1.Range.Font.Name = "Times New Roman";
                p1.Range.Font.Size = 12;
                p1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                p1.Range.InsertParagraphAfter();

                // Додавання таблиці з поточними даними
                Word.Paragraph p2 = document.Content.Paragraphs.Add();
                Word.Table table1 = document.Tables.Add(p2.Range, currentList.Count + 1, 6); // +1 рядок для заголовків
                table1.Range.Font.Name = "Times New Roman";
                table1.Range.Font.Size = 12;
                table1.Borders.Enable = 1;

                // Заголовки таблиці
                string[] headers = { "Дата", "Температура", "Вологість", "Тиск", "Опади", "Вітер" };
                for (int i = 0; i < headers.Length; i++)
                {
                    table1.Cell(1, i + 1).Range.Text = headers[i];
                    table1.Cell(1, i + 1).Range.Bold = 1;
                    table1.Cell(1, i + 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }

                // Заповнення таблиці з currentList
                for (int i = 0; i < currentList.Count; i++)
                {
                    var item = currentList[i];
                    table1.Cell(i + 2, 1).Range.Text = Convert.ToDateTime(item.Date).ToString("dd/MM/yyyy");
                    table1.Cell(i + 2, 2).Range.Text = item.Temperature.ToString("F1") + " °C";
                    table1.Cell(i + 2, 3).Range.Text = item.Humidity.ToString("F1") + " %";
                    table1.Cell(i + 2, 4).Range.Text = item.Pressure.ToString("F1") + " мм.рт.ст.";
                    table1.Cell(i + 2, 5).Range.Text = item.Precipitation + ", " + item.PrecipitationVal.ToString("F1") + " %";
                    table1.Cell(i + 2, 6).Range.Text = item.Wind + ", " + item.WindSpeed.ToString("F1") + " м/с";
                }

                // Додавання результатів розрахунку, якщо вони є
                if (calcWindow != null && calcWindow.IsCalculated)
                {
                    // Вставка розриву сторінки
                    Word.Paragraph pageBreakParagraph = document.Content.Paragraphs.Add();
                    object breakType = Word.WdBreakType.wdPageBreak;
                    pageBreakParagraph.Range.InsertBreak(ref breakType);
                    pageBreakParagraph.Range.InsertParagraphAfter();

                    // Заголовок таблиці з розрахунками
                    Word.Paragraph title2 = document.Content.Paragraphs.Add();
                    title2.Range.Text = "Розрахунок середньомісячних показників";
                    title2.Range.Font.Name = "Times New Roman";
                    title2.Range.Font.Bold = 1;
                    title2.Range.Font.Size = 16;
                    title2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    title2.Range.InsertParagraphAfter();

                    Word.Paragraph p3 = document.Content.Paragraphs.Add();
                    Word.Table table2 = document.Tables.Add(p3.Range, 6, 2);
                    table2.Range.Font.Name = "Times New Roman";
                    table2.Range.Font.Size = 12;
                    table2.Borders.Enable = 1;

                    // Заголовки рядків
                    string[] metricNames = { "Температура", "Вологість", "Тиск", "Дощ", "Сніг", "Град" };
                    // Результати розрахунку
                    string[] metricValues = {
                $"{calcWindow.AverageTemp:F1} °C",
                $"{calcWindow.AverageHumidity:F1} %",
                $"{calcWindow.AveragePressure:F1} мм.рт.ст.",
                $"{calcWindow.AverageRain:F1} %",
                $"{calcWindow.AverageSnow:F1} %",
                $"{calcWindow.AverageHail:F1} %"
                    };

                    // Заповнення таблиці
                    for (int i = 0; i < 6; i++)
                    {
                        table2.Cell(i + 1, 1).Range.Text = metricNames[i];
                        table2.Cell(i + 1, 2).Range.Text = metricValues[i];
                    }
                }

                // Збереження документа
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Звіт_" + DateTime.Now.ToString("dd/MM/yyyy"),
                    Filter = "Word Documents|*.docx",
                    Title = "Зберегти звіт"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    document.SaveAs2(saveDialog.FileName);
                    ValidationService.ShowMessage("Документ збережено успішно", "Успіх", MessageBoxImage.Information);
                }
                else
                {
                    return; // Якщо користувач скасував збереження, вихід із методу
                }

                document.Close();
                wordApp.Quit();
            }

            // Обробка помилок
            catch (Exception ex)
            {
                ValidationService.ShowMessage("Помилка при збереженні: " + ex.Message, "Помилка", MessageBoxImage.Error);
            }
        }

        // Кнопка "Назад"
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Обробники подій для кнопок днів
        private void Day1(object sender, RoutedEventArgs e) { OpenDay(0); }
        private void Day2(object sender, RoutedEventArgs e) { OpenDay(1); }
        private void Day3(object sender, RoutedEventArgs e) { OpenDay(2); }
        private void Day4(object sender, RoutedEventArgs e) { OpenDay(3); }
        private void Day5(object sender, RoutedEventArgs e) { OpenDay(4); }
        private void Day6(object sender, RoutedEventArgs e) { OpenDay(5); }
        private void Day7(object sender, RoutedEventArgs e) { OpenDay(6); }
        private void Day8(object sender, RoutedEventArgs e) { OpenDay(7); }
        private void Day9(object sender, RoutedEventArgs e) { OpenDay(8); }
        private void Day10(object sender, RoutedEventArgs e) { OpenDay(9); }
        private void Day11(object sender, RoutedEventArgs e) { OpenDay(10); }
        private void Day12(object sender, RoutedEventArgs e) { OpenDay(11); }
        private void Day13(object sender, RoutedEventArgs e) { OpenDay(12); }
        private void Day14(object sender, RoutedEventArgs e) { OpenDay(13); }
        private void Day15(object sender, RoutedEventArgs e) { OpenDay(14); }
        private void Day16(object sender, RoutedEventArgs e) { OpenDay(15); }
        private void Day17(object sender, RoutedEventArgs e) { OpenDay(16); }
        private void Day18(object sender, RoutedEventArgs e) { OpenDay(17); }
        private void Day19(object sender, RoutedEventArgs e) { OpenDay(18); }
        private void Day20(object sender, RoutedEventArgs e) { OpenDay(19); }
        private void Day21(object sender, RoutedEventArgs e) { OpenDay(20); }
        private void Day22(object sender, RoutedEventArgs e) { OpenDay(21); }
        private void Day23(object sender, RoutedEventArgs e) { OpenDay(22); }
        private void Day24(object sender, RoutedEventArgs e) { OpenDay(23); }
        private void Day25(object sender, RoutedEventArgs e) { OpenDay(24); }
        private void Day26(object sender, RoutedEventArgs e) { OpenDay(25); }
        private void Day27(object sender, RoutedEventArgs e) { OpenDay(26); }
        private void Day28(object sender, RoutedEventArgs e) { OpenDay(27); }
        private void Day29(object sender, RoutedEventArgs e) { OpenDay(28); }
        private void Day30(object sender, RoutedEventArgs e) { OpenDay(29); }
        private void Day31(object sender, RoutedEventArgs e) { OpenDay(30); }
        private void Day32(object sender, RoutedEventArgs e) { OpenDay(31); }
        private void Day33(object sender, RoutedEventArgs e) { OpenDay(32); }
        private void Day34(object sender, RoutedEventArgs e) { OpenDay(33); }
        private void Day35(object sender, RoutedEventArgs e) { OpenDay(34); }
        private void Day36(object sender, RoutedEventArgs e) { OpenDay(35); }
        private void Day37(object sender, RoutedEventArgs e) { OpenDay(36); }
        private void Day38(object sender, RoutedEventArgs e) { OpenDay(37); }
        private void Day39(object sender, RoutedEventArgs e) { OpenDay(38); }
        private void Day40(object sender, RoutedEventArgs e) { OpenDay(39); }
        private void Day41(object sender, RoutedEventArgs e) { OpenDay(40); }
        private void Day42(object sender, RoutedEventArgs e) { OpenDay(41); }

        // Метод для відкриття вікна з деталями дня
        private void OpenDay(int index)
        {
            if (index >= 0 && index < currentList.Count)
            {
                Day dayWindow = new Day(index, currentList);
                dayWindow.Show();
            }
        }
    }
}