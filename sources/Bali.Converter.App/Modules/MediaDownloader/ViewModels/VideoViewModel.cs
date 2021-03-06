namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using Prism.Mvvm;

    public class VideoViewModel : BindableBase
    {
        private MediaTagsViewModel tags;
        private byte[] thumbnailData;
        private string format;
        private string url;
        private bool isSelected;
        private bool isExpanded;

        public MediaTagsViewModel Tags
        {
            get => this.tags;
            set => this.SetProperty(ref this.tags, value);
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetProperty(ref this.isSelected, value);
        }

        public string Url
        {
            get => this.url;
            set => this.SetProperty(ref this.url, value);
        }

        public string Format
        {
            get => this.format;
            set => this.SetProperty(ref this.format, value);
        }

        public byte[] ThumbnailData
        {
            get => this.thumbnailData;
            set => this.SetProperty(ref this.thumbnailData, value);
        }

        public string ThumbnailPath { get; set; }
    }
}
