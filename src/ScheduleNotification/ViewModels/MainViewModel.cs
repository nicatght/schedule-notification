using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScheduleNotification.Models;
using ScheduleNotification.Services;

namespace ScheduleNotification.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly StorageService _storageService;

        public ObservableCollection<Reminder> Reminders { get; } = new();

        public MainViewModel(StorageService storageService)
        {
            _storageService = storageService;
            LoadReminders();
        }

        public void LoadReminders()
        {
            Reminders.Clear();
            var reminders = _storageService.Load();
            foreach (var reminder in reminders)
            {
                Reminders.Add(reminder);
            }
        }

        public void AddReminder(Reminder reminder)
        {
            Reminders.Add(reminder);
            SaveReminders();
        }

        public void RemoveReminder(Reminder reminder)
        {
            Reminders.Remove(reminder);
            SaveReminders();
        }

        public void SaveReminders()
        {
            _storageService.Save(new List<Reminder>(Reminders));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
