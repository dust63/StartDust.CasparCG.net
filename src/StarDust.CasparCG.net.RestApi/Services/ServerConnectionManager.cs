using MediatR;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.RestApi.Applications.Queries;
using StarDust.CasparCG.net.RestApi.Exceptions;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Services
{
    public class ServerConnectionManager : IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServerConnectionManager>? _logger;
        private bool disposedValue;
        private readonly Dictionary<Guid, ICasparDevice> _servers = new();

        public ServerConnectionManager(IServiceProvider serviceProvider, IMediator mediator)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<ServerConnectionManager>>();
        }

        /// <summary>
        /// Server index accesor
        /// </summary>
        /// <returns></returns>
        public Task<ICasparDevice?> this[Guid id] => GetorAddServerConnection(id);

        /// <summary>
        /// Get the list of caspar CG server instantiate
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CasparCGServer> GetServerList()
        {
            return _servers.Select(kvp => new CasparCGServer(kvp));
        }

        /// <summary>
        /// Get server connection by id
        /// </summary>
        /// <param name="id">identifier of the server</param>
        /// <returns></returns>
        public async Task<ICasparDevice?> GetorAddServerConnection(Guid id)
        {
            if (_servers.ContainsKey(id))
            {
                return _servers[id];
            }

            var serverEntity = await _mediator.Send(new GetServerByIdQuery(id));
            if (serverEntity == null)
            {
                throw new ServerNotFoundException(id);
            }


            var server = _serviceProvider.GetRequiredService<ICasparDevice>();
            server.Connect(serverEntity.Hostname);
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