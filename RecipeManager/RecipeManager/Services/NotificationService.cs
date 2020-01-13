using Microsoft.AspNetCore.SignalR;
using RecipeManager.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
	public class NotificationService : INotificationService
	{
		private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

		public NotificationService(IHubContext<NotifyHub, ITypedHubClient> hubContext)
		{
			_hubContext = hubContext;
		}

		public async Task<string> SendInvitationNotify(int userId, string myName)
		{
			string retMessage;
			try
			{
				await _hubContext.Clients.User(userId.ToString()).BroadcastMessage("Zaproszenie", "Dostałeś zaproszenie do znajomych od użytkownika " + myName);
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
