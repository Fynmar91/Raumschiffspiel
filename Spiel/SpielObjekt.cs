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
}
