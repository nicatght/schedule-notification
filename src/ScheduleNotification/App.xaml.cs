using ScheduleNotification.Services;
using ScheduleNotification.ViewModels;
using ScheduleNotification.Views;

namespace ScheduleNotification
{
    // 明確指定使用 WPF 的 Application（不是 WinForms 的）
    public partial class App : System.Windows.Application
    {
        // 宣告所有服務（在整個 App 生命週期中存活）
        private StorageService _storageService = null!;
        private NotificationService _notificationService = null!;
        private SchedulerService _schedulerService = null!;
        private TrayService _trayService = null!;
        private MainViewModel _mainViewModel = null!;

        // 當應用程式啟動時執行
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            // ===== 1. 建立所有服務 =====
            _storageService = new StorageService();           // 負責存取 JSON 檔案
            _notificationService = new NotificationService(); // 負責顯示通知視窗
            _schedulerService = new SchedulerService(         // 負責定時檢查提醒
                _storageService,
                _notificationService
            );
            _trayService = new TrayService();                 // 負責系統匣圖示
            _mainViewModel = new MainViewModel(_storageService); // 負責主視窗邏輯

            // ===== 2. 把 ViewModel 傳給 MainWindow =====
            // MainWindow 是由 App.xaml 的 StartupUri 自動建立的
            // 我們需要等它建立後再設定 ViewModel
            this.Activated += (s, args) =>
            {
                // 只在第一次啟動時設定
                if (MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SetViewModel(_mainViewModel);
                }
            };

            // ===== 3. 串接事件 =====

            // 當使用者按下通知的「Complete」按鈕時
            _notificationService.OnComplete += (reminder) =>
            {
                reminder.IsCompleted = true;  // 標記為已完成
                _mainViewModel.SaveReminders(); // 儲存到檔案
            };

            // 當使用者按下通知的「Snooze」按鈕時（延後 5 分鐘）
            _notificationService.OnSnooze += (reminder) =>
            {
                reminder.DueTime = System.DateTime.Now.AddMinutes(5); // 延後 5 分鐘
                _mainViewModel.SaveReminders(); // 儲存到檔案

                // 重置通知狀態，這樣 5 分鐘後會再次通知
                _schedulerService.ResetNotification(reminder.Id);
            };

            // 當使用者按下通知的「Dismiss」按鈕時（暫時關閉）
            _notificationService.OnDismiss += (reminder) =>
            {
                // Dismiss 不做任何事，不會再通知（除非使用者編輯時間）
            };

            // 當使用者點擊系統匣「Open Settings」時
            _trayService.OnOpenSettings += () =>
            {
                MainWindow?.Show();      // 顯示主視窗
                MainWindow?.Activate();  // 把視窗帶到最前面
            };

            // 當使用者點擊系統匣「Exit」時
            _trayService.OnExit += () =>
            {
                _trayService.Dispose();   // 清除系統匣圖示
                _schedulerService.Stop(); // 停止排程器
                Shutdown();               // 關閉應用程式
            };

            // ===== 3. 啟動排程器 =====
            _schedulerService.Start(); // 開始每分鐘檢查提醒
        }

        // 當應用程式關閉時執行
        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            _trayService?.Dispose();   // 清除系統匣圖示
            _schedulerService?.Stop(); // 停止排程器
            base.OnExit(e);
        }
    }
}
