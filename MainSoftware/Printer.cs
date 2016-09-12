using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace MainSoftware
{
    public class Printer
    {
        public double NozzleDiameter = 0.3;
        public double layerStep = 0.86, zStep = 0.409;//0.42
        public double CurrentX = 0, CurrentY = 0, CurrentZ = 0;

        public double ExtrusionRate = 0.04;//D
        public double PlateHeight;
        public double PlateWidth;
        public double FeedRate = 8;//Fscale
        public double ExtrusionLength;//E

        public Object3D myObject;
        public List<string> Gcodes;
        public Printer()
        {
            Gcodes = new List<string>();

        }
        public void setObject(Object3D x)
        {
            myObject = x;
        }
        public void Reset()
        {
            CurrentX = 0;
            CurrentY = 0;
            CurrentZ = 0;
        }
        public void setNozzleDiameter(double Nozzlediameter, double layerstep = 0, double z = 0)
        {
            NozzleDiameter = Nozzlediameter;
            if (layerstep != 0)
                layerStep = layerstep;
            else
                layerStep = NozzleDiameter;

           
            if (z != 0)
                zStep = z;
            else
                zStep = NozzleDiameter;

        }
        

        public void 
            PrintLines(double zLevel, List<Line> myLines)
        {
            if (myLines.Count == 0)
                return;
            Point2D p1, p2, temp;
            p1 = myLines[0].P1;
            p2 = myLines[0].P2;
           
            if (Math.Min(p2.GetDistanceTo(myLines[1].P1), p2.GetDistanceTo(myLines[1].P2)) > Math.Min(p1.GetDistanceTo(myLines[1].P1), p1.GetDistanceTo(myLines[1].P2)))
            {
                temp = p1;
                p1 = p2;
                p2 = temp;
            }
            G00(p1.x, p1.y, zLevel);
            if (myLines[0].print)
                G01(p2.x, p2.y, zLevel);
            else
               G00(p2.x, p2.y, zLevel);

            for (int i = 1; i < myLines.Count; i++)
            {
                temp = p2;
                
                if (p2.GetDistanceTo(myLines[i].P1) > p2.GetDistanceTo(myLines[i].P2))
                {

                    p1 = myLines[i].P2;
                    p2 = myLines[i].P1;

                }
                else
                {
                    p1 = myLines[i].P1;
                    p2 = myLines[i].P2;
                }


                G00(p1.x, p1.y, zLevel);
                if(myLines[i].print)
                    G01(p2.x, p2.y, zLevel);
                else
                    G00(p2.x, p2.y, zLevel);
            }

        }

        public bool printAllLayers()
        {
            if (myObject == null)
                MessageBox.Show("No object to print");
            bool ParallelX = true;
            int x = 0;
            int cnt = 0;
            for (double k = myObject.minZ + 0.01; k < myObject.maxZ; k = k + zStep)//myObject.maxZ
            {
                // Print borders
                cnt++;
                //if (cnt < 3 || (k + 2 * zStep > myObject.maxZ))
                //   PrintLayerAtZLevel(k, true, x,0.402);
                //else if (cnt < 4 || (k + 3 * zStep > 3.437))
                //    PrintLayerAtZLevel(k, true, x, 0.42);
                //else
                    PrintLayerAtZLevel(k, true, x, 0.6);
               //if (ParallelX)
                
               // else
                  //  PrintLinesInLayer(k, 2, 70); 
                //ParallelX = !ParallelX;
                 x = x + 90;
            }

            return true;
        }

        public void G00(double x, double y, double z)
        {
            //  Gcodes.Add("N" + (Gcodes.Count + 1) + " G01 X" + x + " Y" + y + " Z" + z);
           
            ExtrusionLength = Math.Sqrt((x - CurrentX) * (x - CurrentX) + (y - CurrentY) * (y - CurrentY) + (z - CurrentZ) * (z - CurrentZ)) * ExtrusionRate;
            yf++;
            expectedTimeSec += ExtrusionLength * 0.1;
            if (ExtrusionLength != 0)
            {
                if (yf == 1)
                    Gcodes.Add("G00 X" + FixString(x) + " Y" + FixString(y) + " Z" + FixString(z) + " F" + FixString(FeedRate));
                else
                {
                    //if (z!= CurrentZ)
                    //    Gcodes.Add("G00 Z" + FixString(z));
                    //Gcodes.Add("G00 X" + FixString(x) + " Y" + FixString(y));
                    Gcodes.Add("G00 X" + FixString(x) + " Y" + FixString(y) + " Z" + FixString(z));
                }
                   
            }
            CurrentX = x;                                                                 
            CurrentY = y;
            CurrentZ = z;
        }


        string FixString(double d)
        {
            return d.ToString().Substring(0, Math.Min(5, d.ToString().Length));

        }
        int yf = 0;
        public double expectedTimeSec;
        public void G01(double x, double y, double z)
        {
            ExtrusionLength = Math.Sqrt((x - CurrentX) * (x - CurrentX) + (y - CurrentY) * (y - CurrentY) + (z - CurrentZ) * (z - CurrentZ)) * ExtrusionRate;
            yf++;
            expectedTimeSec += ExtrusionLength * 0.1;
            if (ExtrusionLength != 0)
            {
                if (yf == 1)
                    Gcodes.Add("G01 X" + FixString(x) + " Y" + FixString(y) + " Z" + FixString(z) + " F" + FixString(FeedRate) + " E" + FixString(ExtrusionLength));
                else
                    Gcodes.Add("G01 X" + FixString(x) + " Y" + FixString(y) + " Z" + FixString(z) + " E" + FixString(ExtrusionLength));
            }
            CurrentX = x;
            CurrentY = y;
            CurrentZ = z;

        }
        public bool PrintLayerAtZLevel(double zLevel, bool PrintBorders, double angle , double layerStep)
        {
            Layer myLayer;
            myObject.GetLayerIntersectionsAtZ(zLevel, out myLayer);
            
            if(PrintBorders)
                 PrintLayerBorders(zLevel, myLayer);
            PrintLinesInLayerWithAngle(zLevel, myLayer,layerStep, angle, PrintBorders);
            return true;
        }

        bool PrintLayerBorders(double zLevel,Layer myLayer)
        {
           
            // Find all Lines in the border in the right order
            List<Line> myOldOrderedBorderLines = new List<Line>();
            List<Line> myNewOrderedBorderLines = new List<Line>();
            List<Line> tempLines = myLayer.myLines;
    
            int loopCnt = 0;
            if (myLayer.myLines.Count == 0)
                return false;
            Line currentLine=tempLines[0];
            tempLines.RemoveAt(0);       
            
            currentLine.loop = 1;
            loopCnt = 1;
            currentLine.print = true;
            myOldOrderedBorderLines.Add(currentLine);

             bool found=false;
            while(tempLines.Count !=0){
                found=false;
                for (int j=0; j < tempLines.Count; j++)
                {
                    if (currentLine.Connected(myLayer.myLines[j]))
                    {
                        found = true;
                        tempLines[j].loop = currentLine.loop;
                        currentLine = tempLines[j];
                        currentLine.print = true;
                        myOldOrderedBorderLines.Add(currentLine);
                        tempLines.RemoveAt(j);
                        break;
                    }

                }
                if (!found)
                {
                    currentLine = tempLines[0];
                    currentLine.loop = ++loopCnt;
                    currentLine.print = true;
                    myOldOrderedBorderLines.Add(currentLine);
                    tempLines.RemoveAt(0);
                }
            }
            myLayer.addLines(myOldOrderedBorderLines);
            //PrintLines(zLevel, myOldOrderedBorderLines);

             for (int i = 0; i < myOldOrderedBorderLines.Count; i++)// Copy and shift borders
            { 
                Line editedLine = new Line();
                editedLine.print = true;
                editedLine.Normal = myOldOrderedBorderLines[i].Normal;
                editedLine.P1=myOldOrderedBorderLines[i].P1;
                editedLine.P2=myOldOrderedBorderLines[i].P2;
                editedLine.loop=myOldOrderedBorderLines[i].loop;
                editedLine.ShiftLine(layerStep * editedLine.Normal.i, layerStep * editedLine.Normal.j);
                myNewOrderedBorderLines.Add(editedLine);
            }


             Point2D p1;

           int loopBegin=0;
           for (int i = 0; i < myNewOrderedBorderLines.Count; i++)
           {
               int next;
               next = i + 1;
               if (i < myNewOrderedBorderLines.Count - 1)
               {
                   if (myNewOrderedBorderLines[i].loop == myNewOrderedBorderLines[i + 1].loop)
                   {
                       next = i + 1;
                   }
                   else
                   {

                       next = loopBegin;
                       loopBegin = i + 1;
                   }
               }
               else
               {
                   next = loopBegin;
               }

               if (myNewOrderedBorderLines[i].P1.GetDistanceTo(myNewOrderedBorderLines[next].P1) < 0.5 * NozzleDiameter)              
                   myNewOrderedBorderLines[i].P1 = myNewOrderedBorderLines[next].P1;
               else if (myNewOrderedBorderLines[i].P2.GetDistanceTo(myNewOrderedBorderLines[next].P1) < 0.5 * NozzleDiameter)
                   myNewOrderedBorderLines[i].P2 = myNewOrderedBorderLines[next].P1;
               else if (myNewOrderedBorderLines[i].P2.GetDistanceTo(myNewOrderedBorderLines[next].P2) < 0.5 * NozzleDiameter)
                   myNewOrderedBorderLines[i].P2 = myNewOrderedBorderLines[next].P2;
               else if (myNewOrderedBorderLines[i].P1.GetDistanceTo(myNewOrderedBorderLines[next].P2) < 0.5 * NozzleDiameter)
                   myNewOrderedBorderLines[i].P1 = myNewOrderedBorderLines[next].P2;
               else if (myNewOrderedBorderLines[i].GetIntersectionWithLine(myNewOrderedBorderLines[next].GetCenter(), (180 / Math.PI) * Math.Atan(myNewOrderedBorderLines[next].GetSlope()), out p1))
               {
                   if (myNewOrderedBorderLines[i].P1.GetDistanceTo(p1) < myNewOrderedBorderLines[i].P2.GetDistanceTo(p1))
                       myNewOrderedBorderLines[i].P1 = p1;
                   else
                       myNewOrderedBorderLines[i].P2 = p1;


                   if (myNewOrderedBorderLines[next].P1.GetDistanceTo(p1) < myNewOrderedBorderLines[next].P2.GetDistanceTo(p1))
                       myNewOrderedBorderLines[next].P1 = p1;
                   else
                       myNewOrderedBorderLines[next].P2 = p1;
               }
           
           }

           FixBorders(myNewOrderedBorderLines);
               PrintLines(zLevel, myNewOrderedBorderLines);


            return true;
        }
        public void FixBorders(List<Line> myLines)
        {
            if (myLines.Count == 0)
                return;
            Point2D p1, p2, temp;
            p1 = myLines[0].P1;
            p2 = myLines[0].P2;

            if (Math.Min(p2.GetDistanceTo(myLines[1].P1), p2.GetDistanceTo(myLines[1].P2)) > Math.Min(p1.GetDistanceTo(myLines[1].P1), p1.GetDistanceTo(myLines[1].P2)))
            {
                temp = p1;
                p1 = p2;
                p2 = temp;
            }
            
            for (int i = 1; i < myLines.Count; i++)
            {
                temp = p2;
                if (p2.GetDistanceTo(myLines[i].P1) > p2.GetDistanceTo(myLines[i].P2))
                {

                    p1 = myLines[i].P2;
                    p2 = myLines[i].P1;
                    if (p1.GetDistanceTo(temp) < 0.1)
                        myLines[i].P2 = temp;
                }
                else
                {
                    p1 = myLines[i].P1;
                    p2 = myLines[i].P2;
                    if (p1.GetDistanceTo(temp) < 0.1)
                        myLines[i].P1 = temp;
                }

             
                
            }

        }
        bool PrintLinesInLayerWithAngle(double zLevel, Layer myLayer, double layerStep, double angle = 0, bool PrintBorders = false)
        {
           
           
            //myLayer.setAngle(angle);
            List<Line> LinesToBePrinted = new List<Line>();
            if (myLayer.myLines.Count == 0)
                return false;
            List<Point2D> intersections = new List<Point2D>();
            double Diameter = Math.Sqrt((myLayer.maxX - myLayer.minX) * (myLayer.maxX - myLayer.minX) + (myLayer.maxY - myLayer.minY) * (myLayer.maxY - myLayer.minY));     
            int LineCounter = 0;
            Point2D start, end;
            start = end = new Point2D();
            start.x = myLayer.minX;
            start.y = myLayer.minY;
            end.x = myLayer.maxX;
            end.y = myLayer.maxY;
           // int x = 1;
            for (double R = -1 * myLayer.center.GetDistanceTo(start); R < myLayer.center.GetDistanceTo(end); R = R + layerStep)
            {
                LineCounter++;

                myObject.FindPointsIntersectingWithLinesAtLevel(myLayer, R, angle,  PrintBorders, layerStep, out intersections);
                if (intersections.Count>3)
                    R = R;

                intersections.Sort();
                //x = x * -1;

                if (LineCounter % 2 == 0)
                {
                    intersections.Reverse();
                   
                }
                    
                Line l = new Line();
                
                
                for (int i = 0; i < intersections.Count; i++)
                {
                     
                    if (i == 0)
                    {
                        if (false)
                        {
                            l.P1.x = intersections[i].x - layerStep * Math.Sign(intersections[i].Normal.i) * Math.Abs( Math.Cos(angle * Math.PI / 180));
                            l.P1.y = intersections[i].y - layerStep * Math.Sign(intersections[i].Normal.j) * Math.Abs( Math.Sin(angle * Math.PI / 180));
                        }
                        else
                        {
                            l.P1.x = intersections[i].x;
                            l.P1.y = intersections[i].y;
                       
                        }
                       
                    }
                    else
                    {
                        if (false)
                        {
                            l.P2.x = intersections[i].x - layerStep * Math.Sign(intersections[i].Normal.i) * Math.Abs(Math.Cos(angle * Math.PI / 180));
                            l.P2.y = intersections[i].y - layerStep * Math.Sign(intersections[i].Normal.j) * Math.Abs(Math.Sin(angle * Math.PI / 180));
                            if (i % 2 == 1)
                                l.print = true;
                            LinesToBePrinted.Add(l);
                            l = new Line();
                            l.P1.x = intersections[i].x - layerStep * Math.Sign(intersections[i].Normal.i) * Math.Abs(Math.Cos(angle * Math.PI / 180));
                            l.P1.y = intersections[i].y - layerStep * Math.Sign(intersections[i].Normal.j) * Math.Abs(Math.Sin(angle * Math.PI / 180));
                        }
                        else
                        {
                            l.P2.x = intersections[i].x;
                            l.P2.y = intersections[i].y;
                            if (i % 2 == 1)
                                l.print = true;
                            LinesToBePrinted.Add(l);
                            l = new Line();
                            l.P1.x = intersections[i].x;
                            l.P1.y = intersections[i].y;
                        }
                    }





                }
            }
            PrintLines(zLevel, LinesToBePrinted);

            return true;
        }


       
    }
}
