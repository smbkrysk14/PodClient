using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Xml;
using System.Xml.Linq;

namespace PodClient.Common
{
    public class Component
    {
        public Card CreateCardComponent(string imgUri)
        {
            Card card = new Card();

            Grid grid1 = new Grid();
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(140, GridUnitType.Pixel) });
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            // Grid.Row="0"
            Button button1 = new Button();
            button1.HorizontalAlignment = HorizontalAlignment.Right;
            button1.VerticalAlignment = VerticalAlignment.Bottom;
            button1.Margin = new Thickness(0, 0, 16, -20);
            PackIcon pk = new PackIcon();
            pk.Kind = PackIconKind.Bike;
            button1.Content = pk;

            Style s1 = new Style();
            //Uri uri = new Uri("../View/MyPodcast.xaml", UriKind.Relative);
            //StreamResourceInfo info = Application.GetResourceStream(uri);
            //XamlReader reader = new XamlReader();
            //var dictionary = reader.LoadAsync(info.Stream) as ResourceDictionary;
            //XmlReader xmlReader = XmlReader.Create(uri.LocalPath);
            //var dictionary = XamlReader.Load(xmlReader) as ResourceDictionary;
            //var element = dictionary["MaterialDesignFloatingActionMiniAccentButton"] as UIElement;

            


            button1.Style = s1;

            button1.SetValue(Grid.RowProperty, 0);

            // Grid.Row="1"
            StackPanel stackpanel1 = new StackPanel();
            stackpanel1.Margin = new Thickness(8, 24, 8, 0);
            TextBlock tb1 = new TextBlock();
            tb1.FontWeight = FontWeights.Bold;
            tb1.Text = "Cycling";
            TextBlock tb2 = new TextBlock();
            tb2.TextWrapping = TextWrapping.Wrap;
            tb2.Text = "A great way to keep fit and forget about the constant grind of IT.";

            stackpanel1.Children.Add(tb1);
            stackpanel1.Children.Add(tb2);
            stackpanel1.SetValue(Grid.RowProperty, 1);

            // Grid.Row="2"
            StackPanel stackpanel2 = new StackPanel();
            stackpanel2.HorizontalAlignment = HorizontalAlignment.Right;
            stackpanel2.Orientation = Orientation.Horizontal;
            stackpanel2.Margin = new Thickness(8);
            Button button2 = new Button();
            button2.Width = 30;
            button2.Padding = new Thickness(2, 0, 2, 0);
            PackIcon pk2 = new PackIcon();
            pk2.Kind = PackIconKind.ShareVariant;
            button2.Content = pk2;

            Button button3 = new Button();
            button3.Width = 30;
            button3.Padding = new Thickness(2, 0, 2, 0);
            PackIcon pk3 = new PackIcon();
            pk3.Kind = PackIconKind.Heart;
            button3.Content = pk3;

            stackpanel2.Children.Add(button2);
            stackpanel2.Children.Add(button3);
            stackpanel2.SetValue(Grid.RowProperty, 2);

            Image im = new Image();
            BitmapImage bi = new BitmapImage(new Uri(imgUri, UriKind.Relative));
            im.Source = bi;
            im.Height = 140;
            im.Width = 196;
            im.Stretch = Stretch.UniformToFill;

            grid1.Children.Add(im);
            grid1.Children.Add(button1);
            grid1.Children.Add(stackpanel1);
            grid1.Children.Add(stackpanel2);

            card.Content = grid1;
            card.Margin = new Thickness(5);

            return card;

        }
    }
}
