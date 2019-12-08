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
	abstract class Gegner : SpielObjekt
	{
		protected static Random zufall = new Random();
		protected Polygon umriss = new Polygon();
		public double MyLeben { get; set; }
		public int MyMass { get; set; }

		protected Gegner(double x, double y, double vx, double vy, double breite, double hoehe) : base(x, y, vx, vy, breite, hoehe)
		{
			MyKollision = new Rect(MyX, MyY, umriss.ActualWidth, umriss.ActualHeight);
		}
		public override bool Zeichne(Canvas zeichenflaeche)
		{
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return false;
		}

		public bool Treffer(double schaden)
		{
			MyLeben -= schaden;
			return MyLeben <= 0;
		}
	}

	class Asteroid : Gegner
	{
		public Asteroid(Canvas zeichenflaeche, int multiplier, int groesse = 48)
			: base(zeichenflaeche.ActualWidth + Ueberhang, zufall.NextDouble() * zeichenflaeche.ActualHeight,
					((zufall.NextDouble() - 1.5) * 100 - 400) * (1 + multiplier / 6), (zufall.NextDouble() - 0.5) * 100 * (1 + multiplier / 6),
					groesse + 20, groesse + 20)
		{

			for (int i = 0; i < 20; i++)
			{
				double alpha = 2 * Math.PI / 20 * i;
				double radius = groesse + groesse / 2 * zufall.NextDouble();
				umriss.Points.Add(new Point(radius * Math.Cos(alpha), radius * Math.Sin(alpha)));
			}
			umriss.Fill = Brushes.Gray;

			MyLeben = multiplier + 10;
			MyMass = 33;
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

	class GegnerSchiff : Gegner
	{
		public GegnerSchiff(Canvas zeichenflaeche, int multiplier) 
			: base(zeichenflaeche.ActualWidth + Ueberhang, zufall.NextDouble() * zeichenflaeche.ActualHeight,
					((zufall.NextDouble() - 1.5) * 100 - 400) * (1 + multiplier / 6), (zufall.NextDouble() - 0.5) * 100 * (1 + multiplier / 6),
					35, 30)
		{
			umriss.Points.Add(new Point(-20, 0));
			umriss.Points.Add(new Point(15, 15));
			umriss.Points.Add(new Point(15, -15));
			umriss.Fill = Brushes.IndianRed;

			MyRotation = new RotateTransform(-90);
			MyMass = 10;
		}

		public void Schiessen(List<Torpedo> gegnerTorpedoObjekte)
		{
			gegnerTorpedoObjekte.Add(new Torpedo(this, 0, Color.FromArgb(255, 255, 0, 0), 5, 24));
		}
	}
}
