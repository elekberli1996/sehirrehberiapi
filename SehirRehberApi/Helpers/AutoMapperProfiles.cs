using AutoMapper;
using SehirRehberApi.Dto;
using SehirRehberApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberApi.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<City, CityForListDto>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);

            });

            CreateMap<City, CityForDetailDto>();

            CreateMap< Photo,PhotoForReturnDto> ();

            CreateMap< PhotoForCreationDto,Photo>();
        }
    }
}
