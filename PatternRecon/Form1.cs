using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;


namespace PatternRecon
{
    public partial class Form1 : Form
    {
   
        public List<Point> Cluster1=new List<Point>();
        public List<Point> Cluster2=new List<Point>();

        public List<Point> afterCluster1 = new List<Point>();
        public List<Point> afterCluster2 = new List<Point>();

        public Form1()
        {
            InitializeComponent();
            InitCombo();
        }
        public void InitCombo()
        {
            this.comboBox1.Items.Add("c均值");
            this.comboBox1.Items.Add("DBSCAN");
            //设置默认值
            this.textBox1.Text = "1";
            this.textBox2.Text = "12";
            //设置背景
            afterCluster1.Clear();
            afterCluster2.Clear();
            this.pictureBox1.Image = (DrawCluster(afterCluster1, afterCluster2, false));
            this.pictureBox2.Image = (DrawCluster(afterCluster1, afterCluster2, false));
        }

        //产生随机数种子
        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        //产生标准正太分布
        public static double[] NormalDistribution()
        {
            Random rand = new Random(GetRandomSeed());
            double[] y;
            double u1, u2, v1=0, v2=0, s = 0, z1=0, z2=0;
            while (s > 1 || s == 0)
            {
                u1 = rand.NextDouble();
                u2 = rand.NextDouble();
                v1 = 2 * u1 - 1;
                v2 = 2 * u2 - 1;
                s = v1 * v1 + v2 * v2;
            }
            z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
            z2 = Math.Sqrt(-2 * Math.Log(s) / s) * v2;
            y = new double[] { z1, z2 };
            return y; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        }
        //确定
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text == "c均值")
            {
                //调用c均值
                Caverage();
                this.pictureBox2.Image = (DrawCluster(afterCluster1, afterCluster2,false));
            }
            else if (this.comboBox1.Text == "DBSCAN")
            {
                //调用DBSCAN
                if (this.textBox1.Text == "" || this.textBox2.Text == "")
                {
                    MessageBox.Show("请输入DBSCAN值");
                }
                else
                {
                    DBSCAN();
                    this.pictureBox2.Image = (DrawCluster(afterCluster1, afterCluster2, false));
                }
            }
            else
            {
                MessageBox.Show("请选择聚类方法！");

            }

        }
        //绘图
        public Bitmap DrawCluster(List<Point> m1,List<Point> m2,bool flag)
        {
            Bitmap bmp = new Bitmap(405, 405);
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black, 2);
            Pen linep = new Pen(Color.FromArgb(30, Color.Black), 2);
          

            Pen pm1;
            Pen pm2;
            if (flag==true)
            {
               pm1 = new Pen(Color.Red, 2);
               pm2 = new Pen(Color.Green, 2);
            }
            else 
            {
                pm1 = new Pen(Color.Brown, 2);
                pm2 = new Pen(Color.Blue, 2);
            }
            Point p1 = new Point(400, 0);
            Point p2 = new Point(400, 400);
            Point p3 = new Point(0, 400);
            Point p4 = new Point(400, 400);
            try
            {
                //画线
                g.Clear(Color.White);
                g.DrawLine(p, p1, p2);
                g.DrawLine(p, p3, p4);
                g.DrawLine(p,new Point(0,0), p1);
                g.DrawLine(p, new Point(0, 0), p3);
                g.DrawString("高斯分布随机产生100个样品，特征值取值为0-10", new Font("宋体", 10), new SolidBrush(Color.Black), new PointF(10, 10));
                for (int i = 1; i <= 9; i++)
                {
                    g.DrawLine(linep, new Point(i * 40, 400), new Point(i * 40, 0));
                    g.DrawLine(linep, new Point(400, i * 40), new Point(0, i * 40));
                    g.DrawString(i.ToString(), new Font("宋体", 10), new SolidBrush(Color.Blue), new PointF(i * 40, 400 - 15));
                    g.DrawString(i.ToString(), new Font("宋体", 10), new SolidBrush(Color.Blue), new PointF(400 - 15, i * 40));
                }
                //画点
                foreach(Point m1Point in m1)
                {
                    g.DrawEllipse(pm1, m1Point.X, m1Point.Y, 2, 2);

                }
                foreach (Point m1Point in m2)
                {
                    g.DrawEllipse(pm2, m1Point.X, m1Point.Y, 2, 2);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                g.Dispose();
                p.Dispose();
                pm1.Dispose();
                pm2.Dispose();
                linep.Dispose();

            }
            return bmp;
            //描点

        }
       
        //用于产生两个均值
        public int count = 0;
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            count++;
           // MessageBox.Show(count.ToString());
            if (count==1)
            {
                for (int i = 0; i < 50;i++ )
                {
                    double[] point = NormalDistribution();
                    double x = point[0] +e.X/40;
                    double y = point[1] +e.Y/40;
                    int x1 = (int)(x * 40);
                    int x2 = (int)(y * 40);
                    Cluster1.Add(new Point(x1,x2));
                }

            }
            if (count == 2)
            {
                for (int i = 0; i < 50; i++)
                {
                    double[] point = NormalDistribution();
                    double x = point[0] + e.X / 40;
                    double y = point[1] + e.Y / 40;
                    int x1 = (int)(x * 40);
                    int x2 = (int)(y * 40);
                    Cluster2.Add(new Point(x1, x2));
                }
                this.pictureBox1.Image=(DrawCluster(Cluster1,Cluster2,true));
            }

        }
        //c均值，按照PPT上的步骤写的
        public void Caverage()
        {
            this.afterCluster1.Clear();
            this.afterCluster2.Clear();
           //初始化聚类中心
            List<Point> Cluster=new List<Point>();
            foreach(Point m1Point in Cluster1)
           {
            Cluster.Add(m1Point);
            }
            foreach(Point m2Point in Cluster2)
            {
            Cluster.Add(m2Point);
            }
            Point C1=Cluster[0];
            Point C2=Cluster[1];

            bool flag= true;
            //判断聚类中心是否相等
            while (flag)
            {
               // MessageBox.Show("聚类中心1:" + C1.X.ToString() + " " + C1.Y.ToString() + "聚类中心2:" + C2.X.ToString() + " " + C2.Y.ToString());
                int N1 = 0;
                int N2 = 0;
                int C1x = 0;
                int C2x = 0;
                int C1y = 0;
                int C2y = 0;
                foreach (Point point in Cluster)
                {
                    int s1 = Math.Abs(point.X - C1.X) + Math.Abs(point.Y - C1.Y);
                    int s2 = Math.Abs(point.X - C2.X) + Math.Abs(point.Y - C2.Y);
                    if(s1<s2)
                    {
                        N1++;
                        C1x += point.X;
                        C1y += point.Y;
                    }
                    else
                    {
                        N2++;
                        C2x += point.X;
                        C2y += point.Y;  
                    }
                }
                if(C1x/N1==C1.X&&C2.X==C2x/N2&& C1.Y==C1y/N1&& C2.Y==C2y/N2)
                {
                    flag = false;
                }
                C1.X=C1x/N1;
                C2.X=C2x/N2;
                C1.Y=C1y/N1;
                C2.Y=C2y/N2;
           }

            foreach (Point point in Cluster)
            {
                int s1 = Math.Abs(point.X - C1.X) + Math.Abs(point.Y - C1.Y);
                int s2 = Math.Abs(point.X - C2.X) + Math.Abs(point.Y - C2.Y);
                if (s1 < s2)
                {
                    afterCluster1.Add(point);
                }
                else
                {
                    afterCluster2.Add(point);
                }
            }
            

        }
        //DBSCAN
        public void DBSCAN()
        {
            try
            {
                double pow = 2.0;
                int radius = int.Parse(this.textBox1.Text) * 40;
                int MinPts = int.Parse(this.textBox2.Text);
                this.afterCluster1.Clear();
                this.afterCluster2.Clear();
                List<Point> Cluster = new List<Point>();
                List<Point> temp1 = new List<Point>();
                List<Point> temp2 = new List<Point>();

                foreach (Point m1Point in Cluster1)
                {
                    Cluster.Add(m1Point);
                }
                foreach (Point m2Point in Cluster2)
                {
                    Cluster.Add(m2Point);
                }
                Point C1;
                Point C2;
                bool isC1Get = false;
                bool isC2Get = false;
                foreach (Point mm in Cluster)
                {
                    if (isC1Get == false)
                    {
                        int count = 0;//记数
                        temp1.Clear();
                        foreach (Point mm1 in Cluster)
                        {
                            double banjing = Math.Sqrt(Math.Pow(mm1.X - mm.X, pow) + Math.Pow(mm1.Y - mm.Y, pow));
                            if (banjing < radius)
                            {
                                count++;
                                temp1.Add(mm1);
                            }
                        }
                        if (count >= MinPts)
                        {
                            C1 = mm;
                            isC1Get = true;
                            foreach (Point mm2 in temp1)
                            {
                                foreach (Point mm3 in Cluster)
                                {
                                    double banjing = Math.Sqrt(Math.Pow(mm3.X - mm2.X, pow) + Math.Pow(mm3.Y - mm2.Y, pow));
                                    if (banjing < radius && (afterCluster1.Contains(mm3) == false))
                                    {
                                        afterCluster1.Add(mm3);
                                    }
                                }

                            }
                        }
                    }
                    else if (isC2Get == false)
                    {
                        if (afterCluster1.Contains(mm) == false)
                        {
                            int count = 0;//记数
                            temp2.Clear();
                            foreach (Point mm1 in Cluster)
                            {
                                double banjing = Math.Sqrt(Math.Pow(mm1.X - mm.X, pow) + Math.Pow(mm1.Y - mm.Y, pow));
                                if (banjing < radius)
                                {
                                    count++;
                                    temp2.Add(mm1);
                                }
                            }
                            if (count >= MinPts)
                            {
                                C2 = mm;
                                isC2Get = true;
                                foreach (Point mm2 in temp2)
                                {
                                    foreach (Point mm3 in Cluster)
                                    {
                                        double banjing = Math.Sqrt(Math.Pow(mm3.X - mm2.X, pow) + Math.Pow(mm3.Y - mm2.Y, pow));
                                        if (banjing < radius && (afterCluster1.Contains(mm3) == false) && afterCluster2.Contains(mm3) == false)
                                        {
                                            afterCluster2.Add(mm3);
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        break;
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //清除
        private void button2_Click(object sender, EventArgs e)
        {
            this.Cluster1.Clear();
            this.Cluster2.Clear();
            this.afterCluster1.Clear();
            this.afterCluster2.Clear();
            count = 0;
            Bitmap bmp = new Bitmap(405, 405);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.Dispose();
            this.pictureBox1.Image = (DrawCluster(afterCluster1, afterCluster2, false));
            this.pictureBox2.Image = (DrawCluster(afterCluster1, afterCluster2, false));
        }
        //圈出不同,圈的之后注意比较一下中心
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)this.pictureBox2.Image;
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black,2);

            int count1 = 0;
            int count2 = 0;

            List<Point> belongCluster1;
            List<Point> belongCluster2;
            foreach (Point m1Point in Cluster1)
            {
                if (afterCluster1.Contains(m1Point))
                    count1++;
                if (afterCluster2.Contains(m1Point))
                    count2++;
            }

            if (count1 > count2)
            {
                belongCluster1 = afterCluster1;
                belongCluster2 = afterCluster2;

            }
            else
            {
                belongCluster1 = afterCluster2;
                belongCluster2 = afterCluster1;
            }
            //画出不同的地方
            foreach (Point m1Point in Cluster1)
            {
                if (belongCluster1.Contains(m1Point) == false)
                {
                    g.DrawEllipse(p, m1Point.X, m1Point.Y, 6, 6);
                }
            }
            foreach (Point m1Point in Cluster2)
            {
                if (belongCluster2.Contains(m1Point) == false)
                {
                    g.DrawRectangle(p, m1Point.X, m1Point.Y, 6, 6);
                }
            }
            
            g.Dispose();
            p.Dispose();
            this.pictureBox2.Image = bmp;

        }

    }
}
