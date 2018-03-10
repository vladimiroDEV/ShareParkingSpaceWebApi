using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Controllers.HUBS
{
    public class ManageParkingHub :Hub
    {
        public void JoinGroup(string group)
        {
            Groups.AddAsync(Context.ConnectionId, group);
        }

        public void leaveGroup(string group)
        {
            Groups.RemoveAsync(Context.ConnectionId, group);
        }
    }
}
