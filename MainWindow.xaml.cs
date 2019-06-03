using MaterialDesignThemes.Wpf;
using MovieRating.EntityFramework;
using System;
using System.Collections.Generic;
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
        private void IndexBinding()
        {
            using (var model = new RatingModel())
            {
                Random random = new Random();
                List<int> Id = new List<int>();
                for (int i = 0; i < 8; i++)
                    Id.Add(random.Next(1, 1682));
                var items = model.item.Include("ratings")
                    .Where(i => Id.Contains(i.movieId)).ToList();
                indexContent.DataContext = items;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
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
                return;

            var point = WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight)
                : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion


        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            IndexBinding();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            using (var model = new RatingModel())
            {
                var res = model.item.Include("ratings").Where(it => it.movieTitle.Contains(searchBox.Text)).ToList();
                searchRes.DataContext = res;
            }
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserBinding()
        {
            using (var model = new RatingModel())
            {
                //var item = model.ratings.Include("item")
                //    .Where(r=>r.userId==Userinfo.currentUser).Select(i=>i.item).Distinct().ToList();
                var itm = from p in model.item
                          join r in model.ratings on p.movieId equals r.movieId
                          where r.userId == Userinfo.currentUser
                          select p;
                var ls = itm.ToList();
                Random random = new Random();
                List<item> items = new List<item>();
                for(int i=0;i<10;i++)
                {
                    items.Add(ls[random.Next(0,ls.Count-1)]);
                }
                myRating.DataContext = items;
                //var usr = model.user.Find(Userinfo.currentUser);
                //zipcode.Text = usr.zipcode;
            }
        }

        private void Tab_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(Userinfo.currentUser== 37787)
            { 
                if (tab.SelectedIndex == 2)
                {
                    tab.SelectedIndex = lastSelectionIndex;
                    var login = new LoginPopup ();
                    DialogHost.Show(login, "RootDialog");
                }
                //tab.SelectedIndex = 2;
                
                lastSelectionIndex = tab.SelectedIndex;
            }
            else
            {
                UserBinding();
            }
            
        }
    }
}
