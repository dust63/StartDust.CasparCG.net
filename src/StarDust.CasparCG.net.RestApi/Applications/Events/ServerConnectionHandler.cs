using MediatR;
using StarDust.CasparCG.net.RestApi.Services;

namespace StarDust.CasparCG.net.RestApi.Applications.Events
{
    public class ServerConnectionHandler : INotificationHandler<CasparCGServerCreated>, INotificationHandler<CasparCGServerDeleted>
    {
        private readonly CasparCGConnectionManager _serverConnectionManager;

        public ServerConnectionHandler(CasparCGConnectionManager serverConnectionManager)
        {
            _serverConnectionManager = serverConnectionManager;
        }
        public Task Handle(CasparCGServerCreated notification, CancellationToken cancellationToken)
        {
            return _serverConnectionManager.GetorAddServerConnection(notification.serverId);
        }

        public Task Handle(CasparCGServerDeleted notification, CancellationToken cancellationToken)
        {
            _serverConnectionManager.RemoveServer(notification.serverId);
            return Task.CompletedTask;
        }
    }
}