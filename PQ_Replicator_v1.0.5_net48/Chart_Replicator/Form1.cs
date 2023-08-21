using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.IO;

namespace Chart_Replicator
{
    public partial class Form1 : Form
    {
        class PQPoint
        {
            double x;
            double y;
            public PQPoint()
            {
                x = 0;
                y = 0;
            }
            public PQPoint(double x, double y)
            {
                this.y = y;
                this.x = x;
            }
            public double X
            {
                get { return x; }
                set { x = value; }
            }
            public double Y
            {
                get { return y; }
                set { y = value; }
            }
        }

        string imagePath = "";
        string savePath = "";
        List<Point> pointsSelected = new List<Point>();
        string unitP = "";
        string unitQ = "";
        public int n = 20;
        PQPoint[] PQCurve;
        int directionForMarquee;
        Point origin = new Point(0, 0);
        Point refPointX = new Point(0, 0);
        Point refPointY = new Point(0, 0);
        int radius = 5;
        public Form1()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            PQCurve = new PQPoint[n];
            for (int i = 0; i < n; i++)
            {
                PQCurve[i] = new PQPoint();
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
            directionForMarquee = 0;
            //textBox2.Text = "100";
            //textBox4.Text = "100";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is under development. \nPlease stay tuned!");
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is under development. \nPlease stay tuned!");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpeg; *.png; *.bmp)|*.jpeg; *.png; *.bmp";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName;

                if (File.Exists(imagePath))
                {
                    pictureBox1.Load(imagePath);
                }
                else
                {
                    MessageBox.Show("The specified file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pointsSelected.Count >= 2)
            {
                Graphics g = pictureBox1.CreateGraphics();
                Pen pen = new Pen(Color.Red, 2);
                Point[] points2 = new Point[pointsSelected.Count];
                for (int i = 0; i < pointsSelected.Count; i++)
                {
                    points2[i] = new Point(pointsSelected[i].X, pointsSelected[i].Y);
                }
                g.DrawCurve(pen, points2);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush whiteBrush = new SolidBrush(Color.White);

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            g.FillRectangle(blackBrush, 0, 0, width, height);
            Bitmap photoBitmap;
            if (!string.IsNullOrEmpty(imagePath))
            {
                photoBitmap = new Bitmap(imagePath);

                int imageWidth = photoBitmap.Width;
                int imageHeight = photoBitmap.Height;

                double ratio = Math.Min((double)pictureBox1.Height / imageHeight, (double)pictureBox1.Width / imageWidth);
                int x = (int)(pictureBox1.Width - imageWidth * ratio) / 2;
                int y = (int)(pictureBox1.Height - imageHeight * ratio) / 2;
                Rectangle formRect = new Rectangle(x, y, (int)(imageWidth * ratio), (int)(imageHeight * ratio));
                Rectangle imageRect = new Rectangle(0, 0, imageWidth, imageHeight);

                g.DrawImage(photoBitmap, formRect, imageRect, System.Drawing.GraphicsUnit.Pixel);
            }
            if (!String.IsNullOrWhiteSpace(textBox1.Text)) //reference point on x axis
            {
                g.DrawEllipse(Pens.Red, refPointX.X - radius, refPointX.Y - radius, radius * 2, radius * 2);
            }
            if (!String.IsNullOrWhiteSpace(textBox3.Text)) //reference point on y axis
            {
                g.DrawEllipse(Pens.Red, refPointY.X - radius, refPointY.Y - radius, radius * 2, radius * 2);
            }

            for (int i = 0; i < pointsSelected.Count; ++i)
            {
                g.DrawLine(Pens.Red, pointsSelected[i].X - 5, pointsSelected[i].Y - 5, pointsSelected[i].X + 5, pointsSelected[i].Y + 5);
                g.DrawLine(Pens.Red, pointsSelected[i].X + 5, pointsSelected[i].Y - 5, pointsSelected[i].X - 5, pointsSelected[i].Y + 5);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Point pt = (Point)listBox1.SelectedItem;
                pointsSelected.Remove(pt);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);


                if (progressBar2.Value >= 5 && progressBar2.Value <= 100)
                {
                    progressBar2.Value -= 5;
                }
                if (pointsSelected.Count < 20)
                {
                    listBox2.Items.Clear();
                }
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Graphics g1 = pictureBox1.CreateGraphics();

            if (String.IsNullOrWhiteSpace(textBox1.Text)) //reference point on x axis
            {
                textBox1.Text = e.X.ToString() + ", " + e.Y.ToString();
                origin.Y = e.Y;
                refPointX.X = e.X;
                refPointX.Y = e.Y;
                pictureBox1.Invalidate();
                //g1.DrawEllipse(Pens.Red, e.X - radius, e.Y - radius, radius * 2, radius * 2);
            }
            else if (String.IsNullOrWhiteSpace(textBox3.Text)) //reference point on y axis
            {
                textBox3.Text = e.X.ToString() + ", " + e.Y.ToString();
                origin.X = e.X; //The x value of y axis is y-coordinate of the origin.
                refPointY.X = e.X;
                refPointY.Y = e.Y;
                pictureBox1.Invalidate();
                //g1.DrawEllipse(Pens.Red, e.X - radius, e.Y - radius, radius * 2, radius * 2);
            }
            else if (pointsSelected.Count < n)
            {
                try
                {
                    double yRef = Convert.ToDouble(textBox2.Text); //maxP
                    double xRef = Convert.ToDouble(textBox4.Text);//maxQ
                    string x = e.X.ToString();
                    string y = e.Y.ToString();
                    string pointCurrent = x + ", " + y;
                    Point pt = new Point();
                    pt.X = e.X;
                    pt.Y = e.Y;

                    listBox1.Items.Add(pt);
                    pointsSelected.Add(pt);

                    pictureBox1.Invalidate();
                    //g1.DrawLine(Pens.Red, e.X - 5, e.Y - 5, e.X + 5, e.Y + 5);
                    //g1.DrawLine(Pens.Red, e.X + 5, e.Y - 5, e.X - 5, e.Y + 5);
                    if (progressBar2.Value >= 0 && progressBar2.Value <= 95)
                    {
                        progressBar2.Value += 5;
                    }
                    if (pointsSelected.Count == n)
                    {
                        //pointsSelected.Sort((p1, p2) => p1.X.CompareTo(p2.X));
                        if (textBox2.Text != null && textBox4.Text != null)
                        {
                            unitP = comboBox2.Text;
                            unitQ = comboBox1.Text;

                            int numPoints = pointsSelected.Count;
                            //double xScale = maxQ / (pointsSelected[numPoints - 1].X - pointsSelected[0].X);
                            //double yScale = maxP / (pointsSelected[0].Y - pointsSelected[numPoints - 1].Y);
                            listBox2.Items.Add("X\tY");
                            listBox2.Items.Add(unitQ + "\t" + unitP);
                            for (int i = 0; i < pointsSelected.Count; ++i)
                            {
                                //PQCurve[i].X = (double)((double)pointsSelected[i].X - (double)pointsSelected[numPoints - 1].X) / ((double)pointsSelected[0].X - (double)pointsSelected[numPoints - 1].X) * (0 - maxQ) + maxQ;
                                //PQCurve[i].Y = (double)((double)pointsSelected[i].Y - (double)pointsSelected[0].Y) / ((double)pointsSelected[numPoints - 1].Y - (double)pointsSelected[0].Y) * (0 - maxP) + maxP;
                                PQCurve[i].X = (double)((double)pointsSelected[i].X - (double)origin.X) / (double)((double)refPointX.X - (double)origin.X) * (double)(xRef);
                                PQCurve[i].Y = (double)((double)pointsSelected[i].Y - (double)origin.Y) / ((double)refPointY.Y - (double)origin.Y) * (double)(yRef);
                                listBox2.Items.Add(Math.Round(PQCurve[i].X, 2) + "\t " + Math.Round(PQCurve[i].Y, 2));
                            }
                        }
                    }
                }
                catch (FormatException formatE)
                {
                    //MessageBox.Show(formatE.Message);
                    MessageBox.Show("Error! Please input numbers in the reference X & reference Y blanks.");
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Only 20 points are allowed.");
                }
            }
            else
            {
                MessageBox.Show("Only 20 points are allowed.");
            }

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                object selectedValue = comboBox1.SelectedValue;
                unitQ = selectedValue.ToString();
            }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue != null)
            {
                object selectedValue = comboBox1.SelectedValue;
                unitP = selectedValue.ToString();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            textBox7.Text = e.X.ToString() + ", " + e.Y.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            progressBar2.Value = 0;
            pointsSelected.Clear();
            for (int i = 0; i < n; i++)
            {
                PQCurve[i].X = 0;
                PQCurve[i].Y = 0;
            }
            pictureBox1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int px = label1.Location.X;
            int py = label1.Location.Y;
            if (px + label1.Width >= listBox2.Location.X + listBox2.Width - 11)
            {
                directionForMarquee = 1;
            }
            else if (px <= panel1.Location.X + 11)
            {
                directionForMarquee = 0;
            }
            if (directionForMarquee == 1)
            {
                px -= 1;
            }
            else if (directionForMarquee == 0)
            {
                px += 1;
            }
            label1.Location = new Point(px, py);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog01 = new SaveFileDialog();
            saveFileDialog01.Filter = "Text documents (*.txt)|*.txt";

            if (saveFileDialog01.ShowDialog() == DialogResult.OK)
            {
                savePath = saveFileDialog01.FileName;

                using (StreamWriter writer = new StreamWriter(savePath))
                {
                    // Loop through the items in the ListBox
                    foreach (var item in listBox2.Items)
                    {
                        // Write the item to the file
                        writer.WriteLine(item.ToString());
                    }
                    writer.Close();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                listBox2.SetSelected(i, true);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (listBox2.Items.Count > 0 && listBox2.SelectedIndex != -1)
            {
                foreach (var item in listBox2.SelectedItems)
                {
                    sb.AppendLine(item.ToString());
                }
                Clipboard.SetText(sb.ToString());
            }
            else
            {
                MessageBox.Show("Please select the items you want to copy.");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2();
            fm2.Show();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            progressBar2.Value = 0;
            pointsSelected.Clear();
            for (int i = 0; i < n; i++)
            {
                PQCurve[i].X = 0;
                PQCurve[i].Y = 0;
            }
            //next step is clear all the crosses
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush whiteBrush = new SolidBrush(Color.White);

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            g.FillRectangle(blackBrush, 0, 0, width, height);
            Bitmap photoBitmap;
            if (!string.IsNullOrEmpty(imagePath))
            {
                photoBitmap = new Bitmap(imagePath);

                int imageWidth = photoBitmap.Width;
                int imageHeight = photoBitmap.Height;

                double ratio = Math.Min((double)pictureBox1.Height / imageHeight, (double)pictureBox1.Width / imageWidth);
                int x = (int)(pictureBox1.Width - imageWidth * ratio) / 2;
                int y = (int)(pictureBox1.Height - imageHeight * ratio) / 2;
                Rectangle formRect = new Rectangle(x, y, (int)(imageWidth * ratio), (int)(imageHeight * ratio));
                Rectangle imageRect = new Rectangle(0, 0, imageWidth, imageHeight);

                g.DrawImage(photoBitmap, formRect, imageRect, System.Drawing.GraphicsUnit.Pixel);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                listBox2.SetSelected(i, true);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (listBox2.Items.Count > 0)
            {
                foreach (var item in listBox2.SelectedItems)
                {
                    sb.AppendLine(item.ToString());
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox2.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox3.Text = "";
            pictureBox1.Invalidate();
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 fm3 = new Form3();
            fm3.Show();
        }
    }
}
