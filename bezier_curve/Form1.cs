using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonPaint_Click(object sender, EventArgs e)
        {
            Picture.Image = Bezier.DrawRandom(Picture.Width, Picture.Height);
        }
    }

    public static class Bezier
    {
        private struct Point
        {
            public int x, y;
        };

        private struct Line
        {
            public int x1, y1, x2, y2;
        };

        public static Bitmap DrawRandom(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Color colorMain = Color.Black;
            Color colorSecondary = Color.Red;
            // количество точек
            const int PointsMin = 3;
            const int PointsMax = 15;
            // получение списка точек
            Random r = new Random();
            int PointsCount = r.Next(PointsMin, PointsMax);
            Point[] Points = new Point[PointsCount];
            for (int i = 0; i < PointsCount; i++)
            {
                Points[i].x = r.Next(width);
                Points[i].y = r.Next(height);
            }
            // получение списка точек кривой
            float t = 0.0f;
            const float delta_t = 0.01f;
            const int iterations = (int)(1 / delta_t);
            List<Point> list = new List<Point>();
            list.Add(Points[0]);
            for (int i = 0; i < iterations; i++)
            {
                t += delta_t;
                list.Add(GetPoint(Points, PointsCount, t));
            }
            list.Add(Points[PointsCount - 1]);
            try
            {
                //отрисовка кривой
                for (int i = 0; i < list.Count - 1; i++)
                {
                    Brezenham.Line(list[i].x, list[i].y, list[i + 1].x, list[i + 1].y, ref bitmap);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            // отрисовка точек
            for (int i = 0; i < PointsCount; i++)
            {
                bitmap.SetPixel(Points[i].x, Points[i].y, colorSecondary);
            }

            return bitmap;
        }

        // t = [a, b]
        private static Point GetPoint(Point[] Points, int PointsCount, float t)
        {
            Point point = new Point();
            Line line = GetLine(Points, PointsCount, t);
            Point vector = GetLineVector(line, t);
            point.x = line.x1 + vector.x;
            point.y = line.y1 + vector.y;
            return point;
        }

        private static Line GetLine(Point[] Points, int PointsCount, float t)
        {
            if (PointsCount > 2)
            {
                Point[] points = new Point[PointsCount - 1];
                Line line = new Line();
                Point vector;
                for (int i = 0; i < PointsCount - 1; i++)
                {
                    line.x1 = Points[i].x; 
                    line.x2 = Points[i + 1].x;
                    line.y1 = Points[i].y; 
                    line.y2 = Points[i + 1].y;
                    vector = GetLineVector(line, t);
                    points[i].x = Points[i].x + vector.x;
                    points[i].y = Points[i].y + vector.y;
                }
                return GetLine(points, PointsCount - 1, t);
            }
            else if (PointsCount == 2)
            {
                Line line = new Line();
                line.x1 = Points[0].x;
                line.y1 = Points[0].y;
                line.x2 = Points[1].x;
                line.y2 = Points[1].y;
                return line;
            }
            else return new Line();
        }

        private static Point GetLineVector(Line line, float t)
        {
            Point length = new Point();
            length.x = (int)((line.x2 - line.x1) * t);
            length.y = (int)((line.y2 - line.y1) * t);
            return length;
        }
    }

    public static class Brezenham
    {
        public static void Line(int x1, int y1, int x2, int y2, ref Bitmap bitmap)
        {
            // проверяем рост отрезка по оси x и по оси y
            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            // отражаем линию по диагонали, если угол наклона слишком большой
            if (steep)
            {
                Swap(ref x1, ref y1);
                Swap(ref x2, ref y2);
            }
            // если линия растёт не слева направо, то меняем начало и конец отрезка местами
            if (x1 > x2)
            {
                Swap(ref x1, ref x2);
                Swap(ref y1, ref y2);
            }
            int dx = x2 - x1;
            int dy = Math.Abs(y2 - y1);
            int error = dx / 2;
            int ystep = (y1 < y2) ? 1 : -1;
            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                bitmap.SetPixel(steep ? y : x, steep ? x : y, Color.Black);
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        private static void Swap(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }
    }
}
