using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Entities.Consts;
using Core.Identity;
using System;
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


            CreateMap<Rental, RentalDTO>().
                ForMember(d => d.RentalDate, o => o.MapFrom(o => o.StartDate)).
                ForMember(d => d.ClientName, o => o.MapFrom(o => o.Client.FirstName + ' ' + o.Client.LastName)).
                ForMember(d => d.ClientID, o => o.MapFrom(o => o.Client.Id)).
                ForMember(d => d.Car, o => o.MapFrom(o => o.Car.Brand.Name + " " + o.Car.Model.Name + " " + o.Car.ModelVariant)).
                ForMember(d => d.ClientPhone, o => o.MapFrom(o => o.Client.Phone)).
                ForMember(d => d.RentalPrice, o => o.MapFrom(o => o.RentalPrice)).
                ForMember(d => d.IsActive, o => o.MapFrom(o => o.EndDate > DateTime.Now && !o.ActualReturnDate.HasValue));





            CreateMap<Rental, DelayedRentalDTO>().
                ForMember(d => d.RentalDate, o => o.MapFrom(o => o.StartDate)).
                ForMember(d => d.ClientName, o => o.MapFrom(o => o.Client.FirstName + ' ' + o.Client.LastName)).
                ForMember(d => d.ClientID, o => o.MapFrom(o => o.Client.Id)).
                ForMember(d => d.Car, o => o.MapFrom(o => o.Car.Brand.Name + " " + o.Car.Model.Name + " " + o.Car.ModelVariant)).
                ForMember(d => d.ClientPhone, o => o.MapFrom(o => o.Client.Phone)).
                ForMember(d => d.ActualReturnDate, o => o.MapFrom(o => o.ActualReturnDate)).
                ForMember(d => d.DelayInDays, o => o.MapFrom(o => o.DelayInDays)).
                ForMember(d => d.RentalPrice, o => o.MapFrom(o => o.TotalRentalPrice));
    
               

            CreateMap<Reservation, ReservationDTO>().
              ForMember(d => d.ReservationDate, o => o.MapFrom(o => o.StartDate)).
              ForMember(d => d.ClientName, o => o.MapFrom(o => o.Client.FirstName + ' ' + o.Client.LastName)).
              ForMember(d => d.ClientID, o => o.MapFrom(o => o.Client.Id)).
              ForMember(d => d.Car, o => o.MapFrom(o => o.Car.Brand.Name + " " + o.Car.Model.Name + " " + o.Car.ModelVariant)).
              ForMember(d => d.ClientPhone, o => o.MapFrom(o => o.Client.Phone)).
              ForMember(d => d.ReservationFee, o => o.MapFrom(o => o.ReservationFee)).
              ForMember(d => d.IsActive, o => o.MapFrom(o => o.EndDate > DateTime.Now));

            CreateMap<Make, MakeDTO>();

            CreateMap<ClientDTO, Client>().ReverseMap();

            CreateMap<Model, ModelDTO>();

            CreateMap<ModelVariant, ModelVariantDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToString()));
            CreateMap<Address, AddressDTO>().ReverseMap();
        }

    }
}
