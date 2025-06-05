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

namespace weatherforecast
{
    public partial class Greeting : Window
    {
        public Greeting()
        {
            InitializeComponent();
        }

        // Кнопка "Увійти"
        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            Authorization authWindow = new Authorization();
            authWindow.Show();
        }

        // Кнопка "Зареєструватися"
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration regWindow = new Registration();
            regWindow.ShowDialog();
            if (regWindow.DialogResult == true)
            {
                // Якщо реєстрація пройшла успішно, перехід на вікно авторизації
                Authorization authWindow = new Authorization();
                authWindow.Show();
            }
        }
    }
}
