using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
//using Kinect.Toolbox.Record;
//using Microsoft.Samples.Kinect.WpfViewers;
//using Kinect.Toolbox.ColorStreamManager;
//using Kinect.Toolbox;
using Kinect.Recorder;
using Kinect.Replay;
using Kinect.Replay.Replay;
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Skeletons;
using Microsoft.Win32;
using System.Threading;
using System.Collections.Generic;
using Kinect.Replay.Record;
using AForge.Video.VFW;
using System.Drawing;




namespace Generator
{

    public partial class MainWindow : Window
    {
        private const KinectRecordOptions RecordOptions = KinectRecordOptions.Frames;
        KinectSensor kinectSensor;
        //class for replay Kinect data from the .replay file
        KinectReplay replay;
        WriteableBitmap imagesource = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
        //<------- Skeleton Stream -------->
        Skeleton[] skeletons;
        private Dictionary<JointType, Ellipse> ellipses;
        public bool IsReplaying = false;
        public bool IsGenerating = false;
        private string message;
        AVIWriter videoWriter = new AVIWriter("wmv3");
        XMLWriter xmlwriter;
        int initialframenumber = 0;
        int lastframenumber = 0;
        bool isfirst = true;
        int CurrentFramenumber = 0;
        int framecounter = 0;
        int missingFrameCount = 0;

        //generate files for analyser
        private void generate_initialization()
        {
            var openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
            if (openFileDialog.ShowDialog() == true)
            {
                //get the pure filename(without path and extension)
                string path = openFileDialog.FileName;
                string filename = openFileDialog.SafeFileName;
                System.Console.WriteLine("path = " + path + "purepath= ");
                string pathwithoutextension = path.Remove(path.IndexOf('.'));
                replay = new KinectReplay(openFileDialog.FileName);
                message = string.Format("Replaying {0}", RecordOptions.ToString());
                videoWriter.FrameRate = 30;
                videoWriter.Open(pathwithoutextension + ".avi", 640, 480);
                //xmlwriter
                xmlwriter = new XMLWriter(pathwithoutextension + "_jointsdata.xml", pathwithoutextension + "_keyframes.xml", filename);

                IsGenerating = true;
                replay.AllFramesReady += ReplayAllFramesReady;
                replay.ReplayFinished += CleanupGeneration;
                replay.Start();
                generationstatus.Content = "generating analysis files, please wait";

            }
     
        }


