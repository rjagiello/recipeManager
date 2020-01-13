using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager
{
    public enum ProductUnit
    {
        Gram = 0,
        Count = 1,
        Milliliter = 2,
		Spoon = 3,
		TeaSpoon = 4,
		Glass = 5,
		Pinch = 6,
    }

    public enum RecipeCategory
    {
        BreakFast = 0,
        Dinner,
        Supper,
        Snacks
    }
}
