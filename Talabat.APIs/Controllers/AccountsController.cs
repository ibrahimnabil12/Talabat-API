using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    public class AccountsController :BaseApiController
    {
       
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager ,SignInManager<AppUser> signInManager,ITokenService tokenService , IMapper mapper)
        {
            _userManager = userManager;
           _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));
            var Result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);
            if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(
                new UserDto()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token = await _tokenService.CreateTokenAsync(user,_userManager),
                });   
        }





        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {

            if (CheckEmailExist(model.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email already exist"));
            }
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],  // this would take what is before @ 
                PhoneNumber = model.PhoneNumber,
            };
            var Result = await _userManager.CreateAsync(user , model.Password);
            if (!Result.Succeeded)
                return BadRequest(new ApiResponse(400));
            var returnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = model.Email,
                Token = await _tokenService.CreateTokenAsync(user,_userManager),
            };
            return Ok(returnedUser);
        }



        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user,_userManager)
            });
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email == userEmail);
            var mappedAddress = _mapper.Map<Address, AddressDto>(user.Address);
            return Ok(mappedAddress);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<Address>> UpdateCurrentUserAddress(AddressDto model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user =await _userManager.Users.Include(U=>U.Address).FirstOrDefaultAsync(U=>U.Email == userEmail);
            var address =  _mapper.Map<AddressDto, Address>(model);
            user.Address = address;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(model);
        }


        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return false;
            return true;
        }
       
    }
}
