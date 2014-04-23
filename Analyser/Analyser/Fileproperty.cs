using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyser
{
    public class Fileproperty
    {
        public Fileproperty()
        {

        }

        public Fileproperty(string filename, int startingFrame)
        {
            this.filename = filename;
            this.startingFrame = startingFrame;
        }


        private String filename;

        public String Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        private int startingFrame;

        public int StartingFrame
        {
            get { return startingFrame; }
            set { startingFrame = value; }
        }

        private int totalFrames;

        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }


    }
}
