using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace StarDust.CasparCG.net.RestApi.Data;

/// <summary>
/// Used to configure IoC for data layer
/// </summary>
public static class DatabaseIoc
{
    /// <summary>
    /// Add database provider
    /// </summary>
    /// <param name="services">services collection to configure</param>
    /// <param name="configuration">acces point to the configuration</param>
    /// <returns></returns>
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOrmLiteConnectioFactory(configuration);
        services.AddHostedService<DatabaseSchemaGenerator>();
        return services;
    }

    /// <summary>
    /// Configure orm lite connection factory. Allow user to set different type of database provider
    /// </summary>
    /// <param name="services">services collection to configure</param>
    /// <param name="configuration">acces point to the configuration (used to get connection string and dbtype configuration)</param>
    private static void ConfigureOrmLiteConnectioFactory(this IServiceCollection services, IConfiguration configuration)
    {
        var dbType = configuration.GetValue<DbType>("DbType");
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            dbType = DbType.Sqlite;
            connectionString = "localDB.db";
        }  

        switch (dbType)
        {
            case DbType.Sqlite:
                services.AddSingleton<IDbConnectionFactory>(x =>
                    new OrmLiteConnectionFactory(connectionString, SqliteDialect.Provider));
                break;
            case DbType.Postgres:
                services.AddSingleton<IDbConnectionFactory>(x =>
                    new OrmLiteConnectionFactory(connectionString, PostgreSqlDialect.Provider));
                break;
            case DbType.SqlServer:
                services.AddSingleton<IDbConnectionFactory>(x =>
                    new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));
                break;
            default:
                throw new InvalidOperationException($"Can't configure OrmLite with the dbType: {dbType}");
        }
    }
}
