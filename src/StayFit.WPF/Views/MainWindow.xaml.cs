using System;
using System.Windows;

namespace StayFit.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                // MessageBox.Show("MainWindow ініціалізовано", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Завантажити Diary View за замовчуванням
                try
                {
                    var diaryView = new DiaryView();
                    // MessageBox.Show("DiaryView створено", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    MainFrame.Navigate(diaryView);
                    // MessageBox.Show("DiaryView завантажено у Frame", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження DiaryView:\n{ex.Message}\n\n{ex.StackTrace}", 
                                   "Помилка", 
                                   MessageBoxButton.OK, 
                                   MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації MainWindow:\n{ex.Message}\n\n{ex.StackTrace}", 
                               "Критична помилка", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
            }
        }

        private void BtnDiary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(new DiaryView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnProgress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(new ProgressView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(new SettingsView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginView();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}