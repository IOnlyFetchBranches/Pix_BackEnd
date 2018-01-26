using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pix_Backend
{
    //This class is the container for dataviews!
    class ContentView : StackPanel
    {
        //The label that is showed at the top of the View
        private  Label contentLabel = new Label();

        private Border baseBorder;
        
        public ContentView()

        {
         

            //Surround a stackpane with border
            baseBorder = new Border();
            //Surrond it here
            baseBorder.Child = this; 
            //Style it
            baseBorder.CornerRadius = new CornerRadius(1);
            //Padding
            baseBorder.Padding = new Thickness(1, 1, 1, 1);
            //Alignment
            baseBorder.HorizontalAlignment = HorizontalAlignment.Center;
            baseBorder.VerticalAlignment = VerticalAlignment.Center;
        



            //Set color of border

            baseBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            //Set thickness of borders
            baseBorder.BorderThickness = new Thickness(1,1,1,1);
            //Set H/W of Border
            baseBorder.Height = this.Height;
            baseBorder.Width = this.Width;

            //Handle Label
            contentLabel.Content = "DEFAULTLABEL";
            //Style it a bit lol
            contentLabel.HorizontalAlignment = HorizontalAlignment.Center;

            //Set Margins
            contentLabel.Margin = new Thickness(1, 1, 1, 15);

            //Add Label as Header
            this.Children.Add(contentLabel);

            //Disable Drop
            this.AllowDrop = false;
            //Not Focusable
            this.Focusable = false;

            








        }

        public Border getContentView()
        {
            return baseBorder;
        }

        public ContentView setDataDisplay(Control displayOfDataView)
        {
            this.contentLabel.Content = (string) displayOfDataView.Tag;
           
            this.Children.Add(displayOfDataView);
            return this;
        }

       

        //Builder Entry
        public static ContentView build()
        {
            return new ContentView();
        }
    }

}
