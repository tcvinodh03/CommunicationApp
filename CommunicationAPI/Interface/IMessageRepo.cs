using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Helpers;

namespace CommunicationAPI.Interface
{
    public interface IMessageRepo
    {
        void AddMessage(Message objMessage);
        void RemoveMessage(Message objMessage);
        Task<Message> GetMessage(int messsageId);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams objMessageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientName);

        Task<bool> SaveAllAsync();
    }
}
