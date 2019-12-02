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

			if (x < 0)
			{
				x = zeichenflaeche.ActualWidth;
				uebertretung = true;
			}
			else if (x > zeichenflaeche.ActualWidth)
			{
				x = 0;
				uebertretung = true;
			}

			if (y < 0)
			{
				y = zeichenflaeche.ActualHeight;
				uebertretung = true;
			}
			else if (y > zeichenflaeche.ActualHeight)
			{
				y = 0;
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
					(zufall.NextDouble() - 0.5) * 400 * (1 + multiplier / 8), (zufall.NextDouble() - 0.5) * 400 * (1 + multiplier / 8))
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
						MyY = 0;
					}
				}

			}
			else
			{
				if (MyX / 1.6 < MyY - zeichenflaeche.ActualHeight && MyX / 1.6 < MyY)
				{
					MyX = 0;
				}
				else
				{
					if (MyY - zeichenflaeche.ActualHeight < MyY)
					{
						MyY = zeichenflaeche.ActualHeight;
					}
					else
					{
						MyY = 0;
					}
				}
			}


			for (int i = 0; i < 20; i++)
			{
				double alpha = 2 * Math.PI / 20 * i;
				double radius = 24 + 12 * zufall.NextDouble();
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

		public bool Treffer(int schaden)
		{
			leben -= schaden;
			return leben <= 0;
		}
	}

	class Raumschiff : SpielObjekt
	{
		Point mausPos;
		Polygon umriss = new Polygon();
		Ellipse schild = new Ellipse();
		int schildRadius = 100;

		RotateTransform rotation = null;
		public RotateTransform Rotation { get => rotation; }
		public int MyHP { get; set; }
		public int MySchild { get; set; }

		public Raumschiff(Canvas zeichenflaeche)
			: base(0.5d * zeichenflaeche.ActualWidth, 0.5d * zeichenflaeche.ActualHeight)
		{
			umriss.Points.Add(new Point(0d, -15d));
			umriss.Points.Add(new Point(10d, 10d));
			umriss.Points.Add(new Point(-10d, 10d));
			umriss.Fill = Brushes.White;

			schild.Width = schildRadius;
			schild.Height = schildRadius;
			schild.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));
			schild.Fill.Opacity = 0;
			MyHP = 100;
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			zeichenflaeche.Children.Add(schild);
			Canvas.SetLeft(schild, MyX - schild.ActualWidth / 2);
			Canvas.SetTop(schild, MyY - schild.ActualHeight / 2);
			Canvas.SetZIndex(schild, -1);
			schild.Width = schildRadius;
			schild.Height = schildRadius;
		}

		public void Rotieren(Canvas zeichenflaeche, Point mausPos)
		{
			this.mausPos = mausPos;
			Point schiffPos = zeichenflaeche.PointToScreen(new Point(this.MyX, this.MyY));
			double xDiff = mausPos.X - 210 - schiffPos.X;
			double yDiff = mausPos.Y - 30 - schiffPos.Y;
			rotation = new RotateTransform(90 + (Math.Atan2(yDiff, xDiff) * 180 / Math.PI));
			umriss.RenderTransform = rotation;
		}

		public bool Damage()
		{
			switch (MySchild)
			{
				case 3:
					MySchild--;
					schildRadius = 80;
					return false;
				case 2:
					MySchild--;
					schildRadius = 60;
					return false;
				case 1:
					MySchild = 0;
					schild.Fill.Opacity = 0;
					return false;
				case 0:
					MyHP -= 33;
					return true;
				default:
					return true;
			}
		}

		public void StarteSchilde()
		{
			MySchild = 3;
			schild.Fill.Opacity = 100;
			schildRadius = 100;
		}
	}

	class PhotonenTorpedo : SpielObjekt
	{
		Ellipse umriss = new Ellipse();
		public int MySchaden { get; set; }

		public PhotonenTorpedo(Raumschiff raumschiff, int abweichung, Color fabe, double geschwindigkeit = 1)
			: base(raumschiff.MyX, raumschiff.MyY,
					Math.Cos((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500 * geschwindigkeit, Math.Sin((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500 * geschwindigkeit)
		{
			umriss.Width = 3;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);
			umriss.RenderTransform = raumschiff.Rotation;
			MySchaden = 1;
		}

		public PhotonenTorpedo(double x, double y, int abweichung, Color fabe, double geschwindigkeit = 1)
			: base(x, y, Math.Cos((-90 + abweichung) * Math.PI / 180) * 1500 * geschwindigkeit
				  , Math.Sin((-90 + abweichung) * Math.PI / 180) * 1500 * geschwindigkeit)
		{
			umriss.Width = 3;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);
			umriss.RenderTransform =  new RotateTransform(abweichung);
			MySchaden = 10;
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
		}
	}

	abstract class PowerUp : SpielObjekt
	{
		static Random zufall = new Random();
		Ellipse umriss = new Ellipse();

		public PowerUp(Canvas zeichenflaeche, SolidColorBrush brush)
			: base(zufall.NextDouble() * zeichenflaeche.ActualWidth, zufall.NextDouble() * zeichenflaeche.ActualHeight)
		{
			umriss.Width = 40;
			umriss.Height = 40;
			umriss.Fill = brush;
		}

		public bool EnthaeltPunkt(double x, double y)
		{
			return umriss.RenderedGeometry.FillContains(new Point(x - MyX, y - MyY));
		}

		public override void Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
		}
	}

	class WaffenUp : PowerUp
	{
		public WaffenUp(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.BlueViolet)
		{
		}
	}

	class SchildUp : PowerUp
	{
		public SchildUp(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.Plum)
		{
		}
	}

	class BombUp : PowerUp
	{
		public BombUp(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.Red)
		{
		}
	}
}
