using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Dtos
{
    public class RecipeForReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RecipeCategory Category { get; set; }
		public bool IsCompleteProducts { get; set; }

		public string PhotoUrl { get; set; }
    }
}
