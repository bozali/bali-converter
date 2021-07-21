namespace Bali.Converter.App.Profiles
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using AutoMapper;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.Common.Media;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<MediaTags, MediaTagsViewModel>()
                .ForMember(dest => dest.Performers, opt => opt.MapFrom(src => new ObservableCollection<string>(src.Performers)))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => new ObservableCollection<string>(src.Genres)));

            this.CreateMap<MediaTagsViewModel, MediaTags>()
                .ForMember(dest => dest.Performers, opt => opt.MapFrom(src => src.Performers.ToArray()))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.ToArray()));
        }
    }
}
