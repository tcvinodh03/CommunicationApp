namespace CommunicationAPI.Interface
{
    public interface IUnitOfWork
    {
        IuserRepo userRepo { get; }
        IMessageRepo messageRepo { get; }
        ILikesRepo likesRepo { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
