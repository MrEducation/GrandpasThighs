using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Fizbin.Kinect.Gestures.Segments;
using Fizbin.Kinect.Gestures;
using Microsoft.Samples.Kinect.WpfViewers;


namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        KinectSensor myKinect;
        private GestureController controller;
        private MapDrawing maps;
        Timer clear;

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (KinectSensor.KinectSensors.Count > 0) 
            {
                myKinect = KinectSensor.KinectSensors[0];
            }
            if (myKinect.Status == KinectStatus.Connected) 
            {
                myKinect.ColorStream.Enable();
                myKinect.DepthStream.Enable();
                myKinect.SkeletonStream.Enable();

                //myKinect.AllFramesReady += myKinect_AllFramesReady;

                myKinect.ColorFrameReady += myKinect_ColorFrameReady;

                myKinect.SkeletonFrameReady += myKinect_SkeletonFrameReady;

                myKinect.Start();
            }

            controller = new GestureController();
            controller.GestureRecognized += OnGestureRecognized;

            // register the gestures for this demo
            RegisterGestures();

            clear = new Timer(1000);
            clear.Elapsed += new ElapsedEventHandler(clearTimer);

            maps = new MapDrawing();
            maps.init();

        }

        void myKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                Image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, colorFrame.Width * 4);
            }
        }

        void myKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //throw new NotImplementedException();

            Skeleton[] allSkeletons = new Skeleton[6];
            Skeleton player1;
            String l;
            String r;

            //var brush = new ImageBrush();
            //brush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Images/BlankMap-USA-states-2000x1444.jpg"));

            ImageBrush brush;

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) 
            {
                if (skeletonFrame == null) 
                {
                    return;
                }

                skeletonFrame.CopySkeletonDataTo(allSkeletons);

                player1 = (from s in allSkeletons
                           where s.TrackingState == SkeletonTrackingState.Tracked
                           select s).FirstOrDefault();
                if (player1 == null) { return; }

                setEllipsePosition(left_hand_e, player1.Joints[JointType.HandLeft], 375, 204);
                setEllipsePosition(right_hand_e, player1.Joints[JointType.HandRight], 535, 204);


                brush = maps.getImage(1);
                C.Background = brush;

                controller.UpdateAllGestures(player1);

                text1.Text = Gesture;
                l = "X:" + player1.Joints[JointType.HandLeft].Position.X.ToString() + " Y:" + player1.Joints[JointType.HandLeft].Position.Y.ToString();
                r = "X:" + player1.Joints[JointType.HandRight].Position.X.ToString() + " Y:" + player1.Joints[JointType.HandRight].Position.Y.ToString();


                left.Text = l;
                right.Text = r;
            }
        }
       
        private void setEllipsePosition(FrameworkElement ellipse, Joint j, int x_, int y_) 
        {
            double scaledX = (j.Position.X * ellipse.Width * 4) + x_;
            double scaledY = (j.Position.Y * ellipse.Height * -4) + y_;

            Canvas.SetLeft(ellipse, scaledX);
            Canvas.SetTop(ellipse, scaledY);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (myKinect != null) 
            {
                myKinect.SkeletonFrameReady -= myKinect_SkeletonFrameReady;
                controller.GestureRecognized -= OnGestureRecognized;
                myKinect.Stop();
                myKinect.AudioSource.Stop();
                clear.Stop();
            }
        }

        void clearTimer(object sender, ElapsedEventArgs e)
        {
            Gesture = "";
            clear.Stop();
        }

        private void RegisterGestures()
        {
            // define the gestures for the demo

            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            controller.AddGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            controller.AddGesture("SwipeLeft", swipeleftSegments);

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            controller.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            controller.AddGesture("SwipeUp", swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            controller.AddGesture("SwipeDown", swipeDownSegments);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                    return;

                _gesture = value;
                
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
                 
            }
        }

        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                case "JoinedHands":
                    Gesture = "Joined Hands";
                    break;
                case "SwipeLeft":
                    Gesture = "Swipe Left";
                    break;
                case "SwipeRight":
                    Gesture = "Swipe Right";
                    break;
                case "SwipeUp":
                    Gesture = "Swipe Up";
                    break;
                case "SwipeDown":
                    Gesture = "Swipe Down";
                    break;
                default:
                    break;
            }
        }

        void currentScreen() 
        {
            
        }

    }
}
