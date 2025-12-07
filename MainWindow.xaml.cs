using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DesktopWidget
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer;
        private DateTime startTime = new DateTime(2025, 3, 17, 0, 27, 0);
        private bool isPinned = true;
        public bool ShowSeconds { get; set; } = true;
        
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public string CustomText { get; set; } = "我们一起";
        public string LeftAvatarPath { get; set; } = "https://lxstu.sgstudio2025.xyz/1.png";
        public string RightAvatarPath { get; set; } = "https://lxstu.sgstudio2025.xyz/2.png";
        public string LeftAvatarName { get; set; } = "XXXXXXXX";
        public string RightAvatarName { get; set; } = "XXXXXXX";

        public MainWindow()
        {
            // 确保此方法存在于 MainWindow.g.i.cs 自动生成文件中
            InitializeComponent();

            // 加载并应用设置
            var settings = AppSettings.Load();
            settings.ApplyToMainWindow(this);

            // 设置窗口初始位置（如果设置中没有位置信息，则使用默认右上角）
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            // 添加右键菜单
            var contextMenu = new ContextMenu();
            
            var pinMenuItem = new MenuItem { Header = isPinned ? "取消固定" : "固定" };
            pinMenuItem.Click += PinMenuItem_Click;
            contextMenu.Items.Add(pinMenuItem);
            
            var settingsMenuItem = new MenuItem { Header = "设置" };
            settingsMenuItem.Click += SettingsMenuItem_Click;
            contextMenu.Items.Add(settingsMenuItem);
            
            var closeMenuItem = new MenuItem { Header = "关闭" };
            closeMenuItem.Click += (sender, e) => this.Close();
            contextMenu.Items.Add(closeMenuItem);
            
            this.ContextMenu = contextMenu;

            // 定时器{diff.Seconds}秒
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) => UpdateElapsed();
            timer.Start();
            UpdateElapsed();
        }

        private void UpdateElapsed()
        {
            var now = DateTime.Now;
            var diff = now - startTime;
            if (diff.TotalSeconds < 0)
            {
                CountdownText.Text = "尚未到达起始时间";
            }
            else
            {
                if (ShowSeconds)
                {
                    CountdownText.Text = $"{CustomText} {diff.Days}天 {diff.Hours}小时 {diff.Minutes}分 {diff.Seconds}秒";
                }
                else
                {
                    CountdownText.Text = $"{CustomText} {diff.Days}天 {diff.Hours}小时 {diff.Minutes}分";
                }
            }
        }

        

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
        }

        private void PinMenuItem_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            UpdatePinMenuItem();
        }

        private void UpdatePinMenuItem()
        {
            if (this.ContextMenu != null && this.ContextMenu.Items.Count > 0)
            {
                var pinMenuItem = this.ContextMenu.Items[0] as MenuItem;
                if (pinMenuItem != null)
                {
                    pinMenuItem.Header = isPinned ? "取消固定" : "固定";
                }
            }
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isPinned && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public void UpdateAvatars()
        {
            try
            {
                // 获取当前设置
                var settings = AppSettings.Load();
                
                // 更新左下角头像
                var leftImage = this.FindName("LeftAvatarImage") as Image;
                if (leftImage != null)
                {
                    var leftUri = GetImageUri(LeftAvatarPath, settings.IsLeftAvatarLocal);
                    if (leftUri != null)
                    {
                        leftImage.Source = new System.Windows.Media.Imaging.BitmapImage(leftUri);
                    }
                }

                // 更新右下角头像
                var rightImage = this.FindName("RightAvatarImage") as Image;
                if (rightImage != null)
                {
                    var rightUri = GetImageUri(RightAvatarPath, settings.IsRightAvatarLocal);
                    if (rightUri != null)
                    {
                        rightImage.Source = new System.Windows.Media.Imaging.BitmapImage(rightUri);
                    }
                }
                
                // 更新左下角头像名称
                var leftNameText = this.FindName("LeftAvatarNameText") as TextBlock;
                if (leftNameText != null)
                {
                    leftNameText.Text = LeftAvatarName;
                }
                
                // 更新右下角头像名称
                var rightNameText = this.FindName("RightAvatarNameText") as TextBlock;
                if (rightNameText != null)
                {
                    rightNameText.Text = RightAvatarName;
                }
            }
            catch
            {
                // 如果图片加载失败，使用默认图片
            }
        }

        private Uri? GetImageUri(string path, bool isLocal)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                if (isLocal)
                {
                    // 本地模式：检查文件是否存在
                    if (System.IO.File.Exists(path))
                    {
                        return new Uri(path);
                    }
                    else
                    {
                        // 如果本地文件不存在，尝试作为URI处理
                        if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                        {
                            return new Uri(path);
                        }
                        return null;
                    }
                }
                else
                {
                    // 在线模式：直接作为URI处理
                    if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                    {
                        return new Uri(path);
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}