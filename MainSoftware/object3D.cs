using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace MainSoftware
{
    public struct Vertex
    {
        public double x, y, z;
    }
    public struct Vector
    {
        public double i, j, k;

        public double GetLength()
        {
            return Math.Sqrt(i * i + j * j + k * k);
        }
    }

    public class Line
    {
        public Point2D P1, P2;
        public int loop = 0; // delete!
        public bool print = false;
        public double GetSlope()
        {
            if (!Double.IsInfinity((Math.Round((P2.y - P1.y) / (P2.x - P1.x), 5))))
                return (Math.Round((P2.y - P1.y) / (P2.x - P1.x), 5));
            else
                return (Math.Round((P2.y - P1.y) / (P2.x - P1.x), 5)) > 0 ? (Math.Round((P2.y - P1.y) / (P2.x - P1.x), 5)) : -1 * (Math.Round((P2.y - P1.y) / (P2.x - P1.x), 5));

        }

        public Point2D GetCenter()
        {

            Point2D d;
            d.Normal.i = 0;
            d.Normal.j = 0;
            d.Normal.k = 0;

            d.x=(P2.x + P1.x) / 2;
            d.y = (P2.y + P1.y) / 2;
            return d;
        }

        public double GetLength()
        {                        
              return Math.Sqrt((P2.y - P1.y) * (P2.y - P1.y) + (P2.x - P1.x) * (P2.x - P1.x));
        }

        public void ShiftLine(double I, double J)
        {
            P1.x += I;
            P2.x += I;

            P1.y += J;
            P2.y += J;
         
        }
        public void AddToLengthFromEachDirection(double addition)
        {
            double angle=Math.Abs(Math.Atan(GetSlope()));
            if (P1.x < P2.x){
                P1.x -= addition * Math.Cos(angle);
                P2.x += addition * Math.Cos(angle);
           }
            else
            {
                P1.x += addition * Math.Cos(angle);
                P2.x -= addition * Math.Cos(angle);
            }

            if (P1.y < P2.y)
            {
                P1.y -= addition * Math.Sin(angle);
                P2.y += addition * Math.Sin(angle);
            }
            else
            {
                P1.y += addition * Math.Sin(angle);
                P2.y -= addition * Math.Sin(angle);
            }
           

        }
        public bool GetIntersectionWithLine(Point2D pointOnLine, double angle,out Point2D IntersectionPoint)
        {
            
            IntersectionPoint.x=0;
            IntersectionPoint.y=0;
            IntersectionPoint.Normal = Normal;
             double a1, a2, b1, b2,I,J, xSol, ySol;
             if (angle % 180 == 0)
                angle = 0;
            else if (angle % 90 == 0)
                angle = 89.99999;

            a1 = Math.Tan(angle * Math.PI / 180);
            b1 = -1 * a1 * pointOnLine.x   + pointOnLine.y ; // Check!    
            I = P2.x - P1.x;
            J = P2.y - P1.y;  
            a2 = J / I;
            b2 = P2.y - a2 * P2.x;


            if (a1 != a2 && !(Double.IsInfinity(a2) && Double.IsInfinity(a1)))
            {
                if (Double.IsInfinity(a2))
                {
                    xSol = P1.x;
                    ySol = a1 * xSol + b1;
                }
                else
                {
                    xSol = (b2 - b1) / (a1 - a2);
                    ySol = a1 * xSol + b1;
                }

                IntersectionPoint.x = xSol;// Math.Round(xSol, 5);
                IntersectionPoint.y = ySol;// Math.Round(ySol, 5);
             
                return true;
            }
            
             return false;
        }

        public Vector Normal;
        public bool Equals(Line x)
        {
            if ((P1.Equals(x.P1) && P2.Equals(x.P2)) || (P1.Equals(x.P2) && P2.Equals(x.P1)))
                return true;
            return false;
        }

        public bool Connected(Line line)
        {
            return (P1.Equals(line.P1) || P2.Equals(line.P2) || P2.Equals(line.P1) || P1.Equals(line.P2));
        }
    }

    public struct Point2D : IComparable
    {
        public double x, y;
        public Vector Normal;
         int IComparable.CompareTo(object Item)
        {
            
            if (((Point2D)Item).x  < x )
                return -1;
            if (((Point2D)Item).x  > x)
                return 1;
            if (((Point2D)Item).y < y)
                return -1;
            if (((Point2D)Item).y > y)
                return 1;
            return 0;
        }

         public int Compare(object Item)
         {

             if (((Point2D)Item).x < x)
                 return -1;
             if (((Point2D)Item).x > x)
                 return 1;
             if (((Point2D)Item).y < y)
                 return -1;
             if (((Point2D)Item).y > y)
                 return 1;
             return 0;
         }
         public double GetDistanceTo(Point2D j)
         {
             return Math.Sqrt((x - j.x) * (x - j.x) + (y - j.y) * (y - j.y));
         }
    }

    public struct Point_Slope
    {
        public Point2D point;
        public double slope;
    }
    public class Facet
    {
        public Vertex V1, V2, V3; // These should be entered in the right order.
        public Vector Normal; // Should conform with the vector resultant from the 3 vertices and the RHR

        public Facet()
        {

        }
        public Facet(Vertex v1, Vertex v2, Vertex v3, Vector normal)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            Normal = normal;

        }
        public bool IsValidFacet()
        {
            Vector Q, R, N;
            Q.i = V1.x - V3.x;
            Q.j = V1.y - V3.y;
            Q.k = V1.z - V3.z;
            R.i = V2.x - V3.x;
            R.j = V2.y - V3.y;
            R.k = V2.z - V3.z;
            N.i = Q.i * R.j - R.i * Q.j;
            N.j = -1 * (Q.i * R.k - R.i * Q.k);
            N.k = (Q.j * R.k - R.j * Q.k);
            double mag = Math.Sqrt((N.i * N.i + N.j * N.j + N.k * N.k));

            bool c1 = N.i / mag == Normal.k;
            bool c2 = N.j / mag == Normal.j;
            bool c3 = N.k / mag == Normal.i;


            if (c1 && c2 && c3){
                return true;
            }
                
            else
            {
                Normal.i = N.k / mag;
                Normal.j = N.j / mag;
                Normal.k = N.i / mag;

            }
            return false;/////////////////////////////////////////////////////////Change!!!!!
        }

    }
    public class Layer
    {
        public List<Line> myLines;
        public Point2D center;
        public double angle, maxX,minX,maxY,minY;
        public Layer()
        {
            myLines = new List<Line>();
            center.x = 0;
            center.y = 0;
            angle = 0;
        }

        public void addLines(List<Line> L) // we assume the angle of the layer 0 when adding lines.
        {
            Line myLine;
            myLines = L;
            for (int i = 0; i < myLines.Count; i++)
            {

                myLine = myLines[i];
                if (i == 0)
                {
                    maxX = minX = myLine.P1.x;
                    maxY = minY = myLine.P1.y;
                }

                if (i == 0)
                {
                    maxX = minX = myLine.P1.x;
                    maxY = minY = myLine.P1.y;
                }

                if (myLine.P1.y < minY)
                    minY = myLine.P1.y;
                if (myLine.P1.x < minX)
                    minX = myLine.P1.x;
                if (myLine.P1.y > maxY)
                    maxY = myLine.P1.y;
                if (myLine.P1.x > maxX)
                    maxX = myLine.P1.x;

                if (myLine.P2.y < minY)
                    minY = myLine.P2.y;
                if (myLine.P2.x < minX)
                    minX = myLine.P2.x;
                if (myLine.P2.y > maxY)
                    maxY = myLine.P2.y;
                if (myLine.P2.x > maxX)
                    maxX = myLine.P2.x;
            }
            center.x = (maxX + minX) / 2;
            center.y = (maxY + minY) / 2;
            angle = 0;
        }
        public void setAngle(double a)
        {
            double sin, cos;
            Line myLine;
            double specificAngle;
            double distanceFromCenter=0;
            for (int i = 0; i < myLines.Count; i++)
            {
              
                myLine = myLines[i];
                if (i == 0)
                {
                    maxX = minX = myLine.P1.x;
                    maxY = minY = myLine.P1.y;
                }

                distanceFromCenter = Math.Sqrt((myLine.P1.x - center.x) * (myLine.P1.x - center.x) + (myLine.P1.y - center.y) * (myLine.P1.y - center.y));
                cos=(myLine.P1.x - center.x)/distanceFromCenter;
                sin = (myLine.P1.y - center.y) / distanceFromCenter;
                specificAngle = Math.Asin(Math.Abs(sin)) * 180 / Math.PI;
                if (sin >= 0 && cos <= 0)
                    specificAngle = 180 - specificAngle;
                else if (sin <= 0 && cos <= 0)
                    specificAngle = 180 + specificAngle;
                else if (sin <= 0 && cos >= 0)
                    specificAngle = 360 - specificAngle;

                myLine.P1.x = Math.Round( center.x + distanceFromCenter * Math.Cos((a - angle + specificAngle) * Math.PI / 180),5);
                myLine.P1.y = Math.Round(center.y + distanceFromCenter * Math.Sin((a - angle + specificAngle) * Math.PI / 180),5);
                if (myLine.P1.y < minY)
                    minY = myLine.P1.y;
                if (myLine.P1.x < minX)
                    minX = myLine.P1.x;
                if (myLine.P1.y > maxY)
                    maxY = myLine.P1.y;
                if (myLine.P1.x > maxX)
                    maxX = myLine.P1.x;

                distanceFromCenter = Math.Sqrt((myLine.P2.x - center.x) * (myLine.P2.x - center.x) + (myLine.P2.y - center.y) * (myLine.P2.y - center.y));
                cos = (myLine.P2.x - center.x) / distanceFromCenter;
                sin = (myLine.P2.y - center.y) / distanceFromCenter;
                
                specificAngle = Math.Asin(Math.Abs(sin)) * 180 / Math.PI;
                if (sin >= 0 && cos <= 0)
                    specificAngle = 180 - specificAngle;
                else if (sin <= 0 && cos <= 0)
                    specificAngle = 180 + specificAngle;
                else if (sin <= 0 && cos >= 0)
                    specificAngle = 360 - specificAngle;


                myLine.P2.x = Math.Round(center.x + distanceFromCenter * Math.Cos((a - angle + specificAngle) * Math.PI / 180),5);
                myLine.P2.y = Math.Round(center.y + distanceFromCenter * Math.Sin((a - angle + specificAngle) * Math.PI / 180),5);
                if (myLine.P2.y < minY)
                    minY = myLine.P2.y;
                if (myLine.P2.x < minX)
                    minX = myLine.P2.x;
                if (myLine.P2.y > maxY)
                    maxY = myLine.P2.y;
                if (myLine.P2.x > maxX)
                    maxX = myLine.P2.x;
            }

              angle = a;
        }


    }
    public class Object3D
    {
        public string name;
        public Facet[] MyFacets;
        public int CurrentNumberOfFacets = 0;
        public bool ValidObject = true;
        public double minX = 0, minY = 0, minZ = 0;
        public double maxX = 0, maxY = 0, maxZ = 0;
        public Object3D()
        {


        }
        public Object3D(int numberoffacets)
        {
            MyFacets = new Facet[numberoffacets];

        }
        void startObjectat0ZLevel()
        {
            for (int i = 0; i < CurrentNumberOfFacets; i++)
            {
                MyFacets[i].V1.z += -1 * minZ;
                MyFacets[i].V2.z += -1 * minZ;
                MyFacets[i].V3.z += -1 * minZ;
            }
          
            maxZ += -1 * minZ;
            minZ = 0;
        }
        // delete the extra parameters
        public void FindPointsIntersectingWithLinesAtLevel(Layer myLayer, double R, double angle,bool  PrintBorders,double layerStep, out List<Point2D> IntersecionPoints)//angle of lines not layer!
        {
            IntersecionPoints = new List<Point2D>();
            double xSol, ySol;
           
            Point2D p1 = new Point2D();
            Point2D p = new Point2D();
            // Find starting point and ending points
            p1.x = myLayer.center.x;
            p1.y = myLayer.center.y;
           
           
                for (int i = 0; i < myLayer.myLines.Count; i++)
                {

                    p1.x = myLayer.center.x + R * Math.Cos((angle + 90) * Math.PI / 180);
                    p1.y = myLayer.center.y + R * Math.Sin((angle + 90) * Math.PI / 180);
                    //myLayer.myLines[i].ShiftLine(-1 * myLayer.myLines[i].Normal.i * layerStep, -1 * myLayer.myLines[i].Normal.j * layerStep);
                    //myLayer.myLines[i].AddToLengthFromEachDirection(layerStep/2);
                    if (myLayer.myLines[i].GetIntersectionWithLine(p1, angle, out p))
                    {

                        xSol = p.x;
                        ySol = p.y;

                        double xDiff = -0.0005;// because of equality problems
                        double yDiff = -0.0005;

                        if ((myLayer.myLines[i].P1.x + xDiff <= xSol && myLayer.myLines[i].P2.x - xDiff >= xSol || myLayer.myLines[i].P2.x + xDiff <= xSol && myLayer.myLines[i].P1.x - xDiff >= xSol) && (myLayer.myLines[i].P1.y + yDiff <= ySol && myLayer.myLines[i].P2.y - yDiff >= ySol || myLayer.myLines[i].P2.y + yDiff <= ySol && myLayer.myLines[i].P1.y - yDiff >= ySol))
                        {

                            p.x = xSol;
                            p.y = ySol;
                           
                            IntersecionPoints.Add(p);
                        }

                    }
                    //myLayer.myLines[i].ShiftLine( myLayer.myLines[i].Normal.i * layerStep, myLayer.myLines[i].Normal.j * layerStep);
                    //myLayer.myLines[i].AddToLengthFromEachDirection(-1*layerStep/2);


                }
            
            if (IntersecionPoints.Count == 1)
            {

                IntersecionPoints.Clear();

                //MessageBox.Show("Error!! The lines intersecting a specific level is odd");
            }
        }

        public bool GetLayerIntersectionsAtZ(double zLevel, out Layer myLayer)
        {
            //zLevel = 10;
            myLayer = new Layer();
            Line myLine;
            Dictionary<Point_Slope, Line> intersectionPoints = new Dictionary<Point_Slope, Line>(); //To refuse having multiple lines with the same slope that are connected
             List<Line> planeLines = new List<Line>();
            bool b = false;
            bool exists = false;
            Point_Slope p;
            List<Line> planeLinesTemporary;
            
            for (int i = 0; i < CurrentNumberOfFacets; i++)
            {
                
                planeLinesTemporary = new List<Line>();
                if (GetFacetIntersectionsAtZ(zLevel, MyFacets[i], out planeLinesTemporary))
                {
                   
                    b = true;
                    for (int j = 0; j < planeLinesTemporary.Count; j++)
                    {
                        exists = false;
                        p.point.x = planeLinesTemporary[j].P1.x;
                        p.point.y = planeLinesTemporary[j].P1.y;
                        p.slope = Math.Round( planeLinesTemporary[j].GetSlope(),5);
                        p.point.Normal.i = 0;
                        p.point.Normal.j = 0;
                        p.point.Normal.k = 0;

                        if (intersectionPoints.ContainsKey(p))
                        {
                            exists = true;
                            if (!intersectionPoints[p].Equals(planeLinesTemporary[j]))
                            {
                               
                                myLine = intersectionPoints[p];


                                if (intersectionPoints[p].P1.x == p.point.x && intersectionPoints[p].P1.y == p.point.y)
                                {

                                    myLine.P1.x = planeLinesTemporary[j].P2.x;
                                    myLine.P1.y = planeLinesTemporary[j].P2.y;
                                }
                                else
                                {
                                    myLine.P2.x = planeLinesTemporary[j].P2.x;
                                    myLine.P2.y = planeLinesTemporary[j].P2.y;
                                }

                                continue;

                            }
                        }

                        if (!exists)
                            intersectionPoints.Add(p, planeLinesTemporary[j]);
                        p.point.x = planeLinesTemporary[j].P2.x;
                        p.point.y = planeLinesTemporary[j].P2.y;

                        if (intersectionPoints.ContainsKey(p))
                        {
                            exists = true;
                            myLine = intersectionPoints[p];
                            if (!intersectionPoints[p].Equals(planeLinesTemporary[j]))
                            {
                                
                                if (intersectionPoints[p].P1.x == p.point.x && intersectionPoints[p].P1.y == p.point.y)
                                {

                                    myLine.P1.x = planeLinesTemporary[j].P1.x;
                                    myLine.P1.y = planeLinesTemporary[j].P1.y;
                                }
                                else
                                {
                                    myLine.P2.x = planeLinesTemporary[j].P1.x;
                                    myLine.P2.y = planeLinesTemporary[j].P1.y;
                                }

                                continue;

                            }
                        }
                        if(!exists)
                            intersectionPoints.Add(p, planeLinesTemporary[j]);


                        myLine = planeLinesTemporary[j];
                        planeLines.Add(myLine);
                    }

                }




            }

            myLayer.addLines(planeLines);

            return b;
        }

       
        void fixLine(Line myLine)
        {
            myLine.P1.x = Math.Round(myLine.P1.x, 5);
            myLine.P2.x = Math.Round(myLine.P2.x, 5);
            myLine.P1.y = Math.Round(myLine.P1.y, 5);
            myLine.P2.y = Math.Round(myLine.P2.y, 5);
            
        }
        public bool GetFacetIntersectionsAtZ(double zLevel, Facet myFacet, out List<Line> planeLines)
        {
            Line myLine=new Line();//= new Line();
            planeLines = new List<Line>();
            myLine.P1.x = 0;
            myLine.P1.y = 0;
            myLine.P2 = myLine.P1;
            double t = 0;
            int found = 0;
            Vector myVector=new Vector();
            myLine.Normal = myFacet.Normal;
            // All Three lines in plane
            if (myFacet.Normal.j == 0 && myFacet.Normal.i == 0 && myFacet.V1.z == zLevel && myFacet.V2.z == zLevel && myFacet.V3.z == zLevel)
            {
                myLine.P1.x = myFacet.V1.x;
                myLine.P1.y = myFacet.V1.y;
                myLine.P2.x = myFacet.V2.x;
                myLine.P2.y = myFacet.V2.y;
               
                planeLines.Add(myLine);
                myLine = new Line();
                myLine.P1.x = myFacet.V1.x;
                myLine.P1.y = myFacet.V1.y;
                myLine.P2.x = myFacet.V3.x;
                myLine.P2.y = myFacet.V3.y;
                myLine.Normal = myFacet.Normal;
                planeLines.Add(myLine);
                myLine = new Line();
                myLine.P1.x = myFacet.V2.x;
                myLine.P1.y = myFacet.V2.y;
                myLine.P2.x = myFacet.V3.x;
                myLine.P2.y = myFacet.V3.y;
                myLine.Normal = myFacet.Normal;
                planeLines.Add(myLine);
                found = 4;
            }
            else
            {
               
                if ((myFacet.V1.z < zLevel && myFacet.V2.z > zLevel) || (myFacet.V2.z < zLevel && myFacet.V1.z > zLevel))
                {
                    myVector.i = myFacet.V2.x - myFacet.V1.x;
                    myVector.j = myFacet.V2.y - myFacet.V1.y;
                    myVector.k = myFacet.V2.z - myFacet.V1.z;
                    t = (zLevel - myFacet.V1.z) / myVector.k;//percentage of distance in respect to z at the level of intersection
                    myLine.P1.x = myFacet.V1.x + myVector.i * t;
                    myLine.P1.y = myFacet.V1.y + myVector.j * t;
                    found++;
                }

                if ((myFacet.V1.z < zLevel && myFacet.V3.z > zLevel) || (myFacet.V3.z < zLevel && myFacet.V1.z > zLevel))
                {
                    myVector.i = myFacet.V3.x - myFacet.V1.x;
                    myVector.j = myFacet.V3.y - myFacet.V1.y;
                    myVector.k = myFacet.V3.z - myFacet.V1.z;
                    t = (zLevel - myFacet.V1.z) / myVector.k;
                    if (found == 1)
                    {
                        myLine.P2.x = myFacet.V1.x + myVector.i * t;
                        myLine.P2.y = myFacet.V1.y + myVector.j * t;
                        
                        planeLines.Add(myLine);
                       
                    }
                    else
                    {
                        myLine.P1.x = myFacet.V1.x + myVector.i * t;
                        myLine.P1.y = myFacet.V1.y + myVector.j * t;
                        found++;
                    }

                    
                }
                if ((myFacet.V2.z < zLevel && myFacet.V3.z > zLevel) || (myFacet.V3.z < zLevel && myFacet.V2.z > zLevel))
                {
                    if (found == 1)
                    {
                        myVector.i = myFacet.V2.x - myFacet.V3.x;
                        myVector.j = myFacet.V2.y - myFacet.V3.y;
                        myVector.k = myFacet.V2.z - myFacet.V3.z;
                        t = (zLevel - myFacet.V3.z) / myVector.k;
                        myLine.P2.x = myFacet.V3.x + myVector.i * t;
                        myLine.P2.y = myFacet.V3.y + myVector.j * t;
                        found++;
                        planeLines.Add(myLine);
                    }
                }


            }
            int deleted = 0;
            for (int i = 0; i < planeLines.Count; i++)// Delete 0 length lines && round their values
            {
                myLine =  planeLines[i - deleted];
                fixLine(myLine);
                if (Math.Sqrt((myLine.P2.x - myLine.P1.x) * (myLine.P2.x - myLine.P1.x) + (myLine.P2.y - myLine.P1.y) * (myLine.P2.y - myLine.P1.y)) == 0)
                {
                    planeLines.RemoveAt(i - deleted++);
                }


            }


            if (found == 0)
                return false;
            return true;

        }


        public void LoadTextFiletoObject(string Extension, ToolStripProgressBar progress = null)
        {
            string str = System.IO.File.ReadAllText(Extension);
            Array strArray = str.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int cnt = 0;

            if ((string)strArray.GetValue(0) != "solid")
            {
                MessageBox.Show("Error while loading STL file \n This file seems not to be an ASCII STL file");
                return;
            }

            int NumOfFacets = str.Length / 180 + 100;
            int CurrentFacetNumber = 0;
            MyFacets = new Facet[NumOfFacets];
            string test;
            int L = strArray.Length;

            try
            {
                for (int i = 0; i < L; i++)
                {
                    cnt++;

                    if ((string)strArray.GetValue(i) == "facet")
                    {
                        CurrentNumberOfFacets++;
                        MyFacets[CurrentFacetNumber] = new Facet();
                        if ((string)strArray.GetValue(++i) == "normal")
                        {
                            MyFacets[CurrentFacetNumber].Normal.i = double.Parse((string)strArray.GetValue(++i));
                            test = str.Split()[i];
                            MyFacets[CurrentFacetNumber].Normal.j = double.Parse((string)strArray.GetValue(++i));
                            test = str.Split()[i];
                            MyFacets[CurrentFacetNumber].Normal.k = double.Parse((string)strArray.GetValue(++i));
                            test = str.Split()[i];
                        }

                        test = str.Split()[i + 1];
                        if ((string)strArray.GetValue(++i) == "outer")
                        {
                            if ((string)strArray.GetValue(++i) == "loop")
                            {
                                if ((string)strArray.GetValue(++i) == "vertex")
                                {
                                    MyFacets[CurrentFacetNumber].V1.x = double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V1.y = double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V1.z = double.Parse((string)strArray.GetValue(++i));

                                    if (cnt == 1)
                                    {
                                        minX = MyFacets[CurrentFacetNumber].V1.x;
                                        minY = MyFacets[CurrentFacetNumber].V1.y;
                                        minZ = MyFacets[CurrentFacetNumber].V1.z;

                                        maxX = MyFacets[CurrentFacetNumber].V1.x;
                                        maxY = MyFacets[CurrentFacetNumber].V1.y;
                                        maxZ = MyFacets[CurrentFacetNumber].V1.z;
                                    }

                                    if (MyFacets[CurrentFacetNumber].V1.x < minX)
                                        minX = MyFacets[CurrentFacetNumber].V1.x;
                                    if (MyFacets[CurrentFacetNumber].V1.y < minY)
                                        minY = MyFacets[CurrentFacetNumber].V1.y;
                                    if (MyFacets[CurrentFacetNumber].V1.z < minZ)
                                        minZ = MyFacets[CurrentFacetNumber].V1.z;

                                    if (MyFacets[CurrentFacetNumber].V1.x > maxX)
                                        maxX = MyFacets[CurrentFacetNumber].V1.x;
                                    if (MyFacets[CurrentFacetNumber].V1.y > maxY)
                                        maxY = MyFacets[CurrentFacetNumber].V1.y;
                                    if (MyFacets[CurrentFacetNumber].V1.z > maxZ)
                                        maxZ = MyFacets[CurrentFacetNumber].V1.z;
                                }
                                if ((string)strArray.GetValue(++i) == "vertex")
                                {
                                    MyFacets[CurrentFacetNumber].V2.x = double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V2.y = double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V2.z = double.Parse((string)strArray.GetValue(++i));

                                    if (MyFacets[CurrentFacetNumber].V2.x < minX)
                                        minX = MyFacets[CurrentFacetNumber].V2.x;
                                    if (MyFacets[CurrentFacetNumber].V2.y < minY)
                                        minY = MyFacets[CurrentFacetNumber].V2.y;
                                    if (MyFacets[CurrentFacetNumber].V2.z < minZ)
                                        minZ = MyFacets[CurrentFacetNumber].V2.z;

                                    if (MyFacets[CurrentFacetNumber].V2.x > maxX)
                                        maxX = MyFacets[CurrentFacetNumber].V2.x;
                                    if (MyFacets[CurrentFacetNumber].V2.y > maxY)
                                        maxY = MyFacets[CurrentFacetNumber].V2.y;
                                    if (MyFacets[CurrentFacetNumber].V2.z > maxZ)
                                        maxZ = MyFacets[CurrentFacetNumber].V2.z;
                                }
                                if ((string)strArray.GetValue(++i) == "vertex")
                                {
                                    MyFacets[CurrentFacetNumber].V3.x =  double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V3.y =  double.Parse((string)strArray.GetValue(++i));
                                    MyFacets[CurrentFacetNumber].V3.z =  double.Parse((string)strArray.GetValue(++i));

                                    if (MyFacets[CurrentFacetNumber].V3.x < minX)
                                        minX = MyFacets[CurrentFacetNumber].V3.x;
                                    if (MyFacets[CurrentFacetNumber].V3.y < minY)
                                        minY = MyFacets[CurrentFacetNumber].V3.y;
                                    if (MyFacets[CurrentFacetNumber].V3.z < minZ)
                                        minZ = MyFacets[CurrentFacetNumber].V3.z;

                                    if (MyFacets[CurrentFacetNumber].V3.x > maxX)
                                        maxX = MyFacets[CurrentFacetNumber].V3.x;
                                    if (MyFacets[CurrentFacetNumber].V3.y > maxY)
                                        maxY = MyFacets[CurrentFacetNumber].V3.y;
                                    if (MyFacets[CurrentFacetNumber].V3.z > maxZ)
                                        maxZ = MyFacets[CurrentFacetNumber].V3.z;

                                }
                                if ((string)strArray.GetValue(++i) == "endloop")
                                {
                                    if ((string)strArray.GetValue(++i) == "endfacet")
                                    {

                                        if (!MyFacets[CurrentFacetNumber++].IsValidFacet())
                                        {

                                            ValidObject = false;
                                           // MessageBox.Show("Facets normal vectors do not conform with vertices");
                                            //goto Error;
                                        }
                                    }
                                    else
                                    {
                                        goto Error;
                                    }
                                }
                                else
                                {
                                    goto Error;
                                }
                            }
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("Error while loading STL file");
            }
           // MessageBox.Show("Done");
            startObjectat0ZLevel();
            return;
        Error:
            {
                MessageBox.Show("Error while loading STL file");
            }



        }
        public bool AlmostEqual(double x, double y)
        {
            return (Math.Abs(x - y) < 0.00001);
        }

    }
}