        //write .avi and store data in .xml for a certain frame 
        private void writeFrame(ReplayAllFramesReadyEventArgs e)
        {
            //colour frame
            BitmapImage bitmapImage_currentframe = new BitmapImage();
            Bitmap bitmap_currentframe = new Bitmap(640, 480);   
            var colorImageFrame = e.AllFrames.ColorImageFrame;
            if (colorImageFrame != null)
            {
                if (isfirst)
                {
                    initialframenumber = colorImageFrame.FrameNumber;
                    System.Console.WriteLine("initialFramenumber = " + initialframenumber);
                    isfirst = false;
                }

                lastframenumber = colorImageFrame.FrameNumber;
                //write xml
                xmlwriter.storeframe(e.AllFrames.SkeletonFrame, colorImageFrame.FrameNumber, replay.CoordinateMapper);

                //write avi
                var pixelData = new byte[colorImageFrame.PixelDataLength];
                colorImageFrame.CopyPixelDataTo(pixelData);
                var stride = colorImageFrame.Width * PixelFormats.Bgr32.BitsPerPixel / 8;
                //converision to bitmap
                imagesource.WritePixels(new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height), pixelData, stride, 0);
                bitmapImage_currentframe = ConvertWriteableBitmapToBitmapImage(imagesource);
                bitmap_currentframe = ConvertBitmapImageToBitmap(bitmapImage_currentframe);
                videoWriter.AddFrame(bitmap_currentframe);
                framecounter++;
            }
               

        }
        //convert WriteableBitmap to BitmapImage
        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        //convert bitmapImage to bitmap
        private Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }




        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //System.Console.WriteLine("update color frame");
            try
            {
                System.Console.WriteLine("window loaded");
                KinectSensor.KinectSensors.StatusChanged += KinectSensorsStatusChanged;
                ellipses = new Dictionary<JointType, Ellipse>();
                //fill UI drop-down menu
                GenerationModeSelection.Items.Add("Deadlift");
                GenerationModeSelection.Items.Add("Clean Jerk");

                kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
                if (kinectSensor == null)
                {
                    message = "No Kinect found on startup";
                    System.Console.WriteLine("sensor = null");
                }
                else
                {
                    Initialize();
                }
                System.Console.WriteLine("initialized");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("not initialized");
                message = ex.Message;
            }

        }

        private void Initialize()
        {
            //if (kinectSensor == null)
            //    return;
            ////img1.Source = imageSource;
            //// bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            ////bw.WorkerSupportsCancellation = true;

            //System.Console.WriteLine("initialize  888888888");
            ////kinectSensor.AllFramesReady += KinectSensorAllFramesReady;
            //kinectSensor.ColorStream.Enable();
            //kinectSensor.SkeletonStream.Enable();
            //kinectSensor.DepthStream.Enable();
            //kinectSensor.Start();
            //System.Console.WriteLine("kinect start");
            ////kinectSensor.AudioSource.Start();
            //message = "Kinect connected";
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //StopKinect(kinectSensorChooser1.Kinect);
            if (null != this.kinectSensor)
            {
                this.kinectSensor.Stop();
            }
        }


        void KinectSensorsStatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Disconnected:
                    if (kinectSensor == e.Sensor)
                    {
                       // Clean();
                        message = "Kinect disconnected";
                    }
                    break;
                case KinectStatus.Connected:
                    System.Console.WriteLine("kinect connected");
                    kinectSensor = e.Sensor;
                    Initialize();
                    break;
                case KinectStatus.NotPowered:
                    message = "Kinect is not powered";
                    //Clean();
                    break;
                case KinectStatus.NotReady:
                    message = "Kinect is not ready";
                    break;
                case KinectStatus.Initializing:
                    message = "Initializing";
                    break;
                default:
                    message = string.Concat("Status: ", e.Status);
                    break;
            }
        }



        private void CleanupReplay()
        {
            if (!IsReplaying) return;
            message = "";
            //if (_soundPlayer != null && _startedAudio)
            //    _soundPlayer.Stop();
            replay.AllFramesReady -= ReplayAllFramesReady;
            replay.Stop();
            replay.Dispose();
            replay = null;
            IsReplaying = false;
        }


        private void CleanupGeneration()
        {
            videoWriter.Close();
            IsGenerating = false;
            replay.AllFramesReady -= ReplayAllFramesReady;
            replay.Stop();
            replay.Dispose();
            replay = null;
            //write xml to disk
            xmlwriter.write();
            System.Console.WriteLine("lastframenumber = " + lastframenumber);
            System.Console.WriteLine("framecounter = " + framecounter);
            System.Console.WriteLine("missingFrameCount = " + missingFrameCount);
            //lastframenumber
            generationstatus.Content = "Generation finished";
        }
       

        private void LoadReplay_Click(object sender, RoutedEventArgs e)
        {
            if (IsReplaying)
            {
                CleanupReplay();
                message = "";
                return;
            }
            //_startedAudio = false;
            var openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };

            if (openFileDialog.ShowDialog() == true)
            {
                replay = new KinectReplay(openFileDialog.FileName);
                message = string.Format("Replaying {0}", RecordOptions.ToString());
                replay.AllFramesReady += ReplayAllFramesReady;
                replay.ReplayFinished += CleanupReplay;
                replay.Start();
            }
            IsReplaying = true;
        }

        private void UpdateSkeletons(ReplaySkeletonFrame frame)
        {
            skeletons = frame.Skeletons;
            var trackedSkeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

            if (trackedSkeleton == null)
                return;

            DrawJoints(trackedSkeleton);
        }

        private void UpdateColorFrame(ReplayColorImageFrame frame)
        {
            //System.Console.WriteLine("update color frame");
            // var uri = new Uri("C:\\Users\\WenxuanZhou\\Desktop\\881856_475135009220488_407602784_o.jpg");
            // img1.Source = new BitmapImage(uri);

            var pixelData = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(pixelData);
            //if (ImageSource == null)
            //    ImageSource = new WriteableBitmap(frame.Width, frame.Height, 96, 96,
            //                                                PixelFormats.Bgr32, null);

            var stride = frame.Width * PixelFormats.Bgr32.BitsPerPixel / 8;
            //ImageSource.WritePixels(new Int32Rect(0, 0, frame.Width, frame.Height), pixelData, stride, 0);
            img1.Source = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
        }



        void ReplayAllFramesReady(ReplayAllFramesReadyEventArgs e)
        {
            if (IsGenerating)
            {
                //indicate missing frame
                CurrentFramenumber++;
                if (CurrentFramenumber != e.AllFrames.ColorImageFrame.FrameNumber)
                {
                    missingFrameCount++;
                    System.Console.WriteLine("color framenumber = " + e.AllFrames.ColorImageFrame.FrameNumber);
                    CurrentFramenumber = e.AllFrames.ColorImageFrame.FrameNumber;
                }

                writeFrame(e);
                //return;
            }

            var colorImageFrame = e.AllFrames.ColorImageFrame;
            if (colorImageFrame != null)
                UpdateColorFrame(colorImageFrame);

            var skeletonFrame = e.AllFrames.SkeletonFrame;
            if (skeletonFrame != null)
                UpdateSkeletons(skeletonFrame);
        }


        private void DrawJoints(Skeleton skeleton)
        {
            foreach (var name in Enum.GetNames(typeof(JointType)))
            {
                var jointType = (JointType)Enum.Parse(typeof(JointType), name);
                var coordinateMapper = (kinectSensor != null && kinectSensor.Status == KinectStatus.Connected) ? new CoordinateMapper(kinectSensor) : replay.CoordinateMapper;
                var joint = skeleton.Joints[jointType];

                var skeletonPoint = joint.Position;
                if (joint.TrackingState == JointTrackingState.NotTracked)
                    continue;

                var colorPoint = coordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                //test print
               // if (name == "Head")
                //{
                  //  System.Console.WriteLine("Head.X = "+colorPoint.X);
                   // System.Console.WriteLine("Head.Y = " + colorPoint.Y);
                //}


                if (!ellipses.ContainsKey(jointType))
                {
                    ellipses[jointType] = new Ellipse { Width = 20, Height = 20, Fill = System.Windows.Media.Brushes.SandyBrown };
                    //skeletonCanvas in the UI
                    SkeletonCanvas.Children.Add(ellipses[jointType]);
                }
                Canvas.SetLeft(ellipses[jointType], colorPoint.X - ellipses[jointType].Width / 2);
                Canvas.SetTop(ellipses[jointType], colorPoint.Y - ellipses[jointType].Height / 2);
            }
        }
  


        private void GenerationModeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Generation_Click(object sender, RoutedEventArgs e)
        {
            generate_initialization();


        }

 





    }
}



