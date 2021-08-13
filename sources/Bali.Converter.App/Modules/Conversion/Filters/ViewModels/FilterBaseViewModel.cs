namespace Bali.Converter.App.Modules.Conversion.Filters.ViewModels
{
    using MahApps.Metro.IconPacks;

    using Prism.Mvvm;

    public abstract class FilterBaseViewModel : BindableBase
    {
        private string displayName;

        protected FilterBaseViewModel(string displayName)
        {
            this.DisplayName = displayName;
        }

        public string DisplayName
        {
            get => this.displayName;
            set => this.SetProperty(ref this.displayName, value);
        }

        public abstract PackIconMaterialKind Icon { get; }
    }
}
