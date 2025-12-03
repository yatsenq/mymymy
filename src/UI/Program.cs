using StayFit.UI.Examples;

namespace StayFit.UI;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = DependencyInjectionExample.ConfigureServicesForWpf();
        
        Console.WriteLine("StayFit Application Started!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
