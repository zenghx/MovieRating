using MovieRating.EntityFramework;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MovieRating
{
    /// <summary>
    /// LoginPopup.xaml 的交互逻辑
    /// </summary>
    /// 


    public partial class LoginPopup : UserControl
    {
        private MainWindow parentWindow;
        private Timer timer;
        private delegate void TimerDispatcherDelegate();
        public LoginPopup(MainWindow window)
        {
            parentWindow = window;
            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            InitializeComponent();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(OnTimed));

        private void OnTimed()
        {
            SnackBar.IsActive = false;
            timer.Enabled = false;
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (uid.Text == "0")
            {
                byte[] hash;
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(pwd.Password));
                }

                var res = BitConverter.ToString(hash);
                res = res.Replace("-", String.Empty).ToLower();
                if (res != Userinfo.adminpwd)
                {
                    SnackBar.IsActive = true;
                    timer.Enabled = true;
                }

                else
                {
                    Userinfo.currentUser = 0;
                    parentWindow.UserBinding();
                }
            }
            else
            {
                using (var model = new RatingModel())
                {
                    int maxuid = model.user.Max(u => u.userId);
                    try
                    {
                        if (Convert.ToInt32(uid.Text) > maxuid || Convert.ToInt32(uid.Text) < 0)
                        {
                            SnackBar.IsActive = true;
                            timer.Enabled = true;
                        }
                        else
                        {
                            Userinfo.currentUser = Convert.ToInt32(uid.Text);
                            parentWindow.UserBinding();
                        }
                    }
                    catch
                    {
                        SnackBar.IsActive = true;
                        timer.Enabled = true;
                    }

                }
            }

        }

    }
}
