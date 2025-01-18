using System.ComponentModel.DataAnnotations;

namespace SignalRDemo.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserConnection> UserConnections { get; set; } = new List<UserConnection>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
