using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using StarDust.CasparCG.net.RestApi.Models;

namespace StarDust.CasparCG.net.RestApi.Data
{
    /// <summary>
    /// Service in charge to initialize table if not exists in db
    /// </summary>
    public class DatabaseSchemaGenerator : IHostedService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public DatabaseSchemaGenerator(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CreateTablesIfNotExists();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void CreateTablesIfNotExists()
        {
            using var db = _connectionFactory.OpenDbConnection();
            
            if(db.CreateTableIfNotExists<CasparCGServer>())
                db.Insert<CasparCGServer>(new CasparCGServer("127.0.0.1", "Local server"));
        }
    }
}