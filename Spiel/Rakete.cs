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
	class Rakete : SpielObjekt
	{
		Ellipse umriss = new Ellipse();
		public double MySchaden { get; set; }

		public Rakete(SpielObjekt start, Color fabe, double schaden)
			: base(start.MyX, start.MyY,
				  Math.Cos((-90 + start.MyRotation.Angle) * Math.PI / 180) * 1500, Math.Sin((-90 + start.MyRotation.Angle) * Math.PI / 180) * 1500,
				  5, 20)
		{
			umriss.Width = 5;
			umriss.Height = 20;
			umriss.Fill = new SolidColorBrush(fabe);
			MySchaden = schaden * 2;

			MyKollision = new Rect(MyX, MyY, umriss.ActualWidth, umriss.ActualHeight);
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
}
