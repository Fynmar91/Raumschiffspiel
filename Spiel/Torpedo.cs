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
	class Torpedo : SpielObjekt
	{
		Ellipse umriss = new Ellipse();
		public double MySchaden { get; set; }
		double MyGeschwindigkeit { get; set; }
		int MyGroesse { get; set; }
		int MyLeben { get; set; }

		public Torpedo(SpielObjekt start, int abweichung, Color fabe, double schaden, int leben, double geschwindigkeit, int groesse)
			: base(start.MyX, start.MyY,
					Math.Cos((-90 + abweichung + start.MyRotation.Angle) * Math.PI / 180) * geschwindigkeit, Math.Sin((-90 + abweichung + start.MyRotation.Angle) * Math.PI / 180) * geschwindigkeit,
					3, 10)
		{
			MySchaden = schaden;
			MyGeschwindigkeit = geschwindigkeit;
			MyGroesse = groesse;
			MyLeben = leben;

			umriss.Width = MyGroesse;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);

			MyKollision = new Rect(MyX, MyY, umriss.ActualWidth, umriss.ActualHeight);
		}

		public Torpedo(double x, double y, int abweichung, Color fabe, double schaden, int leben, double geschwindigkeit, int groesse)
			: base(x, y, Math.Cos((-90 + abweichung) * Math.PI / 180) * 1500
				, Math.Sin((-90 + abweichung) * Math.PI / 180) * 1500,
				  3, 10)
		{
			MySchaden = schaden;
			MyGeschwindigkeit = geschwindigkeit;
			MyGroesse = groesse;
			MyLeben = leben;

			umriss.Width = MyGroesse;
			umriss.Height = 10;
			umriss.Fill = new SolidColorBrush(fabe);
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			umriss.RenderTransform = new RotateTransform(-90 + Math.Atan2(MyYvel, MyXvel) / Math.PI * 180);
			zeichenflaeche.Children.Add(umriss);
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			return --MyLeben < 0;
		}
	}
}
