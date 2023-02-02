using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.net.Microsoft.DependencyInjections;
using StarDust.Demo.AMCP.netcore;

namespace StarDust.CasparCG.AMCP.net.ClientTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services
                .AddCasparCG()
                .AddSingleton<Executor>()
                .BuildServiceProvider()
                .GetService<Executor>()
                .Execute();
        }
    }
}