//private void Window_Loaded(object sender, RoutedEventArgs e)
//{
//    //sign up for the event
//   // kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);


//}

//void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
//{

//    var oldSensor = (KinectSensor)e.OldValue;

//    //stop the old sensor
//    if (oldSensor != null)
//    {
//        StopKinect(oldSensor);
//    }

//    //get the new sensor
//    var newSensor = (KinectSensor)e.NewValue;
//    if (newSensor == null)
//    {
//        return;
//    }

//    //newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(newSensor_AllFramesReady);
//    newSensor.ColorFrameReady += this.newSensor_ColorFrameReady;
//    newSensor.DepthFrameReady += this.newSensor_DepthFrameReady;
//    newSensor.SkeletonFrameReady += this.newSensor_SkeletonFrameReady;

//    //turn on features that you need
//    newSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
//    newSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
//    newSensor.SkeletonStream.Enable();

//    try
//    {
//        newSensor.Start();
//    }
//    catch (System.IO.IOException)
//    {
//        //this happens if another app is using the Kinect
//        kinectSensorChooser1.AppConflictOccurred();
//    }
//}


//        //With this class, you’re now ready to replay a previously recorded Kinect session and integrate it with your existing code. For instance, if you have a ProcessFrame method that takes a ColorImageFrame in parameter, you just have to change the type of the parameter to ReplayColorImageFrame, and without any more changes, your method accepts real-time data (with ColorImageFrame, which will be implicitly cast to ReplayColorImageFrame) and offline data (with ReplayColorImageFrame):
// private void LaunchReplay()
//{
//    OpenFileDialog openFileDialog =
//new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };

