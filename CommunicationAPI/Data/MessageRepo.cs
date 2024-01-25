using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Data
{
    public class MessageRepo : IMessageRepo
    {
        private readonly DataContext _context;
        private readonly IMapper _objMapper;

        public MessageRepo(DataContext context, IMapper objMapper)
        {
            _context = context;
            _objMapper = objMapper;
        }
        public void AddMessage(Message objMessage)
        {
            _context.Messages.Add(objMessage);
        }
        public void RemoveMessage(Message objMessage)
        {
            _context.Messages.Remove(objMessage);
        }

        public async Task<Message> GetMessage(int messsageId)
        {
            return await _context.Messages.FindAsync(messsageId);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams objMessageParams)
        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
            query = objMessageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUserName == objMessageParams.UserName && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUserName == objMessageParams.UserName && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUserName == objMessageParams.UserName && u.RecipientDeleted == false && u.DataRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_objMapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, objMessageParams.PageNumber, objMessageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientName)
        {
            var messages = await _context.Messages
                 .Include(u => u.Sender).ThenInclude(p => p.Photos)
                 .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                 .Where(
                m => m.RecipientUserName == currentUserName && m.RecipientDeleted ==false &&
                m.SenderUserName == recipientName ||
                m.RecipientUserName == recipientName && m.SenderDeleted== false &&
                m.SenderUserName == currentUserName
                )
                 .OrderByDescending(m => m.MessageSent)
                 .ToListAsync();

            var unreadMessages = messages.Where(m => m.DataRead == null && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DataRead = DateTime.UtcNow;

                }
                await _context.SaveChangesAsync();
            }

            return _objMapper.Map<IEnumerable<MessageDto>>(messages);

        }


        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
