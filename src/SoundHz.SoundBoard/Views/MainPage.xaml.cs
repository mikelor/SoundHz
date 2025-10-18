using System;
using System.Linq;
using System.Threading;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.Views;

/// <summary>
///     Represents the main landing page for managing sound boards.
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private CancellationTokenSource? _initializationTokenSource;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    /// <param name="viewModel">The associated view model.</param>
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        BindingContext = _viewModel;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _initializationTokenSource?.Cancel();
        _initializationTokenSource = new CancellationTokenSource();
        await _viewModel.InitializeAsync(_initializationTokenSource.Token).ConfigureAwait(false);
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedBoard = e.CurrentSelection?.OfType<SoundBoardDefinition>().FirstOrDefault();
        if (selectedBoard is not null && _viewModel.SoundBoardSelectedCommand.CanExecute(selectedBoard))
        {
            _viewModel.SoundBoardSelectedCommand.Execute(selectedBoard);
        }

        if (sender is CollectionView collectionView)
        {
            collectionView.SelectedItem = null;
        }
    }
}
