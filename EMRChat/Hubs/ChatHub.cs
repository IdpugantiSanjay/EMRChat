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
using System.Collections.Generic;

namespace EMRChat.Hubs
{
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

        private readonly UserHandler _userHandler = new UserHandler();          
       

        public async Task OnTextMessage(string chatInfo)
        {
            var deserializedChatInfo = JsonSerializer.Deserialize<ChatInfo>(chatInfo);

            await Clients.User(deserializedChatInfo.ToUser.UserIdentifier).SendAsync("OnTextMessage", chatInfo);
        }


        public async Task OnUserConnected(string userInfo)
        {
           // var connectionUser = JsonConvert.DeserializeObject<User>(userInfo);
            var connectionUser = JsonSerializer.Deserialize<User>(userInfo);

            _userHandler.AddConnectedUser(connectionUser, Context);

            var usersToNotify = _userHandler.UserIdsToNotify(connectionUser).ToList();

            await Clients.Users(usersToNotify).SendAsync("OnUserConnected", connectionUser);
        }


        //public override Task OnConnectedAsync()
        //{
        //    return base.OnConnectedAsync();
        //}

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userHandler.RemoveUserConnection(Context);
            return base.OnDisconnectedAsync(exception);
        }
        
        public IEnumerable<User> UsersList(string userInfo)
        {
            var fromUserInfo = JsonSerializer.Deserialize<User>(userInfo);
            
           // await Clients.Caller.SendAsync("UsersList",_userHandler.UsersList(fromUserInfo));
            return _userHandler.UsersList(fromUserInfo);
        }

    }
}
