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
		public const int Ueberhang = 48;

		public Rect MyKollision;
		double MyBreite { get; set; }
		double MyHoehe { get; set; }
		public double MyX { get; set; }
		public double MyY { get; set; }
		public double MyXvel { get; set; }
		public double MyYvel { get; set; }
		public RotateTransform MyRotation { get; set; }

		protected SpielObjekt(double x, double y, double vx, double vy, double breite, double hoehe)
		{
			MyBreite = breite;
			MyHoehe = hoehe;
			this.MyX = x;
			this.MyY = y;
			this.MyXvel = vx;
			this.MyYvel = vy;

			MyKollision = new Rect(MyX, MyY, MyBreite, MyHoehe);
		}

		public bool Animiere(Canvas zeichenflaeche, TimeSpan intervall)
		{
			MyX += MyXvel * intervall.TotalSeconds;
			MyY += MyYvel * intervall.TotalSeconds;

			MyKollision = new Rect(MyX, MyY, MyBreite, MyHoehe);

			if (MyX < -Ueberhang)
			{
				MyXvel = -MyXvel;
				MyX = -Ueberhang;
				return true;
			}
			else if (MyX > zeichenflaeche.ActualWidth + Ueberhang)
			{
				MyXvel = -MyXvel;
				MyX = zeichenflaeche.ActualWidth + Ueberhang;
				return true;
			}

			if (MyY < -Ueberhang)
			{
				MyYvel = -MyYvel;
				MyY = -Ueberhang;
				return true;
			}
			else if (MyY > zeichenflaeche.ActualHeight + Ueberhang)
			{
				MyYvel = -MyYvel;
				MyY = zeichenflaeche.ActualHeight + Ueberhang;
				return true;
			}

			return false;
		}

		public bool EnthaeltPunkt(Rect ziel)
		{
			return MyKollision.IntersectsWith(ziel);
		}

		public abstract bool Zeichne(Canvas zeichenflaeche);
	}
}
