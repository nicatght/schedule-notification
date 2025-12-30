using System;
using System.Windows;
using ScheduleNotification.Models;

namespace ScheduleNotification.Views
{
    public partial class NotificationWindow : Window
    {
        public event Action? OnComplete;
        public event Action? OnSnooze;
        public event Action? OnDismiss;

        public NotificationWindow(Reminder reminder)
        {
            InitializeComponent();

            txtTitle.Text = reminder.Title;
            txtDescription.Text = reminder.Description;

            // Position at bottom right of screen
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 20;
            Top = workArea.Bottom - Height - 20;
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            OnComplete?.Invoke();
            Close();
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            OnSnooze?.Invoke();
            Close();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            OnDismiss?.Invoke();
            Close();
        }
    }
}
