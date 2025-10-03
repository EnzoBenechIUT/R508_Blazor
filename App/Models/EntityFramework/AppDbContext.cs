using Microsoft.EntityFrameworkCore;

namespace App.Models.EntityFramework;

public partial class AppDbContext : DbContext
{
    public DbSet<Produit> Produits { get; set; } = null!;
    public DbSet<Marque> Marques { get; set; } = null!;
    public DbSet<TypeProduit> TypeProduits { get; set; } = null!;

    public AppDbContext()
    {
        // Crée la DB si elle n'existe pas et insère les données
        Database.EnsureCreated();
        SeedData();
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
        SeedData();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=R508;Username=postgres;Password=postgres");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration des clés et relations
        modelBuilder.Entity<Produit>(e =>
        {
            e.HasKey(p => p.IdProduit);

            e.HasOne(p => p.MarqueNavigation)
             .WithMany(m => m.Produits)
             .OnDelete(DeleteBehavior.ClientSetNull)
             .HasConstraintName("FK_produits_marque");

            e.HasOne(p => p.TypeProduitNavigation)
             .WithMany(t => t.Produits)
             .OnDelete(DeleteBehavior.ClientSetNull)
             .HasConstraintName("FK_produits_type_produit");
        });

        modelBuilder.Entity<TypeProduit>(e => e.HasKey(t => t.IdTypeProduit));
        modelBuilder.Entity<Marque>(e => e.HasKey(m => m.IdMarque));

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    private void SeedData()
    {
        // Types de produit
        if (!TypeProduits.Any())
        {
            TypeProduits.AddRange(
                new TypeProduit { IdTypeProduit = 1, NomTypeProduit = "Électroménager" },
                new TypeProduit { IdTypeProduit = 2, NomTypeProduit = "Meuble" },
                new TypeProduit { IdTypeProduit = 3, NomTypeProduit = "Informatique" }
            );
            SaveChanges();
        }

        // Marques
        if (!Marques.Any())
        {
            Marques.AddRange(
                new Marque { IdMarque = 1, NomMarque = "Samsung" },
                new Marque { IdMarque = 2, NomMarque = "Ikea" },
                new Marque { IdMarque = 3, NomMarque = "Apple" }
            );
            SaveChanges();
        }

        // Produits
        if (!Produits.Any())
        {
            Produits.AddRange(
                new Produit
                {
                    IdProduit = 1,
                    NomProduit = "Réfrigérateur",
                    Description = "Réfrigérateur Samsung 300L",
                    NomPhoto = "frigo.jpg",
                    UriPhoto = "/images/frigo.jpg",
                    IdTypeProduit = 1,
                    IdMarque = 1,
                    StockReel = 10,
                    StockMin = 2,
                    StockMax = 20
                },
                new Produit
                {
                    IdProduit = 2,
                    NomProduit = "Chaise",
                    Description = "Chaise Ikea en bois",
                    NomPhoto = "chaise.jpg",
                    UriPhoto = "/images/chaise.jpg",
                    IdTypeProduit = 2,
                    IdMarque = 2,
                    StockReel = 50,
                    StockMin = 5,
                    StockMax = 100
                },
                new Produit
                {
                    IdProduit = 3,
                    NomProduit = "MacBook Pro",
                    Description = "Ordinateur portable Apple 16 pouces",
                    NomPhoto = "macbook.jpg",
                    UriPhoto = "/images/macbook.jpg",
                    IdTypeProduit = 3,
                    IdMarque = 3,
                    StockReel = 5,
                    StockMin = 1,
                    StockMax = 10
                }
            );
            SaveChanges();
        }
    }
}
