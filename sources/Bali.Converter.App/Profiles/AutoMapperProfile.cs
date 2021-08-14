namespace Bali.Converter.App.Profiles
{
    using AutoMapper;

    using Bali.Converter.App.Modules.Conversion.Filters.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.Common.Media;
    using Bali.Converter.FFmpeg.Filters.Audio;
    using Bali.Converter.FFmpeg.Filters.Video;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<MediaTags, MediaTagsViewModel>().ReverseMap();

            this.CreateMap<RotationFilter, RotationFilterViewModel>().ReverseMap();
            this.CreateMap<VolumeFilter, VolumeFilterViewModel>().ReverseMap();
        }
    }
}
