using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.Windows;
using weatherforecast;
using System;

namespace weatherforecast.Tests
{
    [TestClass]
    public class AddTests
    {
        [TestMethod]
        [STAThread]
        public void AddCheckValid_ValidData_ReturnsTrue()
        {
            // 1. Створення вікна додавання
            var window = new Add("Україна", "Київ");

            // 2. Створення Borders (0-4 — TextBox, 5-6 — ComboBox)
            Border[] borders = new Border[7];
            for (int i = 0; i < 5; i++)
            {
                borders[i] = new Border
                {
                    Child = new TextBox()
                };
            }
            var comboBox1 = new ComboBox();
            comboBox1.Items.Add(new ComboBoxItem { Content = "Дощ" });
            comboBox1.SelectedIndex = 0;
            var comboBox2 = new ComboBox();
            comboBox2.Items.Add(new ComboBoxItem { Content = "Пн" });
            comboBox2.SelectedIndex = 0;
            borders[5] = new Border { Child = comboBox1 };
            borders[6] = new Border { Child = comboBox2 };

            // 3. Заповнення TextBox-ів валідними значеннями
            ((TextBox)borders[0].Child).Text = "25";    // Температура
            ((TextBox)borders[1].Child).Text = "60";    // Опади (в діапазоні)
            ((TextBox)borders[2].Child).Text = "50";    // Вологість (в діапазоні)
            ((TextBox)borders[3].Child).Text = "1015";  // Тиск
            ((TextBox)borders[4].Child).Text = "10";    // Швидкість вітру

            // 4. Виклик методу
            bool isValid = window.CheckValid(out string error, borders);

            // 5. Перевірка відповідності очікуваних і фактичних значень
            Assert.IsTrue(isValid, "Метод CheckValid має повертати true при коректних даних");
            Assert.AreEqual("", error, "Повідомлення про помилку має бути порожнім");
        }
        [TestMethod]
        [STAThread]
        public void AddCheckValid_EmptyData_ReturnsFalse()
        {
            // 1. Створення вікна додавання
            var window = new Add("Україна", "Київ");

            // 2. Створення Borders (0-4 — TextBox, 5-6 — ComboBox)
            Border[] borders = new Border[7];
            for (int i = 0; i < 5; i++)
            {
                borders[i] = new Border
                {
                    Child = new TextBox()
                };
            }
            var comboBox1 = new ComboBox();
            comboBox1.Items.Add(new ComboBoxItem { Content = "Дощ" });
            comboBox1.SelectedIndex = 0;
            var comboBox2 = new ComboBox();
            comboBox2.Items.Add(new ComboBoxItem { Content = "Пн" });
            comboBox2.SelectedIndex = 0;
            borders[5] = new Border { Child = comboBox1 };
            borders[6] = new Border { Child = comboBox2 };

            // 3. Заповнення не всіх TextBox-ів
            ((TextBox)borders[0].Child).Text = "25";    // Температура
            ((TextBox)borders[1].Child).Text = "";    // Опади
            ((TextBox)borders[2].Child).Text = "79";    // Вологість
            ((TextBox)borders[3].Child).Text = "1015";  // Тиск
            ((TextBox)borders[4].Child).Text = "";    // Швидкість вітру

            // 4. Виклик методу
            bool isValid = window.CheckValid(out string error, borders);

            // 5. Перевірка відповідності очікуваних і фактичних значень
            Assert.IsFalse(isValid, "Метод CheckValid має повертати false при відсутності даних");
            Assert.AreEqual("Будь ласка, заповніть всі поля коректно", error, "Повинно бути повідомлення про помилку");
        }
        [TestMethod]
        [STAThread]
        public void AddCheckValid_InvalidDataRange_ReturnsFalse()
        {
            // 1. Створення вікна додавання
            var window = new Add("Україна", "Київ");

            // 2. Створення Borders (0-4 — TextBox, 5-6 — ComboBox)
            Border[] borders = new Border[7];
            for (int i = 0; i < 5; i++)
            {
                borders[i] = new Border
                {
                    Child = new TextBox()
                };
            }
            var comboBox1 = new ComboBox();
            comboBox1.Items.Add(new ComboBoxItem { Content = "Дощ" });
            comboBox1.SelectedIndex = 0;
            var comboBox2 = new ComboBox();
            comboBox2.Items.Add(new ComboBoxItem { Content = "Пн" });
            comboBox2.SelectedIndex = 0;
            borders[5] = new Border { Child = comboBox1 };
            borders[6] = new Border { Child = comboBox2 };

            // 3. Заповнення TextBox-ів значеннями, що виходять за межі допустимих діапазонів
            ((TextBox)borders[0].Child).Text = "25";    // Температура
            ((TextBox)borders[1].Child).Text = "-2";    // Опади (не в діапазоні 100)
            ((TextBox)borders[2].Child).Text = "500";    // Вологість (не в діапазоні 100)
            ((TextBox)borders[3].Child).Text = "1015";  // Тиск
            ((TextBox)borders[4].Child).Text = "10";    // Швидкість вітру

            // 4. Виклик методу
            bool isValid = window.CheckValid(out string error, borders);

            // 5. Перевірка відповідності очікуваних і фактичних значень
            Assert.IsFalse(isValid, "Метод CheckValid має повертати false при даних, що виходять за межі допустимих діапазонів");
            Assert.AreEqual("Будь ласка, заповніть всі поля коректно", error, "Повинно бути повідомлення про помилку");
        }
        [TestMethod]
        [STAThread]
        public void AddCheckValid_InvalidDataFormat_ReturnsFalse()
        {
            // 1. Створення вікна додавання
            var window = new Add("Україна", "Київ");

            // 2. Створення Borders (0-4 — TextBox, 5-6 — ComboBox)
            Border[] borders = new Border[7];
            for (int i = 0; i < 5; i++)
            {
                borders[i] = new Border
                {
                    Child = new TextBox()
                };
            }
            var comboBox1 = new ComboBox();
            comboBox1.Items.Add(new ComboBoxItem { Content = "Дощ" });
            comboBox1.SelectedIndex = 0;
            var comboBox2 = new ComboBox();
            comboBox2.Items.Add(new ComboBoxItem { Content = "Пн" });
            comboBox2.SelectedIndex = 0;
            borders[5] = new Border { Child = comboBox1 };
            borders[6] = new Border { Child = comboBox2 };

            // 3. Заповнення TextBox-ів даними неправильного формату
            ((TextBox)borders[0].Child).Text = "25";    // Температура
            ((TextBox)borders[1].Child).Text = "ляляля";    // Опади
            ((TextBox)borders[2].Child).Text = "56";    // Вологість
            ((TextBox)borders[3].Child).Text = "1015";  // Тиск
            ((TextBox)borders[4].Child).Text = "45";    // Швидкість вітру

            // 4. Виклик методу
            bool isValid = window.CheckValid(out string error, borders);

            // 5. Перевірка відповідності очікуваних і фактичних значень
            Assert.IsFalse(isValid, "Метод CheckValid має повертати false при даних неправильного формату");
            Assert.AreEqual("Будь ласка, заповніть всі поля коректно", error, "Повинно бути повідомлення про помилку");
        }
    }
}
