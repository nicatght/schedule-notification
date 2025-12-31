using System;
using System.Linq;
using ScheduleNotification.Models;

// WPF 類別別名（避免和 WinForms 衝突）
using Window = System.Windows.Window;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using RoutedEventArgs = System.Windows.RoutedEventArgs;

namespace ScheduleNotification.Views
{
    public partial class AddReminderDialog : Window
    {
        // 儲存編輯結果，讓呼叫者可以取得
        public Reminder? Result { get; private set; }

        // 是否為編輯模式（true = 編輯現有提醒，false = 新增）
        private readonly bool _isEditMode;

        // 原本的 Reminder（編輯模式用）
        private readonly Reminder? _originalReminder;

        // 建構子：新增模式
        public AddReminderDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            InitializeControls();
        }

        // 建構子：編輯模式
        public AddReminderDialog(Reminder reminder)
        {
            InitializeComponent();
            _isEditMode = true;
            _originalReminder = reminder;
            this.Title = "Edit Reminder"; // 改標題
            btnSave.Content = "Update";   // 改按鈕文字
            InitializeControls();
            LoadReminderData(reminder);   // 載入現有資料
        }

        // 初始化下拉選單
        private void InitializeControls()
        {
            // 設定日期預設為今天
            dpDate.SelectedDate = DateTime.Today;

            // 填入小時選項 (00-23)
            for (int i = 0; i < 24; i++)
            {
                cboHour.Items.Add(i.ToString("D2")); // D2 = 兩位數，例如 "09"
            }

            // 填入分鐘選項 (00, 05, 10, ..., 55)
            for (int i = 0; i < 60; i += 5)
            {
                cboMinute.Items.Add(i.ToString("D2"));
            }

            // 預設時間：現在時間往後 1 小時（取最近的 5 分鐘）
            var defaultTime = DateTime.Now.AddHours(1);
            cboHour.SelectedItem = defaultTime.Hour.ToString("D2");

            // 找最近的 5 分鐘
            int roundedMinute = (defaultTime.Minute / 5) * 5;
            cboMinute.SelectedItem = roundedMinute.ToString("D2");
        }

        // 載入現有提醒的資料（編輯模式）
        private void LoadReminderData(Reminder reminder)
        {
            txtTitle.Text = reminder.Title;
            txtDescription.Text = reminder.Description;
            dpDate.SelectedDate = reminder.DueTime.Date;
            cboHour.SelectedItem = reminder.DueTime.Hour.ToString("D2");

            // 找最近的 5 分鐘選項
            int roundedMinute = (reminder.DueTime.Minute / 5) * 5;
            cboMinute.SelectedItem = roundedMinute.ToString("D2");

            // 重複設定
            if (reminder.Repeat != RepeatType.None)
            {
                chkRepeat.IsChecked = true;
                cboRepeatType.SelectedIndex = (int)reminder.Repeat - 1; // -1 因為 None=0
            }
        }

        // 當「Repeat」CheckBox 變動時
        private void ChkRepeat_Changed(object sender, RoutedEventArgs e)
        {
            // 顯示或隱藏重複選項
            pnlRepeatOptions.Visibility = chkRepeat.IsChecked == true
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }

        // 按下「Save」按鈕
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 驗證 Title
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return;
            }

            // 驗證日期
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 驗證時間
            if (cboHour.SelectedItem == null || cboMinute.SelectedItem == null)
            {
                MessageBox.Show("Please select a time.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 組合日期和時間
            int hour = int.Parse((string)cboHour.SelectedItem);
            int minute = int.Parse((string)cboMinute.SelectedItem);
            DateTime dueTime = dpDate.SelectedDate.Value.AddHours(hour).AddMinutes(minute);

            // 檢查時間是否在未來
            if (dueTime <= DateTime.Now)
            {
                MessageBox.Show("Please select a future time.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 取得重複類型
            RepeatType repeatType = RepeatType.None;
            if (chkRepeat.IsChecked == true)
            {
                repeatType = (RepeatType)(cboRepeatType.SelectedIndex + 1); // +1 因為 None=0
            }

            // 建立或更新 Reminder
            if (_isEditMode && _originalReminder != null)
            {
                // 編輯模式：更新現有的
                _originalReminder.Title = txtTitle.Text;
                _originalReminder.Description = txtDescription.Text;
                _originalReminder.DueTime = dueTime;
                _originalReminder.Repeat = repeatType;
                Result = _originalReminder;
            }
            else
            {
                // 新增模式：建立新的
                Result = new Reminder
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    DueTime = dueTime,
                    Repeat = repeatType
                };
            }

            // 關閉視窗，回傳成功
            this.DialogResult = true;
            this.Close();
        }

        // 按下「Cancel」按鈕
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
