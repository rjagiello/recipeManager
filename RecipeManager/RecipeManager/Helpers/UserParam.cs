using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Helpers
{
	public class UserParam : Params
	{
		public int UserId { get; set; }
		public bool UserFollow { get; set; }
		public bool UserIsFollowed { get; set; }
	}
}
