// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using Kinect.Toolbox.Record;
using System.IO;


namespace CameraFundamentals
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stream recordStream;
        KinectRecorder Recorder;
        KinectReplay Replay;
        bool Isrecording;

        public MainWindow()
        {
            InitializeComponent();
        }
      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //sign up for the event
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);

            
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            var oldSensor = (KinectSensor)e.OldValue;

            //stop the old sensor
            if (oldSensor != null)
            {
                StopKinect(oldSensor); 
            }

            //get the new sensor
            var newSensor = (KinectSensor)e.NewValue;
            if (newSensor == null)
            {
                return; 
            }

            //newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
            newSensor.ColorFrameReady += this.newSensor_ColorFrameReady;
            newSensor.DepthFrameReady += this.newSensor_DepthFrameReady;
            newSensor.SkeletonFrameReady += this.newSensor_SkeletonFrameReady;

            //turn on features that you need
            newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);                
            newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            newSensor.SkeletonStream.Enable();

            try
            {
                newSensor.Start();                
            }
            catch (System.IO.IOException)
            {
                //this happens if another app is using the Kinect
                kinectSensorChooser1.AppConflictOccurred();
            }
        }


        //record colour stream when frame is ready
        void newSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                    return;
                // any other validations...
                if (Isrecording == true)
                    this.Recorder.Record(colorImageFrame);
            }

        }

        void newSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame == null)
                    return;
                // any other validations...
                if (Isrecording == true)
                    this.Recorder.Record(depthImageFrame);
            }

        }


        void newSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;
                // any other validations...
                if (Isrecording == true)
                    this.Recorder.Record(skeletonFrame);
            }

        }


        //this event fires when Color/Depth/Skeleton are synchronized
        //void newSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        //{
        //    //using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
        //    //{
        //    //    if (colorFrame == null)
        //    //    {
        //    //        return;
        //    //    }

        //    //    byte[] pixels = new byte[colorFrame.PixelDataLength];
        //    //    colorFrame.CopyPixelDataTo(pixels);

        //    //    int stride = colorFrame.Width * 4;
        //    //    image1.Source =
        //    //        BitmapSource.Create(colorFrame.Width, colorFrame.Height,
        //    //        96, 96, PixelFormats.Bgr32, null, pixels, stride); 

        //    using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
        //    {
        //        if (colorImageFrame == null)
        //            return;
        //        // any other validations...
        //        if (Isrecording == true)
        //            this.Recorder.Record(colorImageFrame);
        //    }

        //    using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
        //    {
        //        if (depthImageFrame == null)
        //            return;
        //        // any other validations...
        //        if (Isrecording == true)
        //            this.Recorder.Record(depthImageFrame);
        //    }

        //    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
        //    {
        //        if (skeletonFrame == null)
        //            return;
        //        // any other validations...
        //        if (Isrecording == true)
        //            this.Recorder.Record(skeletonFrame);
        //    }



        //    //}
        //}


        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }


                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(kinectSensorChooser1.Kinect); 
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            string generatedName = Guid.NewGuid().ToString();
            string recordStreamPathAndName = @"e:\" + generatedName + ".replay";
            this.recordStream = File.Create(recordStreamPathAndName);
            this.Recorder = new KinectRecorder(KinectRecordOptions.Color | KinectRecordOptions.Depth | KinectRecordOptions.Skeletons, recordStream);
            this.Isrecording = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (Recorder != null)
            {
                Isrecording = false;
                Recorder.Stop();
                Recorder = null;
            }
            System.Console.WriteLine("record stop!!!");
        } 
    }
}
