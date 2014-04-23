using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Analyser
{
    class XMLReader
    {
        public XMLReader(String filename)
        {
            this.readData(filename);
        }

        private Fileproperty fileproperty = new Fileproperty();

        internal Fileproperty Fileproperty
        {
            get { return fileproperty; }
            set { fileproperty = value; }
        }
        private Framedata[] jointsdata;

        internal Framedata[] Jointsdata
        {
            get { return jointsdata; }
            set { jointsdata = value; }
        }


        public void readInFilepropertyElement(XmlReader reader, string dotName, Fileproperty fileproperty)
        {
            //while (reader.NodeType != XmlNodeType.EndElement)
            //{
                reader.Read();//now on text node
                if (reader.NodeType == XmlNodeType.Text)
                {
                    //Console.WriteLine(dotName + " = " + reader.Value);
                    switch (dotName)
                    {
                        case "Name":
                            fileproperty.Filename = reader.Value;
                            reader.Read();//now on end element like </Name>
                            break;
                        case "StartingFrame":
                            fileproperty.StartingFrame = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </Name>
                            break;
                        case "TotalFrames":
                            fileproperty.TotalFrames = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </Name>
                            break;

                    }
                }
            //}
            reader.Read();//now on the next start element like <StartingFrame>

        }

        public void readInFramedataElement(XmlReader reader, string dotName, Framedata framedata)
        {
            //while (reader.NodeType != XmlNodeType.EndElement && reader.Name != "Frame")
            //{
                reader.Read();
                if (reader.NodeType == XmlNodeType.Text)
                {
                    //Console.WriteLine(dotName + " = " + reader.Value);

                    switch (dotName)
                    {
                        case "HipAngle":
                            framedata.HipAngle = Convert.ToDouble(reader.Value);
                             reader.Read();//now on end element like </HipAngle>
                            break;
                        case "ShoulderAngle":
                            framedata.ShoulderAngle = Convert.ToDouble(reader.Value);
                             reader.Read();//now on end element like </HipAngle>
                            break;
                        case "KneeAngle":
                            framedata.KneeAngle = Convert.ToDouble(reader.Value);
                             reader.Read();//now on end element like </HipAngle>
                            break;
                        case "HeadHeight":
                            framedata.HeadHeight = Convert.ToDouble(reader.Value);
                             reader.Read();//now on end element like </HipAngle>
                            break;



                            //joint points
                            //frontal view
                        case "headX":
                            framedata.headX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "headY":
                            framedata.headY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftHandX":
                            framedata.leftHandX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftHandY":
                            framedata.leftHandY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightHandX":
                            framedata.rightHandX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightHandY":
                            framedata.rightHandY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftshoulderX":
                            framedata.leftshoulderX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftshoulderY":
                            framedata.leftshoulderY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightshoulderX":
                            framedata.rightshoulderX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightshoulderY":
                            framedata.rightshoulderY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "hipX":
                            framedata.hipX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "hipY":
                            framedata.hipY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftKneeX":
                            framedata.leftKneeX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftKneeY":
                            framedata.leftKneeY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightKneeX":
                            framedata.rightKneeX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightKneeY":
                            framedata.rightKneeY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftFootX":
                            framedata.leftFootX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftFootY":
                            framedata.leftFootY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightFootX":
                            framedata.rightFootX = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "rightFootY":
                            framedata.rightFootY = Convert.ToInt32(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;

                            //side view

                        case "headYabs":
                            framedata.headYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "headZabs":
                            framedata.headZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftHandYabs":
                            framedata.leftHandYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftHandZabs":
                            framedata.leftHandZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftshoulderYabs":
                            framedata.leftshoulderYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftshoulderZabs":
                            framedata.leftshoulderZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "hipYabs":
                            framedata.hipYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "hipZabs":
                            framedata.hipZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftKneeYabs":
                            framedata.leftKneeYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftKneeZabs":
                            framedata.leftKneeZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftFootYabs":
                            framedata.leftFootYabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                        case "leftFootZabs":
                            framedata.leftFootZabs = Convert.ToDouble(reader.Value);
                            reader.Read();//now on end element like </HipAngle>
                            break;
                    }

                }
           // }
                reader.Read();//now on the next start element like <StartingFrame>

        }



        //read data from XML and store it in a List<T>
        public void readData(String filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        //Console.WriteLine("reader.Name = " + reader.Name);
                        switch (reader.Name)
                        {
                            case "Lift":
                                reader.Read();//now on <Fileproperty>
                                Boolean enterSubelementRegion = false;
                                // Detect this element.
                                //Console.WriteLine("Start <Lift> element.");
                                //string liftname = reader["Name"];
                                //int startingFrame = Convert.ToInt32(reader["StartingFrame"]);
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (enterSubelementRegion == false)
                                    {
                                        reader.Read();//now in the sub element region like <Name>
                                        enterSubelementRegion = true;
                                    }
                                    readInFilepropertyElement(reader, reader.Name, fileproperty);

                                }
                                jointsdata = new Framedata[fileproperty.TotalFrames + 100];
                                break;
                            case "Frame":
                                // Detect this article element.
                                //Console.WriteLine("Start <Frame> element.");
                                // Search for the attribute name on this current node.
                                Boolean enterSubelementRegion2 = false;
                                string frameID = reader["ID"];
                                int id = Convert.ToInt32(frameID);
                                //Console.WriteLine("ID = " +id);
                                //create object for information in this frame
                                Framedata fdata = new Framedata();
                                fdata.FrameID = id;
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (enterSubelementRegion2 == false)
                                    {
                                        reader.Read();
                                        enterSubelementRegion2 = true;
                                    }
                                    readInFramedataElement(reader, reader.Name, fdata);

                                }

                                jointsdata[id] = fdata;
                                break;
                        }
                    }
                }
            }


        }

    }
}
//if (reader.Name == "Name")
//{
//    readInFilepropertyElement(reader, "Name", fileproperty.Filename);
//}

//else if (reader.Name == "StartingFrame")
//{
//    readInFilepropertyElement(reader, "StartingFrame", fileproperty.StartingFrame);
//}

//else if (reader.Name == "TotalFrames")
//{
//    readInFilepropertyElement(reader, "TotalFrames", fileproperty.TotalFrames);
//}

//if (reader.Name == "Hipangle")
//{
//    while (reader.NodeType != XmlNodeType.EndElement)
//    {
//        reader.Read();
//        if (reader.NodeType == XmlNodeType.Text)
//        {
//            Console.WriteLine("Hipangle = " + reader.Value);
//            fdata.HipAngle = Convert.ToDouble(reader.Value);

//        }
//    }
//    reader.Read();
//}