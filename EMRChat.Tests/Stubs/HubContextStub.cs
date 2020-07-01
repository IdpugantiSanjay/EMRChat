using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace EMRChat.Tests.Stubs
{
    class HubContextStub : HubCallerContext
    {
        public override CancellationToken ConnectionAborted => throw new NotImplementedException();

        public override string ConnectionId { get; }

        public override IFeatureCollection Features => throw new NotImplementedException();

        public override IDictionary<object, object> Items => throw new NotImplementedException();

        public override ClaimsPrincipal User => throw new NotImplementedException();

        public override string UserIdentifier { get; }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public HubContextStub(string connectionId, string userIdentifier)
        {
            ConnectionId = connectionId;
            UserIdentifier = userIdentifier;
        }
    }
}
