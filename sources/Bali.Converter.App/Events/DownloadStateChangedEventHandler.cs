namespace Bali.Converter.App.Events
{
    using Bali.Converter.App.Annotations;

    public delegate void DownloadStateChangedEventHandler([CanBeNull] object sender, DownloadStateChangedEventArgs e);
}
