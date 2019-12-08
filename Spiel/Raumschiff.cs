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
	class Raumschiff : SpielObjekt
	{
		Point mausPos;
		Polygon umriss = new Polygon();
		Ellipse schild = new Ellipse();
		int schildRadius = 0;
		
		public double MyHP { get; set; }
		public int MySchild { get; set; }

		public Raumschiff(Canvas zeichenflaeche)
			: base(0.5d * zeichenflaeche.ActualWidth, 0.5d * zeichenflaeche.ActualHeight, 0, 0, 20, 25)
		{
			umriss.Points.Add(new Point(0d, -15d));
			umriss.Points.Add(new Point(10d, 10d));
			umriss.Points.Add(new Point(-10d, 10d));
			umriss.Fill = Brushes.White;

			schild.Width = schildRadius;
			schild.Height = schildRadius;
			schild.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));
			schild.Fill.Opacity = 0;
			MyHP = 100;
		}

		public override bool Zeichne(Canvas zeichenflaeche)
		{
			Canvas.SetLeft(umriss, MyX);
			Canvas.SetTop(umriss, MyY);
			zeichenflaeche.Children.Add(umriss);

			schild.Width = schildRadius;
			schild.Height = schildRadius;
			Canvas.SetZIndex(schild, -1);
			Canvas.SetLeft(schild, MyX - schild.ActualWidth / 2);
			Canvas.SetTop(schild, MyY - schild.ActualHeight / 2);
			zeichenflaeche.Children.Add(schild);

			return false;
		}

		public void Rotieren(Canvas zeichenflaeche, Point mausPos)
		{
			this.mausPos = mausPos;
			Point schiffPos = zeichenflaeche.PointToScreen(new Point(this.MyX, this.MyY));
			double xDiff = mausPos.X - 210 - schiffPos.X;
			double yDiff = mausPos.Y - 30 - schiffPos.Y;
			MyRotation = new RotateTransform(90 + (Math.Atan2(yDiff, xDiff) * 180 / Math.PI));
			umriss.RenderTransform = MyRotation;
		}

		public bool Damage(double mass)
		{
			if (mass <= 33)
			{
				switch (MySchild)
				{
					case 4:
						MySchild--;
						schildRadius = 90;
						return false;
					case 3:
						MySchild--;
						schildRadius = 60;
						return false;
					case 2:
						MySchild--;
						schildRadius = 30;
						return false;
					case 1:
						MySchild = 0;
						schild.Fill.Opacity = 0;
						return false;
					case 0:
						MyHP -= mass;
						return true;
					default:
						return true;
				}
			}
			else
			{
				MyHP -= mass;
				MySchild = 0;
				schild.Fill.Opacity = 0;
				return true;
			}
		}

		public void StarteSchilde()
		{
			if (MySchild < 3)
			{
				MySchild += 2;
				schildRadius += 60;
			}
			else if (MySchild == 3)
			{
				MySchild = 4;
				schildRadius = 120;
			}
			schild.Fill.Opacity = 100;
		}
	}
}
