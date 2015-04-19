using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace MojioDotNet.Sample.Windows.Tasks
{
    public class VehicleObserverBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var toeastElement = notificationXml.GetElementsByTagName("text");
            toeastElement[0].AppendChild(notificationXml.CreateTextNode("This is Notification Message"));
            var toastNotification = new ToastNotification(notificationXml);
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }
    }
}