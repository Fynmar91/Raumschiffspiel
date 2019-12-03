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
	class Asteroid : SpielObjekt
	{
		static Random zufall = new Random();
		Polygon umriss = new Polygon();
		public double MyLeben { get; set; }
		public int MyMass { get; set; }

		public Asteroid(Canvas zeichenflaeche, int multiplier, int groesse = 24)
			: base(zufall.NextDouble() * zeichenflaeche.ActualWidth, zufall.NextDouble() * zeichenflaeche.ActualHeight,
					(zufall.NextDouble() - 0.5) * 800 * (1 + multiplier / 6), (zufall.NextDouble() - 0.5) * 800 * (1 + multiplier / 6))
		{
			if (MyX - zeichenflaeche.ActualWidth < MyX)
			{
				if ((MyX - zeichenflaeche.ActualWidth) / 1.6 < MyY - zeichenflaeche.ActualHeight
						&& (MyX - zeichenflaeche.ActualWidth) / 1.6 < MyY)
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

			MyLeben = multiplier + 20;
			MyMass = 33;
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
			MyLeben -= schaden;
			return MyLeben <= 0;
		}
	}

	class BossAsteroid : Asteroid
	{
		public BossAsteroid(Canvas zeichenflaeche, int multiplier)
			: base(zeichenflaeche, multiplier, Convert.ToInt32(zeichenflaeche.ActualHeight / 8))
		{
			MyLeben = multiplier * 100 + 400;
			MyMass = 99;
		}
	}
}
