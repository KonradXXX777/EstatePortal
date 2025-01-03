using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System;

namespace EstatePortal.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string roomName, int chatId, int senderId, int receiverId, string messageContent)
        {
            Console.WriteLine($"SendMessage - Chat ID: {chatId}, Sender ID: {senderId}, Message: {messageContent}");

            await Clients.Group(roomName).SendAsync("ReceiveMessage", chatId, senderId, receiverId, messageContent, DateTime.Now);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveChat(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}