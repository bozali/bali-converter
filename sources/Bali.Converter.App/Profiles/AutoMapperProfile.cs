namespace Bali.Converter.App.Profiles
{
    using AutoMapper;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.Common.Media;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<MediaTags, MediaTagsViewModel>().ReverseMap();
        }
    }
}
