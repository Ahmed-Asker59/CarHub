using API.DTO;
using AutoMapper;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        public ClientController(IClientRepository clientRepository, IMapper autoMapper)
        {
            _clientRepository = clientRepository;
            _mapper = autoMapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string nationalId)
        {

            var client = await _clientRepository.SearchClientAsync(nationalId);
            

            var mappedClient = _mapper.Map<ClientDTO>(client);

            return Ok(mappedClient);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client is null) return NotFound();

            var mappedClient = _mapper.Map<ClientDTO>(client);

            return Ok(mappedClient);
        }
    }
}
