using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string public_id { get; set; }

        public User User { get; set; }
        public int? UserId { get; set; }
        public Recipe Recipe { get; set; }
        public int? RecipeId { get; set; }
    }
}
