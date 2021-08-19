namespace Bali.Converter.App.Modules.Conversion.Filters.ViewModels
{
    using MahApps.Metro.IconPacks;

    public class FpsFilterViewModel : FilterBaseViewModel
    {
        private int fps;

        public FpsFilterViewModel()
            : base(FilterNameConstants.Video.Fps)
        {
        }

        public int Fps
        {
            get => this.fps;
            set => this.SetProperty(ref this.fps, value);
        }

        public override PackIconMaterialKind Icon
        {
            get => PackIconMaterialKind.ImageMultipleOutline;
        }
    }
}
