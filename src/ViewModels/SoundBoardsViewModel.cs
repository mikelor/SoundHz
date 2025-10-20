namespace SoundHz.ViewModels;

public partial class SoundBoardsViewModel(SampleDataService service) : BaseViewModel
{
    [ObservableProperty]
	public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SampleItem>? Items { get; set; } = [];

    [RelayCommand]
	private async Task OnRefreshing()
	{
		IsRefreshing = true;

		try
		{
			await LoadDataAsync();
		}
		finally
		{
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	public async Task LoadMore()
	{
		if (Items is null)
		{
			return;
		}

		var moreItems = await service.GetItems();

		foreach (var item in moreItems)
		{
			Items.Add(item);
		}
	}

	public async Task LoadDataAsync()
	{
		Items = new ObservableCollection<SampleItem>(await service.GetItems());
	}

	[RelayCommand]
	private async Task GoToDetails(SampleItem item)
	{
		await Shell.Current.GoToAsync(nameof(SoundBoardsDetailPage), true, new Dictionary<string, object>
		{
			{ "Item", item }
		});
	}
}