//    if (openFileDialog.ShowDialog() == true)
//    {
//        Stream recordStream = File.OpenRead(openFileDialog.FileName);

//        replay = new KinectReplay(recordStream);

//        replay.ColorImageFrameReady += replay_ColorImageFrameReady;

//        replay.Start();
//    }
//}
////This method refers to a replay_ColorImageFrameReady event:
// void replay_ColorImageFrameReady (object sender, ReplayColorImageFrameReadyEventArgse)
//{
//    ProcessFrame(e. ColorImageFrame);
//}
////The process frame must be changed from:
// void ProcessFrame(ColorImageFrame frame)
//{
//   ...
//}
////to:
// void ProcessFrame(ReplayColorImageFrame frame)
//{
//   ...
//}
//The process works the same way with the depth and skeleton frames.


//private void replay_initialisation()
//{
//    //COLOUR STREAM
//    // Turn on the color stream to receive color frames
//    //this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

//    // Allocate space to put the pixels we'll receive
//    this.colour_colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

//    // This is the bitmap we'll display on-screen
//    this.colour_colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

//    // Set the image we display to point to the bitmap where we'll put the image data
//    this.ColourImage.Source = this.colour_colorBitmap;





//    //DEPTH STREAM
//    // Allocate space to put the depth pixels we'll receive
//    this.depthPixels = new DepthImagePixel[this.replay.depthReplay.frames.get];

//    // Allocate space to put the color pixels we'll create
//    this.depth_colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

//    // This is the bitmap we'll display on-screen
//    this.depth_colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

//    // Set the image we display to point to the bitmap where we'll put the image data
//    this.DepthImage.Source = this.depth_colorBitmap;

//    // Add an event handler to be called whenever there is new depth frame data
//    //this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

//    //SKELETON STREAM

//    // Create the drawing group we'll use for drawing
//    this.drawingGroup = new DrawingGroup();

//    // Create an image source that we can use in our image control
//    this.imageSource = new DrawingImage(this.drawingGroup);

//    // Display the drawing using our image control
//    SkeletonImage.Source = this.imageSource;
//}


//This method refers to a replay_ColorImageFrameReady event:
// void replay_ColorImageFrameReady(object sender, ReplayColorImageFrameReadyEventArgs e)
//{
//    //ProcessFrame(e. ColorImageFrame);

//    ReplayColorImageFrame colorFrame = e.ColorImageFrame;

//         if (colour_colorPixels == null) 
//        {
//            this.colour_colorPixels = new byte[colorFrame.PixelDataLength];

//            this.colour_colorBitmap = new WriteableBitmap(colorFrame.Width, colorFrame.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

//            this.ColourImage.Source = this.colour_colorBitmap;
//        }

//        if (colorFrame != null)
//        {
//            // Copy the pixel data from the image to a temporary array
//            colorFrame.CopyPixelDataTo(this.colour_colorPixels);

