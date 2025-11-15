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
        private readonly DateTime startTime = new DateTime(2025, 3, 17, 0, 27, 0);
        private bool isPinned = true;

        public MainWindow()
        {
            // 确保此方法存在于 MainWindow.g.i.cs 自动生成文件中
            InitializeComponent();
            UpdatePinIcon();

            // 默认不置顶，避免覆盖其他软件
            this.Topmost = false;

            // 设置窗口初始位置为屏幕右上角
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = SystemParameters.WorkArea.Right - this.Width;
            this.Top = SystemParameters.WorkArea.Top;

            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            // 添加右键菜单用于关闭
            var contextMenu = new ContextMenu();
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
                CountdownText.Text = $"我们一起 {diff.Days}天 {diff.Hours}小时 {diff.Minutes}分 {diff.Seconds}秒";
            }
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            // 移除置顶逻辑，无论钉住与否都不置顶
            UpdatePinIcon();
        }

        private void UpdatePinIcon()
        {
            if (PinIcon != null)
            {
                PinIcon.Text = isPinned ? "📌" : "📍";
            }
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isPinned && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}