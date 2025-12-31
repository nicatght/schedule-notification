using System;
using System.Linq;
using System.Windows.Threading;
using ScheduleNotification.Models;
using ScheduleNotification.ViewModels;

// 明確指定使用 WPF 的類別（避免和 WinForms 衝突）
using Window = System.Windows.Window;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using MessageBoxImage = System.Windows.MessageBoxImage;
using RoutedEventArgs = System.Windows.RoutedEventArgs;
using Visibility = System.Windows.Visibility;
using Button = System.Windows.Controls.Button;

namespace ScheduleNotification.Views
{
    public partial class MainWindow : Window
    {
        // ViewModel：負責處理資料邏輯
        private MainViewModel? _viewModel;

        // Timer：用來更新畫面上的時間
        private DispatcherTimer _timer = null!;

        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
        }

        // 設定計時器，每秒更新一次
        private void SetupTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 每秒執行一次
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        // 每秒執行：更新現在時間
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 更新現在時間
            txtCurrentTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // 更新「沒有提醒」的提示
            UpdateNoRemindersVisibility();
        }

        // 更新「沒有提醒」提示的顯示狀態
        private void UpdateNoRemindersVisibility()
        {
            if (_viewModel == null) return;

            if (_viewModel.Reminders.Count == 0)
            {
                txtNoReminders.Visibility = Visibility.Visible;
                lstReminders.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoReminders.Visibility = Visibility.Collapsed;
                lstReminders.Visibility = Visibility.Visible;
            }
        }

        // 讓外部（App.xaml.cs）可以設定 ViewModel
        public void SetViewModel(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            // 把 ViewModel 的 Reminders 綁定到 ListBox
            lstReminders.ItemsSource = _viewModel.Reminders;

            // 立即更新顯示
            UpdateNoRemindersVisibility();
        }

        // 當使用者按下「+ Add Reminder」按鈕時
        private void AddReminder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            // 開啟新增對話框
            var dialog = new AddReminderDialog();
            dialog.Owner = this; // 設定父視窗，讓對話框在主視窗中央

            // ShowDialog() 會等使用者關閉對話框才繼續
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                // 使用者按了 Save，加入新提醒
                _viewModel.AddReminder(dialog.Result);
                UpdateNoRemindersVisibility();
            }
        }

        // 當使用者按下「Edit」按鈕時
        private void EditReminder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            // 從按鈕的 Tag 取得 Reminder 的 Id
            var button = (Button)sender;
            var id = (Guid)button.Tag;

            // 找到對應的 Reminder
            var reminder = _viewModel.Reminders.FirstOrDefault(r => r.Id == id);
            if (reminder == null) return;

            // 開啟編輯對話框
            var dialog = new AddReminderDialog(reminder);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                // 使用者按了 Update，儲存變更
                _viewModel.SaveReminders();

                // 重新整理 ListBox（因為 Reminder 的屬性變了）
                lstReminders.Items.Refresh();
            }
        }

        // 當使用者按下「Delete」按鈕時
        private void DeleteReminder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            // 從按鈕的 Tag 取得 Reminder 的 Id
            var button = (Button)sender;
            var id = (Guid)button.Tag;

            // 找到對應的 Reminder
            var reminder = _viewModel.Reminders.FirstOrDefault(r => r.Id == id);
            if (reminder == null) return;

            // 確認刪除
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{reminder.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.RemoveReminder(reminder);
                UpdateNoRemindersVisibility();
            }
        }
    }
}