//            // Write the pixel data into our bitmap
//            this.colour_colorBitmap.WritePixels(
//                new Int32Rect(0, 0, this.colour_colorBitmap.PixelWidth, this.colour_colorBitmap.PixelHeight),
//                this.colour_colorPixels,
//                this.colour_colorBitmap.PixelWidth * sizeof(int),
//                0);
//        }

//}

//This method refers to a replay_DepthImageFrameReady event:
//private void replay_DepthImageFrameReady(object sender, ReplayDepthImageFrameReadyEventArgs e)
//{
//    ReplayDepthImageFrame depthFrame = e.DepthImageFrame;

//    if (depth_colorPixels == null)
//    {
//        this.depthPixels = new short[depthFrame.PixelDataLength];

//        this.depth_colorPixels = new byte[depthFrame.PixelDataLength * sizeof(int)];

//        this.depth_colorBitmap = new WriteableBitmap(depthFrame.Width, depthFrame.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

//        this.DepthImage.Source = this.depth_colorBitmap;
//    }


//        if (depthFrame != null)
//        {
//            // Copy the pixel data from the image to a temporary array
//            depthFrame.CopyPixelDataTo(this.depthPixels);
//            //depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

//            // Get the min and max reliable depth for the current frame
//            int minDepth = depthFrame.MinDepth;
//            int maxDepth = depthFrame.MaxDepth;

//            // Convert the depth to RGB
//            int colorPixelIndex = 0;
//            for (int i = 0; i < this.depthPixels.Length; ++i)
//            {
//                // Get the depth for this pixel
//                short depth = this.depthPixels[i];

//                // To convert to a byte, we're discarding the most-significant
//                // rather than least-significant bits.
//                // We're preserving detail, although the intensity will "wrap."
//                // Values outside the reliable depth range are mapped to 0 (black).

//                // Note: Using conditionals in this loop could degrade performance.
//                // Consider using a lookup table instead when writing production code.
//                // See the KinectDepthViewer class used by the KinectExplorer sample
//                // for a lookup table example.
//                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

//                // Write out blue byte
//                this.depth_colorPixels[colorPixelIndex++] = intensity;

//                // Write out green byte
//                this.depth_colorPixels[colorPixelIndex++] = intensity;

//                // Write out red byte                        
//                this.depth_colorPixels[colorPixelIndex++] = intensity;

//                // We're outputting BGR, the last byte in the 32 bits is unused so skip it
//                // If we were outputting BGRA, we would write alpha here.
//                ++colorPixelIndex;
//            }

//            // Write the pixel data into our bitmap
//            this.depth_colorBitmap.WritePixels(
//                new Int32Rect(0, 0, this.depth_colorBitmap.PixelWidth, this.depth_colorBitmap.PixelHeight),
//                this.depth_colorPixels,
//                this.depth_colorBitmap.PixelWidth * sizeof(int),
//                0);
//        }

//}

//PREBLEM, REPLAY LACK OF FUNCTION
//This method refers to a replay_DepthImageFrameReady event:
//     private void replay_DepthImageFrameReady(object sender, ReplayDepthImageFrameReadyEventArgs e)
//     {
//         ReplayDepthImageFrame depthFrame = e.DepthImageFrame;
//         if (depth_colorPixels == null)
//         {
//             this.depthPixels = new short[depthFrame.PixelDataLength];

//             this.depth_colorPixels = new byte[depthFrame.PixelDataLength * sizeof(int)];

//             this.depth_colorBitmap = new WriteableBitmap(depthFrame.Width, depthFrame.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

//             this.DepthImage.Source = this.depth_colorBitmap;
//         }


//        if (depthFrame == null)
//        {
//            return; 
//        }

//        byte[] pixels = GenerateColoredBytes(depthFrame);

//        //number of bytes per row width * 4 (B,G,R,Empty)
//        int stride = depthFrame.Width * 4;

//        //create image
//        DepthImage.Source = 
//            BitmapSource.Create(depthFrame.Width, depthFrame.Height, 
//            96, 96, PixelFormats.Bgr32, null, pixels, stride); 


