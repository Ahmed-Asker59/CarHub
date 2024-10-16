using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{totalPrice}")]
        public async Task<ActionResult<PaymentInfo>> CreatePaymentIntent(decimal totalPrice)
        {
            var paymentInfo = await _paymentService.CreatPaymentIntent(totalPrice);

            if (paymentInfo == null)
            {
                return BadRequest(new
                {
                    message = "Problem in creating payment intent."
                });
            }
            return Ok(paymentInfo); 
        }
    }
}
