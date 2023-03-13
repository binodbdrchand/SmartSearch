using AutoMapper;
using SmartSearch.Modules.DocumentManager.Domain;
using SmartSearch.Modules.DocumentManager.ViewModel;
using SmartSearch.Modules.UserManager.Domain;
using SmartSearch.Modules.UserManager.ViewModel;
using SmartSearch.Modules.VideoManager.Domain;
using SmartSearch.Modules.VideoManager.ViewModel;

namespace SmartSearch.Infrastructure.IoC.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, AppUserViewModel>().ReverseMap();

            CreateMap<Document, DocumentViewModel>().ReverseMap();
            CreateMap<DocumentTopic, DocumentTopicViewModel>().ReverseMap();
            CreateMap<DocumentKeyword, DocumentKeywordViewModel>().ReverseMap();

            CreateMap<Video, VideoViewModel>().ReverseMap();
            CreateMap<VideoTopic, VideoTopicViewModel>().ReverseMap();
            CreateMap<VideoKeyword, VideoKeywordViewModel>().ReverseMap();
        }
    }
}
