using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp.Models;

public class Marque
{

    public int IdMarque { get; set; }
    public string NomMarque { get; set; } = null!;
    public virtual ICollection<Produit> Produits { get; set; } = null!;
}