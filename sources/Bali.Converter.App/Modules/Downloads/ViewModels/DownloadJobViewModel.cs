﻿namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System;
    using System.ComponentModel;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.Common.Media;
    using Prism.Commands;
    using Prism.Mvvm;

    public class DownloadJobViewModel : BindableBase
    {
        private readonly DownloadJob job;

        //private MediaTagsViewModel tags;

        private string url;
        private string headerText;
        private string progressText;
        private float progress;
        private bool isSelected;

        public DownloadJobViewModel(DownloadJob job)
        {
            this.job = job;
            this.job.PropertyChanged += this.OnModelPropertyChanged;

            this.HeaderText = this.job.Tags.Title;
            this.ProgressText = "Pending";
            this.Url = this.job.Url;
            this.Id = this.job.Id;
            this.Progress = 0.0f;

            //this.Tags = new MediaTagsViewModel
            //{
            //    Album = job.Tags.Album,
            //    Artist = job.Tags.Artist,
            //    Year = job.Tags.Year,
            //    Copyright = 
            //};

            this.RequestCancelCommand = new DelegateCommand(() => { });
        }

        public DelegateCommand RequestCancelCommand { get; }

        public Guid Id { get; set; }

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

        public string HeaderText
        {
            get => this.headerText;
            set => this.SetProperty(ref this.headerText, value);
        }

        public string ProgressText
        {
            get => this.progressText;
            set => this.SetProperty(ref this.progressText, value);
        }

        public float Progress
        {
            get => this.progress;
            set => this.SetProperty(ref this.progress, value);
        }

        public MediaTags Tags
        {
            get => this.job.Tags;
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ProgressText":
                    this.ProgressText = ((DownloadJob)sender).ProgressText;
                    break;
                case "Progress":
                    this.Progress = ((DownloadJob)sender).Progress;
                    break;
            }
        }
    }
}
