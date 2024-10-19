using API.DTO;
using Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailingController(IMailService mailService)
        {
            _mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail ([FromForm]MailRequestDto dto)
        {
            await _mailService.SendEmailAsync(dto.ToEmail,dto.Subject,dto.Body);
            return Ok();
        }
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeEmail([FromBody]WelcomeRequestDTO dto)
        {
            var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\EmailTemplate.html";
            var str = new StreamReader(filePath);
            var mailText = str.ReadToEnd();
            str.Close();
            mailText = mailText.Replace("[Email]",dto.Email);
            await _mailService.SendEmailAsync(dto.Email, "Welcome to CarHub", mailText);
            return Ok();
        }

    }
}
