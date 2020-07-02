using EMRChat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace EMRChat.Hubs
{
    public class UserHandler
    {
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, User>> connectedUsers = new ConcurrentDictionary<int, ConcurrentDictionary<string, User>>();

        private object userLock = new object();

        public void AddConnectedUser(User connectedUser, HubCallerContext context)
        {
            connectedUser.ConnectionIds.Add(context.ConnectionId);
            var practiceId = int.Parse(context.UserIdentifier.Split("_").First());

            if (connectedUsers.TryGetValue(practiceId, out ConcurrentDictionary<string, User> practiceUsers))
            {
                if (practiceUsers.TryGetValue(context.UserIdentifier, out User _connectedUser))
                {
                    lock (userLock)
                    {
                        _connectedUser.ConnectionIds.Add(context.ConnectionId);
                    }
                }
                else
                {
                    practiceUsers.TryAdd(context.UserIdentifier, _connectedUser);
                }
            }
            else
            {
                practiceUsers = new ConcurrentDictionary<string, User>() { [context.UserIdentifier] = connectedUser };
                connectedUsers.TryAdd(practiceId, practiceUsers);
            }
        }

        public IEnumerable<string> UserIdsToNotify(User user)
        {
            return connectedUsers[user.PracticeId]
                .Where(x => x.Key != user.ToString())
                .Select(x => x.Key);
        }

        public void RemoveUserConnection(HubCallerContext context)
        {
            var practiceId = int.Parse(context.UserIdentifier.Split("_").First());

            var disconnectedUser = connectedUsers[practiceId][context.UserIdentifier];

            lock (userLock)
            {
                disconnectedUser.ConnectionIds.Remove(context.ConnectionId);
            }

            if (disconnectedUser.ConnectionIds.Count() == 0)
            {
                connectedUsers[practiceId].TryRemove(context.UserIdentifier, out _);
            }
        }

        public bool IsUserConnected(User user)
        {
            if (!connectedUsers.ContainsKey(user.PracticeId))
            {
                return false;
            }
            connectedUsers[user.PracticeId].TryGetValue(user.UserIdentifier.ToString(), out User connectedUser);
            return connectedUser != null;
        }
    }
}
