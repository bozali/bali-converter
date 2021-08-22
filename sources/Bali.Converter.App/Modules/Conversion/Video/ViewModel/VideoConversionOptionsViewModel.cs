namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using System;

    using Prism.Mvvm;

    // TODO For the Max- and MinVideoLengthTime properties we need a maybe a better solutions. We are creating always a new TimeSpan object if the time changes.
    public class VideoConversionOptionsViewModel : BindableBase
    {
        private int quality;
        private double minVideoLength;
        private double maxVideoLength;

        private TimeSpan minVideoLengthTime;
        private TimeSpan maxVideoLengthTime;

        public int Quality
        {
            get => this.quality;
            set => this.SetProperty(ref this.quality, value);
        }

        public double MinVideoLength
        {
            get => this.minVideoLength;
            set
            {
                if (this.SetProperty(ref this.minVideoLength, value))
                {
                    this.MinVideoLengthTime = TimeSpan.FromSeconds(this.MinVideoLength);
                }
            }
        }

        public double MaxVideoLength
        {
            get => this.maxVideoLength;
            set
            {
                if (this.SetProperty(ref this.maxVideoLength, value))
                {
                    this.MaxVideoLengthTime = TimeSpan.FromSeconds(this.MaxVideoLength);
                }
            }
        }

        public TimeSpan MinVideoLengthTime
        {
            get => this.minVideoLengthTime;
            set => this.SetProperty(ref this.minVideoLengthTime, value);
        }

        public TimeSpan MaxVideoLengthTime
        {
            get => this.maxVideoLengthTime;
            set => this.SetProperty(ref this.maxVideoLengthTime, value);
        }

        public bool HasMinLengthTimeChanges()
        {
            return TimeSpan.Zero != this.MinVideoLengthTime;
        }

        public bool HasMaxLengthTimeChanges(TimeSpan max)
        {
            return this.MaxVideoLengthTime != max;
        }
    }
}
