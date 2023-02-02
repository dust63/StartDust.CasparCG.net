using StarDusrt.CasparCG.net.Demo.WPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using Unity.Lifetime;
using StarDust.CasparCG.net.Device;

namespace StarDusrt.CasparCG.net.Demo.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IServerConnection,ServerConnection>();
            containerRegistry.Register(typeof(IAmcpTcpParser), typeof(AmcpTcpParser));
            containerRegistry.RegisterSingleton<IDataParser, CasparCGDataParser>();
            containerRegistry.Register(typeof(IAMCPProtocolParser), typeof(AMCPProtocolParser));
            containerRegistry.Register(typeof(ICasparDevice), typeof(CasparDevice));
        }
      
    }
}
