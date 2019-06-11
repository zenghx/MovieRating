using MaterialDesignThemes.Wpf;
using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieRating
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
            => System.Diagnostics.Debug.WriteLine("Card is flipped = " + e.NewValue);




        /// <summary>
        /// 利用visualtreehelper寻找对象的子级对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        List<T> FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> TList = new List<T> { };
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        TList.Add((T)child);
                    }
                    else
                    {
                        List<T> childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                }
                return TList;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return null;
            }
        }

        //修改电影信息按钮的功能实现，异步更新数据库
        private async void Modify_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var blockchildrenlist = FindVisualChild<TextBlock>(btn.TemplatedParent);//从按钮对象的TemplatedParent获取所有TextBlock
            var boxchildrenlist = FindVisualChild<TextBox>(btn.TemplatedParent);//从按钮对象的TemplatedParent获取所有TextBox
            #region 还原出各TextBlock和TextBox
            //正面的TextBlock
            var frontGenre = blockchildrenlist.Where(b => b.Name == "genre").FirstOrDefault();
            var frontRelease = blockchildrenlist.Where(b => b.Name == "releaseD").FirstOrDefault();
            var frontTitle = blockchildrenlist.Where(b => b.Name == "RTT").FirstOrDefault();
            //背面的TextBox
            var mTitle = boxchildrenlist.Where(b => b.Name == "mTitle").FirstOrDefault();
            var mGenre = boxchildrenlist.Where(b => b.Name == "mGenre").FirstOrDefault();
            var nReleaseD = boxchildrenlist.Where(b => b.Name == "nReleaseD").FirstOrDefault();
            //北面的TextBox，指示电影编号
            var movieId = blockchildrenlist.Where(b => b.Name == "movieId").FirstOrDefault();
            #endregion
            string[] genreList = new string[] { "动作","冒险","动画","儿童","喜剧","犯罪","纪录片","剧情",
                                                "奇幻","黑色", "恐怖", "音乐","悬疑","爱情","科幻","惊悚","未知","战争","西部"};
            char[] temp = new char[19];
            var thisgenre = mGenre.Text.Split('/');
            foreach (var genre in thisgenre)
            {
                temp[genreList.ToList().IndexOf(genre)] = '1';
            }
            if (frontGenre.Text != mGenre.Text)
            {
                frontGenre.Text = mGenre.Text;
            }

            if (frontRelease.Text.Split('：')[1] != nReleaseD.Text)
            {
                frontRelease.Text = nReleaseD.Text;
            }

            if (frontTitle.Text != mTitle.Text)
            {
                frontTitle.Text = mTitle.Text;
            }

            var id = Convert.ToInt32(movieId.Text.Split(':')[1]);
            using (var model = new RatingModel())
            {
                var itm = model.item.Where(i => i.movieId == id).FirstOrDefault();
                itm.movieTitle = mTitle.Text;
                itm.genre = new string(temp).Replace('\0', '0');
                itm.releaseDate = DateTime.Parse(nReleaseD.Text);
                model.Entry(itm).State = System.Data.Entity.EntityState.Modified;
                await model.SaveChangesAsync();
            }


        }

        //评分条上评分发生改变，最后一步更新数据库
        private async void RatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            if (Userinfo.currentUser != 37787)
            {
                var bar = sender as RatingBar;
                var mvId = (int)bar.Tag;//使用Tag获取到电影编号
                int OldRatingValue = 0;
                using (var model = new RatingModel())//从数据库中获取该用户对电影的评分
                {
                    var OldRatingEntity = model.ratings.Where(r => r.movieId == mvId).Where(r => r.userId == Userinfo.currentUser).FirstOrDefault();
                    if (OldRatingEntity != null)
                    {
                        OldRatingValue = OldRatingEntity.rating;
                    }
                }
                if (OldRatingValue == e.OldValue)//如果从数据库中获取到的评分和事件发生时的旧评分相同说明是用户手动更改评分而不是UI初始化时发生的评分变更
                {
                    using (var model = new RatingModel())
                    {

                        if (OldRatingValue == 0)//如果原评分为零说明是一个新条目
                        {
                            var rating = new ratings
                            {
                                movieId = mvId,
                                userId = Convert.ToInt16(Userinfo.currentUser),
                                rating = (byte)bar.Value,
                            };
                            model.ratings.Add(rating);
                        }
                        else//否则更新条目
                        {
                            var rating = model.ratings.Where(r => r.movieId == mvId).Where(r => r.userId == Userinfo.currentUser).FirstOrDefault();
                            rating.rating = (byte)bar.Value;
                            model.ratings.Attach(rating);
                            model.Entry(rating).State = System.Data.Entity.EntityState.Modified;
                        }
                        await model.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
