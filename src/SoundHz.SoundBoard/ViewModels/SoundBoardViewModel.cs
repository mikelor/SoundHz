using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using SoundHz.SoundBoard.CommandHandling;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Resources.Localization;
using SoundHz.SoundBoard.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.ViewModels;

/// <summary>
///     Represents the view model for the sound board details page.
/// </summary>
public sealed class SoundBoardViewModel(ISoundPlaybackService playbackService, INavigationService navigationService, IServiceProvider serviceProvider, CommandHandler<SoundBoardConfiguration> saveCommandHandler, ILogger<SoundBoardViewModel> logger) : ViewModelBase
{
    private readonly ILogger<SoundBoardViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly INavigationService _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    private readonly CommandHandler<SoundBoardConfiguration> _saveCommandHandler = saveCommandHandler ?? throw new ArgumentNullException(nameof(saveCommandHandler));
    private readonly ISoundPlaybackService _playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private Command<SoundDefinition?>? _playSoundCommand;
    private Command? _addSoundCommand;
    private Command<SoundDefinition?>? _editSoundCommand;
    private SoundBoardDefinition? _soundBoard;
    private SoundBoardConfiguration? _configuration;
    private ObservableCollection<SoundDefinition>? _sounds;

    /// <summary>
    ///     Gets the title of the sound board.
    /// </summary>
    public string Title => _soundBoard?.Name ?? string.Empty;

    /// <summary>
    ///     Gets the description of the sound board.
    /// </summary>
    public string Description => _soundBoard?.Description ?? string.Empty;

    /// <summary>
    ///     Gets the sounds associated with the board.
    /// </summary>
    public ObservableCollection<SoundDefinition> Sounds => _sounds ??= _soundBoard?.Sounds ?? new ObservableCollection<SoundDefinition>();

    /// <summary>
    ///     Gets the command that plays a sound definition.
    /// </summary>
    public ICommand PlaySoundCommand => _playSoundCommand ??= new Command<SoundDefinition?>(async sound => await PlaySoundAsync(sound).ConfigureAwait(false));

    /// <summary>
    ///     Gets the command that begins creation of a new sound definition.
    /// </summary>
    public ICommand AddSoundCommand => _addSoundCommand ??= new Command(async () => await AddSoundAsync().ConfigureAwait(false));

    /// <summary>
    ///     Gets the command that edits an existing sound definition.
    /// </summary>
    public ICommand EditSoundCommand => _editSoundCommand ??= new Command<SoundDefinition?>(async sound => await EditSoundAsync(sound).ConfigureAwait(false));

    /// <summary>
    ///     Initializes the view model for the specified sound board.
    /// </summary>
    /// <param name="soundBoard">The target sound board.</param>
    /// <param name="configuration">The owning configuration.</param>
    public void Initialize(SoundBoardDefinition soundBoard, SoundBoardConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(soundBoard);
        ArgumentNullException.ThrowIfNull(configuration);
        _soundBoard = soundBoard;
        _configuration = configuration;
        _sounds = soundBoard.Sounds;
        RaisePropertyChanged(nameof(Title));
        RaisePropertyChanged(nameof(Description));
        RaisePropertyChanged(nameof(Sounds));
    }

    /// <summary>
    ///     Persists the sound board configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task PersistAsync(CancellationToken cancellationToken)
    {
        if (_configuration is null)
        {
            return;
        }

        await _saveCommandHandler.ExecuteAsync(_configuration, cancellationToken).ConfigureAwait(false);
    }

    private async Task PlaySoundAsync(SoundDefinition? sound)
    {
        if (sound is null)
        {
            return;
        }

        _logger.LogInformation(LogMessagesResourceManager.Instance.GetString("SoundPlayed", CultureInfo.CurrentCulture), sound.Name);
        await _playbackService.PlayAsync(sound, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task AddSoundAsync()
    {
        if (_soundBoard is null)
        {
            return;
        }

        var editViewModel = _serviceProvider.GetRequiredService<EditSoundViewModel>();
        editViewModel.PrepareForCreate(_soundBoard);
        editViewModel.SoundSaved += HandleSoundUpdated;
        editViewModel.SoundDeleted += HandleSoundDeleted;
        await _navigationService.NavigateToEditSoundAsync(editViewModel).ConfigureAwait(false);
    }

    private async Task EditSoundAsync(SoundDefinition? sound)
    {
        if (_soundBoard is null || sound is null)
        {
            return;
        }

        var editViewModel = _serviceProvider.GetRequiredService<EditSoundViewModel>();
        editViewModel.PrepareForEdit(_soundBoard, sound);
        editViewModel.SoundSaved += HandleSoundUpdated;
        editViewModel.SoundDeleted += HandleSoundDeleted;
        await _navigationService.NavigateToEditSoundAsync(editViewModel).ConfigureAwait(false);
    }

    private void HandleSoundUpdated(object? sender, SoundDefinition e)
    {
        if (sender is EditSoundViewModel editViewModel)
        {
            editViewModel.SoundSaved -= HandleSoundUpdated;
            editViewModel.SoundDeleted -= HandleSoundDeleted;
        }

        RaisePropertyChanged(nameof(Sounds));
    }

    private void HandleSoundDeleted(object? sender, SoundDefinition e)
    {
        if (sender is EditSoundViewModel editViewModel)
        {
            editViewModel.SoundSaved -= HandleSoundUpdated;
            editViewModel.SoundDeleted -= HandleSoundDeleted;
        }

        Sounds.Remove(e);
        RaisePropertyChanged(nameof(Sounds));
    }
}
