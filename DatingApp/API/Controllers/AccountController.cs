﻿using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _DataContext;

        public AccountController(DataContext dataContext)
        {
            _DataContext = dataContext;
        }


        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest($"Username {registerDto.UserName} is taken.");

            using (var hmac = new HMACSHA512())
            {
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };

                _DataContext.Users.Add(appUser);
                await _DataContext.SaveChangesAsync();

                return appUser;
            }
        }

        private Task<bool> UserExists(string userName)
        {
            return _DataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}
