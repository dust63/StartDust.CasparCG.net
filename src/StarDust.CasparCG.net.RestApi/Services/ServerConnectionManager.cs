using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Services
{
    public class ServerConnectionManager : IDisposable
    {
        private IServiceProvider _serviceProvider;
        private ILogger<ServerConnectionManager>? _logger;
        private bool disposedValue;
        private readonly Dictionary<Guid, ICasparDevice> _servers = new();

        public ServerConnectionManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<ServerConnectionManager>>();
        }

        /// <summary>
        /// Server index accesor
        /// </summary>
        /// <returns></returns>
        public ICasparDevice this[Guid id] => GetServerConnection(id);


        public IEnumerable<CasparCGServer> GetServerList()
        {
            return _servers.Select(kvp => new CasparCGServer(kvp));
        }

        public ICasparDevice GetServerConnection(Guid id)
        {
            if (_servers.ContainsKey(id))
            {
                return _servers[id];
            }

            var server = _serviceProvider.GetRequiredService<ICasparDevice>();
            server.Connect("127.0.0.1");
            _servers.Add(id, server);
            return server;
        }

        public void RemoveServer(Guid id)
        {
            if (!_servers.TryGetValue(id, out var server))
                return;
            server.Disconnect();
            _servers.Remove(id);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }
            
            if (disposing)
            {
                DisconnectAllServers();
            }
            disposedValue = true;
        }

        private void DisconnectAllServers()
        {
            foreach (var server in _servers.Values)
            {
                try
                {
                    server?.Disconnect();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error while disconnecting CasparCGServer:{server}", server.ConnectionSettings.Hostname);
                }
            }
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}