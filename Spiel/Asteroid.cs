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
}
