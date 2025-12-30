using System;
using ScheduleNotification.Models;
using ScheduleNotification.Views;

namespace ScheduleNotification.Services
{
    public class NotificationService
    {
        public event Action<Reminder>? OnComplete;
        public event Action<Reminder>? OnSnooze;
        public event Action<Reminder>? OnDismiss;

        public void Show(Reminder reminder)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new NotificationWindow(reminder);
                window.OnComplete += () => OnComplete?.Invoke(reminder);
                window.OnSnooze += () => OnSnooze?.Invoke(reminder);
                window.OnDismiss += () => OnDismiss?.Invoke(reminder);
                window.Show();
            });
        }
    }
}
