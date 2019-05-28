using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieRating
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public class Stateobj : INotifyPropertyChanged
    {
        public WindowState state { set; get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public partial class Login : Window
    {
        public Login()
        {
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            Stateobj stateobj = new Stateobj { state = WindowState.Normal };
            InitializeComponent();
            pth.DataContext = stateobj;

        }

        private void Window_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Name == "minW")
            { WindowState = WindowState.Minimized; }
            else if (btn.Name == "maxW")
            {
                if (WindowState == WindowState.Normal)
                {
                    Stateobj stateobj = new Stateobj { state = WindowState.Maximized };
                    pth.DataContext = stateobj;
                    BorderThickness = new Thickness(5);
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    Stateobj stateobj = new Stateobj { state = WindowState.Normal };
                    pth.DataContext = stateobj;
                    BorderThickness = new Thickness(10);
                    WindowState = WindowState.Normal;
                }
            }
            else if (btn.Name == "closeW")
            { this.Close(); }
        }

        private void DockPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
