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
using System.Windows.Forms;
using System.Windows.Threading;
using System.Drawing;
//using Microsoft.Kinect;
//using WPFMediaKit.DirectShow.MediaPlayers;

namespace Analyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //timers for video 1 and 2
        private DispatcherTimer timer1 = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();
        //record the slider value while dragged
        private TimeSpan ts1 = new TimeSpan();
        private TimeSpan ts2 = new TimeSpan();

        private XMLReader xmlreader1;
        private XMLReader xmlreader2;
        private Ellipse[] ellipses1 = new Ellipse[10];
        private Ellipse[] ellipses2 = new Ellipse[10];
        private Ellipse[] ellipses1_side = new Ellipse[6];
        private Ellipse[] ellipses2_side = new Ellipse[6];
        // Create  Lines
        Line Shoulder_Hip1 = new Line();
        Line Shoulder_Hand1 = new Line();
        Line Hip_Knee1 = new Line();
        Line Knee_Foot1 = new Line();
        Line Shoulder_Hip2 = new Line();
        Line Shoulder_Hand2 = new Line();
        Line Hip_Knee2 = new Line();
        Line Knee_Foot2 = new Line();

        //timer tick event handler
        private void SetDisplayMessage1(Object sender, System.EventArgs e)
        {
            if (mediaElement1.NaturalDuration.HasTimeSpan)
            {

                TimeSpan currentPositionTimeSpan = mediaElement1.Position;

                string currentPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds, currentPositionTimeSpan.Milliseconds / 100, (double)currentPositionTimeSpan.TotalMilliseconds / 1000 * 30);

                TimeSpan totaotp = mediaElement1.NaturalDuration.TimeSpan;
                string totalPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds, totaotp.Milliseconds / 100, (double)totaotp.TotalMilliseconds / 1000 * 30);
                //System.Console.WriteLine(currentPosition);
                timetxt1.Text = currentPosition;
                //dataGrid1.ItemsSource = reader.Jointsdata;
                double currentFrameID = (double)mediaElement1.Position.TotalMilliseconds / 1000 * 30;
                int currentFrameIDint = Convert.ToInt32(currentFrameID);
                //System.Console.WriteLine("currentFrameID = {0}", Convert.ToInt32(currentFrameID)); 
                //V1Hipangle.Text = string.Format("Left video hip angle at Frame {0} = {1} ", currentFrameID, reader.Jointsdata[0].Hipangle);
                timelineSlider1.Value = (double)mediaElement1.Position.TotalMilliseconds / 1000 * 30;
                V1CurrentFrame.Text = "CurrentFrame = " + currentFrameIDint.ToString();
                if (xmlreader1 != null && xmlreader1.Jointsdata[currentFrameIDint] != null)
                {
                    V1HipAngle.Text = "HipAngle = " + String.Format("{0:0.00}", xmlreader1.Jointsdata[currentFrameIDint].HipAngle);
                    V1ShoulderAngle.Text = "ShoulderAngle = " + String.Format("{0:0.00}", xmlreader1.Jointsdata[currentFrameIDint].ShoulderAngle);
                    V1KneeAngle.Text = "KneeAngle = " + String.Format("{0:0.00}", xmlreader1.Jointsdata[currentFrameIDint].KneeAngle);
                    V1HeadHeight.Text = "HeadHeight = " + String.Format("{0:0.00}", xmlreader1.Jointsdata[currentFrameIDint].HeadHeight);
                    drawSkeletons1(currentFrameIDint, ellipses1, xmlreader1);
                    drawSkeletons1_side(currentFrameIDint, ellipses1_side, xmlreader1);      
                    ResetLines1(currentFrameIDint, xmlreader1);
                }
              
            }
        }

        private void SetDisplayMessage2(Object sender, System.EventArgs e)
        {
            if (mediaElement2.NaturalDuration.HasTimeSpan)
            {

                TimeSpan currentPositionTimeSpan = mediaElement2.Position;

                string currentPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds, currentPositionTimeSpan.Milliseconds / 100, (double)currentPositionTimeSpan.TotalMilliseconds / 1000 * 30);

                TimeSpan totaotp = mediaElement2.NaturalDuration.TimeSpan;
                string totalPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds, totaotp.Milliseconds / 100, (double)totaotp.TotalMilliseconds / 1000 * 30);
                //System.Console.WriteLine(currentPosition);
                timetxt2.Text = currentPosition;

                double currentFrame2ID = (double)mediaElement2.Position.TotalMilliseconds / 1000 * 30;
                int currentFrame2IDint = Convert.ToInt32(currentFrame2ID);
                timelineSlider2.Value = (double)mediaElement2.Position.TotalMilliseconds / 1000 * 30;



                V2CurrentFrame.Text = "CurrentFrame = " + currentFrame2IDint.ToString();
                if (xmlreader2 != null && xmlreader2.Jointsdata[currentFrame2IDint] != null)
                {
                    V2HipAngle.Text = "HipAngle = " + String.Format("{0:0.00}", xmlreader2.Jointsdata[currentFrame2IDint].HipAngle);
                    V2ShoulderAngle.Text = "ShoulderAngle = " + String.Format("{0:0.00}", xmlreader2.Jointsdata[currentFrame2IDint].ShoulderAngle);
                    V2KneeAngle.Text = "KneeAngle = " + String.Format("{0:0.00}", xmlreader2.Jointsdata[currentFrame2IDint].KneeAngle);
                    V2HeadHeight.Text = "HeadHeight = " + String.Format("{0:0.00}", xmlreader2.Jointsdata[currentFrame2IDint].HeadHeight);
                    drawSkeletons1(currentFrame2IDint, ellipses2, xmlreader2);
                    drawSkeletons1_side(currentFrame2IDint, ellipses2_side, xmlreader2);
                    ResetLines2(currentFrame2IDint, xmlreader2);
                }

            }
        }

        private void timelineSlider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts1 = TimeSpan.FromSeconds(e.NewValue / 30);
            //ts1 = TimeSpan.FromMilliseconds(e.NewValue / 1000 * 30);
            string currentPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)ts1.TotalHours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds / 100, (double)ts1.TotalMilliseconds / 1000 * 30);

            timetxt1.Text = currentPosition;

        }

        private void timelineSlider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts2 = TimeSpan.FromSeconds(e.NewValue / 30);
            //ts1 = TimeSpan.FromMilliseconds(e.NewValue / 1000 * 30);
            string currentPosition = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)ts2.TotalHours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds / 100, (double)ts2.TotalMilliseconds / 1000 * 30);

            timetxt2.Text = currentPosition;
        }



        private void timelineSlider1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //mediaElement1.Pause();
            timer1.Stop();
        }

        private void timelineSlider2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //mediaElement2.Pause();
            timer2.Stop();
        }


        private void timelineSlider1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mediaElement1.Position = ts1;
            timer1.Start();
            mediaElement1.Play();
            mediaElement1.Pause();
            //timer1.Stop();
        }


        private void timelineSlider2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mediaElement2.Position = ts2;
            timer2.Start();
            mediaElement2.Play();
            mediaElement2.Pause();
        }


        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider1.Minimum = 0;
            timelineSlider1.Maximum = (double)mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds / 1000 * 30;
            TimeSpan totaotp = mediaElement1.NaturalDuration.TimeSpan;
            totalTimetxt1.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds, totaotp.Milliseconds / 100, totaotp.TotalSeconds * 30);
            timetxt1.Text = "00:00:00.0 Frame No. 0";
        }

        private void mediaElement2_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider2.Minimum = 0;
            timelineSlider2.Maximum = (double)mediaElement2.NaturalDuration.TimeSpan.TotalMilliseconds / 1000 * 30;
            TimeSpan totaotp = mediaElement2.NaturalDuration.TimeSpan;
            totalTimetxt2.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds, totaotp.Milliseconds / 100, totaotp.TotalSeconds * 30);
            timetxt2.Text = "00:00:00.0 Frame No. 0";
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (timer1.IsEnabled)
            {
                timer1.Stop();
            }

            if (timer2.IsEnabled)
            {
                timer2.Stop();
            }

        }


        public MainWindow()
        {
            InitializeComponent();

            timer1 = new DispatcherTimer();
            timer2 = new DispatcherTimer();
            //set interval of the clock to 0.033s (30fps)
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 33);
            timer2.Interval = new TimeSpan(0, 0, 0, 0, 33);
            //handle the firing of the timer
            timer1.Tick += SetDisplayMessage1;
            timer2.Tick += SetDisplayMessage2;
            SetImageForMediaElement();
        }




        //put the first frame of the video as the cover image of the media section
        public void SetImageForMediaElement()
        {
            mediaElement1.Play();
            mediaElement1.Pause();
            mediaElement1.Position = TimeSpan.Zero;
            mediaElement2.Play();
            mediaElement2.Pause();
            mediaElement2.Position = TimeSpan.Zero;
        }



        private void BrowseVideo1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".avi";
            dlg.Title = "Select video file..";
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3;*.mp4|All Files|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                Videodirectory1.Content = selectedFileName;
                mediaElement1.Source = new Uri(selectedFileName);

            }
        }

        private void BrowseVideo2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".avi";
            dlg.Title = "Select video file..";
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3;*.mp4|All Files|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                Videodirectory2.Content = selectedFileName;
                mediaElement2.Source = new Uri(selectedFileName);
                //mediaElement1.Play();
            }
        }

        private void LoadData1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".XML";
            dlg.Title = "Select associated data file..";
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "XML Files|*.XML|All Files|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                datadirectory1.Content = selectedFileName;
                xmlreader1 = new XMLReader(selectedFileName);
                System.Console.WriteLine("finish reading");
                V1Name.Text = xmlreader1.Fileproperty.Filename;
                //System.Console.WriteLine("reader.Jointsdata[1].Hipangle = "+reader.Jointsdata[1].HipAngle);
                //mediaElement1.Play();

                //initialiseDrawing(ellipses1, sCanvas1);

                //initialiseDrawing(ellipses1, sCanvas1, true);
                initialiseDrawing(ellipses1, sCanvas1_window, true);
                initialiseDrawing(ellipses1_side, sCanvas1, false);
                InitialiseLines1(sCanvas1);
             

            }
        }

        private void LoadData2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".XML";
            dlg.Title = "Select associated data file..";
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "XML Files|*.XML|All Files|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                datadirectory2.Content = selectedFileName;
                xmlreader2 = new XMLReader(selectedFileName);
                V2Name.Text = xmlreader2.Fileproperty.Filename;
                //System.Console.WriteLine("xmlreader2.Jointsdata[0].Hipangle =" + xmlreader2.Jointsdata[0].HipAngle);
                //System.Console.WriteLine("xmlreader2.Jointsdata[1].Hipangle = " + xmlreader2.Jointsdata[1].HipAngle);
                //mediaElement1.Play();
                initialiseDrawing(ellipses2, sCanvas2_window, true);
                initialiseDrawing(ellipses2_side, sCanvas2, false);
                InitialiseLines2(sCanvas2);
            }

        }

        private void Playvideo1_Click(object sender, RoutedEventArgs e)
        {
            // The Play method will begin the media if it is not currently active or 
            // resume media if it is paused. This has no effect if the media is
            // already running.

            if (!timer1.IsEnabled)
            {
                timer1.Start();
            }
            mediaElement1.Play();

            // Initialize the MediaElement property values.
            //InitializePropertyValues();
        }

        private void Playvideo2_Click(object sender, RoutedEventArgs e)
        {
            if (!timer2.IsEnabled)
            {
                timer2.Start();
            }
            mediaElement2.Play();
        }


        private void Pause1_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Pause();
        }


        private void Pause2_Click(object sender, RoutedEventArgs e)
        {
            mediaElement2.Pause();
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
        }

        private void Stop2_Click(object sender, RoutedEventArgs e)
        {
            mediaElement2.Stop();
        }


        private void Playboth_Click(object sender, RoutedEventArgs e)
        {
            if (!timer1.IsEnabled)
            {
                timer1.Start();
            }
            mediaElement1.Play();
            if (!timer2.IsEnabled)
            {
                timer2.Start();
            }
            mediaElement2.Play();
        }

        private void Pauseboth_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Pause();
            mediaElement2.Pause();
        }

        private void auto_align_Click(object sender, RoutedEventArgs e)
        {
            ts1 = TimeSpan.FromSeconds((double)xmlreader1.Fileproperty.StartingFrame / 30);
            string currentPosition1 = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)ts1.TotalHours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds / 100, (double)ts1.TotalMilliseconds / 1000 * 30);
            mediaElement1.Position = ts1;
            timetxt1.Text = currentPosition1;

            ts2 = TimeSpan.FromSeconds((double)xmlreader2.Fileproperty.StartingFrame / 30);
            mediaElement2.Position = ts2;
            string currentPosition2 = string.Format("{0:00}:{1:00}:{2:00}.{3:0} Frame No.{4:0.}", (int)ts1.TotalHours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds / 100, (double)ts1.TotalMilliseconds / 1000 * 30);

            timetxt2.Text = currentPosition2;
        }



        private void initialiseDrawing(Ellipse[] ellipses, Canvas sCanvas, Boolean frontal)
        {
            //initilaise drawing
            if (frontal == true)
            {
                //head
                ellipses[0] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.SandyBrown };
                //leftHand
                ellipses[1] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Black };
                //rightHand
                ellipses[2] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Black };
                //leftshoulder
                ellipses[3] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Green };
                //rightshoulder
                ellipses[4] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Green };
                //hip
                ellipses[5] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Yellow };
                //leftKnee
                ellipses[6] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Blue };
                //rightKnee
                ellipses[7] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Blue };
                //leftFoot
                ellipses[8] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Red };
                //rightFoot
                ellipses[9] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Red };
            }
            else
            {   //head
                ellipses[0] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.SandyBrown };
                //leftHand
                ellipses[1] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Black };
                //shoulder
                ellipses[2] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Green };
                //hip
                ellipses[3] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Yellow };
                //knee
                ellipses[4] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Blue };
                //foot
                ellipses[5] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.Red };
            }
            for (int i = 0; i < ellipses.Length; i++)
            {
                //ellipses[i] = new Ellipse { Width = 10, Height = 10, Fill = System.Windows.Media.Brushes.SandyBrown };
                //skeletonCanvas in the UI
                sCanvas.Children.Add(ellipses[i]);
            }
        }

        private void drawSkeletons1(int framenumber, Ellipse[] ellipses, XMLReader reader)
        {
            double shiftX = -20;
            double shiftY = -20;
                for (int i = 0; i < ellipses.Length; i++)
                {
                    //ellipses[i] = new Ellipse { Width = 20, Height = 20, Fill = System.Windows.Media.Brushes.SandyBrown };
                    ////skeletonCanvas in the UI
                    //sCanvas1.Children.Add(ellipses[i]);
                    switch (i)
                    {
                        case 0:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].headX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY * 0.75);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].headY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 1:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].leftHandX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].leftHandY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 2:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].rightHandX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].rightHandY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 3:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].leftshoulderX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].leftshoulderY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 4:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].rightshoulderX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].rightshoulderY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 5:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].hipX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].hipY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 6:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].leftKneeX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].leftKneeY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 7:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].rightKneeX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].rightKneeY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 8:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].leftFootX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].leftFootY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                        case 9:
                            Canvas.SetLeft(ellipses[i], reader.Jointsdata[framenumber].rightFootX * 0.75 - ellipses[i].Width / 2 + shiftX);
                            Canvas.SetTop(ellipses[i], reader.Jointsdata[framenumber].rightFootY * 0.75 - ellipses[i].Height / 2 + shiftY);
                            break;
                    }
                }
        }


       // private double coordinateCovert(double abs, Boolean isZ, Canvas sCanvas)
        //{


       // }


        //side-view drawing

        //normalisation
        private double coordinateCovert_side1(double abs, Boolean isZ, Canvas sCanvas)
        {
            double Convertfactor = 250;
            if (isZ)
            {
                return abs * Convertfactor - 250;
            } 
            else
            {
                return sCanvas.ActualHeight - (abs * Convertfactor + 100);
            }
         

        }

        private void drawSkeletons1_side(int framenumber, Ellipse[] ellipses1_side, XMLReader reader)
        {
            for (int i = 0; i < ellipses1_side.Length; i++)
            {
                //ellipses[i] = new Ellipse { Width = 20, Height = 20, Fill = System.Windows.Media.Brushes.SandyBrown };
                ////skeletonCanvas in the UI
                //sCanvas1.Children.Add(ellipses[i]);
                switch (i)
                {
                    case 0:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].headZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + coordinateCovert_side1(reader.Jointsdata[framenumber].headZabs / 2, true) );
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].headYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);                        
                        break;
                    case 1:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY / 2);
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);
                        break;
                    case 2:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY / 2);
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);
                        break;
                    case 3:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].hipZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY / 2);
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].hipYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);
                        break;
                    case 4:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY / 2);
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);
                        break;
                    case 5:
                        Canvas.SetLeft(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootZabs / 2, true, sCanvas1) - ellipses1_side[i].Width / 2);
                        //System.Console.WriteLine("headY = " + reader.Jointsdata[framenumber].headY / 2);
                        Canvas.SetTop(ellipses1_side[i], coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootYabs / 2, false, sCanvas1) - ellipses1_side[i].Width / 2);
                        break;

                }
            }
        }

        /// <summary>
        /// initalize lines 
        /// </summary>
        private void InitialiseLines1(Canvas sCanvas)
        {

            //initial position
            Shoulder_Hip1.X1 = 0;
            Shoulder_Hip1.Y1 = 0;
            Shoulder_Hip1.X2 = 20;
            Shoulder_Hip1.Y2 = 20;

            Shoulder_Hand1.X1 = 0;
            Shoulder_Hand1.Y1 = 0;
            Shoulder_Hand1.X2 = 20;
            Shoulder_Hand1.Y2 = 20;

            Hip_Knee1.X1 = 0;
            Hip_Knee1.Y1 = 0;
            Hip_Knee1.X2 = 20;
            Hip_Knee1.Y2 = 20;

            Knee_Foot1.X1 = 0;
            Knee_Foot1.Y1 = 0;
            Knee_Foot1.X2 = 20;
            Knee_Foot1.Y2 = 20;



            // Create Brushes
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            SolidColorBrush brownBrush = new SolidColorBrush();
            brownBrush.Color = Colors.Brown;

            // Set Line's width and color
            Shoulder_Hip1.StrokeThickness = 2;
            Shoulder_Hip1.Stroke = redBrush;
            Shoulder_Hand1.StrokeThickness = 2;
            Shoulder_Hand1.Stroke = redBrush;
            Hip_Knee1.StrokeThickness = 2;
            Hip_Knee1.Stroke = brownBrush;
            Knee_Foot1.StrokeThickness = 2;
            Knee_Foot1.Stroke = brownBrush;


            // Add lines to the Canvas.
            sCanvas.Children.Add(Shoulder_Hip1);
            sCanvas.Children.Add(Shoulder_Hand1);
            sCanvas.Children.Add(Hip_Knee1);
            sCanvas.Children.Add(Knee_Foot1);
        }

        private void InitialiseLines2(Canvas sCanvas)
        {

            //initial position
            Shoulder_Hip2.X1 = 0;
            Shoulder_Hip2.Y1 = 0;
            Shoulder_Hip2.X2 = 20;
            Shoulder_Hip2.Y2 = 20;

            Shoulder_Hand2.X1 = 0;
            Shoulder_Hand2.Y1 = 0;
            Shoulder_Hand2.X2 = 20;
            Shoulder_Hand2.Y2 = 20;

            Hip_Knee2.X1 = 0;
            Hip_Knee2.Y1 = 0;
            Hip_Knee2.X2 = 20;
            Hip_Knee2.Y2 = 20;

            Knee_Foot2.X1 = 0;
            Knee_Foot2.Y1 = 0;
            Knee_Foot2.X2 = 20;
            Knee_Foot2.Y2 = 20;



            // Create Brushes
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            SolidColorBrush brownBrush = new SolidColorBrush();
            brownBrush.Color = Colors.Brown;
            //SolidColorBrush greenBrush = new SolidColorBrush();
            //greenBrush.Color = Colors.Green;
            //SolidColorBrush blueBrush = new SolidColorBrush();
            //blueBrush.Color = Colors.Blue;

            // Set Line's width and color
            Shoulder_Hip2.StrokeThickness = 2;
            Shoulder_Hip2.Stroke = redBrush;
            Shoulder_Hand2.StrokeThickness = 2;
            Shoulder_Hand2.Stroke = redBrush;
            Hip_Knee2.StrokeThickness = 2;
            Hip_Knee2.Stroke = brownBrush;
            Knee_Foot2.StrokeThickness = 2;
            Knee_Foot2.Stroke = brownBrush;


            // Add lines to the Canvas.
            sCanvas.Children.Add(Shoulder_Hip2);
            sCanvas.Children.Add(Shoulder_Hand2);
            sCanvas.Children.Add(Hip_Knee2);
            sCanvas.Children.Add(Knee_Foot2);
        }


        //reset lines positions
        private void ResetLines1(int framenumber, XMLReader reader)
        {

            Shoulder_Hip1.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderZabs / 2, true, sCanvas1));
            Shoulder_Hip1.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderYabs / 2, false, sCanvas1));
            Shoulder_Hip1.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipZabs / 2, true, sCanvas1));
            Shoulder_Hip1.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipYabs / 2, false, sCanvas1));

            Shoulder_Hand1.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderZabs / 2, true, sCanvas1));
            Shoulder_Hand1.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderYabs / 2, false, sCanvas1));
            Shoulder_Hand1.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandZabs / 2, true, sCanvas1));
            Shoulder_Hand1.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandYabs / 2, false, sCanvas1));

            Hip_Knee1.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipZabs / 2, true, sCanvas1));
            Hip_Knee1.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipYabs / 2, false, sCanvas1));
            Hip_Knee1.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeZabs / 2, true, sCanvas1));
            Hip_Knee1.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeYabs / 2, false, sCanvas1));

            Knee_Foot1.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeZabs / 2, true, sCanvas1));
            Knee_Foot1.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeYabs / 2, false, sCanvas1));
            Knee_Foot1.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootZabs / 2, true, sCanvas1));
            Knee_Foot1.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootYabs / 2, false, sCanvas1));

        }


        private void ResetLines2(int framenumber, XMLReader reader)
        {

            Shoulder_Hip2.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderZabs / 2, true, sCanvas2));
            Shoulder_Hip2.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderYabs / 2, false, sCanvas2));
            Shoulder_Hip2.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipZabs / 2, true, sCanvas2));
            Shoulder_Hip2.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipYabs / 2, false, sCanvas2));

            Shoulder_Hand2.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderZabs / 2, true, sCanvas2));
            Shoulder_Hand2.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftshoulderYabs / 2, false, sCanvas2));
            Shoulder_Hand2.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandZabs / 2, true, sCanvas2));
            Shoulder_Hand2.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftHandYabs / 2, false, sCanvas2));

            Hip_Knee2.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipZabs / 2, true, sCanvas2));
            Hip_Knee2.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].hipYabs / 2, false, sCanvas2));
            Hip_Knee2.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeZabs / 2, true, sCanvas2));
            Hip_Knee2.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeYabs / 2, false, sCanvas2));

            Knee_Foot2.X1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeZabs / 2, true, sCanvas2));
            Knee_Foot2.Y1 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftKneeYabs / 2, false, sCanvas2));
            Knee_Foot2.X2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootZabs / 2, true, sCanvas2));
            Knee_Foot2.Y2 = (coordinateCovert_side1(reader.Jointsdata[framenumber].leftFootYabs / 2, false, sCanvas2));

        }









    }
}
