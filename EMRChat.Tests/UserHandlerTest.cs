using EMRChat.Hubs;
using EMRChat.Models;
using EMRChat.Tests.Stubs;
using System;
using System.Collections.Generic;
using Xunit;

namespace EMRChat.Tests
{
    public class UserHandlerTest
    {
        [Fact]
        public void ShouldReturnTrue_WhenConnectedUserIsAdded()
        {
            UserHandler userHandler = new UserHandler();
            string connectionId = Guid.NewGuid().ToString();

            User user = new User()
            {
                ApplicationType = ApplicationType.Ehr,
                UserId = 42,
                PracticeId = 36,
                ConnectionIds = new HashSet<string> { connectionId }
            };

            string userIdentifier = user.UserIdentifier;

            userHandler.AddConnectedUser(user, new HubContextStub(connectionId, userIdentifier));
            Assert.True(userHandler.IsUserConnected(user));
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoUserIsConnected()
        {
            UserHandler userHandler = new UserHandler();
            string connectionId = Guid.NewGuid().ToString();

            User user = new User()
            {
                ApplicationType = ApplicationType.Ehr,
                UserId = 42,
                PracticeId = 36,
                ConnectionIds = new HashSet<string> { connectionId }
            };

            Assert.False(userHandler.IsUserConnected(user));
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoUserInPracticeIsConnected()
        {
            UserHandler userHandler = new UserHandler();
            string connectionId = Guid.NewGuid().ToString();

            User user = new User()
            {
                ApplicationType = ApplicationType.Ehr,
                UserId = 42,
                PracticeId = 36,
                ConnectionIds = new HashSet<string> { connectionId }
            };

            User otherPracticeUser = new User()
            {
                ApplicationType = ApplicationType.Ehr,
                UserId = 42,
                PracticeId = 103,
                ConnectionIds = new HashSet<string> { connectionId }
            };
            string userIdentifier = otherPracticeUser.UserIdentifier;

            userHandler.AddConnectedUser(otherPracticeUser, new HubContextStub(connectionId, userIdentifier));
            Assert.False(userHandler.IsUserConnected(user));
        }
    }
}
