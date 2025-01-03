using System.ComponentModel.DataAnnotations;

namespace SignalRDemo.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Hours { get; set; }
        public string Department { get; set; }
        public string Instructor { get; set; }
    }
}
