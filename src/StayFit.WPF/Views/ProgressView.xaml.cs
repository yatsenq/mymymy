using System;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StayFit.UI.ViewModels;

namespace StayFit.WPF.Views;

public partial class ProgressView : Page
{
    public ProgressView()
    {
        try
        {
            Console.WriteLine("[ProgressView] InitializeComponent START");
            InitializeComponent();
            Console.WriteLine("[ProgressView] InitializeComponent END");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProgressView] InitializeComponent ERROR: {ex.Message}");
            Console.WriteLine($"[ProgressView] StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[ProgressView] InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"[ProgressView] Inner StackTrace: {ex.InnerException.StackTrace}");
            }
            throw;
        }

        try
        {
            Console.WriteLine("[ProgressView] Setting DataContext START");
            var app = (App)System.Windows.Application.Current;
            var serviceProvider = app.Services;
            if (serviceProvider == null)
            {
                Console.WriteLine("[ProgressView] ERROR: serviceProvider is NULL!");
            }
            else
            {
                DataContext = serviceProvider.GetRequiredService<ProgressViewModel>();
                Console.WriteLine("[ProgressView] DataContext SET OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProgressView] DataContext ERROR: {ex.Message}");
            Console.WriteLine($"[ProgressView] StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[ProgressView] InnerException: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}