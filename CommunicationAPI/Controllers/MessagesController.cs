﻿using AutoMapper;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Extension;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SQLitePCL;

namespace CommunicationAPI.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _objMapper;

        public MessagesController(IUnitOfWork unitOfWork, IMapper objMapper)
        {
            _unitOfWork = unitOfWork;
            _objMapper = objMapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto objCreateMessage)
        {
            var userName = User.getUserName();
            if (userName == objCreateMessage.RecipientUserName.ToLower())
                return BadRequest("You cant send messages to yourself");
            var sender = await _unitOfWork.userRepo.GetUserByNameAsync(userName);
            var recipient = await _unitOfWork.userRepo.GetUserByNameAsync(objCreateMessage.RecipientUserName);
            if (recipient == null) return NotFound();
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = objCreateMessage.Content
            };

            _unitOfWork.messageRepo.AddMessage(message);
            if (await _unitOfWork.Complete()) return Ok(_objMapper.Map<MessageDto>(message));

            return BadRequest("Failed");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> getMessagesForUsers([FromQuery] MessageParams objMessageParams)
        {
            objMessageParams.UserName = User.getUserName();
            var messages = await _unitOfWork.messageRepo.GetMessagesForUser(objMessageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        //[HttpGet("thread/{username}")]
        //public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userName)
        //{
        //    var currentUserName = User.getUserName();
        //    return Ok(await _unitOfWork.messageRepo.GetMessageThread(currentUserName, userName));
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName = User.getUserName();
            var message = await _unitOfWork.messageRepo.GetMessage(id);
            if (message.SenderUserName != userName && message.RecipientUserName != userName) return Unauthorized();
            if (message.SenderUserName == userName) message.SenderDeleted = true;
            if (message.RecipientUserName == userName) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted)
                _unitOfWork.messageRepo.RemoveMessage(message);
            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Issue in deleting");

        }
    }
}
