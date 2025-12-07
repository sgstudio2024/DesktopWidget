using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopWidget
{
    public class AppSettings
    {
        public string ThemeColor { get; set; } = "#AA222222";
        public double Opacity { get; set; } = 0.7;
        public bool ShowInTaskbar { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public bool IsTopmost { get; set; } = false;
        public string FontSize { get; set; } = "中";
        public string Position { get; set; } = "右上角";
        public bool ShowSeconds { get; set; } = true;
        public DateTime StartTime { get; set; } = new DateTime(2025, 3, 17, 0, 27, 0);
        public string CustomText { get; set; } = "我们一起";
        public string LeftAvatarPath { get; set; } = "https://lxstu.sgstudio2025.xyz/1.png";
        public string RightAvatarPath { get; set; } = "https://lxstu.sgstudio2025.xyz/2.png";
        public string LeftAvatarName { get; set; } = "XX";
        public string RightAvatarName { get; set; } = "XX";
        public bool IsLeftAvatarLocal { get; set; } = true;  // true=本地模式, false=在线模式
        public bool IsRightAvatarLocal { get; set; } = true; // true=本地模式, false=在线模式
        
        // 各元素的透明度设置
        public double BackgroundOpacity { get; set; } = 0.7;
        public double TextOpacity { get; set; } = 1.0;
        public double LeftAvatarOpacity { get; set; } = 1.0;
        public double RightAvatarOpacity { get; set; } = 1.0;
        public double CenterHeartOpacity { get; set; } = 1.0;
        public double CornerRadius { get; set; } = 16;

        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DesktopWidget",
            "settings.json"
        );

        public static AppSettings Load()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var options = new JsonSerializerOptions 
                    { 
                        Converters = { new DateTimeJsonConverter() }
                    };
                    return JsonSerializer.Deserialize<AppSettings>(json, options) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载设置失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Converters = { new DateTimeJsonConverter() }
                };
                var json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApplyToMainWindow(MainWindow mainWindow)
        {
            if (mainWindow == null) return;

            // 应用透明度（只影响背景）
            ApplyOpacity(mainWindow);

            // 应用任务栏显示
            mainWindow.ShowInTaskbar = ShowInTaskbar;

            // 应用置顶
            mainWindow.Topmost = IsTopmost;

            // 应用主题颜色
            ApplyThemeColor(mainWindow);

            // 应用字体大小
            ApplyFontSize(mainWindow);

            // 应用位置
            ApplyPosition(mainWindow);

            // 应用显示秒数设置
            ApplyShowSeconds(mainWindow);

            // 应用起始时间
            ApplyStartTime(mainWindow);

            // 应用自定义文本和头像
            ApplyCustomTextAndAvatars(mainWindow);

            // 应用圆角设置
            ApplyCornerRadius(mainWindow);
        }

        private void ApplyOpacity(MainWindow mainWindow)
        {
            // 窗口本身保持不透明
            mainWindow.Opacity = 1.0;
            
            // 应用背景透明度
            var backgroundBorder = mainWindow.FindName("BackgroundBorder") as Border;
            if (backgroundBorder != null)
            {
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ThemeColor);
                var brush = new System.Windows.Media.SolidColorBrush(color);
                brush.Opacity = BackgroundOpacity;
                backgroundBorder.Background = brush;
            }
            
            // 应用文字透明度
            var countdownText = mainWindow.FindName("CountdownText") as TextBlock;
            if (countdownText != null)
            {
                countdownText.Opacity = TextOpacity;
            }
            
            // 应用左下角头像透明度
            var leftAvatar = mainWindow.FindName("LeftAvatarImage") as Image;
            if (leftAvatar != null)
            {
                leftAvatar.Opacity = LeftAvatarOpacity;
            }
            
            // 应用右下角头像透明度
            var rightAvatar = mainWindow.FindName("RightAvatarImage") as Image;
            if (rightAvatar != null)
            {
                rightAvatar.Opacity = RightAvatarOpacity;
            }
            
            // 应用中间爱心透明度
            var centerHeart = mainWindow.FindName("CenterHeartImage") as Image;
            if (centerHeart != null)
            {
                centerHeart.Opacity = CenterHeartOpacity;
            }
        }

        private void ApplyThemeColor(MainWindow mainWindow)
        {
            // 主题颜色现在在ApplyOpacity中处理
        }

        private void ApplyFontSize(MainWindow mainWindow)
        {
            double fontSize = FontSize switch
            {
                "小" => 12,
                "大" => 18,
                _ => 14 // 中
            };

            // 查找并设置文本字体大小
            if (mainWindow.FindName("CountdownText") is System.Windows.Controls.TextBlock textBlock)
            {
                textBlock.FontSize = fontSize;
            }
        }

        private void ApplyPosition(MainWindow mainWindow)
        {
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;
            double windowWidth = mainWindow.Width;
            double windowHeight = mainWindow.Height;

            switch (Position)
            {
                case "左上角":
                    mainWindow.Left = SystemParameters.WorkArea.Left;
                    mainWindow.Top = SystemParameters.WorkArea.Top;
                    break;
                case "右上角":
                    mainWindow.Left = SystemParameters.WorkArea.Right - windowWidth;
                    mainWindow.Top = SystemParameters.WorkArea.Top;
                    break;
                case "左下角":
                    mainWindow.Left = SystemParameters.WorkArea.Left;
                    mainWindow.Top = SystemParameters.WorkArea.Bottom - windowHeight;
                    break;
                case "右下角":
                    mainWindow.Left = SystemParameters.WorkArea.Right - windowWidth;
                    mainWindow.Top = SystemParameters.WorkArea.Bottom - windowHeight;
                    break;
                case "中央":
                    mainWindow.Left = SystemParameters.WorkArea.Left + (screenWidth - windowWidth) / 2;
                    mainWindow.Top = SystemParameters.WorkArea.Top + (screenHeight - windowHeight) / 2;
                    break;
            }
        }

        private void ApplyShowSeconds(MainWindow mainWindow)
        {
            // 这个设置会在MainWindow的UpdateElapsed方法中使用
            // 通过设置一个公共属性来控制是否显示秒数
            var type = typeof(MainWindow);
            var property = type.GetProperty("ShowSeconds");
            property?.SetValue(mainWindow, ShowSeconds);
        }

        private void ApplyStartTime(MainWindow mainWindow)
        {
            // 设置起始时间
            var type = typeof(MainWindow);
            var property = type.GetProperty("StartTime");
            property?.SetValue(mainWindow, StartTime);
        }

        public void ApplyCustomTextAndAvatars(MainWindow mainWindow)
        {
            // 设置自定义文本
            var type = typeof(MainWindow);
            var property = type.GetProperty("CustomText");
            property?.SetValue(mainWindow, CustomText);

            // 设置头像路径
            var leftAvatarProperty = type.GetProperty("LeftAvatarPath");
            leftAvatarProperty?.SetValue(mainWindow, LeftAvatarPath);

            var rightAvatarProperty = type.GetProperty("RightAvatarPath");
            rightAvatarProperty?.SetValue(mainWindow, RightAvatarPath);

            // 设置头像名称
            var leftAvatarNameProperty = type.GetProperty("LeftAvatarName");
            leftAvatarNameProperty?.SetValue(mainWindow, LeftAvatarName);

            var rightAvatarNameProperty = type.GetProperty("RightAvatarName");
            rightAvatarNameProperty?.SetValue(mainWindow, RightAvatarName);

            // 更新头像显示
            mainWindow.UpdateAvatars();
        }

        private void ApplyCornerRadius(MainWindow mainWindow)
        {
            // 获取背景边框并应用圆角
            var backgroundBorder = mainWindow.FindName("BackgroundBorder") as Border;
            if (backgroundBorder != null)
            {
                backgroundBorder.CornerRadius = new System.Windows.CornerRadius(CornerRadius);
            }
        }

        // 保存设置到MainWindow，包含头像模式
        public void SaveToMainWindow(MainWindow mainWindow)
        {
            if (mainWindow == null) return;

            // 由于MainWindow没有直接的属性来存储头像模式，我们通过AppSettings来访问
            // 这里我们确保mainwindow中的路径设置正确
            mainWindow.LeftAvatarPath = this.LeftAvatarPath;
            mainWindow.RightAvatarPath = this.RightAvatarPath;
            mainWindow.LeftAvatarName = this.LeftAvatarName;
            mainWindow.RightAvatarName = this.RightAvatarName;
        }
    }

    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return new DateTime(2025, 3, 17, 0, 27, 0);
            }
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}