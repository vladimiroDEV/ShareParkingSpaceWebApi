using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShareParkingSpaceWebApi.Controllers.HUBS;

namespace ShareParkingSpaceWebApi.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Test/[action]")]
    public class TestController : Controller
    {
        private IHubContext<MessageHub> _messageHubContext;
        public TestController(IHubContext<MessageHub> messageHubContext)
        {
            _messageHubContext = messageHubContext;
        }

        [HttpPost]
        public IActionResult testPost()
        {
            //_messageHubContext.Clients.All.InvokeAsync("send", "Hello from server");
            _messageHubContext.Clients.Group("datat").InvokeAsync("send", "hello from server groups");
            return Ok();
        }
    }
}