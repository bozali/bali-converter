namespace Bali.Converter.App.Events
{
    using Bali.Converter.App.Annotations;

    public delegate void DownloadProgressChangedEventHandler([CanBeNull] object sender, DownloadProgressChangedEventArgs e);
}
