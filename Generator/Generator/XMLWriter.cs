using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinect.Recorder;
using Kinect.Replay;
using Kinect.Replay.Replay;
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Skeletons;
using Analyser;
using Microsoft.Kinect;
using System.Xml;


public struct Vector3
{
    public double x, y, z;

    public Vector3(double p1, double p2, double p3)
    {
        x = p1;
        y = p2;
        z = p3;
    }

    static public double dot(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    static public Vector3 minus(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public Vector3 normalise()
    {
        double length = Math.Sqrt(x * x + y * y + z * z);
        x = x / length;
        y = y / length;
        z = z / length;
        return this;
    }

}

namespace Generator
{
    class XMLWriter
    {
        XmlWriter frameWriter;
        //XmlWriter keyFrameWriter;
        Fileproperty fileproperty = new Fileproperty();
        List<Framedata> jointsdata = new List<Framedata>();
        //List<int> keyframes = new List<int>();
        //List<double> headHeight = new List<double>();
        //string liftname;
        bool isStartingFrame = true;
        //the first frame number of the video
        int initialFrameNumber = 0;
        //the frame number which the lift actually starts
        int startingFrameNumber;
        int currentFrameNumber = 0;

        public XMLWriter(string AngleWriterPath, string keyFrameWriterPath, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            //settings.WriteEndDocumentOnClose = true;

            frameWriter = XmlWriter.Create(AngleWriterPath, settings);
            fileproperty.Filename = filename;
            //keyframes.
            //keyFrameWriter = XmlWriter.Create(keyFrameWriterPath);
        }

        //XmlWriter angleWriter = XmlWriter.Create("employees.xml");

        static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public double AngleBetweenTwoVectors(Vector3 vectorA, Vector3 vectorB)
        {
            double dotProduct = 0.0f;
            dotProduct = Vector3.dot(vectorA, vectorB);

            return (double)Math.Acos(dotProduct);
        }


        private double JointAngle(Joint main, Joint s1, Joint s2)
        {
            ////30/01/2013 Retrieve 3D coordinate of joints
            ////angle measurement
            Vector3 a1 = new Vector3(main.Position.X, main.Position.Y, main.Position.Z);
            Vector3 a2 = new Vector3(s1.Position.X, s1.Position.Y, s1.Position.Z);
            Vector3 a3 = new Vector3(s2.Position.X, s2.Position.Y, s2.Position.Z);

            Vector3 b1 = Vector3.minus(a3, a1);
            Vector3 b2 = Vector3.minus(a2, a1);
            ////AngleBetweenTwoVectors(b1.normalise(), b2.normalise()).ToString();
            double angle = AngleBetweenTwoVectors(b1.normalise(), b2.normalise());
            //System.Console.WriteLine("angle = {0}", RadianToDegree(angle));
            return RadianToDegree(angle);
        }


        //public void initialstore(ReplaySkeletonFrame frame, int framenumber)


        public void storeframe(ReplaySkeletonFrame frame, int framenumber, CoordinateMapper replayCoordinateMapper)
        {
            //int framenumber;
            double leftShoulderAngle, leftHipAngle, leftKneeAngle, headHeight;
            double headX, headY, leftHandX, leftHandY, rightHandX, rightHandY;
            double leftshoulderX, leftshoulderY, rightshoulderX, rightshoulderY;
            double hipX, hipY, leftKneeX, leftKneeY, rightKneeX, rightKneeY;
            double leftFootX, leftFootY, rightFootX, rightFootY;

            //side view

            double headYabs, headZabs, leftHandYabs, leftHandZabs;
            double leftshoulderYabs, leftshoulderZabs, hipYabs, hipZabs;
            double leftKneeYabs, leftKneeZabs, leftFootYabs, leftFootZabs;



            if (isStartingFrame == true)
            {
                initialFrameNumber = framenumber;
                System.Console.WriteLine("initialFrame = " + initialFrameNumber);
                isStartingFrame = false;

            }

            if (frame == null)
            {
                //currentFrameNumber = framenumber - initialFrameNumber;
                leftShoulderAngle = 999;
                leftHipAngle = 999;
                leftKneeAngle = 999;
                headHeight = 999;
                headX = 999;
                headY = 999;
                leftHandX = 999;
                leftHandY = 999;
                rightHandX = 999;
                rightHandY = 999;
                leftshoulderX = 999;
                leftshoulderY = 999;
                rightshoulderX = 999;
                rightshoulderY = 999;
                hipX = 999;
                hipY = 999;
                leftKneeX = 999; 
                leftKneeY = 999;
                rightKneeX = 999;
                rightKneeY = 999;
                leftFootX = 999;
                leftFootY = 999;
                rightFootX = 999;
                rightFootY = 999;

                //side view
                headYabs = 999;
                headZabs = 999;
                leftHandYabs = 999;
                leftHandZabs = 999;
                leftshoulderYabs = 999;
                leftshoulderZabs = 999;
                hipYabs = 999;
                hipZabs = 999;
                leftKneeYabs = 999;
                leftKneeZabs = 999;
                leftFootYabs = 999;
                leftFootZabs = 999;
                

                //currentFrameNumber++;
            }

            else
            {


                var trackedSkeleton = frame.Skeletons.FirstOrDefault(a => a.TrackingState == SkeletonTrackingState.Tracked);
                //IMPORTATNT: 23 occurences frame here for deadlift1.replay
                if (trackedSkeleton == null)
                    return;

                //set starting frame of the video to index 1
                //framenumber = frame.FrameNumber - initialFrameNumber + 1;
                //currentFrameNumber = framenumber;
                leftShoulderAngle = JointAngle(trackedSkeleton.Joints[JointType.ShoulderLeft], trackedSkeleton.Joints[JointType.WristLeft], trackedSkeleton.Joints[JointType.HipLeft]);
                leftHipAngle = JointAngle(trackedSkeleton.Joints[JointType.HipLeft], trackedSkeleton.Joints[JointType.ShoulderLeft], trackedSkeleton.Joints[JointType.KneeLeft]);
                leftKneeAngle = JointAngle(trackedSkeleton.Joints[JointType.KneeLeft], trackedSkeleton.Joints[JointType.FootLeft], trackedSkeleton.Joints[JointType.HipLeft]);
                headHeight = trackedSkeleton.Joints[JointType.Head].Position.Y;
                //frontal view
                headX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.Head].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                headY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.Head].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                leftHandX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                leftHandY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                rightHandX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                rightHandY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                leftshoulderX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                leftshoulderY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                rightshoulderX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                rightshoulderY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                hipX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HipCenter].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                hipY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HipCenter].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                leftKneeX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                leftKneeY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                rightKneeX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                rightKneeY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                leftFootX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                leftFootY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
                rightFootX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
                rightFootY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;


                //side view 22/03/2013
                headYabs = trackedSkeleton.Joints[JointType.Head].Position.Y;
                headZabs = trackedSkeleton.Joints[JointType.Head].Position.Z;
                leftHandYabs = trackedSkeleton.Joints[JointType.HandLeft].Position.Y;
                leftHandZabs = trackedSkeleton.Joints[JointType.HandLeft].Position.Z;
                leftshoulderYabs = trackedSkeleton.Joints[JointType.ShoulderLeft].Position.Y;
                leftshoulderZabs = trackedSkeleton.Joints[JointType.ShoulderLeft].Position.Z;
                hipYabs = trackedSkeleton.Joints[JointType.HipCenter].Position.Y;
                hipZabs = trackedSkeleton.Joints[JointType.HipCenter].Position.Z;
                leftKneeYabs = trackedSkeleton.Joints[JointType.KneeLeft].Position.Y;
                leftKneeZabs = trackedSkeleton.Joints[JointType.KneeLeft].Position.Z;
                leftFootYabs = trackedSkeleton.Joints[JointType.FootLeft].Position.Y;
                leftFootZabs = trackedSkeleton.Joints[JointType.FootLeft].Position.Z;
                

            }

