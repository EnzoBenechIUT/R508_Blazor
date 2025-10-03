using BlazorApp.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.ViewModels;
using BlazorApp.DTO;

namespace BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddScoped<WSServiceProduit>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7008/api/") });
            builder.Services.AddScoped<IService<ProduitDto>, WSServiceProduit>();
            builder.Services.AddScoped<IService<MarqueDto>, WSServiceMarque>();
            builder.Services.AddScoped<IService<TypeProduitDto>, WSServiceTypeProduit>();
            builder.Services.AddScoped<MarqueViewModel>();
            builder.Services.AddScoped<TypeProduitViewModel>();
            builder.Services.AddScoped<ProductsViewModel>();
            await builder.Build().RunAsync();
        }
    }
}
