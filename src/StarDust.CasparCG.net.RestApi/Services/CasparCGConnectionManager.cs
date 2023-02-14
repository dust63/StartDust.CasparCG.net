using MediatR;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Applications.Queries;
using StarDust.CasparCG.net.RestApi.Exceptions;

namespace StarDust.CasparCG.net.RestApi.Services
{
    public class CasparCGConnectionManager : IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CasparCGConnectionManager>? _logger;
        private bool disposedValue;
        private readonly Dictionary<Guid, ICasparDevice> _servers = new();

        public CasparCGConnectionManager(IServiceProvider serviceProvider, IMediator mediator)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<CasparCGConnectionManager>>();
        }

        /// <summary>
        /// Is server connection was instantiated
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public bool IsServerInstantiated(Guid serverId)=> _servers.ContainsKey(serverId);

        /// <summary>
        /// Server index accesor
        /// </summary>
        /// <returns></returns>
        public Task<ICasparDevice?> this[Guid id] => GetorAddServerConnection(id);

        /// <summary>
        /// Get server connection by id
        /// </summary>
        /// <param name="id">identifier of the server</param>
        /// <returns></returns>
        public async Task<ICasparDevice?> GetorAddServerConnection(Guid id)
        {
            if (_servers.ContainsKey(id))
            {
                var existingConnection = _servers[id];
                if (!existingConnection.IsConnected) existingConnection.Connect();
                return existingConnection;
            }

            var serverEntity = await _mediator.Send(new GetServerByIdQuery(id));
            if (serverEntity == null)
            {
                throw new ServerNotFoundException(id);
            }

            var server = _serviceProvider.GetRequiredService<ICasparDevice>();
            _servers.Add(id, server);
            server.Connect(serverEntity.Hostname);
            return server;
        }

        /// <summary>
        /// Disconnect and remove server
        /// </summary>
        /// <param name="id">id of the CasparCg Server</param>
        public void RemoveServer(Guid id)
        {
            if (!_servers.TryGetValue(id, out var server))
                return;
            server.Disconnect();
            _servers.Remove(id);
        }

        /// <summary>
        /// Used to dispose all connections
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Disconnect all CasparCG Servers
        /// </summary>
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