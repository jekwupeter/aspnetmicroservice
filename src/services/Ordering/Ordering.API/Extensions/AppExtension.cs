using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions
{
    public static class AppExtension
    {
        public static void MigrateDatabase<T>(this WebApplication app,
            Action<T,IServiceProvider> seeder,
            int? retry =0) where T : DbContext
        {
            int retryForAvailability = retry.Value;

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var log = services.GetRequiredService<ILogger<T>>();
                var context = services.GetService<T>();

                try 
                {
                    log.LogInformation("Migrating db with associated context {dbctx}", typeof(T).Name);

                    InvokeSeeder(seeder, context, services);

                    log.LogInformation("Migrated db with associated context {dbctx}", typeof(T).Name);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error occured while migrating the db used on context {dbctx}", typeof(T).Name);
                       
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<T>(app, seeder, retryForAvailability);
                    }
                }
                
                // shouldve return host because of method chaining -  to be able to add .run() in dotnet 5 program.cs 
                //return host;
            }
        }

        private static void InvokeSeeder<T>(Action<T, IServiceProvider> seeder,
        T context,
        IServiceProvider services) where T : DbContext
        {
            // migrate
            context.Database.Migrate();
            
            // seed db based on the context : dbcontext and service provider needed from preseed class 
            seeder(context, services);
        }
    }
}
