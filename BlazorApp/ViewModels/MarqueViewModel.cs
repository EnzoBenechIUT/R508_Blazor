using BlazorApp.DTO;
using BlazorApp.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlazorApp.ViewModels;

public partial class MarqueViewModel : ObservableObject
{
    private readonly IService<MarqueDto> _marqueService;

    public MarqueViewModel(IService<MarqueDto> marqueService)
    {
        _marqueService = marqueService;
    }

    [ObservableProperty]
    private IEnumerable<MarqueDto> _marques = Array.Empty<MarqueDto>();

    [ObservableProperty]
    private bool _isLoading;

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            Marques = (await _marqueService.GetAllAsync() ?? new List<MarqueDto>()).ToList();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
