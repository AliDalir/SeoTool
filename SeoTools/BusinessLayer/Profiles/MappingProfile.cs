using AutoMapper;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using UtilityLayer.Convertors;

namespace BusinessLayer.Profiles;

public class MappingProfile : Profile

{
    public MappingProfile()
    {
        CreateMap<Keyword, KeywordDto>().ReverseMap();
        CreateMap<Keyword, KeywordResponseDto>().ReverseMap();
        CreateMap<Keyword, KeywordELK>().ReverseMap();
        CreateMap<Rank, BaseRankDto>().ForMember(dest => dest.CreationDateTime,
            opt =>
                opt.MapFrom(src =>
                    DateConvertor.ConvertMiladiToShamsi(src.CreationDateTime, "yyyy/MM/dd"))).ReverseMap();
        CreateMap<Site, SiteDto>().ReverseMap();
        CreateMap<KeywordGroup, KeywordGroupDto>().ReverseMap();
        CreateMap<KeywordUrl, KeywordUrlDto>().ReverseMap();
        CreateMap<User, RegisterDto>().ReverseMap();
        CreateMap<User, UserResponse>().ReverseMap();
        CreateMap<Site, SiteReqDto>().ReverseMap();
        CreateMap<Tag, TagDto>().ReverseMap();
        CreateMap<Tag, TagReqDto>().ReverseMap();
        CreateMap<View, ViewDto>().ReverseMap();
        CreateMap<KeywordGroup, KeywordGroupReqDto>().ReverseMap();
        CreateMap<KeywordGroupHistory, KeywordGroupHistoryDto>().ReverseMap();
        CreateMap<CrawlDate, CrawlDateDto>().ForMember(dest => dest.CrawlDateTime,
                opt =>
                    opt.MapFrom(src =>
                        DateConvertor.ConvertMiladiToShamsi(src.CrawlDateTime, "yyyy/MM/dd")))
            .ReverseMap();
        CreateMap<KeywordTag, Tag>()
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag.TagName))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id));
        

    }
}