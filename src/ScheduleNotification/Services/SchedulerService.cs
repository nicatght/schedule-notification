using System;
using System.Collections.Generic;
using ScheduleNotification.Models;

namespace ScheduleNotification.Services
{
    public class SchedulerService
    {
        private readonly System.Timers.Timer _timer;
        private readonly StorageService _storageService;
        private readonly NotificationService _notificationService;

        public SchedulerService(StorageService storageService, NotificationService notificationService)
        {
            _storageService = storageService;
            _notificationService = notificationService;

            _timer = new System.Timers.Timer(60000); // check every minute
            _timer.Elapsed += CheckReminders;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void CheckReminders(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var reminders = _storageService.Load();
            var now = DateTime.Now;

            foreach (var reminder in reminders)
            {
                if (!reminder.IsCompleted && reminder.DueTime <= now)
                {
                    _notificationService.Show(reminder);
                }
            }
        }
    }
}
