
namespace SampleService
{
    public interface IPostNotificationPermissionService
    {
        Task<bool> CheckAndRequestPermissions();
    }
}