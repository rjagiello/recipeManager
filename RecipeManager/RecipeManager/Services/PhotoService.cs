using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RecipeManager.Dtos;
using RecipeManager.Models;
using RecipeManager.ServicesInterfaces;
using RecipeManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
	public class PhotoService : IPhotoService
	{
		private readonly IMapper _mapper;
		private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
		private Cloudinary _cloudinary;

		public PhotoService(IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
		{
			_mapper = mapper;
			_cloudinaryConfig = cloudinaryConfig;

			Account account = new Account(
			_cloudinaryConfig.Value.CloudName,
			_cloudinaryConfig.Value.ApiKey,
			_cloudinaryConfig.Value.ApiSecret
			);

			_cloudinary = new Cloudinary(account);
		}

		public Photo AddPhoto(PhotoForCreationDto photo, int sizeX, int sizeY)
		{
			var file = photo.File;
			var uploadResult = new ImageUploadResult();

			if (file.Length > 0)
			{
				using (var stream = file.OpenReadStream())
				{
					var uploadParams = new ImageUploadParams()
					{
						File = new FileDescription(file.Name, stream),
						Transformation = new Transformation().Width(sizeX).Height(sizeY).Crop("fill").Gravity("face")
					};

					uploadResult = _cloudinary.Upload(uploadParams);
				}
			}

			photo.Url = uploadResult.Uri.ToString();
			photo.PublicId = uploadResult.PublicId;

			return _mapper.Map<Photo>(photo);
		}

		public bool DeletePhoto(string public_id)
		{
			var deleteParams = new DeletionParams(public_id);
			var result = _cloudinary.Destroy(deleteParams);

			return result.Result == "ok";
		}
	}
}
