using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.ViewModels;

namespace StayFit.WPF.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            
            var app = Application.Current as App;
            if (app?.Services != null)
            {
                DataContext = app.Services.GetRequiredService<SettingsViewModel>();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("[SettingsView] Button CLICKED - executing command...");
            var vm = DataContext as SettingsViewModel;
            if (vm?.UpdateProfileCommand?.CanExecute(null) == true)
            {
                vm.UpdateProfileCommand.Execute(null);
            }
            else
            {
                System.Console.WriteLine("[SettingsView] Command CanExecute = FALSE!");
            }
        }
    }
}
