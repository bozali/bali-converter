namespace Bali.Converter.App.Modules.Conversion.Filters.ViewModels
{
    using MahApps.Metro.IconPacks;

    public class RotationFilterViewModel : FilterBaseViewModel
    {
        private int rotation;

        public RotationFilterViewModel()
            : base(FilterNameConstants.Video.Rotation)
        {
        }

        public int Rotation
        {
            get => this.rotation;
            set => this.SetProperty(ref this.rotation, value);
        }

        public override PackIconMaterialKind Icon
        {
            get => PackIconMaterialKind.ScreenRotation;
        }
    }
}
