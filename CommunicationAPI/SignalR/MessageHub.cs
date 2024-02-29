using AutoMapper;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Extension;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;

namespace CommunicationAPI.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepo _messageRepo;
        private readonly IuserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageRepo messageRepo, IuserRepo userRepo, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var otherUser = Context.GetHttpContext().Request.Query["user"];
            var groupName = GetGroupName(Context.User.getUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddtoGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepo.GetMessageThread(Context.User.getUserName(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDTO)
        {
            var userName = Context.User.getUserName();
            if (userName == createMessageDTO.RecipientUserName.ToLower())
                throw new HubException("You cant send messages to yourself");

            var sender = await _userRepo.GetUserByNameAsync(userName);
            var recipient = await _userRepo.GetUserByNameAsync(createMessageDTO.RecipientUserName);
            if (recipient == null) throw new HubException("user not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDTO.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepo.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.UserName == recipient.UserName))
            {
                message.DataRead = DateTime.Now;
            }
            else
            {
                var connections = await PresenceTracker.GetconnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { userName = sender.UserName, knownAs = sender.KnownAs });

                }
            }

            _messageRepo.AddMessage(message);
            if (await _messageRepo.SaveAllAsync())
            {
                //  var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(groupName).SendAsync("newmessage", _mapper.Map<MessageDto>(message));

            }
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddtoGroup(string groupName)
        {
            var group = await _messageRepo.GetMessageGroup(groupName);

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepo.AddGroup(group);
            }
            var connection = new Connection(Context.ConnectionId, Context.User.getUserName());
            group.Connections.Add(connection);

            if (await _messageRepo.SaveAllAsync()) return group;

            throw new HubException("Failed to add from group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepo.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepo.RemoveConnection(connection);
            if (await _messageRepo.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }
    }
}
