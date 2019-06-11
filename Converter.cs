using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace MovieRating
{
    //最大化/还原转换器，根据窗口状态判断第标题栏第二个按钮显示还原还是最大化
    public class Max_RestoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (WindowState)value;
            if (state == WindowState.Normal)//如果当前窗口状态为Normal
            {
                if (parameter.ToString() == "RestoreButton")//调用converter的是还原按钮
                {
                    return Visibility.Collapsed;//不显示
                }
                else//不是还原按钮（即为最大化按钮）
                {
                    return Visibility.Visible;//显示
                }
            }
            else if (parameter.ToString() == "RestoreButton")//如果当前窗口为最小化或者最大化，调用converter的是还原按钮
            {
                return Visibility.Visible;//显示还原按钮
            }
            else//不是还原按钮（即为最大化按钮）
            {
                return Visibility.Collapsed;//不显示
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //使用openimdb的api通过电影标题获取电影海报图URL
    public class Title2ImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tt = value.ToString().Split(new Char[] { '(', ')' });//对传入的电影标题（标题 (年份)）形式进行分割，提取出年份和片名
            var URLTitle = tt[0].Trim().Replace("%", "%25").Replace("+", "%2B").Replace(' ', '+').Replace("&", "%26").Replace("#", "%23").Replace("?", "%3F").Replace("/", "%2F").Replace("=", "%3D");
            //对标题进行URL转义，将不能直接接收的字符转为相应的转义符，注意加号要先转然后就可以用加号代表空格,百分号一定要在最前替换
            string imagePath=string.Empty;
            string xmlUrl=string.Empty;
            try
            {
                var requesturl = "http://www.omdbapi.com/?apikey=85486115&r=xml&t=" + URLTitle + "&y=" + tt[tt.Length - 2];//拼接出api访问URL
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturl);
                request.Timeout = 2000;//设置超时为两秒
                using (var response = request.GetResponse())//获取响应
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        xmlUrl = reader.ReadToEnd();                //获取xml字符串
                    }
                }               
                //实例Xml文档
                XmlDocument xmlDoc = new XmlDocument();
                //从指定字符串载入xml文档
                xmlDoc.LoadXml(xmlUrl);
                //从xml中获取海报URL
                imagePath = xmlDoc.DocumentElement.FirstChild.Attributes["poster"].Value;
                if (imagePath == "N/A")//如果XML中说明没有海报
                {
                    imagePath = "/Resources/noImg.png";//使用默认图
                }
            }
            catch (Exception)//访问API出现错误（找不到片名等）
            {

                imagePath = "/Resources/noImg.png";//使用默认图
            }
            return imagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //计算电影平均分
    public class ScoreCalculatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rt = value as HashSet<ratings>;
            double sum = rt.Sum(r => r.rating);//调用Converter的电影评分总和
            double avg = sum / rt.Count;//计算平均分
            return string.Format("{0:N}", avg);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //计数评分人数
    public class RatingCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rt = value as HashSet<ratings>;
            return " " + rt.Count.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //将电影类型01字串转换为其类型
    public class GenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //顺序对应的类型数组
            string[] genreList = new string[] { "动作","冒险","动画","儿童","喜剧","犯罪","纪录片","剧情",
                                                "奇幻","黑色", "恐怖", "音乐","悬疑","爱情","科幻","惊悚","未知","战争","西部"};
            //将01字串打散到字符数组            
            var arr = value.ToString().ToCharArray();
            //根据字符数组中1所在位置选出电影类型
            string genre = "";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '1')
                {
                    genre += genreList[i];
                    genre += '/';
                }

            }
            genre = genre.TrimEnd('/');
            return genre;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //去掉从数据库读到的日期中的时分秒部分
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "不详";
            }

            var val = value.ToString();
            var spacepos = val.IndexOf(' ');
            var res = val.Substring(0, spacepos);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //判断是否为管理员，决定某些按钮的可见性
    public class AdminConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Userinfo.IsAdmin())
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //根据是否登录判断是否显示评分条
    public class LoginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Userinfo.currentUser == 37787)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //获取当前登录用户对当前电影的评分
    public class RatingBarValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Userinfo.currentUser == 37787)
            {
                return 0;
            }
            else
            {
                var rt = value as HashSet<ratings>;
                var myrating = rt.Where(r => r.userId == Userinfo.currentUser).FirstOrDefault();
                if (myrating != null)//有评分
                {
                    return myrating.rating;
                }
                else//无评分
                {
                    return 0;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
