using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyser
{
    class KeyframesInfo
    {

             public KeyframesInfo()
        {
        }

             int frameID;
             double headHeight;

             public KeyframesInfo(int id, double headHeight)
        {
            this.frameID = id;
            this.headHeight = headHeight;
        }

        public int FrameID
        {
            get { return frameID; }
            set { frameID = value; }
        }


        public double HeadHeight
        {
            get { return headHeight; }
            set { headHeight = value; }
        }

    }
}
