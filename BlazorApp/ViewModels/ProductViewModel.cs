using BlazorApp.DTO;
using BlazorApp.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlazorApp.ViewModels;

public sealed partial class ProductsViewModel : ObservableObject
{
    private readonly IService<ProduitDto> _produitService;

    public ProductsViewModel(IService<ProduitDto> produitService)
    {
        _produitService = produitService;
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private List<ProduitDto> _produits = new();

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            Produits = (await _produitService.GetAllAsync() ?? new List<ProduitDto>()).ToList();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddDataAsync(ProduitDto produit)
    {
        await _produitService.AddAsync(produit);
    }

    public async Task EditDataAsync(ProduitDto produit)
    {
        await _produitService.UpdateAsync(produit);
    }

    public async Task RemoveDataAsync(int id)
    {
        await _produitService.DeleteAsync(id);
    }
}
