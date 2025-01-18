using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRDemo.Models
{
    public class UserConnection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ConnectionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
