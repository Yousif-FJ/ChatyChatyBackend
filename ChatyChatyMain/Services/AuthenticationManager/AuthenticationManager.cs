﻿using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.NotficationHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IPictureProvider pictureProvider;
        private readonly INotificationHandler notificationHandler;

        public AuthenticationManager(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IPictureProvider pictureProvider,
            INotificationHandler notificationHandler
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.pictureProvider = pictureProvider;
            this.notificationHandler = notificationHandler;
        }

        public async Task<AuthenticationResult> CreateAccount(string username, string password, string displayName)
        {
            if (Environment.GetEnvironmentVariable("DISABLE_REGESTRATION") == "true")
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string>() { new string("Account creation is disabled for security reasons") }
                };
            }
            AppUser identityUser = new AppUser(username)
            {
                DisplayName = displayName
            };
            var AccountCreationResult = await userManager.CreateAsync(identityUser, password);
            if (!AccountCreationResult.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = AccountCreationResult.Errors.Select(x => x.Description)
                };
            }
            var user = await userManager.FindByNameAsync(username);
            await notificationHandler.IntializeNofificationHandler(user.Id);
            var profile = new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PhotoURL = null
            };

            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(user.UserName,user.Id.ToString()),
                Profile = profile
            };
        }

        public async Task<AuthenticationResult> Login(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            var LoginResult = await userManager.CheckPasswordAsync(user, password);
            if (!LoginResult)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { new string("Invalid Login cridentials") }
                };
            }
            await notificationHandler.IntializeNofificationHandler(user.Id);
            var profile = new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PhotoURL = await pictureProvider.GetPhotoURL(user.Id, user.UserName)
            };
            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(user.UserName, user.Id.ToString()),
                Profile = profile
            };

        }


        public async Task<AuthenticationResult> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                throw new ArgumentException("Passed user doesn't exist");
            }
            var LoginResult = await userManager.CheckPasswordAsync(user, currentPassword);
            if (!LoginResult)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { new string("Incorrect password") }
                };
            }

            var ChangePasswordResult =
                await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return new AuthenticationResult
            {
                Success = ChangePasswordResult.Succeeded,
                Errors = ChangePasswordResult.Errors.Select(e => e.Description)
            };
        }

        private string JwtTokenGenerator(string UserName, string Id)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.UniqueName, UserName),
                    new Claim(type: JwtRegisteredClaimNames.NameId, Id),
                    new Claim(type: JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }
                ),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(7),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Issuer"]
            };
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
