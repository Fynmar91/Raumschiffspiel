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
		public double MyX { get; set; }
		public double MyY { get; set; }
		public double MyXvel { get; set; }
		public double MyYvel { get; set; }


		protected SpielObjekt(double x, double y)
		{
			this.MyX = x;
			this.MyY = y;
		}

		protected SpielObjekt(double x, double y, double vx, double vy)
		{
			this.MyX = x;
			this.MyY = y;
			this.MyXvel = vx;
			this.MyYvel = vy;
		}

		public bool Animiere(Canvas zeichenflaeche, TimeSpan intervall)
		{
			MyX += MyXvel * intervall.TotalSeconds;
			MyY += MyYvel * intervall.TotalSeconds;

			if (MyX < 0)
			{
				MyXvel = -MyXvel;
				return true;
			}
			else if (MyX > zeichenflaeche.ActualWidth)
			{
				MyXvel = -MyXvel;
				return true;
			}

			if (MyY < 0)
			{
				MyYvel = -MyYvel;
				return true;
			}
			else if (MyY > zeichenflaeche.ActualHeight)
			{
				MyYvel = -MyYvel;
				return true;
			}

			return false;
		}

		public abstract bool Zeichne(Canvas zeichenflaeche);
	}

	class Asteroid : SpielObjekt
	{
		static Random zufall = new Random();
		Polygon umriss = new Polygon();
		double leben;

		public Asteroid(Canvas zeichenflaeche, int multiplier, int groesse = 24, int extraLeben = 0)
			: base(zufall.NextDouble() * zeichenflaeche.ActualWidth, zufall.NextDouble() * zeichenflaeche.ActualHeight,
					(zufall.NextDouble() - 0.5) * 400 * (1 + multiplier / 6), (zufall.NextDouble() - 0.5) * 400 * (1 + multiplier / 6))
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
				double radius = groesse + groesse / 2 * zufall.NextDouble();
				umriss.Points.Add(new Point(radius * Math.Cos(alpha), radius * Math.Sin(alpha)));
			}
			umriss.Fill = Brushes.Gray;

			leben = multiplier / 4 + 20 + extraLeben;
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return false;
		}

		public bool EnthaeltPunkt(double x, double y)
		{
			return umriss.RenderedGeometry.FillContains(new Point(x - MyX, y - MyY));
		}

		public bool Treffer(double schaden)
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

		public override bool Zeichne(Canvas zeichenflaeche)
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
			return false;
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

	class Torpedo : SpielObjekt
	{
		Ellipse umriss = new Ellipse();
		public double MySchaden { get; set; }
		int leben;

		public Torpedo(Raumschiff raumschiff, int abweichung, Color fabe, double schaden, int leben)
			: base(raumschiff.MyX, raumschiff.MyY,
					Math.Cos((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500, Math.Sin((-90 + abweichung + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500)
		{
			umriss.Width = 3;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);
			MySchaden = schaden;
			this.leben = leben;
		}

		public Torpedo(double x, double y, int abweichung, Color fabe, double schaden, int leben)
			: base(x, y, Math.Cos((-90 + abweichung) * Math.PI / 180) * 1500
				, Math.Sin((-90 + abweichung) * Math.PI / 180) * 1500)
		{
			umriss.Width = 3;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);
			MySchaden = schaden * 40;
			this.leben = leben;
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			umriss.RenderTransform = new RotateTransform(-90 + Math.Atan2(MyYvel, MyXvel) / Math.PI * 180);
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return --leben < 0;
		}
	}

	class Rakete : SpielObjekt
	{
		Ellipse umriss = new Ellipse();
		public double MySchaden { get; set; }

		public Rakete(Raumschiff raumschiff, Color fabe, double schaden)
			: base(raumschiff.MyX, raumschiff.MyY,
				  Math.Cos((-90 + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500, Math.Sin((-90 + raumschiff.Rotation.Angle) * Math.PI / 180) * 1500)
		{
			umriss.Width = 5;
			umriss.Height = 20;
			umriss.Fill = new SolidColorBrush(fabe);
			MySchaden = schaden * 5;
		}

		public void Ziel(Vector vector)
		{
			RotateTransform rotateTransform = new RotateTransform(-90 + Math.Atan2(vector.Y, vector.X) / Math.PI * 180);
			MyXvel = Math.Cos((-90 + rotateTransform.Angle) * Math.PI / 180) * 1500;
			MyYvel = Math.Sin((-90 + rotateTransform.Angle) * Math.PI / 180) * 1500;
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			umriss.RenderTransform = new RotateTransform(-90 + Math.Atan2(MyYvel, MyXvel) / Math.PI * 180);
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return false;
		}
	}

	abstract class XpKugel : SpielObjekt
	{
		static Random zufall = new Random();
		Ellipse umriss = new Ellipse();

		public XpKugel(Canvas zeichenflaeche, SolidColorBrush brush, int groesse)
			: base(zufall.NextDouble() * zeichenflaeche.ActualWidth, zufall.NextDouble() * zeichenflaeche.ActualHeight)
		{
			umriss.Width = groesse;
			umriss.Height = groesse;
			umriss.Fill = brush;
		}

		public bool EnthaeltPunkt(double x, double y)
		{
			return umriss.RenderedGeometry.FillContains(new Point(x - MyX, y - MyY));
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return false;
		}
	}

	class XpKlein : XpKugel
	{
		public XpKlein(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.SaddleBrown, 40)
		{
		}
	}

	class XpMittel : XpKugel
	{
		public XpMittel(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.Silver, 35)
		{
		}
	}

	class XpHoch : XpKugel
	{
		public XpHoch(Canvas zeichenflaeche) : base(zeichenflaeche, Brushes.Gold, 30)
		{
		}
	}
}
