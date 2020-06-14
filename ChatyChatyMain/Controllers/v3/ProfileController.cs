﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using ChatyChaty.ControllerSchema.v3;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.Controllers.v3
{
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [Route("api/v3/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountManager accountManager;
        private readonly IMessageService messageService;

        public ProfileController(IAccountManager accountManager, IMessageService messageService)
        {
            this.accountManager = accountManager;
            this.messageService = messageService;
        }


        /// <summary>
        /// Set photo or replace existing one (Require authentication)
        /// </summary>
        /// <remarks>
        /// Return the photo URL as a string (surrounded by "")</remarks>
        /// <returns></returns>
        /// <response code="400">The uploaded Photo must be a vaild img with png, jpg or jpeg with less than 4MB size</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPost("SetPhotoForSelf")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var setPhotoResult = await accountManager.SetPhoto(long.Parse(userId), uploadFile.PhotoFile);
            if (setPhotoResult.Success == true)
            {
                return Ok(new ResponseBase<string>
                {
                    Success = true,
                    Data = setPhotoResult.URL
                }); 
            }
            else
            {
                return BadRequest(new ResponseBase<string>
                {
                    Success = false,
                    Errors = setPhotoResult.Errors
                });
            }

        }


        /// <summary>
        /// Find users and return user profile with a chat Id,
        /// which is used to send messages and get other users info(Require authentication)
        /// </summary>
        /// <remarks><br>This is used to start a chat with a user</br>
        /// <br>You may get the DisplayName as null due to account greated before the last change</br>
        /// Example response:
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data":{
        ///     "chatId": 1,
        ///     "profile":{
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "PhotoURL": "*URL*"}
        ///         }
        /// }
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">When user was found</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromHeader]string userName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await accountManager.NewConversation(long.Parse(userId), userName);
            if (result.Error != null)
            {
                return NotFound(new ResponseBase<GetUserProfileResponseBase>
                {
                    Success = false,
                    Errors = new Collection<string> { result.Error }
                });
            }
            var response = new ResponseBase<GetUserProfileResponseBase>
            {
                Success = true,
                Data = new GetUserProfileResponseBase
                { 
                    ChatId = result.Conversation.ConversationId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = result.Conversation.SecondUserDisplayName,
                        Username = result.Conversation.SecondUserUsername,
                        PhotoURL = result.Conversation.SecondUserPhoto
                    }
                }
            };
            return Ok(response);
        }

        /// <summary>
        /// Get a list of all chat's information like username and ... so on (Require authentication)
        /// </summary>
        /// <remarks>This is used when there is an update in chat info example response:
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data":[
        ///         {
        ///     "chatId": 1,
        ///     "profile":{
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "PhotoURL": "*URL*"}
        ///         }
        ///     ]
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var chats = await messageService.GetConversations(long.Parse(userId));

            var chatList = new List<GetUserProfileResponseBase>();

            foreach (var chat in chats)
            {
                chatList.Add(
                new GetUserProfileResponseBase
                {
                    ChatId = chat.ConversationId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = chat.SecondUserDisplayName,
                        Username = chat.SecondUserUsername,
                        PhotoURL = chat.SecondUserPhoto
                    }
                }
                    );
            };
            var response = new ResponseBase<IEnumerable<GetUserProfileResponseBase>>()
            {
                Success = true,
                Data = chatList
            };
            return Ok(response);
        }


        /// <summary>
        /// Set or update the DisplayName of the authenticated user (Require authentication)
        /// </summary>
        /// <remarks>
        /// example reposne:
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data": "*newName*"
        /// }
        /// </br>
        /// </remarks>
        /// <param name="newDisplayName"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPatch("UpdateDisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody]string newDisplayName)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var newName = await accountManager.UpdateDisplayName(UserId, newDisplayName);
            return Ok(new ResponseBase<string>
            {
                Success = true,
                Data = newName
            });
        }
    }
}