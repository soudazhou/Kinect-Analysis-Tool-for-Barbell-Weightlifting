using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyser
{
    public class Framedata : IComparable<Framedata>
    {
        int frameID;

        public Framedata()
        {

        }

        public Framedata(int id, double hipAngle, double shoulderAngle, double kneeAngle, double headHeight, double headX, double headY, double leftHandX, double leftHandY, double rightHandX, double rightHandY, double leftshoulderX, double leftshoulderY, double rightshoulderX, double rightshoulderY, double hipX, double hipY, double leftKneeX, double leftKneeY, double rightKneeX, double rightKneeY, double leftFootX, double leftFootY, double rightFootX, double rightFootY, double headYabs, double headZabs, double leftHandYabs, double leftHandZabs, double leftshoulderYabs, double leftshoulderZabs, double hipYabs, double hipZabs, double leftKneeYabs, double leftKneeZabs, double leftFootYabs, double leftFootZabs)
        {
            this.frameID = id;
            this.hipAngle = hipAngle;
            this.shoulderAngle = shoulderAngle;
            this.kneeAngle = kneeAngle;
            this.headHeight = headHeight;
            //frontal view
            this.headX = headX;
            this.headY = headY;
            this.leftHandX = leftHandX;
            this.leftHandY = leftHandY;
            this.rightHandX = rightHandX;
            this.rightHandY = rightHandY;
            this.leftshoulderX = leftshoulderX;
            this.leftshoulderY = leftshoulderY;
            this.rightshoulderX = rightshoulderX;
            this.rightshoulderY = rightshoulderY;
            this.hipX = hipX;
            this.hipY = hipY;
            this.leftKneeX = leftKneeX;
            this.leftKneeY = leftKneeY;
            this.rightKneeX = rightKneeX;
            this.rightKneeY = rightKneeY;
            this.leftFootX = leftFootX;
            this.leftFootY = leftFootY;
            this.rightFootX = rightFootX;
            this.rightFootY = rightFootY;

            //side view
            this.headYabs = headYabs;
            this.headZabs = headZabs;
            this.leftHandYabs = leftHandYabs;
            this.leftHandZabs = leftHandZabs;
            this.leftshoulderYabs = leftshoulderYabs;
            this.leftshoulderZabs = leftshoulderZabs;
            this.hipYabs = hipYabs;
            this.hipZabs = hipZabs;
            this.leftKneeYabs = leftKneeYabs;
            this.leftKneeZabs = leftKneeZabs;
            this.leftFootYabs = leftFootYabs;
            this.leftFootZabs = leftFootZabs;

        }

        public int FrameID
        {
            get { return frameID; }
            set { frameID = value; }
        }

        double hipAngle;

        public double HipAngle
        {
            get { return hipAngle; }
            set { hipAngle = value; }
        }


        double shoulderAngle;

        public double ShoulderAngle
        {
            get { return shoulderAngle; }
            set { shoulderAngle = value; }
        }

        double kneeAngle;

        public double KneeAngle
        {
            get { return kneeAngle; }
            set { kneeAngle = value; }
        }

        public double headHeight;
        public double HeadHeight
        {
            get { return headHeight; }
            set { headHeight = value; }
        }

        //frontal view
        public double headX { get; set; }
        public double headY { get; set; }
        public double leftHandX { get; set; }
        public double leftHandY { get; set; }
        public double rightHandX { get; set; }
        public double rightHandY { get; set; }
        public double leftshoulderX { get; set; }
        public double leftshoulderY { get; set; }
        public double rightshoulderX { get; set; }
        public double rightshoulderY { get; set; }
        public double hipX { get; set; }
        public double hipY { get; set; }
        public double leftKneeX { get; set; }
        public double leftKneeY { get; set; }
        public double rightKneeX { get; set; }
        public double rightKneeY { get; set; }
        public double leftFootX { get; set; }
        public double leftFootY { get; set; }
        public double rightFootX { get; set; }
        public double rightFootY { get; set; }

        //side-view
        public double headYabs { get; set; }
        public double headZabs { get; set; }
        public double leftHandYabs { get; set; }
        public double leftHandZabs { get; set; }
        //public double rightHandYabs { get; set; }
        //public double rightHandZabs { get; set; }
        public double leftshoulderYabs { get; set; }
        public double leftshoulderZabs { get; set; }
        //public double rightshoulderYabs { get; set; }
        //public double rightshoulderZabs { get; set; }
        public double hipYabs { get; set; }
        public double hipZabs { get; set; }
        public double leftKneeYabs { get; set; }
        public double leftKneeZabs { get; set; }
        //public double rightKneesYabs { get; set; }
        //public double rightKneesZabs { get; set; }
        public double leftFootYabs { get; set; }
        public double leftFootZabs { get; set; }
        //public double rightFootYabs { get; set; }
        //public double rightFootZabs { get; set; }



        //headX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.Head].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        headY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.Head].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        leftHandX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        leftHandY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        rightHandX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        rightHandY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HandRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        leftshoulderX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        leftshoulderY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        rightshoulderX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        rightshoulderY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.ShoulderRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        hipX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HipCenter].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        hipY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.HipCenter].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        leftKneeX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        leftKneeY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        rightKneeX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        rightKneeY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.KneeRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        leftFootX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        leftFootY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootLeft].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;
        //        rightFootX = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootRight].Position, ColorImageFormat.RgbResolution640x480Fps30).X;
        //        rightFootY = replayCoordinateMapper.MapSkeletonPointToColorPoint(trackedSkeleton.Joints[JointType.FootRight].Position, ColorImageFormat.RgbResolution640x480Fps30).Y;








        public int CompareTo(Framedata other)
        {
            //sort by framenuber if headheight are equal.[LOW TO HIGH]
            if (this.headHeight == other.headHeight)
            {
                return this.frameID.CompareTo(other.frameID);
            }
            // Default to headheight sort.[LOW TO HIGH]
            return this.headHeight.CompareTo(other.headHeight);
        }

    }
}
