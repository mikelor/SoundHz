using System;
using SoundHz.SoundBoard.ViewModels;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.Views;

/// <summary>
///     Provides the user interface for editing sound definitions.
/// </summary>
public partial class EditSoundPage : ContentPage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EditSoundPage"/> class.
    /// </summary>
    /// <param name="viewModel">The associated view model.</param>
    public EditSoundPage(EditSoundViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
