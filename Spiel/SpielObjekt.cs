using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Spiel
{
	abstract class SpielObjekt
	{
		double x;
		double y;
		double xVel;
		double yVel;
		public double MyX { get => x; set => x = value; }
		public double MyY { get => y; set => y = value; }
		public double MyXvel { get => xVel; set => xVel = value; }
		public double MyYvel { get => yVel; set => yVel = value; }


		protected SpielObjekt(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		protected SpielObjekt(double x, double y, double vx, double vy)
		{
			this.x = x;
			this.y = y;
			this.xVel = vx;
			this.yVel = vy;
		}

		public bool Animiere(Canvas zeichenflaeche, TimeSpan intervall)
		{
			x += MyXvel * intervall.TotalSeconds;
			y += MyYvel * intervall.TotalSeconds;

			bool uebertretung = false;

			if (x < 0d)
			{
				x = zeichenflaeche.ActualWidth;
				uebertretung = true;
			}
			else if (x > zeichenflaeche.ActualWidth)
			{
				x = 0d;
				uebertretung = true;
			}

			if (y < 0d)
			{
				y = zeichenflaeche.ActualHeight;
				uebertretung = true;
			}
			else if (y > zeichenflaeche.ActualHeight)
			{
				y = 0d;
				uebertretung = true;
			}

			return uebertretung;
		}

		public abstract void Zeichne(Canvas zeichenflaeche);
	}

	class Asteroid : SpielObjekt
	{
		static Random zufall = new Random();
		Polygon umriss = new Polygon();
		int leben = 10;

		public Asteroid(Canvas zeichenflaeche, int multiplier)
			: base(zufall.NextDouble() * zeichenflaeche.ActualWidth, zufall.NextDouble() * zeichenflaeche.ActualHeight,
					(zufall.NextDouble() - 0.5) * 400d * (1 + multiplier / 8), (zufall.NextDouble() - 0.5) * 400d * (1 + multiplier / 8))
		{
			if (MyX - zeichenflaeche.ActualWidth < MyX)
			{
				if ((MyX - zeichenflaeche.ActualWidth) / 1.6 < MyY - zeichenflaeche.ActualHeight && (MyX - zeichenflaeche.ActualWidth) / 1.6 < MyY)
				{
					MyX = zeichenflaeche.ActualWidth;
				}
				else
				{
					if (MyY - zeichenflaeche.ActualHeight < MyY)
					{
						MyY = zeichenflaeche.ActualHeight;
					}
					else
					{
						MyY = 0d;
					}
				}

			}
			else
			{
				if (MyX / 1.6 < MyY - zeichenflaeche.ActualHeight && MyX / 1.6 < MyY)
				{
					MyX = 0d;
				}
				else
				{
					if (MyY - zeichenflaeche.ActualHeight < MyY)
					{
						MyY = zeichenflaeche.ActualHeight;
					}
					else
					{
						MyY = 0d;
					}
				}
			}


			for (int i = 0; i < 20; i++)
			{
				double alpha = 2d * Math.PI / 20d * i;
				double radius = 24d + 12d * zufall.NextDouble();
				umriss.Points.Add(new Point(radius * Math.Cos(alpha), radius * Math.Sin(alpha)));
			}
			umriss.Fill = Brushes.Gray;

			leben = multiplier / 8 + 10;
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
		}

		public bool EnthaeltPunkt(double x, double y)
		{
			return umriss.RenderedGeometry.FillContains(new Point(x - MyX, y - MyY));
		}

		public bool Damage()
		{
			leben -= 1;
			return leben <= 0;
		}
	}

	class Raumschiff : SpielObjekt
	{
		Polygon umriss = new Polygon();
		RotateTransform rotation = null;
		public RotateTransform Rotation { get => rotation; }

		public Raumschiff(Canvas zeichenflaeche)
			: base(0.5d * zeichenflaeche.ActualWidth, 0.5d * zeichenflaeche.ActualHeight)
		{
			umriss.Points.Add(new Point(0d, -15d));
			umriss.Points.Add(new Point(10d, 10d));
			umriss.Points.Add(new Point(-10d, 10d));
			umriss.Fill = Brushes.White;
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
		}

		public void Rotieren(Canvas zeichenflaeche, Point mausPos)
		{
			Point schiffPos = zeichenflaeche.PointToScreen(new Point(this.MyX, this.MyY));
			double xDiff = mausPos.X - 210d - schiffPos.X;
			double yDiff = mausPos.Y - 30d - schiffPos.Y;
			rotation = new RotateTransform(90 + (Math.Atan2(yDiff, xDiff) * 180 / Math.PI));
			umriss.RenderTransform = rotation;
		}
	}

	class PhotonenTorpedo : SpielObjekt
	{
		Ellipse umriss = new Ellipse();

		public PhotonenTorpedo(Raumschiff raumschiff, int abweichung, Color fabe, double geschwindigkeit = 1)
			: base(raumschiff.MyX, raumschiff.MyY,
					Math.Cos((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500d * geschwindigkeit, Math.Sin((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500d* geschwindigkeit)
		{
			umriss.Width = 3d;
			umriss.Height = 10d;
			umriss.Fill = new SolidColorBrush(fabe);
			umriss.RenderTransform = raumschiff.Rotation;
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
		}
	}
}
