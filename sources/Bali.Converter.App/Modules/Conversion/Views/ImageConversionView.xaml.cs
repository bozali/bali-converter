namespace Bali.Converter.App.Modules.Conversion.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class ImageConversionView : UserControl
    {
        public ImageConversionView()
        {
            this.InitializeComponent();
        }

        private void OnCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var canvas = (Canvas)sender;

            var transform = (ScaleTransform)this.EditingImage.Transform;

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;

        }
    }
}
