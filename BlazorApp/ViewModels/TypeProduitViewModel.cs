using BlazorApp.DTO;
using BlazorApp.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlazorApp.ViewModels;

public partial class TypeProduitViewModel : ObservableObject
{
    private readonly IService<TypeProduitDto> _typeService;

    public TypeProduitViewModel(IService<TypeProduitDto> typeService)
    {
        _typeService = typeService;
    }

    [ObservableProperty]
    private IEnumerable<TypeProduitDto> _types = Array.Empty<TypeProduitDto>();

    [ObservableProperty]
    private bool _isLoading;

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            Types = (await _typeService.GetAllAsync() ?? new List<TypeProduitDto>()).ToList();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
