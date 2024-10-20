using API.DTO;
using AutoMapper;
using Core.Entities;
using Core.Interface;
using Hangfire;
using Infrastructure.Data;

namespace API.Tasks
{
    public class hangfireTasks
    {
        private readonly ICarRepository _carRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IRentRepository _rentRepository;
        private readonly IMailService _emailService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;

        public hangfireTasks
            (ICarRepository repository,
            IClientRepository clientRepository, IReservationRepository reservationRepository,
            IRentRepository rentRepository ,IMailService emailService, IEmailBodyBuilder emailBodyBuilder)
        {
            _carRepository = repository;
            _clientRepository = clientRepository;
            _reservationRepository = reservationRepository;
            _rentRepository = rentRepository;
            _emailService = emailService;
            _emailBodyBuilder = emailBodyBuilder;
        }
        public async Task PrepareExpirationAlert()
        {
            var clientToAlert = await _reservationRepository.GetClientsToAlert();
            


            foreach (var client in clientToAlert)
            {
                var reserveToAlert = client.Reservations.First();
                var car = reserveToAlert.Car;
                var carName = await _carRepository.GetCarNameAsync(car.Id);

                var placeholders = new Dictionary<string, string>()
            {
             {"FirstName", $"{client.FirstName}" },
             {"CarModel", $"{carName}" },
             { "EndDate", reserveToAlert.EndDate.ToString("MMMM dd, yyyy") },
           
            };

                var body = _emailBodyBuilder.GenerateEmailBody("ReserveExpiration", placeholders);
                
                await _emailService.SendEmailAsync(client.Email, "CarHub Reservation Expiration", body);
            }

            clientToAlert = await _rentRepository.GetClientsToAlert();
            foreach (var client in clientToAlert)
            {
                var rentalsToAlert = client.Rentals;
                
             
               foreach(var rental in rentalsToAlert)
                {
                    var car = rental.Car;
                    var carName = await _carRepository.GetCarNameAsync(car.Id);
                    var placeholders = new Dictionary<string, string>()
                {
                  {"FirstName", $"{client.FirstName}" },
                  {"CarModel", $"{carName}" },
                  { "EndDate", rental.EndDate.ToString("MMMM dd, yyyy") },

                };

                    var body = _emailBodyBuilder.GenerateEmailBody("RentExpiration", placeholders);
                    await _emailService.SendEmailAsync(client.Email, "CarHub Rental Expiration", body);

                }

               

                
            }

        }
    }
}
