using System;
using System.Drawing;
using System.Windows.Forms;
using ScheduleNotification.Views;

namespace ScheduleNotification.Services
{
    public class TrayService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        public event Action? OnOpenSettings;
        public event Action? OnExit;

        public TrayService()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Schedule Notification"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open Settings", null, (s, e) => OnOpenSettings?.Invoke());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => OnExit?.Invoke());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => OnOpenSettings?.Invoke();
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}
