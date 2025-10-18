using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.ViewModels;
using SoundHz.SoundBoard.Views;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides navigation orchestration between application pages.
/// </summary>
public interface INavigationService
{
    /// <summary>
    ///     Navigates to the sound board details page for the supplied definition.
    /// </summary>
    /// <param name="viewModel">The target view model.</param>
    /// <returns>An awaitable task.</returns>
    Task NavigateToSoundBoardAsync(SoundBoardViewModel viewModel);

    /// <summary>
    ///     Navigates to the sound editor view.
    /// </summary>
    /// <param name="viewModel">The view model to use for editing.</param>
    /// <returns>An awaitable task.</returns>
    Task NavigateToEditSoundAsync(EditSoundViewModel viewModel);

    /// <summary>
    ///     Returns to the previous page.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    Task GoBackAsync();
}
