using AutoMapper;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Extension;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;

namespace CommunicationAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(obj => obj.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();

        }
    }
}
