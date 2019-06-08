﻿using System.Windows.Controls;

namespace MovieRating
{
    /// <summary>
    /// Loading.xaml 的交互逻辑
    /// </summary>
    public partial class Loading : UserControl
    {
        public Loading()
        {
            InitializeComponent();
        }

        public int CloseMe()
        {
            close.Command.Execute(close.Command);
            return 1;
        }
    }
}
