using EMRChat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Collections.Concurrent;

using System.IO;

namespace EMRChat.Hubs
{

    public static class Users
    {

    }



    public class MessageHandler
    {

    }



    public class ChatHub : Hub
    {

        
        /// Step 1: Connect
        ///     Save ConnectionId and 
        ///     UserId filter
        ///     When connection closes. ConnectionId filter then remove
        ///     
        /// Step 2:
        ///     Text Message -> Filter by UserId and send Clients.User(deserializedChatInfo.ToUser.ToString())
        ///     
        /// Step 3:
        ///     Show Online -> Connection Id no need
        ///     When all connections are closed remove the respective user
        /// 
        ConcurrentDictionary<int, ConcurrentDictionary<string, User>> connectedUsers = new ConcurrentDictionary<int, ConcurrentDictionary<string, User>>();

        private static object userLock = new object();

        public async Task OnTextMessage(string chatInfo)
        {
            var deserializedChatInfo = JsonSerializer.Deserialize<ChatInfo>(chatInfo);
            await Clients.User(deserializedChatInfo.ToUser.ToString()).SendAsync("OnTextMessage", chatInfo);
        }


        public async Task OnUserConnected(string userInfo)
        {
            var connectionUser = JsonSerializer.Deserialize<User>(userInfo);
            connectionUser.ConnectionIds.Add(Context.ConnectionId);
            var practiceId = int.Parse(Context.UserIdentifier.Split("_").First());


            if (connectedUsers.TryGetValue(practiceId, out ConcurrentDictionary<string, User> practiceUsers))
            {
                if (practiceUsers.TryGetValue(Context.UserIdentifier, out User connectedUser))
                {
                    lock (userLock)
                    {
                        connectedUser.ConnectionIds.Add(Context.ConnectionId);
                    }
                }
                else
                {
                    practiceUsers.TryAdd(Context.UserIdentifier, connectedUser);
                }
            }
            else
            {
                practiceUsers = new ConcurrentDictionary<string, User>() { [Context.UserIdentifier] = connectionUser };
                connectedUsers.TryAdd(practiceId, practiceUsers);
            }

            var usersToNotify = connectedUsers[practiceId]
                .Where(x => x.Key != Context.UserIdentifier)
                .Select(x => x.Key)
                .ToList();

            await Clients.Users(usersToNotify).SendAsync("OnUserConnected", connectionUser);
        }


        //public override Task OnConnectedAsync()
        //{
        //    return base.OnConnectedAsync();
        //}

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var practiceId = int.Parse(Context.UserIdentifier.Split("_").First());

            var disconnectedUser = connectedUsers[practiceId][Context.UserIdentifier];

            lock (userLock)
            {
                disconnectedUser.ConnectionIds.Remove(Context.ConnectionId);

                if (disconnectedUser.ConnectionIds.Count() == 0)
                {
                    connectedUsers[practiceId].TryRemove(Context.UserIdentifier, out _);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
