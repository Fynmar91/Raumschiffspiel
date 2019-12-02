using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Spiel
{
	/// <summary>
	/// Interaktionslogik für Upgrades.xaml
	/// </summary>
	public partial class Upgrades : Window
	{
		MainWindow window;

		int speed = 0;
		int dmg = 0;

		public Upgrades(MainWindow w)
		{
			InitializeComponent();
			this.window = w;
			Refresh();
		}

		void Refresh()
		{
			textBlock_speed.Text = (window.schiffGeschwindikeit + speed * 20).ToString();
			textBlock_dmg.Text = (window.schiffSchaden + speed * 0.1).ToString();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			window.schiffGeschwindikeit += speed * 20;
			window.schiffSchaden += dmg * 0.1;
			this.Close();
			window.Weiter();
		}

		private void Button_speedMinus_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_speedPlus_Click(object sender, RoutedEventArgs e)
		{
			if (window.xp > 100)
			{
				speed++;
				window.xp -= 100;
			}
			Refresh();
		}

		private void Button_dmgMinus_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_dmgPlus_Click(object sender, RoutedEventArgs e)
		{
			if (window.xp > 100)
			{
				dmg++;
				window.xp -= 100;
			}
			Refresh();
		}
	}
}
