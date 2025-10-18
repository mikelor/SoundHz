using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Resources.Localization;
using SoundHz.SoundBoard.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.ViewModels;

/// <summary>
///     Represents the view model for the main sound board listing page.
/// </summary>
public sealed class MainViewModel(ISoundBoardRepository repository, ISoundBoardStateStore stateStore, INavigationService navigationService, IServiceProvider serviceProvider, ILogger<MainViewModel> logger) : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly INavigationService _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    private readonly ISoundBoardRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ISoundBoardStateStore _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));

    private Command? _addSoundBoardCommand;
    private Command<SoundBoardDefinition?>? _selectSoundBoardCommand;
    private bool _isInitialized;

    /// <summary>
    ///     Gets the collection of sound boards displayed to the user.
    /// </summary>
    public ObservableCollection<SoundBoardDefinition> SoundBoards => _stateStore.Configuration.SoundBoards;

    /// <summary>
    ///     Gets the command that creates a new sound board.
    /// </summary>
    public ICommand AddSoundBoardCommand => _addSoundBoardCommand ??= new Command(async () => await AddSoundBoardAsync().ConfigureAwait(false));

    /// <summary>
    ///     Gets the command executed when the user selects a sound board from the list.
    /// </summary>
    public ICommand SoundBoardSelectedCommand => _selectSoundBoardCommand ??= new Command<SoundBoardDefinition?>(async board => await NavigateToBoardAsync(board).ConfigureAwait(false));

    /// <summary>
    ///     Initializes the view model by loading the persisted configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_isInitialized)
        {
            return;
        }

        var configuration = await _repository.LoadAsync(cancellationToken).ConfigureAwait(false);
        _stateStore.Configuration = configuration;
        _logger.LogInformation(LogMessagesResourceManager.Instance.GetString("SoundBoardLoaded", CultureInfo.CurrentCulture), nameof(MainViewModel));
        _isInitialized = true;
        RaisePropertyChanged(nameof(SoundBoards));
    }

    private async Task AddSoundBoardAsync()
    {
        await InitializeAsync(CancellationToken.None).ConfigureAwait(false);
        var newBoard = new SoundBoardDefinition
        {
            Name = string.Format(CultureInfo.CurrentCulture, "Sound Board {0}", SoundBoards.Count + 1),
            Description = "New sound board"
        };
        _stateStore.Configuration.SoundBoards.Add(newBoard);
        await NavigateToBoardAsync(newBoard).ConfigureAwait(false);
    }

    private async Task NavigateToBoardAsync(SoundBoardDefinition? board)
    {
        if (board is null)
        {
            return;
        }

        var soundBoardViewModel = _serviceProvider.GetRequiredService<SoundBoardViewModel>();
        soundBoardViewModel.Initialize(board, _stateStore.Configuration);
        await _navigationService.NavigateToSoundBoardAsync(soundBoardViewModel).ConfigureAwait(false);
    }
}
