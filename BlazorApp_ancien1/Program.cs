using BlazorApp.Components;
using BlazorApp.Models;
using BlazorApp.Service;
using BlazorApp.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
 
namespace BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped<IService<Produit>, WSService>(_ => new WSService());
            builder.Services.AddScoped<GetProductsViewModel>();
            builder.Services.AddBlazorBootstrap();
            var app = builder.Build();
            await builder.Build().RunAsync();
        }
    }

}      