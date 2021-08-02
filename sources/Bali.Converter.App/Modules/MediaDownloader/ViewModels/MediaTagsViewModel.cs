namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System.ComponentModel;

    using Prism.Mvvm;

    public class MediaTagsViewModel : BindableBase, IDataErrorInfo
    {
        private string title;
        private string artist;
        private string album;
        private string comment;
        private string copyright;
        private int year;
        private string albumArtists;
        private string composers;
        private string genres;
        private string performers;

        private string collectionInput;
        private byte[] thumbnailData;

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public string Artist
        {
            get => this.artist;
            set => this.SetProperty(ref this.artist, value);
        }

        public string Album
        {
            get => this.album;
            set => this.SetProperty(ref this.album, value);
        }

        public string Comment
        {
            get => this.comment;
            set => this.SetProperty(ref this.comment, value);
        }

        public string Copyright
        {
            get => this.copyright;
            set => this.SetProperty(ref this.copyright, value);
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public string AlbumArtists
        {
            get => this.albumArtists;
            set => this.SetProperty(ref this.albumArtists, value);
        }

        public string Genres
        {
            get => this.genres;
            set => this.SetProperty(ref this.genres, value);
        }

        public string Performers
        {
            get => this.performers;
            set => this.SetProperty(ref this.performers, value);
        }

        public string Composers
        {
            get => this.composers;
            set => this.SetProperty(ref this.composers, value);
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(this.Title) && string.IsNullOrEmpty(this.Title))
                {
                    return "Title cannot be empty.";
                }

                return null;
            }
        }
    }
}
