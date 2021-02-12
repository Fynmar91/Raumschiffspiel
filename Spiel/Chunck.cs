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
	class Chunck : SpielObjekt
	{
		Rectangle umriss = new Rectangle();

		public Chunck(Canvas zeichenflaeche, double x, double y) : base(x, y)
		{
			umriss.Fill = Brushes.BlueViolet;
			umriss.Opacity = 0.25;
			umriss.Height = zeichenflaeche.ActualHeight / 3;
			umriss.Width = zeichenflaeche.ActualWidth / 3;
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
	}
}
