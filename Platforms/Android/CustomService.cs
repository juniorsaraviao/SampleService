using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;

namespace SampleService.Platforms.Android
{
    [Service]
    public class CustomService : Service
    {
        const string CHANNEL_ID = "default";
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 100;

        bool isStarted;

        static void CreateChannel(NotificationManager notificationManager, string channelId, string chanName)
        {
            var chan = new NotificationChannel(channelId, chanName, NotificationImportance.Low);
            chan.EnableVibration(false);
            chan.LockscreenVisibility = NotificationVisibility.Public;

            notificationManager.CreateNotificationChannel(chan);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent == null || intent.Action?.Equals(StringConstants.ActionStartService) == true)
            {
                if (!isStarted)
                {
                    RegisterForegroundService();
                    isStarted = true;
                }
            }
            else if (intent?.Action?.Equals(StringConstants.ActionStopApp) == true)
            {
                StopApp();
            }
            else if (intent?.Action?.Equals(StringConstants.ActionStopService) == true)
            {
                Stop();
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        async void StopApp()
        {
            MainActivity.Close();

            Stop();
        }

        void Stop()
        {
            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            StopForeground(true);
            StopSelf();
            isStarted = false;
        }

        public override void OnDestroy()
        {
            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);

            base.OnDestroy();
        }

        void RegisterForegroundService()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string chanName = "Default";
                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                CreateChannel(notificationManager, CHANNEL_ID, chanName);
            }

            var icon = Resources!.GetIdentifier("dotnet_bot", "drawable", PackageName);
            Bitmap bm = BitmapFactory.DecodeResource(Resources, icon);

            var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Service")
                .SetContentText("This is a sample devices")
                .SetSmallIcon(icon)
                .SetLargeIcon(bm)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .AddAction(BuildActionIntentToStopServiceHandler())
                .Build();

            notification.Flags = NotificationFlags.OngoingEvent | NotificationFlags.NoClear;

            // Enlist this instance of the service as a foreground service
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            return PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        }

        NotificationCompat.Action BuildActionIntentToStopServiceHandler()
        {
            var notificationIntent = new Intent(this, typeof(CustomService));
            notificationIntent.SetAction(StringConstants.ActionStopApp);

            var stopServicePendingIntent = PendingIntent.GetService(this, 0, notificationIntent, PendingIntentFlags.Immutable);

            var builder = new NotificationCompat.Action.Builder(0, "Turn off", stopServicePendingIntent);

            return builder.Build();
        }

        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }
    }
}
