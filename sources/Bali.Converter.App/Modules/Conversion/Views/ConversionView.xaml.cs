namespace Bali.Converter.App.Modules.Conversion.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Bali.Converter.App.Modules.Conversion.ViewModels;

    public partial class ConversionView : UserControl
    {
        public ConversionView()
        {
            this.InitializeComponent();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Any())
            {
                var context = (ConversionViewModel)this.DataContext;
                context.HandleDrop(files.First());
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
        }
    }
}
