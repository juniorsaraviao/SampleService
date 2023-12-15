using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using SampleService.Platforms.Android;
using System.Runtime.Versioning;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace SampleService
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const int requestCode = 0;
        readonly string notificationPermission = Manifest.Permission.PostNotifications;

        public static void Close()
        {
            Platform.CurrentActivity.FinishAffinity();
        }

        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestAppPermissions();
        }

        [System.Runtime.Versioning.SupportedOSPlatform("android23.0")]
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StopServiceHandler();
        }

        void StartServiceHandler()
        {
            var startServiceIntent = new Intent(this, typeof(CustomService));
            startServiceIntent.SetAction(StringConstants.ActionStartService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(startServiceIntent);
            }
            else
            {
                StartService(startServiceIntent);
            }
        }

        void StopServiceHandler()
        {
            var stopServiceIntent = new Intent(this, typeof(CustomService));
            stopServiceIntent.SetAction(StringConstants.ActionStopService);

            StopService(stopServiceIntent);
        }

        protected override void OnStop()
        {
            base.OnStop();
            StartServiceHandler();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StringConstants.Message = string.Empty;
        }

        async Task CheckAndRequestPermissions()
        {
            var permission = DependencyService.Get<IPostNotificationPermissionService>();

            await permission.CheckAndRequestPermissions();
        }

        void RequestAppPermissions()
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(23))
                return;
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                List<string> permissionArray = new List<string>();
                if (!NotificationPermissionGranted)
                    permissionArray.Add(notificationPermission);
                if (permissionArray.Any())
                    RequestPermissions(permissionArray.ToArray(), requestCode);

                return;
            }
        }
        [SupportedOSPlatform("android23.0")]
        bool NotificationPermissionGranted => CheckSelfPermission(notificationPermission) == Permission.Granted;
    }
}
