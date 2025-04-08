using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Проект_3_4_Кривая_Безье
{
    public partial class Form1 : Form
    {
        // Контрольные точки для левой кривой
        private PointF[] controlPointsLeft = new PointF[]
        {
            new PointF(50, 350),  // P0
            new PointF(150, 50),  // P1
            new PointF(250, 50),  // P2
            new PointF(350, 350)  // P3
        };

        // Контрольные точки для правой кривой
        private PointF[] controlPointsRight = new PointF[]
        {
            new PointF(400, 350),  // P0
            new PointF(500, 50),   // P1
            new PointF(600, 50),   // P2
            new PointF(700, 350)   // P3
        };

        private Button drawButton;
        private Button animateButton;
        private Label timeLabel;

        private PointF[] algebraicCurveLeft;
        private PointF[] geometricCurveRight;
        private float tValue = 0.0f;

        public Form1()
        {
            this.Text = "Bezier Curve Visualization";
            this.Size = new Size(1200, 800); // Увеличение размера окна

            // Создаем кнопку для рисования кривых
            drawButton = new Button
            {
                Text = "Нарисовать кривые",
                Location = new Point(10, 750),
                Width = 150,
                Height = 30
            };
            drawButton.Click += DrawButton_Click;
            this.Controls.Add(drawButton);

            // Создаем кнопку для анимации
            animateButton = new Button
            {
                Text = "Анимация кривой",
                Location = new Point(200, 750),
                Width = 150,
                Height = 30
            };
            animateButton.Click += AnimateButton_Click;
            this.Controls.Add(animateButton);

            // Создаем метку для отображения времени выполнения
            timeLabel = new Label
            {
                Location = new Point(10, 600),
                AutoSize = true,
                ForeColor = Color.Blue,
                Text = "Время выполнения: ..."
            };
            this.Controls.Add(timeLabel);
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            // Алгебраический метод для левой кривой
            Stopwatch algebraicTimer = Stopwatch.StartNew();
            algebraicCurveLeft = BezierAlgebraic(controlPointsLeft, 1000);
            algebraicTimer.Stop();

            // Геометрический метод для правой кривой
            Stopwatch geometricTimer = Stopwatch.StartNew();
            geometricCurveRight = BezierGeometric(controlPointsRight, 1000);
            geometricTimer.Stop();

            // Обновляем метку с временем выполнения
            timeLabel.Text = $"Время выполнения: Алгебраический - {algebraicTimer.Elapsed.TotalMilliseconds:F6} мс, Геометрический - {geometricTimer.Elapsed.TotalMilliseconds:F6} мс";

            // Перерисовываем форму
            Invalidate();
        }

        private void AnimateButton_Click(object sender, EventArgs e)
        {
            // Запускаем анимацию геометрического метода
            AnimateGeometricCurve(controlPointsRight, 100);
        }

        private async void AnimateGeometricCurve(PointF[] points, int numPoints)
        {
            for (int i = 0; i < numPoints; i++)
            {
                tValue = (float)i / (numPoints - 1);
                geometricCurveRight = BezierGeometric(points, i + 1);
                Invalidate(); // Перерисовываем форму для анимации
                await Task.Delay(50); // Задержка для анимации
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            // Рисуем контрольные точки для левой кривой
            foreach (var point in controlPointsLeft)
            {
                g.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10); // Увеличение размера точек
            }

            // Рисуем контрольные точки для правой кривой
            foreach (var point in controlPointsRight)
            {
                g.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10); // Увеличение размера точек
            }

            // Рисуем алгебраическую кривую для левой части
            if (algebraicCurveLeft != null)
            {
                DrawCurve(g, algebraicCurveLeft, Pens.Blue);
                g.DrawString("Алгебраический метод", new Font("Arial", 12), Brushes.Blue, new PointF(50, 300));
            }

            // Рисуем геометрическую кривую для правой части
            if (geometricCurveRight != null)
            {
                DrawCurve(g, geometricCurveRight, Pens.Green);
                g.DrawString("Геометрический метод", new Font("Arial", 12), Brushes.Green, new PointF(700, 300));
                DrawGuidelines(g, controlPointsRight, tValue, Pens.Gray); // Рисуем направляющие
                DrawTangent(g, controlPointsRight, tValue, Pens.Black); // Рисуем касательную
                g.DrawString($"t = {tValue:F2}", new Font("Arial", 12), Brushes.Black, new PointF(600, 400));
            }

            // Теоретическая база
            string theory = "Алгебраический метод: Кривая Безье строится с использованием полиномов Бернштейна.\n" +
                            "Геометрический метод (алгоритм де Кастельжо): Кривая строится путем рекурсивного интерполирования контрольных точек.";
            g.DrawString(theory, new Font("Arial", 10), Brushes.Black, new PointF(10, 400));
        }

        // Алгебраический метод
        private PointF[] BezierAlgebraic(PointF[] points, int numPoints)
        {
            PointF[] curve = new PointF[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                float x = (1 - t) * (1 - t) * (1 - t) * points[0].X +
                          3 * (1 - t) * (1 - t) * t * points[1].X +
                          3 * (1 - t) * t * t * points[2].X +
                          t * t * t * points[3].X;
                float y = (1 - t) * (1 - t) * (1 - t) * points[0].Y +
                          3 * (1 - t) * (1 - t) * t * points[1].Y +
                          3 * (1 - t) * t * t * points[2].Y +
                          t * t * t * points[3].Y;
                curve[i] = new PointF(x, y);
            }
            return curve;
        }

        // Геометрический метод (алгоритм де Кастельжо)
        private PointF[] BezierGeometric(PointF[] points, int numPoints)
        {
            PointF[] curve = new PointF[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                curve[i] = DeCasteljau(points, t);
            }
            return curve;
        }

        private PointF DeCasteljau(PointF[] points, float t)
        {
            PointF[] currentPoints = (PointF[])points.Clone();
            while (currentPoints.Length > 1)
            {
                PointF[] nextPoints = new PointF[currentPoints.Length - 1];
                for (int i = 0; i < nextPoints.Length; i++)
                {
                    nextPoints[i] = Lerp(currentPoints[i], currentPoints[i + 1], t);
                }
                currentPoints = nextPoints;
            }
            return currentPoints[0];
        }

        private PointF Lerp(PointF p1, PointF p2, float t)
        {
            return new PointF(
                (1 - t) * p1.X + t * p2.X,
                (1 - t) * p1.Y + t * p2.Y
            );
        }

        private void DrawCurve(Graphics g, PointF[] curve, Pen pen)
        {
            for (int i = 0; i < curve.Length - 1; i++)
            {
                g.DrawLine(pen, curve[i], curve[i + 1]);
            }
        }

        private void DrawGuidelines(Graphics g, PointF[] points, float t, Pen pen)
        {
            PointF[] Q = new PointF[points.Length - 1];
            PointF[] R = new PointF[points.Length - 2];

            for (int i = 0; i < Q.Length; i++)
            {
                Q[i] = Lerp(points[i], points[i + 1], t);
                g.DrawLine(pen, points[i], Q[i]);
            }

            for (int i = 0; i < R.Length; i++)
            {
                R[i] = Lerp(Q[i], Q[i + 1], t);
                g.DrawLine(pen, Q[i], R[i]);
            }

            if (R.Length > 0)
            {
                PointF B = R[0];
                g.FillEllipse(Brushes.Black, B.X - 3, B.Y - 3, 6, 6);
                g.DrawString("B", new Font("Arial", 12), Brushes.Black, B);
            }
        }

        private void DrawTangent(Graphics g, PointF[] points, float t, Pen pen)
        {
            PointF p0 = DeCasteljau(points, t);
            PointF p1 = DeCasteljau(points, t + 0.01f);
            g.DrawLine(pen, p0, p1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}