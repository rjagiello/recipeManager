using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
	public class Friendship
	{
		public int UserFollowId { get; set; }
		public int UserIsFollowedId { get; set; }

		public bool IsAccepted { get; set; }

		public User UserFollow { get; set; }
		public User UserIsFollowed { get; set; }
	}
}
