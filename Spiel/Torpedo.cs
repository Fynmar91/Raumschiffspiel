using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Spiel
{
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
			MySchaden = schaden * 80;
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
}
