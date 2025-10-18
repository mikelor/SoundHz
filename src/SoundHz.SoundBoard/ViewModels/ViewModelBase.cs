using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoundHz.SoundBoard.ViewModels;

/// <summary>
///     Provides a base implementation of <see cref="INotifyPropertyChanged"/> for view models.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
