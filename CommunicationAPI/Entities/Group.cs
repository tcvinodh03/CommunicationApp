using System.ComponentModel.DataAnnotations;

namespace CommunicationAPI.Entities
{
    public class Group
    {
        public Group() { }

        public Group(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
