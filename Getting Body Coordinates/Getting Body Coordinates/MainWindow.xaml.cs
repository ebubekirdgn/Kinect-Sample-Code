using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;


namespace Getting_Body_Coordinates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        KinectSensor kinectSensor;
        BodyFrameReader bodyFrameReader;
        Body[] bodies = null;
        public MainWindow()
        {
            InitializeComponent();
            KinectAcilis();
        }

        public void KinectAcilis()
        {
            kinectSensor = KinectSensor.GetDefault();
            if(kinectSensor != null) 
            {
                //Kinect Ac
                kinectSensor.Open();
            }
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            if(bodyFrameReader != null)
            {
                bodyFrameReader.FrameArrived += Reader_Frame_Arrived;
            }
        }

        private void Reader_Frame_Arrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if(bodyFrame != null)
                {
                    if(bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }
                }
                bodyFrame.GetAndRefreshBodyData(bodies);
                dataReceived = true;
            }
            if(dataReceived)
            {
                foreach (Body body in bodies)
                {
                    if(body.IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType,Point>();

                        Joint midSpine = joints[JointType.SpineMid];
                        float ms_distance_x = midSpine.Position.X;
                        float ms_distance_y = midSpine.Position.Y;
                        float ms_distance_z = midSpine.Position.Z;

                        Coordinats.Content += ms_distance_x.ToString();
                        Coordinats.Content += ms_distance_x.ToString();
                        Coordinats.Content += ms_distance_x.ToString();



                    }
                }
            }
        }
    }
}
