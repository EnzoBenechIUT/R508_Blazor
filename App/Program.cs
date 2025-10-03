using App.DTO;
using App.Mapper;
using App.Mapping;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;

namespace App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<IDataRepository<Produit>, ProductManager>();
        builder.Services.AddScoped<IDataRepository<Marque>, MarqueManager>();
        builder.Services.AddScoped<IMapper<Produit, ProduitDto>, AutoMapperAdapter<Produit, ProduitDto>>();
        builder.Services.AddScoped<IMapper<TypeProduit, TypeProduitDto>, AutoMapperAdapter<TypeProduit, TypeProduitDto>>();
        builder.Services.AddScoped<IMapper<Marque, MarqueDto>, AutoMapperAdapter<Marque, MarqueDto>>();
        builder.Services.AddScoped<IDataRepository<TypeProduit>, TypeProduitManager>();
        builder.Services.AddAutoMapper(typeof(MappingProfile));


        builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient", policy =>
            {
                policy
                    .WithOrigins("https://localhost:7082") 
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors("AllowBlazorClient");


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}