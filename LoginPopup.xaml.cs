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
        private readonly MainWindow parentWindow;
        private readonly Timer timer;
        private delegate void TimerDispatcherDelegate();
        public LoginPopup(MainWindow window)
        {
            parentWindow = window;
            timer = new Timer(1000);//设置时间间隔为1秒
            timer.Elapsed += Timer_Elapsed;
            InitializeComponent();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(OnTimed));

        //时间到了关掉timer，关闭SnackBar
        private void OnTimed()
        {
            SnackBar.IsActive = false;
            timer.Enabled = false;
        }

        //登录按钮
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (uid.Text == "0")//如果是管理员，需要密码
            {
                byte[] hash;
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(pwd.Password));
                }//使用sha1算法计算输入密码的hsah

                var res = BitConverter.ToString(hash);
                res = res.Replace("-", String.Empty).ToLower();
                if (res != Userinfo.adminpwd)//如果不匹配，使用Snack bar提示密码错误，并在1秒后自动关闭
                {
                    SnackBar.IsActive = true;
                    timer.Enabled = true;
                }

                else//成功登录
                {
                    Userinfo.currentUser = 0;
                    parentWindow.UserBinding();
                    timer.Dispose();
                }
            }
            else//不是管理员，不需要密码
            {
                using (var model = new RatingModel())
                {
                    int maxuid = model.user.Max(u => u.userId);//获取最大用户id
                    try
                    {
                        if (Convert.ToInt32(uid.Text) > maxuid || Convert.ToInt32(uid.Text) < 0)//如果id输入不在数据库所有的uid范围内使用报错
                        {
                            SnackBar.IsActive = true;
                            timer.Enabled = true;
                        }
                        else//成功登录
                        {
                            Userinfo.currentUser = Convert.ToInt32(uid.Text);
                            parentWindow.UserBinding();
                            timer.Dispose();
                        }
                    }
                    catch//输入的uid不是数字
                    {
                        SnackBar.IsActive = true;
                        timer.Enabled = true;
                    }

                }
            }

        }
    }
}
