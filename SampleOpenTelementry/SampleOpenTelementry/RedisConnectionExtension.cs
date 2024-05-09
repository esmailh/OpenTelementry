using StackExchange.Redis;

namespace SampleOpenTelementry
{
    public static class RedisConnectionExtension
    {

        public static async Task AddRedisConnectionProvider(this IServiceCollection serviceCollections, IConfiguration configuration)
        {

            var redisSection = configuration.GetSection("Redis");
            serviceCollections.AddOptions<RedisOptions>()
                .Bind(redisSection, options => options.ErrorOnUnknownConfiguration = true)
                .Validate(settings =>
                    string.IsNullOrWhiteSpace(settings.Password) is false);

            var options = redisSection.Get<RedisOptions>();
            var config = new ConfigurationOptions()
            {
                Ssl = options!.Ssl,
                Password = options.Password,
                ConnectRetry = options.ConnectRetry,
                ConnectTimeout = options.ConnectTimeout,
                DefaultDatabase = options.DefaultDatabase,
                AllowAdmin = options.AllowAdmin,
                ClientName = $"{Environment.MachineName}",
                AbortOnConnectFail=false
            };

            options.EndPoints.ForEach(host => config.EndPoints.Add(host));
            var connection = await ConnectionMultiplexer.ConnectAsync(config);

            serviceCollections.AddSingleton<IConnectionMultiplexer>(connection);
        }

    }
}
