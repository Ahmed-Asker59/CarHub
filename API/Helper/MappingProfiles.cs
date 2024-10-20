﻿using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Identity;
namespace API.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Car, CarDTO>().
                ForMember(d => d.ImagePath, o => o.MapFrom<CarUrlResolver>()).
                ForMember(d => d.Name, o => o.MapFrom(o => o.Brand.Name + " " + o.Model.Name + " " + o.ModelVariant)).
                ForMember(d => d.ReservationFee, o => o.MapFrom(o => o.Price * CarServicesPrices.ReservationRatio))
                .ForMember(d => d.RentalFeePerDay, o => o.MapFrom(o => o.Price * CarServicesPrices.RentalRatio));
                


            CreateMap<Make, MakeDTO>();

            CreateMap<ClientDTO, Client>();

            CreateMap<Model, ModelDTO>();

            CreateMap<ModelVariant, ModelVariantDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToString()));
            CreateMap<Address, AddressDTO>().ReverseMap();
        }

    }
}
