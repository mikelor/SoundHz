using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SoundHz.SoundBoard.ViewModels;

/// <summary>
///     Represents the view model for creating or editing a sound definition.
/// </summary>
public sealed class EditSoundViewModel(INavigationService navigationService, ILogger<EditSoundViewModel> logger) : ViewModelBase
{
    private readonly ILogger<EditSoundViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly INavigationService _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

    private Command? _pickFileCommand;
    private Command? _saveCommand;
    private Command? _cancelCommand;
    private Command? _deleteCommand;
    private SoundBoardDefinition? _parentBoard;
    private SoundDefinition? _sound;
    private bool _isNew;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _filePath = string.Empty;

    /// <summary>
    ///     Occurs when a sound definition has been added or updated.
    /// </summary>
    public event EventHandler<SoundDefinition>? SoundSaved;

    /// <summary>
    ///     Occurs when a sound definition has been deleted.
    /// </summary>
    public event EventHandler<SoundDefinition>? SoundDeleted;

    /// <summary>
    ///     Gets or sets the sound name.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                RaisePropertyChanged();
                _saveCommand?.ChangeCanExecute();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the sound description.
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the file path of the sound.
    /// </summary>
    public string FilePath
    {
        get => _filePath;
        set
        {
            if (_filePath != value)
            {
                _filePath = value;
                RaisePropertyChanged();
                _saveCommand?.ChangeCanExecute();
            }
        }
    }

    /// <summary>
    ///     Gets the command used to invoke the file picker.
    /// </summary>
    public ICommand PickFileCommand => _pickFileCommand ??= new Command(async () => await PickFileAsync().ConfigureAwait(false));

    /// <summary>
    ///     Gets the command used to save the current sound.
    /// </summary>
    public ICommand SaveCommand => _saveCommand ??= new Command(async () => await SaveAsync().ConfigureAwait(false), CanSave);

    /// <summary>
    ///     Gets the command used to cancel editing.
    /// </summary>
    public ICommand CancelCommand => _cancelCommand ??= new Command(async () => await _navigationService.GoBackAsync().ConfigureAwait(false));

    /// <summary>
    ///     Gets the command used to delete the current sound.
    /// </summary>
    public ICommand DeleteCommand => _deleteCommand ??= new Command(async () => await DeleteAsync().ConfigureAwait(false), () => CanDelete);

    /// <summary>
    ///     Gets a value indicating whether the delete action is available.
    /// </summary>
    public bool CanDelete => !_isNew;

    /// <summary>
    ///     Prepares the view model for creating a sound definition.
    /// </summary>
    /// <param name="parentBoard">The parent sound board.</param>
    public void PrepareForCreate(SoundBoardDefinition parentBoard)
    {
        ArgumentNullException.ThrowIfNull(parentBoard);
        _parentBoard = parentBoard;
        _sound = new SoundDefinition();
        _isNew = true;
        Name = string.Empty;
        Description = string.Empty;
        FilePath = string.Empty;
        _deleteCommand?.ChangeCanExecute();
        _saveCommand?.ChangeCanExecute();
        RaisePropertyChanged(nameof(CanDelete));
    }

    /// <summary>
    ///     Prepares the view model for editing a sound definition.
    /// </summary>
    /// <param name="parentBoard">The parent sound board.</param>
    /// <param name="sound">The sound being edited.</param>
    public void PrepareForEdit(SoundBoardDefinition parentBoard, SoundDefinition sound)
    {
        ArgumentNullException.ThrowIfNull(parentBoard);
        ArgumentNullException.ThrowIfNull(sound);
        _parentBoard = parentBoard;
        _sound = sound;
        _isNew = false;
        Name = sound.Name;
        Description = sound.Description;
        FilePath = sound.FilePath;
        _deleteCommand?.ChangeCanExecute();
        _saveCommand?.ChangeCanExecute();
        RaisePropertyChanged(nameof(CanDelete));
    }

    private async Task PickFileAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select audio file"
            }).ConfigureAwait(false);

            if (result is not null)
            {
                FilePath = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File picking failed");
        }
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(FilePath);
    }

    private async Task SaveAsync()
    {
        if (_parentBoard is null || _sound is null)
        {
            return;
        }

        _sound.Name = Name;
        _sound.Description = Description;
        _sound.FilePath = FilePath;

        if (_isNew)
        {
            _parentBoard.Sounds.Add(_sound);
        }

        SoundSaved?.Invoke(this, _sound);
        await _navigationService.GoBackAsync().ConfigureAwait(false);
    }

    private async Task DeleteAsync()
    {
        if (_parentBoard is null || _sound is null)
        {
            return;
        }

        _parentBoard.Sounds.Remove(_sound);
        SoundDeleted?.Invoke(this, _sound);
        await _navigationService.GoBackAsync().ConfigureAwait(false);
    }
}
