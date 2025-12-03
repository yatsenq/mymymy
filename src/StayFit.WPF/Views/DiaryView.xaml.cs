using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.ViewModels;

namespace StayFit.WPF.Views
{
    public partial class DiaryView : UserControl
    {
        private DiaryViewModel? _viewModel;

        public DiaryView()
        {
            InitializeComponent();
            
            var app = System.Windows.Application.Current as App;
            if (app?.Services != null)
            {
                // ✅ Тепер завжди буде той самий екземпляр (Singleton)
                _viewModel = app.Services.GetRequiredService<DiaryViewModel>();
                DataContext = _viewModel;
            }

            // ✅ Підписуємося на подію Loaded
            this.Loaded += DiaryView_Loaded;
        }

        // ✅ Кожен раз при відображенні вікна - перезавантажуємо дані з БД
        private async void DiaryView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.RefreshDataAsync();
            }
        }
    }
}