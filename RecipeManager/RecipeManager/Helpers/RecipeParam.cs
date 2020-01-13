using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Helpers
{
	public class RecipeParam : Params
	{		
		public int UserId { get; set; }
		public int Category { get; set; } = 0;
	}
}
