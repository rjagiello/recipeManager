using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeManager.DataInterfaces;
using RecipeManager.Dtos;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;

namespace RecipeManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
		private readonly IEmailService _emailSvc;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repository, IConfiguration config, IMapper mapper, IEmailService emailSvc)
        {
            _repository = repository;
            _config = config;
            _mapper = mapper;
			_emailSvc = emailSvc;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _repository.UserExist(userForRegisterDto.UserName))
                return BadRequest("Użytkownik o takiej nazwie już istnieje!");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _repository.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtRoute("GetUser", new { controller = "Users", createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repository.Login(userForLoginDto.UserName, userForLoginDto.Password);

			if (userFromRepo == null)
				return Unauthorized("Niepoprawny login lub hasło");

            // create Token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForDetailedDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

		[HttpPut]
		public async Task<IActionResult> RemindPassword(RemidPasswordDto remidPasswordDto)
		{
			if (remidPasswordDto.Email != null &&
				await _repository.UserExistEmail(remidPasswordDto.Email))
			{
				string password = await _repository.SetTempPassword(remidPasswordDto.Email);
				if (password != null)
				{
					await _emailSvc.SendEmailAsync(remidPasswordDto.Email, "Reset hasła", "Twoje tymczasowe hasło to: " + password);

					return Ok();
				}
			}

			throw new Exception("Niepoprawny adres e-mail");
		}

    }
}