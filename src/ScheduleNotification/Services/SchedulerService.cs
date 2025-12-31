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

        // 記錄已經顯示過通知的提醒 ID，避免重複顯示
        private readonly HashSet<Guid> _notifiedIds = new();

        public SchedulerService(StorageService storageService, NotificationService notificationService)
        {
            _storageService = storageService;
            _notificationService = notificationService;

            // 每 10 秒檢查一次（比較即時）
            _timer = new System.Timers.Timer(10000);
            _timer.Elapsed += CheckReminders;
        }

        public void Start()
        {
            // 立即檢查一次
            CheckReminders(null, null!);
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        // 當使用者按下 Complete/Snooze/Dismiss 後，從已通知清單移除
        // 這樣 Snooze 之後時間到會再次通知
        public void ResetNotification(Guid reminderId)
        {
            _notifiedIds.Remove(reminderId);
        }

        private void CheckReminders(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var reminders = _storageService.Load();
            var now = DateTime.Now;

            foreach (var reminder in reminders)
            {
                // 條件：未完成、時間到了、還沒通知過
                if (!reminder.IsCompleted &&
                    reminder.DueTime <= now &&
                    !_notifiedIds.Contains(reminder.Id))
                {
                    // 標記為已通知
                    _notifiedIds.Add(reminder.Id);

                    // 顯示通知
                    _notificationService.Show(reminder);
                }
            }
        }
    }
}
