using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp.Models;

public class Produit
{

    public int IdProduit { get; set; }
    public string NomProduit { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string NomPhoto { get; set; } = null!;
    public string UriPhoto { get; set; } = null!;
    public int? IdTypeProduit { get; set; }
    public int? IdMarque { get; set; }

    public int? StockReel { get; set; }

    public int StockMin { get; set; }

    public int StockMax { get; set; }

    public virtual Marque? MarqueNavigation { get; set; }

    public virtual TypeProduit? TypeProduitNavigation { get; set; }

    private bool Equals(Produit other)
    {
        return NomProduit == other.NomProduit;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Produit)obj);
    }
}