//}


//     private byte[] GenerateColoredBytes(ReplayDepthImageFrame depthFrame)
//{

//    //get the raw data from kinect with the depth for every pixel
//    short[] rawDepthData = new short[depthFrame.PixelDataLength];
//    depthFrame.CopyPixelDataTo(rawDepthData); 

//    //use depthFrame to create the image to display on-screen
//    //depthFrame contains color information for all pixels in image
//    //Height x Width x 4 (Red, Green, Blue, empty byte)
//    Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

//    //Bgr32  - Blue, Green, Red, empty byte
//    //Bgra32 - Blue, Green, Red, transparency 
//    //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

//    //hardcoded locations to Blue, Green, Red (BGR) index positions       
//    const int BlueIndex = 0;
//    const int GreenIndex = 1;
//    const int RedIndex = 2;


//    //loop through all distances
//    //pick a RGB color based on distance
//    for (int depthIndex = 0, colorIndex = 0; 
//        depthIndex < rawDepthData.Length && colorIndex < pixels.Length; 
//        depthIndex++, colorIndex += 4)
//    {
//        //get the player (requires skeleton tracking enabled for values)
//        int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;

//        //gets the depth value
//        int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

//        //.9M or 2.95'
//        if (depth <= 900)
//        {
//            //we are very close
//            pixels[colorIndex + BlueIndex] = 255;
//            pixels[colorIndex + GreenIndex] = 0;
//            pixels[colorIndex + RedIndex] = 0;

//        }
//        // .9M - 2M or 2.95' - 6.56'
//        else if (depth > 900 && depth < 2000)
//        {
//            //we are a bit further away
//            pixels[colorIndex + BlueIndex] = 0;
//            pixels[colorIndex + GreenIndex] = 255;
//            pixels[colorIndex + RedIndex] = 0;
//        }
//        // 2M+ or 6.56'+
//        else if (depth > 2000)
//        {
//            //we are the farthest
//            pixels[colorIndex + BlueIndex] = 0;
//            pixels[colorIndex + GreenIndex] = 0;
//            pixels[colorIndex + RedIndex] = 255;
//        }


//        ////equal coloring for monochromatic histogram
//        byte intensity = CalculateIntensityFromDepth(depth);
//        pixels[colorIndex + BlueIndex] = intensity;
//        pixels[colorIndex + GreenIndex] = intensity;
//        pixels[colorIndex + RedIndex] = intensity;


//        //Color all players "gold"
//        if (player > 0)
//        {
//            pixels[colorIndex + BlueIndex] = Colors.Gold.B;
//            pixels[colorIndex + GreenIndex] = Colors.Gold.G;
//            pixels[colorIndex + RedIndex] = Colors.Gold.R;
//        }

//    }


//    return pixels;
//}


//public static byte CalculateIntensityFromDepth(int distance)
//{
//    //formula for calculating monochrome intensity for histogram
//    return (byte)(255 - (255 * Math.Max(distance - MinDepthDistance, 0) 
//        / (MaxDepthDistanceOffset)));
//}

//This method refers to a replay_SkeletonImageFrameReady event:
//private void replay_SkeletonImageFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
//{
//    Skeleton[] skeletons = new Skeleton[0];

//    ReplaySkeletonFrame skeletonFrame = e.SkeletonFrame;

//        if (skeletonFrame != null)
//        {
//            //skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
//            //skeletonFrame.CopySkeletonDataTo(skeletons);
//            skeletons = skeletonFrame.Skeletons;

//        }


//    using (DrawingContext dc = this.drawingGroup.Open())
//    {
//        // Draw a transparent background to set the render size
//        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

//        if (skeletons.Length != 0)
//        {
//            foreach (Skeleton skel in skeletons)
//            {
//                RenderClippedEdges(skel, dc);

