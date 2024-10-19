using API.DTO;
using API.Extentions;
using AutoMapper;
using Core.Identity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController :ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _Tokenservices;
        private readonly IMapper _mapper;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService Tokenservices, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _Tokenservices = Tokenservices;
            _mapper = mapper;

        }

        [HttpGet("Secret")]
        [Authorize]
        public string GetSecret()
        {
            return "Secret Sting";
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new ApiResponse (401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));
            return new UserDTO { Email = user.Email , Token = _Tokenservices.CreateToken(user), DisplayName = user.DisplayName };

        }
        [HttpPost("Register")]

        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email
            };


            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));


            return new UserDTO
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _Tokenservices.CreateToken(user)
            };


        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetCurentUser()
        {
            //var Email = HttpContext.User?.Claims?.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value;
            //var User= await _userManager.FindByEmailAsync(Email);
            var user = await _userManager.FindByEmailFromclaimPrinciple(User);
            return new UserDTO
            {
                Email = user.Email,
                Token = _Tokenservices.CreateToken(user),
                DisplayName = user.DisplayName
            };


        }
        [HttpGet("emailexists")]

        public async Task<ActionResult<bool>> CheckEmailExistAsyn([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [HttpGet("address")]

        public async Task<ActionResult<AddressDTO>> GetUserAddress()
        {
            //var email= HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            //var user = await _userManager.FindByEmailAsync(email);

            var user = await _userManager.FindByEmailFromclaimPrinciple(User);

            return _mapper.Map<Address, AddressDTO>(user.Address);
        }

        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult<AddressDTO>> UpdateUserAddress(AddressDTO address)
        {
            var user = await _userManager.FindUserByClaimsPrincipleWithAddress(User);
            user.Address = _mapper.Map<AddressDTO, Address>(address);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(_mapper.Map<AddressDTO>(user.Address));
            return BadRequest("Problem Updating The User");

        }

     
    }
}
