using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DesktopWidget
{
    public partial class SettingsWindow : Window
    {
        private AppSettings settings = null!;
        private MainWindow? mainWindow;

        public SettingsWindow()
        {
            InitializeComponent();
            
            // 设置窗口位置在屏幕中央
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // 允许拖动窗口
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            
            // 加载设置
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = AppSettings.Load();
            
            // 获取主窗口引用
            mainWindow = Application.Current.MainWindow as MainWindow;
            
            // 设置控件值
            ThemeColorTextBox.Text = settings.ThemeColor;
            ShowInTaskbarCheckBox.IsChecked = settings.ShowInTaskbar;
            StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
            IsTopmostCheckBox.IsChecked = settings.IsTopmost;
            
            // 设置各元素的透明度
            BackgroundOpacitySlider.Value = settings.BackgroundOpacity;
            BackgroundOpacityText.Text = $"{(int)(settings.BackgroundOpacity * 100)}%";
            
            TextOpacitySlider.Value = settings.TextOpacity;
            TextOpacityText.Text = $"{(int)(settings.TextOpacity * 100)}%";
            
            LeftAvatarOpacitySlider.Value = settings.LeftAvatarOpacity;
            LeftAvatarOpacityText.Text = $"{(int)(settings.LeftAvatarOpacity * 100)}%";
            
            RightAvatarOpacitySlider.Value = settings.RightAvatarOpacity;
            RightAvatarOpacityText.Text = $"{(int)(settings.RightAvatarOpacity * 100)}%";
            
            CenterHeartOpacitySlider.Value = settings.CenterHeartOpacity;
            CenterHeartOpacityText.Text = $"{(int)(settings.CenterHeartOpacity * 100)}%";
            
            // 设置圆角角度
            CornerRadiusSlider.Value = settings.CornerRadius;
            CornerRadiusText.Text = $"{(int)settings.CornerRadius}";
            
            // 设置新增控件的值
            SetComboBoxValue(FontSizeComboBox, settings.FontSize);
            SetComboBoxValue(PositionComboBox, settings.Position);
            ShowSecondsCheckBox.IsChecked = settings.ShowSeconds;
            
            // 设置自定义文本
            CustomTextTextBox.Text = settings.CustomText;
            
            // 设置时间控件的值
            StartDatePicker.SelectedDate = settings.StartTime.Date;
            StartTimeTextBox.Text = settings.StartTime.ToString("HH:mm:ss");
            
            // 设置头像路径
            LeftAvatarPathTextBox.Text = settings.LeftAvatarPath;
            RightAvatarPathTextBox.Text = settings.RightAvatarPath;
            
            // 设置头像名称
            LeftAvatarNameTextBox.Text = settings.LeftAvatarName;
            RightAvatarNameTextBox.Text = settings.RightAvatarName;
            
            // 设置头像模式选择
            LeftAvatarLocalModeRadioButton.IsChecked = settings.IsLeftAvatarLocal;
            LeftAvatarOnlineModeRadioButton.IsChecked = !settings.IsLeftAvatarLocal;
            RightAvatarLocalModeRadioButton.IsChecked = settings.IsRightAvatarLocal;
            RightAvatarOnlineModeRadioButton.IsChecked = !settings.IsRightAvatarLocal;
            
            // 添加事件处理
            BackgroundOpacitySlider.ValueChanged += (s, e) => BackgroundOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            TextOpacitySlider.ValueChanged += (s, e) => TextOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            LeftAvatarOpacitySlider.ValueChanged += (s, e) => LeftAvatarOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            RightAvatarOpacitySlider.ValueChanged += (s, e) => RightAvatarOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            CenterHeartOpacitySlider.ValueChanged += (s, e) => CenterHeartOpacityText.Text = $"{(int)(e.NewValue * 100)}%";
            CornerRadiusSlider.ValueChanged += (s, e) => CornerRadiusText.Text = $"{(int)e.NewValue}";
            
            // 添加头像模式选择事件处理
            LeftAvatarLocalModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(LeftAvatarPathTextBox, BrowseLeftAvatarButton, LeftAvatarLocalModeRadioButton.IsChecked == true);
            LeftAvatarOnlineModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(LeftAvatarPathTextBox, BrowseLeftAvatarButton, LeftAvatarLocalModeRadioButton.IsChecked == true);
            RightAvatarLocalModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(RightAvatarPathTextBox, BrowseRightAvatarButton, RightAvatarLocalModeRadioButton.IsChecked == true);
            RightAvatarOnlineModeRadioButton.Checked += (s, e) => UpdateAvatarPathTextBoxState(RightAvatarPathTextBox, BrowseRightAvatarButton, RightAvatarLocalModeRadioButton.IsChecked == true);
        }

        private void UpdateAvatarPathTextBoxState(TextBox pathTextBox, Button browseButton, bool isLocalMode)
        {
            pathTextBox.IsReadOnly = false; // 始终允许编辑，让用户可以输入本地路径或URL
            browseButton.IsEnabled = isLocalMode; // 本地模式时启用浏览按钮，否则禁用
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            var item = comboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == value);
            if (item != null)
            {
                comboBox.SelectedItem = item;
            }
        }

        

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取当前设置值
            settings.ThemeColor = ThemeColorTextBox.Text.Trim();
            settings.ShowInTaskbar = ShowInTaskbarCheckBox.IsChecked ?? false;
            settings.StartWithWindows = StartWithWindowsCheckBox.IsChecked ?? false;
            settings.IsTopmost = IsTopmostCheckBox.IsChecked ?? false;
            
            // 获取各元素的透明度设置
            settings.BackgroundOpacity = BackgroundOpacitySlider.Value;
            settings.TextOpacity = TextOpacitySlider.Value;
            settings.LeftAvatarOpacity = LeftAvatarOpacitySlider.Value;
            settings.RightAvatarOpacity = RightAvatarOpacitySlider.Value;
            settings.CenterHeartOpacity = CenterHeartOpacitySlider.Value;
            
            // 获取圆角角度设置
            settings.CornerRadius = CornerRadiusSlider.Value;
            
            // 获取字体大小
            if (FontSizeComboBox.SelectedItem is ComboBoxItem fontItem)
            {
                settings.FontSize = fontItem.Content.ToString() ?? "中";
            }
            
            // 获取位置
            if (PositionComboBox.SelectedItem is ComboBoxItem positionItem)
            {
                settings.Position = positionItem.Content.ToString() ?? "右上角";
            }
            
            // 获取显示秒数设置
            settings.ShowSeconds = ShowSecondsCheckBox.IsChecked ?? true;
            
            // 获取自定义文本
            settings.CustomText = CustomTextTextBox.Text.Trim();
            
            // 获取头像路径
            settings.LeftAvatarPath = LeftAvatarPathTextBox.Text.Trim();
            settings.RightAvatarPath = RightAvatarPathTextBox.Text.Trim();
            
            // 获取头像名称
            settings.LeftAvatarName = LeftAvatarNameTextBox.Text.Trim();
            settings.RightAvatarName = RightAvatarNameTextBox.Text.Trim();
            
            // 获取头像模式
            settings.IsLeftAvatarLocal = LeftAvatarLocalModeRadioButton.IsChecked == true;
            settings.IsRightAvatarLocal = RightAvatarLocalModeRadioButton.IsChecked == true;
            
            // 获取起始时间设置
            if (StartDatePicker.SelectedDate.HasValue)
            {
                var date = StartDatePicker.SelectedDate.Value;
                if (TimeSpan.TryParse(StartTimeTextBox.Text, out TimeSpan time))
                {
                    settings.StartTime = date.Date + time;
                }
                else
                {
                    MessageBox.Show("时间格式错误，请使用 HH:mm:ss 格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("请选择起始日期", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // 应用到主窗口
            if (mainWindow != null)
            {
                settings.ApplyToMainWindow(mainWindow);
            }
            
            // 处理开机自启动
            SetStartupWithWindows(settings.StartWithWindows);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 先应用设置
            ApplyButton_Click(sender, e);
            
            // 保存到文件
            settings.Save();
            
            MessageBox.Show("设置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void BrowseLeftAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择左下角头像",
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LeftAvatarPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseRightAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择右下角头像",
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                RightAvatarPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SetStartupWithWindows(bool enable)
        {
            try
            {
                var appName = "DesktopWidget";
                var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (enable)
                    {
                        key?.SetValue(appName, executablePath);
                    }
                    else
                    {
                        key?.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}