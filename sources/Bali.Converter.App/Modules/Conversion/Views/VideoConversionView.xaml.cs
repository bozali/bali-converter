namespace Bali.Converter.App.Modules.Conversion.Views
{
    using System.Windows.Controls;

    using Bali.Converter.App.Modules.Conversion.ViewModels;

    public partial class VideoConversionView : UserControl
    {
        public VideoConversionView()
        {
            this.InitializeComponent();

            this.Media.Volume = 0.0;
            this.Media.Play();

            ((VideoConversionViewModel)this.DataContext).MediaElement = this.Media;
        }
    }
}
