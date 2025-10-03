namespace BlazorApp.DTO;

public class ProduitDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string Description { get; set; } = "";
    public string NomPhoto { get; set; } = "";
    public string UriPhoto { get; set; } = "";

    // Pour stocker les IDs des relations
    public int? TypeId { get; set; }
    public int? MarqueId { get; set; }

    public int? StockReel { get; set; }
    public int StockMin { get; set; } = 0;
    public int StockMax { get; set; } = 0;

    // Pour l'affichage uniquement
    public string Type { get; set; } = "";
    public string Marque { get; set; } = "";

    protected bool Equals(ProduitDto other)
    {
        return Nom == other.Nom &&
               Type == other.Type &&
               Marque == other.Marque &&
               Description == other.Description &&
               NomPhoto == other.NomPhoto &&
               UriPhoto == other.UriPhoto &&
               TypeId == other.TypeId &&
               MarqueId == other.MarqueId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ProduitDto)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Nom, Type, Marque, Description, NomPhoto, UriPhoto, TypeId, MarqueId);
    }
}
