using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using po2tsv_converter.Commands;
using po2tsv_converter.Settings;

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

var serviceProvider = new ServiceCollection()

    .Configure<MainSettings>(configuration.GetSection(nameof(MainSettings)))
    .AddTransient<MainToPoCommand>()
    .AddTransient<MainFromPoCommand>()
    .AddTransient<CollectableToPoCommand>()
    .AddTransient<CollectableFromPoCommand>()
    .BuildServiceProvider();

var options = serviceProvider.GetService<IOptions<MainSettings>>();
try
{
    switch (options.Value.Mode)
    {
        case Mode.MainToPo:
            var mainToPoCommand = serviceProvider.GetService<MainToPoCommand>();
            if (!mainToPoCommand.HasErrors)
                mainToPoCommand.Execute();
            break;
        case Mode.MainFromPo:
            var mainFromPoCommand = serviceProvider.GetService<MainFromPoCommand>();
            if (!mainFromPoCommand.HasErrors)
                mainFromPoCommand.Execute();
            break;
        case Mode.CollectableToPo:
            var collectableToPoCommand = serviceProvider.GetService<CollectableToPoCommand>();
            if (!collectableToPoCommand.HasErrors)
                collectableToPoCommand.Execute();
            break;
        case Mode.CollectableFromPo:
            var collectableFromPoCommand = serviceProvider.GetService<CollectableFromPoCommand>();
            if (!collectableFromPoCommand.HasErrors)
                collectableFromPoCommand.Execute();
            break;
        default:
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Console.ReadKey();
}
