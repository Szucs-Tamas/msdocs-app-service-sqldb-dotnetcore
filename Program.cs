using Microsoft.EntityFrameworkCore;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<MovieCatalogDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("JL7ID0")));
            builder.Services.AddScoped(typeof(IMovieCatalogDataService), typeof(MovieCatalogDataService));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
