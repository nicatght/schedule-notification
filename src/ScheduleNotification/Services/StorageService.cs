using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ScheduleNotification.Models;

namespace ScheduleNotification.Services
{
    public class StorageService
    {
        private readonly string _filePath;

        public StorageService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appData, "ScheduleNotification");
            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, "reminders.json");
        }

        public List<Reminder> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Reminder>();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Reminder>>(json) ?? new List<Reminder>();
        }

        public void Save(List<Reminder> reminders)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(reminders, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
