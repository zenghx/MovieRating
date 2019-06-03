using System;
using System.Collections.Generic;
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
using MovieRating.EntityFramework;
using MaterialDesignThemes.Wpf;

namespace MovieRating
{
    /// <summary>
    /// LoginPopup.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPopup : UserControl
    {
        //private int mode;//指示触发登录框时所在的tab，并以此为模式进行不同操作
        public LoginPopup()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if(uid.Text=="0")
            {
                System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                byte[] combined = encoder.GetBytes(pwd.Password);
                hash.ComputeHash(combined);
                if (Convert.ToBase64String(hash.Hash) == Userinfo.adminpwd)
                {
                    Userinfo.currentUser = 0;
                    close.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));

                }
                else
                {
                    HintAssist.SetForeground(pwd, new SolidColorBrush(Colors.Red));
                    HintAssist.SetHint(pwd, "密码错误");
                }
            }
            else
            {
                using (var model = new RatingModel())
                {
                    int maxuid=model.user.Max(u => u.userId);
                    try
                    {
                        if (Convert.ToInt32(uid.Text) > maxuid || Convert.ToInt32(uid.Text) < 0)
                        {
                            HintAssist.SetForeground(uid, new SolidColorBrush(Colors.Red));
                            HintAssist.SetHint(uid, "ID有误");
                        }
                        else
                        {
                            Userinfo.currentUser = Convert.ToInt32(uid.Text);
                            close.Command?.Execute(close.Command);
                        }
                    }
                    catch
                    {
                        HintAssist.SetForeground(uid, new SolidColorBrush(Colors.Red));
                        HintAssist.SetHint(uid, "ID有误");
                    }

                }
            }

        }

    }
}
