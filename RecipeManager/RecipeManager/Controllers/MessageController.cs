using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RecipeManager.Models;
using RecipeManager.Services;
using RecipeManager.ServicesInterfaces;

namespace RecipeManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
		private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

		public MessageController(IHubContext<NotifyHub, ITypedHubClient> hubContext)
		{
			_hubContext = hubContext;
		}

		[HttpPost]
		public string Post([FromBody]Message msg)
		{
			string retMessage = string.Empty;
			try
			{
				 _hubContext.Clients.User("1").BroadcastMessage(msg.Type, msg.Payload);
				retMessage = "Success";
			}
			catch (Exception e)
			{
				retMessage = e.ToString();
			}
			return retMessage;
		}
	}
}