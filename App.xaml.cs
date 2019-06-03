using System.Windows;

namespace MovieRating
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) 
            => System.Diagnostics.Debug.WriteLine("Card is flipped = " + e.NewValue);

        private void RatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
