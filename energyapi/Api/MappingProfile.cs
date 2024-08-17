using Api.Models;
using AutoMapper;
using Data.Entities;

namespace Api {
    public class MappingProfile : Profile{
        public MappingProfile() {
            CreateMap<UploadMeterReadingRequest, MeterReading>()
                .ForMember(dest => dest.Account, opt => opt.Ignore()); // this is handled separately by EF
        }
    }
}