            currentFrameNumber = framenumber - initialFrameNumber + 1;
            //System.Console.WriteLine("framenumber = " + framenumber);
            //System.Console.WriteLine("initialFrameNumber = " + initialFrameNumber);
            //System.Console.WriteLine("currentFrameNumber = " + currentFrameNumber);
            Framedata currentframe = new Framedata(currentFrameNumber, leftHipAngle, leftShoulderAngle, leftKneeAngle, headHeight, headX, headY, leftHandX, leftHandY, rightHandX, rightHandY, leftshoulderX, leftshoulderY, rightshoulderX, rightshoulderY, hipX, hipY, leftKneeX, leftKneeY, rightKneeX, rightKneeY, leftFootX, leftFootY, rightFootX, rightFootY, headYabs, headZabs, leftHandYabs, leftHandZabs, leftshoulderYabs, leftshoulderZabs, hipYabs, hipZabs, leftKneeYabs, leftKneeZabs, leftFootYabs, leftFootZabs);
            jointsdata.Add(currentframe);



        }

        private int getStartingFrameNumberofLift()
        {
            List<Framedata> jointsdata_clone = new List<Framedata>(jointsdata);
            jointsdata_clone.Sort();
            Framedata startingFrame = jointsdata_clone[0];
            startingFrameNumber = startingFrame.FrameID;
            return startingFrameNumber;
        }

        private void setFileproperty()
        {
            fileproperty.StartingFrame = getStartingFrameNumberofLift();
            fileproperty.TotalFrames = jointsdata.Count;

        }

        public void write()
        {
            setFileproperty();


            frameWriter.WriteStartDocument();
            frameWriter.WriteStartElement("Lift");
            frameWriter.WriteStartElement("Fileproperty");
            frameWriter.WriteElementString("Name", fileproperty.Filename);
            frameWriter.WriteElementString("StartingFrame", fileproperty.StartingFrame.ToString());
            frameWriter.WriteElementString("TotalFrames", fileproperty.TotalFrames.ToString());
            frameWriter.WriteEndElement();
            foreach (Framedata frame in jointsdata)
            {
                frameWriter.WriteStartElement("Frame");
                frameWriter.WriteAttributeString("ID", frame.FrameID.ToString());
                frameWriter.WriteElementString("HipAngle", frame.HipAngle.ToString());
                frameWriter.WriteElementString("ShoulderAngle", frame.ShoulderAngle.ToString());
                frameWriter.WriteElementString("KneeAngle", frame.KneeAngle.ToString());
                frameWriter.WriteElementString("HeadHeight", frame.HeadHeight.ToString());

                frameWriter.WriteElementString("headX", frame.headX.ToString());
                frameWriter.WriteElementString("headY", frame.headY.ToString());
                frameWriter.WriteElementString("leftHandX", frame.leftHandX.ToString());
                frameWriter.WriteElementString("leftHandY", frame.leftHandY.ToString());
                frameWriter.WriteElementString("rightHandX", frame.rightHandX.ToString());
                frameWriter.WriteElementString("rightHandY", frame.rightHandY.ToString());
                frameWriter.WriteElementString("leftshoulderX", frame.leftshoulderX.ToString());
                frameWriter.WriteElementString("leftshoulderY", frame.leftshoulderY.ToString());
                frameWriter.WriteElementString("rightshoulderX", frame.rightshoulderX.ToString());
                frameWriter.WriteElementString("rightshoulderY", frame.rightshoulderY.ToString());
                frameWriter.WriteElementString("hipX", frame.hipX.ToString());
                frameWriter.WriteElementString("hipY", frame.hipY.ToString());
                frameWriter.WriteElementString("leftKneeX", frame.leftKneeX.ToString());
                frameWriter.WriteElementString("leftKneeY", frame.leftKneeY.ToString());
                frameWriter.WriteElementString("rightKneeX", frame.rightKneeX.ToString());
                frameWriter.WriteElementString("rightKneeY", frame.rightKneeY.ToString());
                frameWriter.WriteElementString("leftFootX", frame.leftFootX.ToString());
                frameWriter.WriteElementString("leftFootY", frame.leftFootY.ToString());
                frameWriter.WriteElementString("rightFootX", frame.rightFootX.ToString());
                frameWriter.WriteElementString("rightFootY", frame.rightFootY.ToString());
                //side view

                frameWriter.WriteElementString("headYabs", frame.headYabs.ToString());
                frameWriter.WriteElementString("headZabs", frame.headZabs.ToString());
                frameWriter.WriteElementString("leftHandYabs", frame.leftHandYabs.ToString());
                frameWriter.WriteElementString("leftHandZabs", frame.leftHandZabs.ToString());
                frameWriter.WriteElementString("leftshoulderYabs", frame.leftshoulderYabs.ToString());
                frameWriter.WriteElementString("leftshoulderZabs", frame.leftshoulderZabs.ToString());
                frameWriter.WriteElementString("hipYabs", frame.hipYabs.ToString());
                frameWriter.WriteElementString("hipZabs", frame.hipZabs.ToString());
                frameWriter.WriteElementString("leftKneeYabs", frame.leftKneeYabs.ToString());
                frameWriter.WriteElementString("leftKneeZabs", frame.leftKneeZabs.ToString());
                frameWriter.WriteElementString("leftFootYabs", frame.leftFootYabs.ToString());
                frameWriter.WriteElementString("leftFootZabs", frame.leftFootZabs.ToString());



                frameWriter.WriteEndElement();
            }

            frameWriter.WriteEndElement();
            frameWriter.WriteEndDocument();
            frameWriter.Close();
            System.Console.WriteLine("Writing finished");

        }


    }
}
//XML TEMPLATE:
//<Lift>
//<Fileproperty>
//<Name>lift1</Name> 
//<StartingFrame>150</StartingFrame>
//<TotalFrames>5000</TotalFrames>
//</Fileproperty>
// <Frame ID = “1”>
//    <HipAngle>50</HipAngle>
//    <ShoulderAngle>100</ShoulderAngle>
//    <KneeAngle>120</KneeAngle>
//<HeadHeight>1.5</headHeight>
// </Frame>
//  <Frame ID = “1”>
//    <HipAngle>50</HipAngle>
//    <ShoulderAngle>100</ShoulderAngle>
//    <KneeAngle>120</KneeAngle>
// </Frame>
//</Lift>





//private double leftKneeAngle(Skeleton sk)
//{
//    ////30/01/2013 Retrieve 3D coordinate of joints
//    ////angle measurement
//    Vector3 a1 = new Vector3(sk.Joints[JointType.KneeLeft].Position.X, sk.Joints[JointType.ElbowLeft].Position.Y, sk.Joints[JointType.ElbowLeft].Position.Z);
//    Vector3 a2 = new Vector3(sk.Joints[JointType.FootLeft].Position.X, sk.Joints[JointType.ShoulderLeft].Position.Y, sk.Joints[JointType.ShoulderLeft].Position.Z);
//    Vector3 a3 = new Vector3(sk.Joints[JointType.HipLeft].Position.X, sk.Joints[JointType.WristLeft].Position.Y, sk.Joints[JointType.WristLeft].Position.Z);

//    Vector3 b1 = Vector3.minus(a3, a1);
//    Vector3 b2 = Vector3.minus(a2, a1);
//    ////AngleBetweenTwoVectors(b1.normalise(), b2.normalise()).ToString();
//    double angle = AngleBetweenTwoVectors(b1.normalise(), b2.normalise());
//    //System.Console.WriteLine("angle = {0}", RadianToDegree(angle));
//    return RadianToDegree(angle);
//}