//                if (skel.TrackingState == SkeletonTrackingState.Tracked)
//                {
//                    this.DrawBonesAndJoints(skel, dc);
//                }
//                else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
//                {
//                    dc.DrawEllipse(
//                    this.centerPointBrush,
//                    null,
//                    this.SkeletonPointToScreen(skel.Position),
//                    BodyCenterThickness,
//                    BodyCenterThickness);
//                }
//            }
//        }

//        // prevent drawing outside of our render area
//        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
//    }
//}
//void replay_ColorImageFrameReady(object sender, ReplayColorImageFrameReadyEventArgs e)
//{
//    ReplayColorImageFrame frame = e.ColorImageFrame;
//    if (frame == null)
//        return;
//    this.colorManager.Update(frame);
//    System.Console.WriteLine("Color frame number = "+frame.FrameNumber);

//}


//private void replay_DepthImageFrameReady(object sender, ReplayDepthImageFrameReadyEventArgs e)
//{
//    ReplayDepthImageFrame frame = e.DepthImageFrame;
//    if (frame == null)
//        return;

//    this.depthManager.Update(frame);
//    //System.Console.WriteLine("update depth");
//}

//private void replay_SkeletonImageFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
//{
//    ReplaySkeletonFrame frame = e.SkeletonFrame;

//    if (frame == null)
//        return;
//    if (frame.Skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
//        return;
//    if (frame.Skeletons == null)
//        return;
//    skeletons = frame.Skeletons;
//    skeletonManager.Draw(skeletons, false);
//    //System.Console.WriteLine("update skeleton");
//}
//private void Window_Loaded(object sender, RoutedEventArgs e)
//{
//    //initialise kinect

//    // Look through all sensors and start the first connected one.
//    // This requires that a Kinect is connected at the time of app startup.
//    // To make your app robust against plug/unplug, 
//    // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
//    try
//    {
//        //listen to any status change for Kinects
//        KinectSensor.KinectSensors.StatusChanged += Kinects_StatusChanged;

//        //loop through all the Kinects attached to this PC, and start the first that is connected without an error.
//        foreach (KinectSensor kinect in KinectSensor.KinectSensors)
//        {
//            if (kinect.Status == KinectStatus.Connected)
//            {
//                sensor = kinect;
//                break;
//            }
//        }

//        if (KinectSensor.KinectSensors.Count == 0)
//            messageBox.Show("No Kinect found");
//        else
//            Initialize();   // Initialization of the current sensor

//    }
//    catch (Exception ex)
//    {
//        messageBox.Show("Kinect Sensor is not connected, please re-start the application after connected a Kinect Sensor");
//    }



//    this.sensor.SkeletonStream.Enable();


//    //binding elements in UI with code
//    skeletonManager = new SkeletonDisplayManager(sensor, skeletonCanvas);
//    this.colorManager = new ColorStreamManager();
//    this.depthManager = new DepthStreamManager();
//    ColourImage.DataContext = colorManager;
//    DepthImage.DataContext = depthManager;

//    //fill UI drop-down menu
//    GenerationModeSelection.Items.Add("Deadlift");
//    GenerationModeSelection.Items.Add("Clean Jerk");

//}
////<------- Colour Stream -------->

///// <summary>
///// Bitmap that will hold color information
///// </summary>
//private WriteableBitmap colour_colorBitmap;

///// <summary>
///// Intermediate storage for the color data received from the camera
///// </summary>
//private byte[] colour_colorPixels;



////<------- Depth Stream -------->

///// <summary>
///// Bitmap that will hold color information
///// </summary>
//private WriteableBitmap depth_colorBitmap;

///// <summary>
///// Intermediate storage for the depth data received from the camera
///// </summary>
////private DepthImagePixel[] depthPixels;
//private short[] depthPixels;

///// <summary>
///// Intermediate storage for the depth data converted to color
///// </summary>
//private byte[] depth_colorPixels;

//// color divisors for tinting depth pixels

//private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

//const float MaxDepthDistance = 4095; // max value returned
//const float MinDepthDistance = 850; // min value returned
//const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;