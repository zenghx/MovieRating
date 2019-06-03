using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace MovieRating
{
    public class Max_RestoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (WindowState)value;
            if (state == WindowState.Normal)
                if (parameter.ToString() == "RestoreButton")
                    return Visibility.Collapsed;
                else return Visibility.Visible;
            else if (parameter.ToString() == "RestoreButton")
                return Visibility.Visible;
            else return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Title2ImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tt = value.ToString().Split(new Char[] { '(', ')' });
            var URLTitle = tt[0].Trim().Replace("%", "%25").Replace("+", "%2B").Replace(' ', '+').Replace("&", "%26").Replace("#","%23").Replace("?","%3F").Replace("/","%2F").Replace("=","%3D");
           //对URL进行转义，将不能直接接收的字符转为相应的转义符，注意加号要先转然后就可以用加号代表空格,百分号一定要在最前替换
            try
            {
                var requesturl = "http://www.omdbapi.com/?apikey=85486115&r=xml&t=" + URLTitle + "&y=" + tt[tt.Length - 2];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturl);
                var response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string xmlUrl = reader.ReadToEnd();
                reader.Close();
                response.Close();
                //实例Xml文档
                XmlDocument xmlDoc = new XmlDocument();
                //从指定字符串载入xml文档
                xmlDoc.LoadXml(xmlUrl);

                string imagePath = xmlDoc.DocumentElement.FirstChild.Attributes["poster"].Value;
                if(imagePath=="N/A")
                    return "/Resources/noImg.png";
                return imagePath;

            }
            catch (Exception)
            {

                return "/Resources/noImg.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScoreCalculatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rt = value as HashSet<ratings>;
            double sum = rt.Sum(r => r.rating);
            double avg = sum / rt.Count;
            return string.Format("{0:N}", avg);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rt = value as HashSet<ratings>;
            return " "+rt.Count.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] genreList = new string[] { "动作","冒险","动画","儿童","喜剧","犯罪","纪录片","剧情",
                                                "奇幻","黑色", "恐怖", "音乐","悬疑","爱情","科幻","惊悚","未知","战争","西部"};
            var arr = value.ToString().ToCharArray();
            string genre="";
            for(int i=0;i<arr.Length;i++)
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

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "不详";
            var val = value.ToString();
            var spacepos = val.IndexOf(' ');
            var res= val.Substring(0, spacepos);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AdminConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Userinfo.IsAdmin())
                return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
