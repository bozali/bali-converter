namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    using Prism.Mvvm;

    public class MediaTagsViewModel : BindableBase
    {
        private string title;
        private string artist;
        private string album;
        private string description;
        private string copyright;
        private int year;
        private ObservableCollection<string> genres;
        private ObservableCollection<string> performers;

        public MediaTagsViewModel()
        {
            this.Genres = new ObservableCollection<string>();
            this.Performers = new ObservableCollection<string>();
        }

        [XmlAttribute]
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        [XmlAttribute]
        public string Artist
        {
            get => this.artist;
            set => this.SetProperty(ref this.artist, value);
        }

        [XmlAttribute]
        public string Album
        {
            get => this.album;
            set => this.SetProperty(ref this.album, value);
        }

        [XmlAttribute]
        public string Description
        {
            get => this.description;
            set => this.SetProperty(ref this.description, value);
        }

        [XmlAttribute]
        public string Copyright
        {
            get => this.copyright;
            set => this.SetProperty(ref this.copyright, value);
        }

        [XmlAttribute]
        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        [XmlElement("Genres")]
        public ObservableCollection<string> Genres
        {
            get => this.genres;
            set => this.SetProperty(ref this.genres, value);
        }

        [XmlElement("Performers")]
        public ObservableCollection<string> Performers
        {
            get => this.performers;
            set => this.SetProperty(ref this.performers, value);
        }
    }
}
