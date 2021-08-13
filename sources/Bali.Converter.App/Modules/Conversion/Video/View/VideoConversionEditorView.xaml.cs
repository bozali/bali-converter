namespace Bali.Converter.App.Modules.Conversion.Video.View
{
    using System.Windows.Controls;

    using Bali.Converter.App.Modules.Conversion.Video.ViewModel;

    public partial class VideoConversionEditorView : UserControl
    {
        public VideoConversionEditorView()
        {
            this.InitializeComponent();

            this.Media.Volume = 0.0;
            this.Media.Play();

            ((VideoConversionEditorViewModel)this.DataContext).MediaElement = this.Media;
        }
    }
}
