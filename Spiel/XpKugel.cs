﻿using System;
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
