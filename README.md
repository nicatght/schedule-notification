# Schedule Notification

Windows 桌面提醒應用程式 - 使用 WPF 和 .NET 10 建立的排程通知工具

## 功能
- 新增/編輯/移除 提醒事項
- 提醒時間到時，顯示通知視窗，直到使用者將其關閉
- 提醒事項可選擇重複提醒（每日/每週/每月）與限制假日提醒
- 可備份提醒資料
- 可客製化提醒頁面（背景圖片）

## 依賴
- .NET 10.0 SDK (Windows)
- Visual Studio 2022 或 VS Code

## 快速開始
```bash
# 1. 還原套件
dotnet restore src/ScheduleNotification/ScheduleNotification.csproj

# 2. 建置專案
dotnet build src/ScheduleNotification/ScheduleNotification.csproj

# 3. 執行應用程式
dotnet run --project src/ScheduleNotification/ScheduleNotification.csproj
```

## 專案結構 (MVVM)
```
src/ScheduleNotification/
│
├── Models/                     # 資料結構
│   └── Reminder.cs
│
├── ViewModels/                 # 邏輯處理
│   └── MainViewModel.cs
│
├── Views/                      # UI 介面
│   ├── MainWindow.xaml
│   └── NotificationWindow.xaml
│
├── Services/                   # 背景服務
│   ├── StorageService.cs       #   存取 JSON 檔案
│   ├── SchedulerService.cs     #   定時檢查排程
│   ├── NotificationService.cs  #   顯示通知視窗
│   └── TrayService.cs          #   系統匣圖示
│
└── App.xaml
```

## 更多說明
- 應用程式會在系統匣顯示圖示
- 每分鐘檢查一次是否有到期的提醒
- 通知視窗會出現在螢幕右下角

## 開發人員
Stanley Huang