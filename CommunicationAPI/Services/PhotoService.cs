using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.Extensions.Options;

namespace CommunicationAPI.Services
{
    public class PhotoService : IPhotoService
    {
        public readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {

            var acc = new Account(
                config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            try
            {

           
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "VI-01"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<DeletionResult> DeletePhotoAsync(string photoId)
        {
            var deleteParams = new DeletionParams(photoId);
            return await _cloudinary.DestroyAsync(deleteParams);

        }
    }
}
