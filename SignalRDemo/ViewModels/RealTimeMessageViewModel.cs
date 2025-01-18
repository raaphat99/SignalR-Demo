namespace SignalRDemo.ViewModels
{
    public class RealTimeMessageViewModel
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
