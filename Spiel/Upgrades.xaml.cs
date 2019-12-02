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
		int upgradePunkte = 5;

		int speed = 0;

		public Upgrades(MainWindow w)
		{
			InitializeComponent();
			this.window = w;
			Refresh();
		}

		void Refresh()
		{
			textBlock_speed.Text = (window.schiffGeschwindikeit + speed * 20).ToString();
		}

		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			if (upgradePunkte == 0)
			{
				window.schiffGeschwindikeit += speed * 20;
				this.Close();
				window.Weiter();
			}
		}

		private void Button_speedMinus_Click(object sender, RoutedEventArgs e)
		{
			if (speed > 0)
			{
				speed--;
				upgradePunkte++;
			}
			Refresh();
		}

		private void Button_speedPlus_Click(object sender, RoutedEventArgs e)
		{
			if (upgradePunkte > 0)
			{
				speed++;
				upgradePunkte--;
			}
			Refresh();
		}
	}
}
