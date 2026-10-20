using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;

namespace CatologService.Api.Extensions
{
    public static class HostExtension
    {

        public static IWebHost MigragateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seader) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Migragate db {DbContextName}", typeof(TContext).Name);
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });
                    retry.Execute(() => InvokeSeeder(seader, context, services));
                    logger.LogInformation("Migragate db {DbContextName}", typeof(TContext).Name);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Migragate db {DbContextName}", typeof(TContext).Name);

                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seader, TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();
            seader(context, services);
        }
    }
}
