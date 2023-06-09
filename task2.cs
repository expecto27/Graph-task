﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Graph_tasks
{
    public partial class task2 : Form
    {
        private int n = -1;

        private bool shouldDrawGraph = false;

        private string dfs = "";
        
        private int[,] matrix;

        private Font vertexFont = new Font("Impact", 18);
        public task2()
        {

            this.Icon = new Icon("../../icon.ico");
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.n = Convert.ToInt32(textBox2.Text);
                if (n < 0 || n > 20) throw new FormatException();
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Неверное значение", "Введите количество вершин от 1 до 20", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }  
            this.ClientSize = new System.Drawing.Size(950, 473);
            this.close.Location = new System.Drawing.Point(925, 4);
            this.matrix = GetAdjacencyMatrix(n);
            Matrix M = new Matrix(matrix);
            
            M.Show();
            shouldDrawGraph = true;
            pictureBox1.Invalidate();
        }
        private int[,] GetAdjacencyMatrix(int n)
        {
            int[,] AdjacencyMatrix = new int[n,n];
            Random rnd = new Random();
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    double t = rnd.NextDouble();
                    int temp = t > 0.70 ? 1 : 0;
                    AdjacencyMatrix[i, j] = temp;
                }
            }
            return AdjacencyMatrix;

        }
        private void DrawArrow(Graphics g, Pen pen, PointF startPoint, PointF endPoint)
        {
            float arrowSize = 30; // размер стрелки 

            PointF[] arrowHead = new PointF[2];
            float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
            float sinAngle = (float)Math.Sin(angle);
            float cosAngle = (float)Math.Cos(angle);

            arrowHead[0] = endPoint;
            arrowHead[1] = new PointF(endPoint.X - arrowSize * cosAngle + arrowSize * 0.5f * sinAngle,
                                      endPoint.Y - arrowSize * sinAngle - arrowSize * 0.5f * cosAngle);

            PointF[] arrowHead1 = new PointF[2];
            arrowHead1[0] = endPoint;
            arrowHead1[1] = new PointF(endPoint.X - arrowSize * cosAngle - arrowSize * 0.5f * sinAngle,
                                    endPoint.Y - arrowSize * sinAngle + arrowSize * 0.5f * cosAngle);
            g.DrawLines(pen, arrowHead1);
            g.DrawLines(pen, arrowHead);
        }


        private bool IsGraphDirected(int[,] adjacencyMatrix)
        {
            int size = adjacencyMatrix.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (adjacencyMatrix[i, j] != adjacencyMatrix[j, i])
                    {
                        return true; // Если найдена различная пара значений, граф является ориентированным
                    }
                }
            }

            return false; // Если все пары значений совпадают, граф не является ориентированным
        }


        private void close_Click(object sender, EventArgs e)
        {
            this.Hide();
            main M = new main();
            M.ShowDialog();
            this.Close();
        }

        Point last;
        private void task1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last.X;
                this.Top += e.Y - last.Y;
            }
        }

        private void task1_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Point(e.X, e.Y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string val = textBox1.Text;
            int[] s = new int[val.Length];
            for(int i = val.Length - 1; i >= 0; i--)
            {
                try
                {
                    int t = Convert.ToInt32(val[i]) - 48;
                    s[i] = t - 1;
                    if (t > this.n || t < 0) throw new FormatException();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверное значение", "Проверьте введенные данные", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.dfs = "";
            PerformDFS(s[0]);

            if (val == this.dfs)
            {
                MessageBox.Show("Все верно", "Поздравляем", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Не верно", "Поздравляем", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DFS(int vertex, bool[] visited)
        {
            visited[vertex] = true;
            this.dfs += (vertex + 1);
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (matrix[vertex, i] != 0 && !visited[i])
                {
                    DFS(i, visited);
                }
            }
        }


        private void PerformDFS(int startVertex)
        {
            int vertexCount = matrix.GetLength(0);
            bool[] visited = new bool[vertexCount];

            // Найдите вершину, из которой есть путь в первую вершину (vertex 0)

            // Если такая вершина найдена, начните обход с неё
            if (startVertex != -1)
            {
                DFS(startVertex, visited);
            }

            // Продолжите обход для остальных непосещенных вершин
            for (int i = 0; i < vertexCount; i++)
            {
                if (!visited[i])
                {
                    DFS(i, visited);
                }
            }
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            if (shouldDrawGraph)
            {

                int[,] adjacencyMatrix = this.matrix;

                bool direct = IsGraphDirected(adjacencyMatrix);

                Graphics g = e.Graphics;

                Pen pen = new Pen(Color.White, 2);

                int pictureBoxWidth = pictureBox1.Width;
                int pictureBoxHeight = pictureBox1.Height;

                int vertexRadius = Math.Min(pictureBoxWidth, pictureBoxHeight) / 10;

                int vertexCount = adjacencyMatrix.GetLength(0);

                PointF[] vertexCenters = new PointF[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    float x = pictureBoxWidth / 2 + (pictureBoxWidth / 2 - vertexRadius) * (float)Math.Cos(2 * Math.PI * i / vertexCount);
                    float y = pictureBoxHeight / 2 + (pictureBoxHeight / 2 - vertexRadius) * (float)Math.Sin(2 * Math.PI * i / vertexCount);
                    vertexCenters[i] = new PointF(x, y);
                }
                // вершины графа
                Brush vertexBrush = Brushes.Green;
                for (int i = 0; i < vertexCenters.Length; i++)
                {
                    PointF center = vertexCenters[i];
                    float x = center.X - vertexRadius;
                    float y = center.Y - vertexRadius;
                    g.FillEllipse(vertexBrush, x, y, 2 * vertexRadius, 2 * vertexRadius);

                    // номер вершины внутри вершины
                    string vertexNumber = (i + 1).ToString();
                    SizeF numberSize = g.MeasureString(vertexNumber, vertexFont);
                    float numberX = center.X - numberSize.Width;
                    float numberY = center.Y - numberSize.Height;
                    g.DrawString(vertexNumber, vertexFont, Brushes.White, numberX, numberY);
                }

                // ребра графа
                Pen edgePen = new Pen(Color.White, 2);
                edgePen.EndCap = LineCap.ArrowAnchor;

                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        if (adjacencyMatrix[i, j] == 1 && i != j)
                        {

                            PointF startPoint = vertexCenters[i];
                            PointF endPoint = vertexCenters[j];
                            g.DrawLine(edgePen, startPoint, endPoint);

                            // стрелка на конце ребра
                            if (direct && adjacencyMatrix[i, j] != adjacencyMatrix[j, i]) DrawArrow(g, edgePen, startPoint, endPoint);
                        }
                    }
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process proc = Process.Start("notepad.exe");
        }
    }
}

