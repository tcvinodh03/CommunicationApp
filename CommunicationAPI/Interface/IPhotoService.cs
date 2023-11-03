using CloudinaryDotNet.Actions;

namespace CommunicationAPI.Interface
{
    public interface IPhotoService
    {
        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        public Task<DeletionResult> DeletePhotoAsync(string photoId);
    }
}
