using RecipeManager.Dtos;
using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.ServicesInterfaces
{
	public interface IPhotoService
	{
		Photo AddPhoto(PhotoForCreationDto photo, int sizeX, int sizeY);
		bool DeletePhoto(string public_id);
	}
}
