using System.Collections.Generic;

namespace SoundHz.Views;

/// <summary>
/// Presents a form for creating or editing a <see cref="SoundBoard"/> entry.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage, IQueryAttributable
{
        private readonly SoundBoardDetailEntryViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model providing page behavior.</param>
        public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
        {
                InitializeComponent();
                BindingContext = this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
                this.viewModel.PrepareForCreate();
        }

        /// <inheritdoc />
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
                if (query.TryGetValue("Item", out var item) && item is SoundBoard soundBoard)
                {
                        viewModel.PrepareForEdit(soundBoard);
                }
                else
                {
                        viewModel.PrepareForCreate();
                }
        }
}
