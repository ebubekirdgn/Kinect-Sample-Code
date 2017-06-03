using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectHandTracking
{
    public static class Extensions
    {
        #region Camera

        public static ImageSource ToBitmap(this ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] pixelData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < pixelData.Length; ++depthIndex)
            {
                ushort depth = pixelData[depthIndex];

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this InfraredFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort[] frameData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < frameData.Length; infraredIndex++)
            {
                ushort ir = frameData[infraredIndex];

                byte intensity = (byte)(ir >> 7);

                pixels[colorIndex++] = (byte)(intensity / 1); // Blue
                pixels[colorIndex++] = (byte)(intensity / 1); // Green   
                pixels[colorIndex++] = (byte)(intensity / 0.4); // Red

                colorIndex++;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        #endregion

        #region Body

        public static Point Scale(this Joint joint, CoordinateMapper mapper)
        {
            Point point = new Point();

            ColorSpacePoint colorPoint = mapper.MapCameraPointToColorSpace(joint.Position);
            point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
            point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

            return point;
        }

        #endregion

        #region Drawing

        public static void DrawSkeleton(this Canvas canvas, Body body, CoordinateMapper mapper)
        {
            if (body == null) return;

            foreach (Joint joint in body.Joints.Values)
            {
                canvas.DrawPoint(joint, mapper);
            }

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck], mapper);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid], mapper);
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], mapper);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight], mapper);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight], mapper);
            canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight], mapper);
            canvas.DrawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight], mapper);
            canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight], mapper);
            canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight], mapper);
            canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight], mapper);
        }

        public static void DrawPoint(this Canvas canvas, Joint joint, CoordinateMapper mapper)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            Point point = joint.Scale(mapper);

            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawHand(this Canvas canvas, Joint hand, CoordinateMapper mapper)
        {
            if (hand.TrackingState == TrackingState.NotTracked) return;

            Point point = hand.Scale(mapper);

            Ellipse ellipse = new Ellipse
            {
                Width = 100,
                Height = 100,
                Stroke = new SolidColorBrush(Colors.LightBlue),
                StrokeThickness = 4
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawThumb(this Canvas canvas, Joint thumb, CoordinateMapper mapper)
        {
            if (thumb.TrackingState == TrackingState.NotTracked) return;

            Point point = thumb.Scale(mapper);

            Ellipse ellipse = new Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = new SolidColorBrush(Colors.LightBlue),
                Opacity = 0.7
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second, CoordinateMapper mapper)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            Point firstPoint = first.Scale(mapper);
            Point secondPoint = second.Scale(mapper);

            Line line = new Line
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(Colors.LightBlue)
            };

            canvas.Children.Add(line);
        }

        #endregion
    }
}
