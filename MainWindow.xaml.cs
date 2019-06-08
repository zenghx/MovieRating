using MaterialDesignThemes.Wpf;
using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace MovieRating
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private int lastSelectionIndex = 0;
        private LoginPopup login;
        private BackgroundWorker worker;

        public MainWindow()
        {
            DefaultStyleKey = typeof(MainWindow);
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
            InitializeComponent();
            IndexBinding();
        }

        private void IndexBinding()//不知道为什么这个不能用backgroundworker异步运行，加载框关不掉
        {

            using (var model = new RatingModel())
            {
                Random random = new Random();
                List<int> Id = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    Id.Add(random.Next(1, 1682));
                }

                var items = model.item.Include("ratings")
                    .Where(i => Id.Contains(i.movieId)).ToList();
                indexContent.DataContext = items;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
            {
                InvalidateMeasure();
            }
        }

        #region Window Commands

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            //SystemCommands.CloseWindow(this);
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }


        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null)
            {
                return;
            }

            var point = WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight)
                : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            var items = new List<item>();
            var loading = new Loading();
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += new DoWorkEventHandler((r, rs) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DialogHost.Show(loading);
                }));
                using (var model = new RatingModel())
                {
                    Random random = new Random();
                    List<int> Id = new List<int>();
                    for (int i = 0; i < 8; i++)
                    {
                        Id.Add(random.Next(1, 1682));
                    }

                    items = model.item.Include("ratings")
                        .Where(i => Id.Contains(i.movieId)).ToList();
                }
            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((r, rs) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    loading.close.Command?.Execute(loading.close.Command);
                }));
                indexContent.DataContext = items;

            });
            worker.RunWorkerAsync();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var loading = new Loading();
            var res = new List<item>();
            var searchstr = searchBox.Text;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler((s, ss) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DialogHost.Show(loading);
                }));
                using (var model = new RatingModel())
                {
                    res = model.item.Include("ratings").Where(it => it.movieTitle.ToUpper().Contains(searchstr.ToUpper())).ToList();//原来直接使用contains，导致区分大小写
                }
                //res = model.item.Include("ratings").Where(it => it.movieTitle.Contains(searchBox.Text)).ToList(); 

            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((r, rs) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    loading.close.Command?.Execute(loading.close.Command);
                }));
                if (res.Count > 10)
                {
                    searchRes.DataContext = res.Take(10);
                    ResString.Content = string.Format("共搜索到{0}条结果，已显示10条。", res.Count);
                }
                else
                {
                    searchRes.DataContext = res;
                    ResString.Content = String.Format("共搜索到{0}条结果，已全部显示。", res.Count);
                }
            });
            worker.RunWorkerAsync();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var ModifiedUser = new user
            {
                userId=(short)Userinfo.currentUser,
                occupationId = (byte)occupation.SelectedIndex,
                age = Convert.ToByte(age.Text),
                gender = gender.SelectedIndex == 0 ? "M" : "F",
                zipcode = zipcode.Text
            };
            using (var model = new RatingModel())
            {
                model.user.Attach(ModifiedUser);
                model.Entry(ModifiedUser).State = System.Data.Entity.EntityState.Modified;
                await model.SaveChangesAsync();
            }
            UserBinding();
        }

        public void UserBinding()
        {
            var loading = new Loading();
            List<item> items = new List<item>();
            user usr = new user();
            int ratedCount = 0;
            var occupList = new List<String>();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler((s, es) =>
            {
                using (var model = new RatingModel())
                {
                    //var itm = model.item.Include("ratings").SelectMany(it => it.ratings).Where(r => r.userId == Userinfo.currentUser).Select(r => r.item).Distinct().ToList();
                    var itm = model.item.Include("ratings")
                              .Where(it => it.ratings.Any(r => r.userId == Userinfo.currentUser))
                              .ToList();
                    //var itm = from p in model.item
                    //          join r in model.ratings on p.movieId equals r.movieId
                    //          where r.userId == Userinfo.currentUser
                    //          select p;
                    ratedCount = itm.Count;
                    Random random = new Random();
                    if (itm.Count < 11)
                    {
                        items = itm;
                        System.Threading.Thread.Sleep(1000);//如果数据量太少，就休眠一秒，防止loading的关闭事件在show之前发生
                    }

                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {

                            var oneitem = itm[random.Next(0, itm.Count - 1)];//随机选择一个电影条目
                            items.Add(oneitem);//添加到要绑定的列表中
                            itm.Remove(oneitem);//从查询结果中删除
                        }
                    }
                    usr = model.user.Find(Userinfo.currentUser);
                    var occupations = model.occupation;
                    foreach (var occup in occupations)
                    {
                        occupList.Add(occup.occupation1);
                    }
                }
            }
            );
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((s, es) =>
              {

                  
                  //loading.CloseMe();
                  myRating.DataContext = items;
                  loading.close.Command?.Execute(loading.close.Command);
                  expander.Header = String.Format("个人信息(uid:{0})", usr.userId);
                  ratingCount.Text = String.Format("我评过的影片(共{0}部，已显示{1}部)", ratedCount, items.Count);
                  zipcode.Text = usr.zipcode;
                  gender.SelectedIndex = (usr.gender == "M") ? 0 : 1;
                  age.Text = usr.age.ToString();
                  occupation.DataContext = occupList;
                  occupation.SelectedIndex = usr.occupationId;
                  tab.SelectedIndex = 2;                  
                  IndexBinding();
              });
            login.close.Command?.Execute(login.close.Command);
            DialogHost.Show(loading);
            worker.RunWorkerAsync();
        }

        private void Tab_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Userinfo.currentUser == 37787)
            {
                if (tab.SelectedIndex == 2)
                {
                    tab.SelectedIndex = lastSelectionIndex;
                    login = new LoginPopup(this);
                    DialogHost.Show(login, "RootDialog");
                }
                //tab.SelectedIndex = 2;

                else
                {
                    lastSelectionIndex = tab.SelectedIndex;
                }
            }


        }

        private void RefreshU_Click(object sender, RoutedEventArgs e) => UserBinding();
    }
}
