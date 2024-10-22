using Core.Entities;
using Core.Interface;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ICarRepository _carRepository;

        public PaymentService(IConfiguration configuration, ICarRepository carRepository)
        {
            _configuration = configuration;
            _carRepository = carRepository;
        }

        public async Task<PaymentInfo> CreatPaymentIntent(decimal totalPrice)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            // Create or update the PaymentIntent
            var service = new PaymentIntentService();
            PaymentIntent intent;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(totalPrice * 100), // Price in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            intent = await service.CreateAsync(options);
            PaymentInfo payInfo = new PaymentInfo();
            payInfo.PaymentIntentId = intent.Id;
            payInfo.ClientSecret = intent.ClientSecret;
            return payInfo;

        }
    }
}
