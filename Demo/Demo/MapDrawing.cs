using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Demo
{
    public class MapDrawing
    {
        ImageBrush current;
        ArrayList screenStrings;
        public MapDrawing() 
        {
            current = new ImageBrush();
            screenStrings = new ArrayList();
        }
        

        public void init() 
        {
            screenStrings.Add("Images/BlankMap-USA-states-2000x1444.jpg");
            screenStrings.Add("Images/MidWest.png");
            screenStrings.Add("Images/NorthEast.png");
            screenStrings.Add("Images/Pacific.png");
            screenStrings.Add("Images/south.png");
            screenStrings.Add("Images/west.png");

        }

        public ImageBrush getImage(int screen)
        {
            ImageBrush s = new ImageBrush();

            //System.Console.WriteLine(screenStrings[screen]);

            s.ImageSource = new BitmapImage(new Uri( screenStrings[screen].ToString()));

            return s;
        }

    }
}
