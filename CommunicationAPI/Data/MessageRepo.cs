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
            var query = _context.Messages                
                 .Where(
                m => m.RecipientUserName == currentUserName && m.RecipientDeleted == false &&
                m.SenderUserName == recipientName ||
                m.RecipientUserName == recipientName && m.SenderDeleted == false &&
                m.SenderUserName == currentUserName
                )
                 .OrderByDescending(m => m.MessageSent).AsQueryable();
                 

            var unreadMessages = query.Where(m => m.DataRead == null && m.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DataRead = DateTime.UtcNow;

                }
               // await _context.SaveChangesAsync();
            }

            return await query.ProjectTo<MessageDto>(_objMapper.ConfigurationProvider).ToListAsync();

        }


        //public async Task<bool> SaveAllAsync()
        //{
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(c => c.Connections).FirstOrDefaultAsync(n => n.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(x => x.Connections).Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                  .FirstOrDefaultAsync();
        }
    }
}
