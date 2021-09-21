using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using System.Threading;
using System.IO;

namespace LPT_Test_Form
{
    public partial class Form1 : Form
    {
        LPT lpt;
        //Thread thread;
        INIManager ini;
        int Width, Height;
        void setStateD(ushort data)
        {
            byte b = 128;
            for (int i = 7; i >= 0; i--) {

                Panel pn = (groupBox1.Controls["stateD"+i.ToString()] as Panel);
                if((data & b) == b)
                    pn.BackColor = Color.Green;
                else
                    pn.BackColor = Color.LightCoral;

                b /= 2;
            }
        }
        void setStateC(ushort control)
        {
            byte b = 8;
            for (int i = 3; i >= 0; i--)
            {

                Panel pn = (groupBox1.Controls["stateC" + i.ToString()] as Panel);
                if ((control & b) == b)
                    pn.BackColor = Color.Green;
                else
                    pn.BackColor = Color.LightCoral;

                b /= 2;
            }
        }
        //void ThreadMain()
        //{
        //    while(true)
        //    {
        //        groupBox1.Invoke(new MethodInvoker(delegate
        //        {
        //            Draw();
        //        }));

        //        Thread.Sleep(200);
        //    }
        //}
        public void Draw()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bitmap);

            int w = 30, h = 30;
            int num = 13;
            SolidBrush sb = new SolidBrush(Color.LightCoral);

            byte status = lpt.Status,
                 control = lpt.Control,
                 data = lpt.Data;

            setStateD(data);
            setStateC(control);

            byte b = 128, b3=0;
            for (int i = 0; i < 13; i++)
            {
                if (num >= 10)
                {
                    if (num == 12)
                    {
                        if ((status & 0x20) == 0x20)
                            sb = new SolidBrush(Color.Blue);
                        else
                            sb = new SolidBrush(Color.LightBlue);
                    }
                    if(num == 13)
                    {
                        if ((status & 0x10) == 0x10)
                            sb = new SolidBrush(Color.Blue);
                        else
                            sb = new SolidBrush(Color.LightBlue);
                    }
                    if (num == 10)
                    {
                        if ((status & 0x40) == 0x40)
                            sb = new SolidBrush(Color.Blue);
                        else
                            sb = new SolidBrush(Color.LightBlue);
                    }
                    if (num == 11)
                    {
                        if ((status & 0x80) == 0x80)
                            sb = new SolidBrush(Color.Blue);
                        else
                            sb = new SolidBrush(Color.LightBlue);
                    }
                }
                else
                if (num >= 2)
                {
                    if ((data & b) == b)
                        sb = new SolidBrush(Color.Yellow);
                    else
                        sb = new SolidBrush(Color.LightYellow);
                    b /= 2;
                }
                else
                if (num == 1)
                {
                    if((control & 0x1)==0x1)
                        sb = new SolidBrush(Color.Red);
                    else
                        sb = new SolidBrush(Color.LightCoral);
                }

               g.FillEllipse(sb, 25 + i * (w + 15), 75, w, h);
               g.DrawString((num--).ToString(), new Font("Arial", 8), new SolidBrush(Color.Black), 33 + i * (w + 15), 83, new StringFormat());
            }

            num = 25;
            b3 = 8;
            for (int i = 0; i < 12; i++)
            {
                if (num >= 18)
                    sb = new SolidBrush(Color.Gray);
                else
                if (num == 15)
                {
                    if ((status & 0x8) == 0x8)
                        sb = new SolidBrush(Color.Blue);
                    else
                        sb = new SolidBrush(Color.LightBlue);
                }
                else
                if (num >= 16)
                {
                    if ((control & b3) == b3)
                        sb = new SolidBrush(Color.Red);
                    else
                        sb = new SolidBrush(Color.LightCoral);

                    b3 /= 2;
                }
                else
                if(num == 14){

                    if ((control & 0X2) == 0X2)
                        sb = new SolidBrush(Color.Red);
                    else
                        sb = new SolidBrush(Color.LightCoral);
                }

                g.FillEllipse(sb, 50 + i * (w + 15), 148, w, h);
                g.DrawString((num--).ToString(), new Font("Arial", 8), new SolidBrush(Color.Black), 55 + i * (w + 15), 155, new StringFormat());
            }

            pictureBox1.Invoke(new MethodInvoker(delegate
            {
                pictureBox1.BackgroundImage = bitmap;
            }));
        }
        public Form1()
        {
            InitializeComponent();
            
            Width  = pictureBox1.Width;
            Height = pictureBox1.Height;

            string pathIni = Directory.GetCurrentDirectory() + "\\Confiruration.ini";
            ini = new INIManager(pathIni);
            ushort BaseAddr = Convert.ToUInt16(ini.GetPrivateString("LPT_Port", "BaseAddr"), 16);

            lpt = new LPT(BaseAddr);
            lpt.Control = 0;

            for (int i = 0; i < 8; i++)
                (groupBox1.Controls["D" + i.ToString()] as Button).Click += btn_Click;

            for (int i = 0; i < 4; i++)
                (groupBox1.Controls["C" + i.ToString()] as Button).Click += btn_Click_C;

            //thread = new Thread(ThreadMain);
            //thread.Start();
        }
        private void btn_Click(object sender, EventArgs e)
        {
            lpt.Data ^= Convert.ToByte(((Button)sender).Tag);
        }
        private void btn_Click_C(object sender, EventArgs e)
        {
            lpt.Control ^= Convert.ToByte(((Button)sender).Tag);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //thread.Abort();
        }

        private void btnOpenAll_Click(object sender, EventArgs e)
        {
            lpt.Data = 0xff;
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            lpt.Data = 0;
        }

         private void timer1_Tick(object sender, EventArgs e)
        {
            Draw();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            lpt.Control = 0xf;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            lpt.Control = 0;
        }

    }
    class LPT
    {
        [DllImport("inpout32.dll")]
        private static extern void Out32(ushort PortAddress, ushort Data);
        [DllImport("inpout32.dll")]
        private static extern ushort Inp32(ushort PortAddress);

        ushort BaseAddr;

        public LPT(ushort addr)
        {
            BaseAddr = addr;
        }

        //2-9
        public byte Data
        {
            get 
            {
                return Convert.ToByte(Inp32(Convert.ToUInt16(BaseAddr)));
            }

            set 
            {
                Out32(Convert.ToUInt16(BaseAddr), Convert.ToUInt16(value));
            }
        }

        //10-13,15
        public byte Status
        {
            get
            {
                return Convert.ToByte(Inp32(Convert.ToUInt16(BaseAddr + 1)));
            }

        }

        //1, 14, 16-17
        public byte Control
        {
            get
            {
                return Convert.ToByte(Inp32(Convert.ToUInt16(BaseAddr + 2)));
            }
            set
            {
                Out32(Convert.ToUInt16(BaseAddr + 2), Convert.ToUInt16(value));
            }
        }

    }
}
