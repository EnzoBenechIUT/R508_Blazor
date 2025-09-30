using BlazorApp.Pages;
using BlazorApp.Models;
using BlazorApp.Service;
using CommunityToolkit.Mvvm.ComponentModel;
namespace BlazorApp.ViewModels;
internal sealed partial class GetProductsViewModel : ObservableObject
{
    private readonly WSService _produitService;
    public GetProductsViewModel(WSService productService)
    {
        _produitService = productService;
    }
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private IEnumerable<Produit> _produits = Array.Empty<Produit>();
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2)); // simulate loading
            Produits = await _produitService.GetAllAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}