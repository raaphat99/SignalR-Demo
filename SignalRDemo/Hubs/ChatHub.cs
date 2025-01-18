using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRDemo.Data;
using SignalRDemo.Models;
using SignalRDemo.ViewModels;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;

namespace SignalRDemo.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationContext _context;
        public ChatHub(ApplicationContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            //var userId = Context.UserIdentifier;

            //if (userId == null)
            //    return;

            var user = _context.Users
                .Include(user => user.Rooms)
                .FirstOrDefault(user => user.Id == 2);

            if (user == null || user.Rooms == null)
                return;

            // Reconnect user by adding them to their joined rooms
            foreach (var room in user.Rooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);
            }

            var userConnection = new UserConnection()
            {
                UserId = user.Id,
                ConnectionId = Context.ConnectionId
            };

            await _context.UserConnections.AddAsync(userConnection);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = _context.UserConnections.Find(Context.ConnectionId);

            if (connection != null)
            {
                _context.UserConnections.Remove(connection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(int userId, int roomId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(user => user.Rooms)
                    .FirstOrDefaultAsync(user => user.Id == userId);
                var room = await _context.Rooms.FindAsync(roomId);

                // Handle potential null values
                if (user == null || room == null)
                    return;

                // if the user is already in the room, they can't join
                if (!user.Rooms.Any(room => room.Id == roomId))
                {
                    user.Rooms.Add(room);
                    await _context.SaveChangesAsync();
                }

                // Handle Duplicate Connections
                if (!_context.UserConnections.Any(connection => connection.ConnectionId == Context.ConnectionId))
                {
                    await _context.AddAsync(new UserConnection()
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = userId
                    });
                    await _context.SaveChangesAsync();
                }

                // Commit the transaction after successful execution
                await transaction.CommitAsync();

                // Ensure that SignalR group operations are performed outside the transaction,
                // as they are unrelated to database consistency. (It would unnecessarily cause the database changes to roll back if it failed)
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);

                // Broadcast the event
                await Clients.Group(room.Name).SendAsync("NewMember", new
                {
                    UserName = user.Name,
                    RoomId = roomId,
                    RoomName = room.Name,
                    TotalParticipants = user.Rooms.Count
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SendMessage(string messageContent, int userId, int roomId)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
                return;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(user => user.Rooms)
                    .FirstOrDefaultAsync(user => user.Id == userId);
                var room = await _context.Rooms.FindAsync(roomId);

                if (user == null || room == null)
                    return;

                // if the user is not in the room, they can't send messages
                if (!user.Rooms.Any(room => room.Id == roomId))
                    return;

                var message = new Message()
                {
                    UserId = userId,
                    RoomId = roomId,
                    Body = messageContent,
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                RealTimeMessageViewModel messageData = new RealTimeMessageViewModel()
                {
                    MessageId = message.Id,
                    MessageContent = messageContent,
                    SenderId = userId,
                    SenderName = user?.Name ?? "Anonymous",
                    RoomId = roomId,
                    CreatedAt = message.CreatedAt
                };

                // Broadcast message to all connected clients in that group
                await Clients.Group(room.Name).SendAsync("ReceiveMessage", messageData);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
