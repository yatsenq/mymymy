using StayFit.UI.Examples;

namespace StayFit.UI;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = DependencyInjectionExample.ConfigureServicesForWpf();
        
        Console.WriteLine("StayFit Application Started!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}