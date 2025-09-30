using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp.Models;

public class TypeProduit
{
    public int IdTypeProduit { get; set; }
    public string NomTypeProduit { get; set; } = null!;
    public virtual ICollection<Produit> Produits { get; set; } = null!;
}