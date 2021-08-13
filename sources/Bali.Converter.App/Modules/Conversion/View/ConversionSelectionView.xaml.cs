namespace Bali.Converter.App.Modules.Conversion.View
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Bali.Converter.App.Modules.Conversion.ViewModel;

    public partial class ConversionSelectionView : UserControl
    {
        public ConversionSelectionView()
        {
            this.InitializeComponent();
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Any())
            {
                var context = (ConversionSelectionViewModel)this.DataContext;
                context.HandleDrop(files.First());
            }
        }
    }
}
