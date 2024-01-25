using CommunicationAPI.Entities;

namespace CommunicationAPI.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderPhotoUrl { get; set; }  
        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientrPhotoUrl { get; set; }       
        public string Content { get; set; }
        public DateTime? DataRead { get; set; }
        public DateTime? MessageSent { get; set; }

    }
}
