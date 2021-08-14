namespace Bali.Converter.App.Modules.Conversion.Filters.ViewModels
{
    using System;

    using MahApps.Metro.IconPacks;

    public class VolumeFilterViewModel : FilterBaseViewModel
    {
        private float multiplier;
        private int decibel;
        private bool useDecibel;

        public VolumeFilterViewModel()
            : base(FilterNameConstants.Audio.Volume)
        {
            this.Multiplier = 1.0f;
        }

        public float Multiplier
        {
            get => this.multiplier;
            set => this.SetProperty(ref this.multiplier, MathF.Round(value, 1));
        }

        public int Decibel
        {
            get => this.decibel;
            set => this.SetProperty(ref this.decibel, value);
        }

        public bool UseDecibel
        {
            get => this.useDecibel;
            set => this.SetProperty(ref this.useDecibel, value);
        }

        public override PackIconMaterialKind Icon
        {
            get => PackIconMaterialKind.VolumeVibrate;
        }
    }
}
