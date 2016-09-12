using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;




namespace MainSoftware
{


    public partial class Form1 : Form
    {

        //Units in milimeters
        // constants
     


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            printer.ExtrusionRate = Double.Parse(textBox3.Text);
        

        }

        

        Printer printer = new Printer();
     

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
        }

       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //label2.Text = trackBar1.Value.ToString();
           // List<Line> Lines = new List<Line>();
           // obj1.GetLayerIntersectionsAtZ(trackBar1.Value, out Lines);
            //pictureBox1.CreateGraphics().Clear(Color.White);
           // obj1.DrawLayer(Lines, pictureBox1.CreateGraphics(), new Point(200, 200), new Point(0, 0));

        }
    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        List<Object3D> myObjects =new List<Object3D>();
        private void loadSTLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "STL files (*.stl)|*.stl|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //try
                //{
                    Object3D obj = new Object3D();

                    obj.LoadTextFiletoObject(openFileDialog1.FileName, toolStripProgressBar1);
                    obj.name = openFileDialog1.FileName.Split('\\')[openFileDialog1.FileName.Split('\\').Count()-1];
                    myObjects.Add(obj);
                    UpdateObjects();

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                //}
            }
           
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Simulation.IsBusy == false)
            {
                Simulation.RunWorkerAsync();
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

                 SerialPort serial = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        int x = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            
            try
            {
            if(x==0)
                serial.DataReceived+= new SerialDataReceivedEventHandler(  DataReceviedHandler);
            x = 1;

            serial.PortName = "COM" + textBox4.Text;
            serial.RtsEnable = true;
            serial.BaudRate = 38400;
            serial.Open();
            label7.Show();
            textBox2.Enabled = true;
            button5.Enabled = true;
            Temperature_Request.Enabled = true;
            textBox4.Enabled = false;
         
           // radioButton2.Enabled=true;
           // radioButton3.Enabled = true;
           // Send("M03");
               
            }
            catch ( Exception ex)
            {
                // Extract some information from this exception, and then 
                // throw it to the parent method.
                if (ex.Source != null)
                    MessageBox.Show(ex.Message);
               // throw;
            }

            
        }
        List<string> oldInstructions = new List<string>();
        int currentOldInstruction = -1;
        void Send(string str)
        {
            oldInstructions.Add(str);
            currentOldInstruction = oldInstructions.Count-1;
            str = str.ToUpper();
            
               try
            {

                       serial.WriteLine(str);
                       if (textBox1.InvokeRequired == true)
                           textBox1.Invoke((MethodInvoker)delegate { textBox1.Text += "Me: " + str + Environment.NewLine; });

                else
                           textBox1.Text += "Me: " + str + Environment.NewLine;

               

            }
            catch
           {
                textBox1.Text += "Failed to send: " + textBox2.Text + Environment.NewLine;
            }
                
        }

        string newMsg = "";
        

       double Temp=0;
       bool IsPrinting=false;
        private void DataReceviedHandler(object sender,SerialDataReceivedEventArgs e)
        {

          
                newMsg = "";
                
                newMsg = serial.ReadExisting() ;
                newMsg= newMsg.Replace("\n", "\r\n");

            if (newMsg.IndexOf('T') !=-1)
            {
               
                try
                {
                    Temp = double.Parse(newMsg.Substring(newMsg.IndexOf('T') + 1).Split()[0]);
                }
                catch
                {

                }
               

            }
            //else
            {


                if (textBox1.InvokeRequired == true)
                    textBox1.Invoke((MethodInvoker)delegate { textBox1.Text += "Machine: " + newMsg; textBox1.Refresh(); });

                else
                    textBox1.Text += "Machine: " + newMsg;

               
            }


            if ((newMsg.IndexOf("ECHO") != -1 || newMsg.IndexOf("DONE") != -1) && IsPrinting)
            {

                if (GcodesCNT == printer.Gcodes.Count)
                {
                    IsPrinting = false;
                    GcodesCNT = 0;
                    Send("G00 z60");
                }
                Send(printer.Gcodes[GcodesCNT]);
                GcodesCNT++;

               
            }
            if (!IsPrinting && GcodesCNT !=0  )
            {

                    //Send("G00 z60");
                    
            }

            
           
        }
      
        private void button5_Click(object sender, EventArgs e)
        {

          

                Send(textBox2.Text);
               
            textBox2.Text = "";

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                

                Send(textBox2.Text);
                   
                textBox2.Text = "";
            }

            if (e.KeyCode == Keys.Up)
            {

                try
                {
                    textBox2.Text = oldInstructions[currentOldInstruction];
                    currentOldInstruction--;
                }
                catch
                {
                    textBox2.Text = "";
                    currentOldInstruction = 0;
                }
                 

               
            }

            if (e.KeyCode == Keys.Down)
            {
                try
                {
                    textBox2.Text = oldInstructions[currentOldInstruction];
                    currentOldInstruction++;
                }
                catch
                {
                    textBox2.Text = "";
                    currentOldInstruction = oldInstructions.Count - 1;

                }
                   

               
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
              // if (serial.IsOpen)
                   //serial.WriteLine("M105");
            }
            catch
            {

            }



        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            DrawGcodes(simulationPrinter, new Point(pictureBox2.Size.Width, pictureBox2.Size.Height)); 
        }


      
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
           
        }

     
        private void button7_Click(object sender, EventArgs e)
        {

        }

       
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Send("M00");// Relative
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            Send("G91");// Relative
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Send("G00 Y0.5");
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
          
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Send("G00 X-0.5");
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
             if (e.KeyCode == Keys.W)
            {
                Send("G00 Y100");
            }

             else if (e.KeyCode == Keys.S)
             {

                 Send("G00 Y-100");

             }
             else if (e.KeyCode == Keys.D)
             {
                 Send("G00 X100");
             }

             else if (e.KeyCode == Keys.A)
             {

                 Send("G00 X-100");

             }
             else if (e.KeyCode == Keys.PageDown)
             {
                 Send("G00 Z-100");
             }

             else if (e.KeyCode == Keys.PageUp)
             {

                 Send("G00 Z100");

             }
        }

        private void button11_Click(object sender, EventArgs e)
        {
         
        }

        private void button12_Click(object sender, EventArgs e)
        {
          
        }

        private void button13_Click(object sender, EventArgs e)
        {
           
        }

        private void button14_Click(object sender, EventArgs e)
        {
           
        }

        private void KeysUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.W || e.KeyCode == Keys.D || e.KeyCode == Keys.S || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
            {
                Send("M00");
                Send("M96");
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            Send("M00");
            Send("M96");
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 Y100");
        }

        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 X100");
        }

        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 Y-100");
        }

        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 X-100");
        }

        private void button11_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 Z100");
        }

      
        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 E100");
        }

        private void button12_MouseDown_1(object sender, MouseEventArgs e)
        {
            Send("G00 Z-100");
        }

        private void button14_MouseDown(object sender, MouseEventArgs e)
        {
            Send("G00 E-100");
        }

       

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void button15_Click(object sender, EventArgs e)
        {
          
        }
     
      

        int GcodesCNT = 0;

        private void button16_Click(object sender, EventArgs e)
        {
            Send("M00");
            Send("M96");
            IsPrinting = false;

        }

        private void button17_Click(object sender, EventArgs e)
        {
            Send("G92 X0 Y0 Z0");
        }

       
        private void button18_Click(object sender, EventArgs e)
        {
          
        }

        private void button19_Click(object sender, EventArgs e)
        {
            IsPrinting = false;
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            Send("G90");
            if (serial.IsOpen == false)
                MessageBox.Show("NOT CONNECTED!!");

        }

        private void tabPage4_Enter(object sender, EventArgs e)// Simulation
        {
            UpdateObjects();
              

        }
        void UpdateObjects()
        {
            listBox1.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            for (int i = 0; i < myObjects.Count; i++)
            {
                listBox1.Items.Add(myObjects[i].name);
                listBox4.Items.Add(myObjects[i].name);
                listBox5.Items.Add(myObjects[i].name);
            }

          
        }
        private void tabControl1_Enter(object sender, EventArgs e)//Slicing
        {
            UpdateObjects();
            label14.Text = "Z = " + trackBar3.Value;
        }
        Object3D currentObject;
        private void button22_Click(object sender, EventArgs e)
        {
          
            button3.Enabled = true;
            if (currentObject == null)
                currentObject = myObjects[0];

           
           
            //printer.setObject(obj1);
            //printer.PrintLinesInLayer_ParellelToY(10.0, 2);
            //MessageBox.Show(printer.Gcodes[0]);
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        Printer simulationPrinter = new Printer();
        private void button1_Click_1(object sender, EventArgs e)
        {
         
            try
            {
                if (currentObject == null)
                    currentObject = myObjects[0];

                simulationPrinter.setObject(currentObject);

                button2.Enabled = true;
            }
            catch
            {
                MessageBox.Show("No Object selected!");
            }



           
           
           
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {  
          
        }
         
        private void button3_Click(object sender, EventArgs e)
        {
            updateSlicingScreen();        

        }

        private void listBox4_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                currentObject = myObjects[listBox4.SelectedIndex];
            }
            catch
            {

            }
            
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                currentObject = myObjects[listBox1.SelectedIndex];
            }
            catch
            {

            }
         
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.CreateGraphics().Clear(Color.White);
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
           
                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            simulationPrinter.Gcodes.Clear();
            simulationPrinter.printAllLayers();
            for (int i = 0; i < simulationPrinter.Gcodes.Count; i++)
            {
                 listBox2.Items.Add( simulationPrinter.Gcodes[i]);
            }

          //  button21.Enabled = true;
            button20.Enabled = true;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Simulation.RunWorkerAsync();
            IsSimulating = true;
            Simulation_Update.Enabled = true;
            button20.Enabled = false;
            button21.Enabled = true;
            button19.Enabled = true;

        }

        private void button21_Click(object sender, EventArgs e)
        {
            button20.Enabled = true;
            button21.Enabled = false;
            pictureBox2.CreateGraphics().Clear(Color.White);
            
                IsSimulating = false;
                Simulation_Update.Enabled = false;
                simcnt = 0;
                simulationPrinter.Reset();
                newSim = true;
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
        public void DrawLayer(Layer myLayer, Object3D myObj, Point Limit)// Has to be moved to UI
        {
            pictureBox1.CreateGraphics().Clear(Color.White);
           
            int xScale, yScale, scale;
            double minX=myObj.minX;
            double minY = myObj.minY;
            double maxX = myObj.maxX;
            double maxY = myObj.maxY;

            Pen blackPen = new Pen(Color.Black, 3);
            Pen RedPen = new Pen(Color.Red, 3);
            Point origin = new Point(100, 10);
            xScale = (int)((Limit.X - 50) / (maxX - minX));
            yScale = (int)((Limit.Y - 50) / (maxY - minY));
            scale = Math.Min(xScale, yScale);

            for (int i = 0; i < myLayer.myLines.Count; i++)
            {
                try
                {
                    if (i % 2 == 0)
                    {
                        Point p1 = new Point((int)((myLayer.myLines[i].P1.x - minX) * scale + origin.X), (int)((myLayer.myLines[i].P1.y - minY) * scale + origin.Y));
                        Point p2 = new Point((int)((myLayer.myLines[i].P2.x - minX) * scale + origin.X), (int)((myLayer.myLines[i].P2.y - minY) * scale + origin.Y));
                        pictureBox1.CreateGraphics().DrawLine(blackPen, p1, p2);
                    }
                    else
                    {
                        Point p1 = new Point((int)((myLayer.myLines[i].P1.x - minX) * scale + origin.X), (int)((myLayer.myLines[i].P1.y - minY) * scale + origin.Y));
                        Point p2 = new Point((int)((myLayer.myLines[i].P2.x - minX) * scale + origin.X), (int)((myLayer.myLines[i].P2.y - minY) * scale + origin.Y));
                        pictureBox1.CreateGraphics().DrawLine(RedPen, p1, p2);
                    }
                }
                catch
                {

                }
               
            }

        }

        void updateSlicingScreen()
        {
            if (currentObject == null && myObjects.Count != 0)
                currentObject = myObjects[0];
            Layer myLayer = new Layer();
            double myzLevel = Math.Round((currentObject.minZ + (trackBar3.Value - 1) * (currentObject.maxZ - currentObject.minZ) / 100), 5);

            label14.Text = "Z = " + myzLevel;
            currentObject.GetLayerIntersectionsAtZ(myzLevel, out myLayer);
            myLayer.setAngle(0);
            DrawLayer(myLayer, currentObject,new Point(pictureBox1.Size.Width, pictureBox1.Size.Height));
            
           
        }
        int simcnt = 0;
        double simulationSpeed=280;
        string strL1, strL2;
        bool IsSimulating=false;
        bool newSim = true;
        Graphics graphicsSimulation;
        double gX = 0, gY = 0, oldX = 0, oldY = 0,cz=0;
        public void DrawGcodes(Printer myPrinter, Point Limit)
        {
            graphicsSimulation = pictureBox2.CreateGraphics();
            if (newSim)
            {
                oldX = 0;
                oldY = 0;
                cz = 0;
                newSim = false;
            }
            bool change = false;
            Pen blackPen = new Pen(Color.Black, 1);
            Pen RedPen = new Pen(Color.Red, 5);
            bool print = false;
            string str = "";
            double xScale, yScale, scale;

            Point origin = new Point(20, 20);
            xScale = (int)((Limit.X - 100) / (myPrinter.myObject.maxX - myPrinter.myObject.minX));
            yScale = (int)((Limit.Y - 100) / (myPrinter.myObject.maxY - myPrinter.myObject.minY));
            //scale = Math.Min(xScale, zyScale);
            scale = 8;
        
            Array g;
            for (; simcnt < myPrinter.Gcodes.Count; simcnt++)
            {
                change = false;

                g = myPrinter.Gcodes[simcnt].Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < g.Length; j++)
                {
                    print = false;
                    str = (string)g.GetValue(j);
                    if (str.Length == 0)
                        break;
                    if (str[0] == 'X')
                    {
                        gX = double.Parse(str.Substring(1));
                    }

                    if (str[0] == 'Y')
                    {
                        gY = double.Parse(str.Substring(1));
                    }

                    if (str[0] == 'Z')
                    {

                        if (cz != double.Parse(str.Substring(1)))
                        {
                            change = true;
                            cz = double.Parse(str.Substring(1));
                            graphicsSimulation.Clear(Color.White);
                        }


                    }

                    if (str[0] == 'E')
                    {
                        print = true;
                    }
                }

                if (!IsSimulating)
                      return;
                //if (simcnt != 0)
                {
                    if (print)
                        graphicsSimulation.DrawLine(RedPen, new Point((int)((gX - myPrinter.myObject.minX) * scale + origin.X), (int)((gY - myPrinter.myObject.minY) * scale + origin.Y)), new Point((int)((oldX - myPrinter.myObject.minX) * scale + origin.X), (int)((oldY - myPrinter.myObject.minY) * scale + origin.Y)));
                    else
                        graphicsSimulation.DrawLine(blackPen, new Point((int)((gX - myPrinter.myObject.minX) * scale + origin.X), (int)((gY - myPrinter.myObject.minY) * scale + origin.Y)), new Point((int)((oldX - myPrinter.myObject.minX) * scale + origin.X), (int)((oldY - myPrinter.myObject.minY) * scale + origin.Y)));
                }
               
                            System.Threading.Thread.Sleep((1201 - (int)(simulationSpeed*4)));
                            if (change)
                                strL1 = "Z = " + cz;
                            strL2 = "Current Gcode: " + myPrinter.Gcodes[simcnt];
                        

               

                oldX = gX;
                oldY = gY;

            }

        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {

            updateSlicingScreen();            
        }

        private void button26_Click(object sender, EventArgs e)
        {
            Send("G90");
            try
            {
                if (currentObject == null)
                    currentObject = myObjects[0];

                printer.setObject(currentObject);
                //Send("G92 Z" + currentObject.minZ.ToString());
               
            }
            catch
            {
                MessageBox.Show("No Object selected!");
            }

        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                currentObject = myObjects[listBox5.SelectedIndex];
            }
            catch
            {

            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            printer.printAllLayers();
           for (int i = 0; i < printer.Gcodes.Count; i++)
            {
                listBox3.Items.Add(printer.Gcodes[i]);
               
            }
           textBox7.Text = ( printer.expectedTimeSec / 60).ToString();
           pause = false;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            
           
          
                IsPrinting = true;
                Send("M77");
            
        }

        private void button23_Click(object sender, EventArgs e)
        {
            IsPrinting = false;
            GcodesCNT = 0;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                printer.ExtrusionRate = double.Parse(textBox3.Text);
            }
            catch
            {

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {





        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            try
            {
                serial.Close();
                label7.Hide();
                textBox4.Enabled = true;
            }
            catch
            {

            }
        }

        private void UpdateInterface_Tick(object sender, EventArgs e)
        {
         
                try
                {
                    if (GcodesCNT - 1 >= 0 && GcodesCNT - 1 < printer.Gcodes.Count())
                         toolStripStatusLabel3.Text = "Current Gcode #" + GcodesCNT.ToString() + ": " + printer.Gcodes[GcodesCNT - 1];
                    toolStripStatusLabel2.Text = "Temerature: " + Temp.ToString() + " C";

                }
                catch
                {

                }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            try
            {
                Object3D obj = new Object3D();

                obj.LoadTextFiletoObject(@"C:\Users\Qusai\Desktop\3D printing Project\space_invader_magnet.stl", toolStripProgressBar1);
                myObjects.Add(obj);
                obj.name = "Square";
                UpdateObjects();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            simulationSpeed = trackBar2.Value;

        }

        private void Simulation_Update_Tick(object sender, EventArgs e)
        {
            label15.Text = strL1;
            toolStripStatusLabel3.Text = strL2;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Simulation_Update.Enabled = false;
            Simulation.CancelAsync();
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            button20.Enabled = true;
            button21.Enabled = false;
            button19.Enabled = false;
          
            IsSimulating = false;
            Simulation_Update.Enabled = false;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            printer.G00(30, 30, currentObject.minZ);
            printer.G01(0, 30, currentObject.minZ);
            printer.G00(0, 0, currentObject.minZ);
            
        }
        bool pause = false;
        double lastZ;
        private void button28_Click(object sender, EventArgs e)
        {
            IsPrinting = true;
            Send("G00 z" + lastZ);

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
     }

   
 
 
}

