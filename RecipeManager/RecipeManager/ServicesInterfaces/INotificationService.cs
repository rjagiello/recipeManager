using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ServicesInterfaces
{
	public interface INotificationService
	{
		Task<string> SendInvitationNotify(int userId, string myName);
	}
}
