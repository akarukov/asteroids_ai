using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ai_tests
{
    public partial class MainForm : Form
    {
        int scale = 25;
        double maxThrust = 0.5;
        double maxTurn = Math.PI / 16.0;
        int divThrust = 1;
        int divTurn = 2;
        int calcTurn = 1;
        double coordEpsilon = 0.001;
        double speedEpsilon = 0.001;
        double angleEpsilon = 0.001;
        double xlookfrom = 0;
        double ylookfrom = 0;
        
        private Bitmap _backBuffer=null;
        
        List<ShipState> all_combinations=new List<ShipState>();
        List<ShipState> foundStates = new List<ShipState>();
        public MainForm()
        {
            InitializeComponent();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_backBuffer != null)
            {
                Graphics gr = e.Graphics;
                gr.DrawImageUnscaled(_backBuffer, 0, 0);
                
                gr.ResetTransform();
                gr.TranslateTransform(100, panel1.ClientSize.Height / 2);
                gr.FillRectangle(Brushes.Red,
                    (int)(xlookfrom * scale), (int)(ylookfrom * scale),
                    1, 1);
                foreach (ShipState ss in foundStates)
                {
                    gr.FillRectangle(Brushes.Cyan, (int)(ss.xpos * scale), (int)(ss.ypos * scale), 1, 1);
                }
            }
            else
            {
                prepareGraphics(e.Graphics);
            }
        }        
        private void prepareGraphics(Graphics gr)
        {
            gr.Clear(Color.Black);
            gr.ResetTransform();
            gr.TranslateTransform(100, panel1.ClientSize.Height/2);
            Pen pen = new Pen(Color.FromArgb(40, 40, 40));
            for (int x = -10; x < 30; x++)
                for (int y = -10; y < 10; y++)
                {
                    gr.DrawRectangle(pen, x * scale, y * scale, scale, scale);
                }
        }
        private void cleanLists()
        {
            foundStates.Clear();
            foreach (ShipState ss in all_combinations)
            {
                ss.putToPool();
            }
            all_combinations.Clear();
        }
        private void drawBackBuffer()
        {
            if (_backBuffer != null &&
                (_backBuffer.Width != panel1.ClientSize.Width ||
                 _backBuffer.Height != panel1.ClientSize.Height)
                )
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
            if (_backBuffer == null)
            {
                _backBuffer = new Bitmap(panel1.ClientSize.Width, panel1.ClientSize.Height);
            }

            Graphics gr = Graphics.FromImage(_backBuffer);
            prepareGraphics(gr);
            cleanLists();
            DateTime startTime = DateTime.Now;
            CalcNextShipState(0, ShipState.getFromPool());
            TimeSpan deltaTime = DateTime.Now.Subtract(startTime);
            int comb_count = all_combinations.Count;

            //if(true)
            //{
            //    ShipState ssNext = ShipState.getFromPool();
            //    foreach (ShipState ss in all_combinations)
            //    {
            //        ssNext.set(ss);
            //        ssNext.move(0,0);
            //        gr.DrawLine(Pens.Green,
            //            (int)(ss.xpos * 50), (int)(ss.ypos * 50),
            //            (int)(ssNext.xpos * 50), (int)(ssNext.ypos * 50));

            //    }
            //    ssNext.putToPool();
            //}
            foreach (ShipState ss in all_combinations)
            {
                gr.FillRectangle(Brushes.White, (int)(ss.xpos * scale), (int)(ss.ypos * scale), 1, 1);
            }
          
            gr.Dispose();
            
            label7.Text = Convert.ToString(comb_count);
            label14.Text = "?";
            label9.Text = Convert.ToString((int)deltaTime.TotalMilliseconds);
        }

        private void CalcNextShipState(int turnNumber, ShipState ss)
        {
            if (turnNumber == calcTurn)
            {
                ShipState putss = ShipState.getFromPool();
                putss.set(ss);
                all_combinations.Add(putss);
                return;
            }
            ShipState newss = ShipState.getFromPool();
            for (int thr = 0; thr <= divThrust; thr++)
                for (int tur = 0; tur <= divTurn; tur++)
                {
                    newss.set(ss);
                    newss.move(thr * maxThrust / divThrust, tur * (2*maxTurn) / divTurn - maxTurn);
                    CalcNextShipState(turnNumber + 1, newss);
                }
            newss.putToPool();
        }
        private void ParseParams()
        {
            try
            {
                maxThrust = Double.Parse(textBox1.Text);
                maxTurn = Math.PI / Double.Parse(textBox2.Text);
                divThrust = Int32.Parse(textBox3.Text);
                divTurn = Int32.Parse(textBox4.Text);
                calcTurn = Int32.Parse(textBox5.Text);
                coordEpsilon = Double.Parse(textBox6.Text);
                speedEpsilon = Double.Parse(textBox7.Text);
                angleEpsilon = Math.PI / Double.Parse(textBox8.Text);
            }
            catch(Exception )
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ParseParams();            
            drawBackBuffer();
            panel1.Invalidate();            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int uniqueCombinations = 0;
            int comb_count = all_combinations.Count;
            for (int i = 0; i < comb_count; i++)
            {
                bool isunique = true;
                for (int j = i + 1; j < comb_count; j++)
                {
                    ShipState ss1 = all_combinations[i];
                    ShipState ss2 = all_combinations[j];
                    if (
                        Math.Abs(ss1.xpos - ss2.xpos) < coordEpsilon &&
                        Math.Abs(ss1.ypos - ss2.ypos) < coordEpsilon &&
                        Math.Abs(ss1.xspeed - ss2.xspeed) < speedEpsilon &&
                        Math.Abs(ss1.yspeed - ss2.yspeed) < speedEpsilon &&
                        Math.Abs(ss1.angle - ss2.angle) < angleEpsilon
                        )
                    {
                        isunique = false;
                        break;
                    }
                }
                if (isunique) uniqueCombinations++;
            }
            label14.Text = Convert.ToString(uniqueCombinations);
        }
        private void setLookFrom(Point p)
        {
            xlookfrom = (p.X-100.0) / scale;
            ylookfrom = (p.Y - panel1.ClientSize.Height / 2.0) / scale;
            label15.Text = String.Format("Look from {0:0.00} {0:0.00}", xlookfrom, ylookfrom);
            panel1.Invalidate();
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) setLookFrom(e.Location);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) setLookFrom(e.Location);
        }
        
       
        private void button3_Click(object sender, EventArgs e)
        {
            foundStates.Clear();
            
            foreach (ShipState ss in all_combinations)
            {
                  if (
                        Math.Abs(ss.xpos - xlookfrom) < coordEpsilon &&
                        Math.Abs(ss.ypos - ylookfrom) < coordEpsilon 
                      )
                {
                    foundStates.Add(ss);          
                }
            }
            panel1.Invalidate();
        }

      

       
    }
}
