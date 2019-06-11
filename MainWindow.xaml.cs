using MaterialDesignThemes.Wpf;
using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            //添加窗口命令
            DefaultStyleKey = typeof(MainWindow);
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
            InitializeComponent();
            IndexBinding();
        }


        #region 自定义窗口
        //响应拖动事件
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

        #region 实现窗口命令

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
            if (!(e.OriginalSource is FrameworkElement element))
            {
                return;
            }

            var point = WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight)
                : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion

        #endregion

        //首页数据绑定，同步加载
        private void IndexBinding()//不知道为什么这个不能用backgroundworker异步运行，加载框关不掉
        {

            using (var model = new RatingModel())
            {
                //Random random = new Random();
                //List<int> Id = new List<int>();
                //for (int i = 0; i < 8; i++)
                //{
                //    Id.Add(random.Next(1, 1682));
                //}

                var items = model.item.Include("ratings").OrderBy(i => Guid.NewGuid())
                    .Take(8).ToList();//从数据库随机获取8个条目
                indexContent.DataContext = items;//显示到首页
            }

        }

        //再次随机获取8个条目到首页，异步加载
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
                using(var model=new RatingModel())
                {
                    items = model.item.Include("ratings").OrderBy(i => Guid.NewGuid())
                    .Take(8).ToList();
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

        //实现搜索功能，显示最多十个条目，异步加载
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var loading = new Loading();
            var res = new List<item>();
            var searchstr = searchBox.Text;
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += new DoWorkEventHandler((s, ss) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DialogHost.Show(loading);
                }));
                using (var model = new RatingModel())
                {
                    res = model.item.Include("ratings").Where(it => it.movieTitle.ToUpper().Contains(searchstr.ToUpper())).Take(10).ToList();//原来直接使用contains，导致区分大小写
                }
                //res = model.item.Include("ratings").Where(it => it.movieTitle.Contains(searchBox.Text)).ToList(); 

            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((r, rs) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    loading.close.Command?.Execute(loading.close.Command);
                }));
                searchRes.DataContext = res; 
                ResString.Content = string.Format("共搜索到{0}条结果，已显示10条。", res.Count);
            });
            worker.RunWorkerAsync();
        }

        //个人信息修改保存，异步提交到数据库，并刷新“我的”页面
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var ModifiedUser = new user
            {
                userId = (short)Userinfo.currentUser,
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

        //登录后异步进行数据绑定
        public void UserBinding()
        {
            var loading = new Loading();
            List<item> items = new List<item>();
            user usr = new user();
            int ratedCount = 0;
            var occupList = new List<String>();
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += new DoWorkEventHandler((s, es) =>
            {
                using (var model = new RatingModel())
                {
                    //var itm = model.item.Include("ratings").SelectMany(it => it.ratings).Where(r => r.userId == Userinfo.currentUser).Select(r => r.item).Distinct().ToList();
                    var itm = model.item.Include("ratings")
                              .Where(it => it.ratings.Any(r => r.userId == Userinfo.currentUser))
                              .OrderBy(r=>Guid.NewGuid()).Take(10)
                              .ToList();
                    //var itm = from p in model.item
                    //          join r in model.ratings on p.movieId equals r.movieId
                    //          where r.userId == Userinfo.currentUser
                    //          select p;
                    ratedCount = itm.Count;
                    //if (itm.Count < 11)
                    //{
                        items = itm;
                        System.Threading.Thread.Sleep(1000);//如果数据量太少，就休眠一秒，防止loading的关闭事件在show之前发生
                    //}

                    //else
                    //{

                    //    //for (int i = 0; i < 10; i++)
                    //    //{

                    //    //    var oneitem = itm[random.Next(0, itm.Count - 1)];//随机选择一个电影条目
                    //    //    items.Add(oneitem);//添加到要绑定的列表中
                    //    //    itm.Remove(oneitem);//从查询结果中删除
                    //    //}
                    //}
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

        //第一次选中“我的”页，触发登录事件
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

        //刷新“我的”页
        private void RefreshU_Click(object sender, RoutedEventArgs e) => UserBinding();
    }
}
