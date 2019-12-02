using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spiel
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			timer.Interval = TimeSpan.FromSeconds(0.02);
			timer.Tick += Update;
		}

		List<Asteroid> asteroidObjekte = new List<Asteroid>();
		List<Torpedo> torpedoObjekte = new List<Torpedo>();
		List<XpKugel> powerUpObjekte = new List<XpKugel>();

		Raumschiff raumschiff = null;

		DispatcherTimer timer = new DispatcherTimer();
		static Random zufall = new Random();

		bool spielLaeuft = false;
		bool spielPausiert = false;
		double zeit;
		double spawnZeit;
		int durchlaeufe;

		bool schiessen;
		int schiessWarte;
		double score;
		int bomben;
		int upgrades;

		const int UpgradePreis = 400;
		const int maxUpgrades = 6;

		int xp;
		int schiffGeschwindikeit = 400;
		double schiffSchaden = 1;

		public int MyUpgradePreis
		{
			get
			{
				return UpgradePreis + upgrades * 200 - 100;
			}
		}

		public int MySpeedPreis
		{
			get
			{
				return Convert.ToInt32(schiffGeschwindikeit - 200);
			}
		}

		public int MyDmgPreis
		{
			get
			{
				return Convert.ToInt32(schiffSchaden * 400 - 200);
			}
		}


		void Update(object sender, EventArgs e)
		{
			if (spielLaeuft == true)
			{
				zeit += 0.2 * 0.2;
				spawnZeit += 0.2 * 0.2;

				if (raumschiff.MyHP > 0)
				{
					if (spawnZeit + durchlaeufe / 100 > 0.6)
					{
						spawnZeit = 0;

						asteroidObjekte.Add(new Asteroid(zeichenflaeche, durchlaeufe));
					}

					if (zeit / durchlaeufe >= 10d)
					{
						durchlaeufe += 1;

						if (raumschiff.MyHP < 97d)
						{
							raumschiff.MyHP += 3;
						}
						else
						{
							raumschiff.MyHP = 100;
						}

						powerUpObjekte.Clear();

						switch (zufall.Next(0, 8))
						{
							case 1:
								powerUpObjekte.Add(new XpKlein(zeichenflaeche));
								break;
							case 2:
								powerUpObjekte.Add(new XpKlein(zeichenflaeche));
								break;
							case 3:
								powerUpObjekte.Add(new XpKlein(zeichenflaeche));
								break;
							case 4:
								powerUpObjekte.Add(new XpKlein(zeichenflaeche));
								break;
							case 5:
								powerUpObjekte.Add(new XpMittel(zeichenflaeche));
								break;
							case 6:
								powerUpObjekte.Add(new XpMittel(zeichenflaeche));
								break;
							case 7:
								powerUpObjekte.Add(new XpHoch(zeichenflaeche));
								break;
							default:
								break;
						}

					}

					if (schiessen)
					{
						List<Color> farbe = new List<Color>();
						farbe.Add(Color.FromArgb(255, 0, 255, 255));
						farbe.Add(Color.FromArgb(255, 0, 140, 255));
						farbe.Add(Color.FromArgb(255, 0, 105, 180));
						farbe.Add(Color.FromArgb(255, 128, 105, 180));
						farbe.Add(Color.FromArgb(255, 180, 105, 180));
						farbe.Add(Color.FromArgb(255, 255, 105, 180));
						farbe.Add(Color.FromArgb(255, 255, 140, 220));

						schiessWarte++;

						for (int i = 0; i < upgrades + 1; i++)
						{
							if (i == 0 || ((i >= 1 && i < 3) && schiessWarte % 6 == 0) || (i >= 3 && schiessWarte % 10 == 0))
							{
								torpedoObjekte.Add(new Torpedo(raumschiff, (5 * i + 2), farbe[i], schiffSchaden));
								torpedoObjekte.Add(new Torpedo(raumschiff, -(5 * i + 2), farbe[i], schiffSchaden));
							}

							if (i >= 3 && schiessWarte % 3 == 0)
							{
								torpedoObjekte.Add(new Torpedo(raumschiff, 180 - 2, farbe[i], schiffSchaden));
								torpedoObjekte.Add(new Torpedo(raumschiff, 180 + 2, farbe[i], schiffSchaden));
							}
						}
					}

					Animate();
					UpdateText();
				}
				else if (raumschiff.MyHP <= 0)
				{
					zeichenflaeche.Children.Clear();
					asteroidObjekte.Clear();
					torpedoObjekte.Clear();
					powerUpObjekte.Clear();
					raumschiff = null;
					border.Background = Brushes.Red;
					button_start.IsEnabled = true;
					spielLaeuft = false;
					timer.Stop();
				}
			}
		}

		void Animate()
		{
			raumschiff.Animiere(zeichenflaeche, timer.Interval);
			raumschiff.Rotieren(zeichenflaeche, zeichenflaeche.PointToScreen(Mouse.GetPosition(this)));

			List<Torpedo> abfall = new List<Torpedo>();

			foreach (var item in asteroidObjekte)
			{
				item.Animiere(zeichenflaeche, timer.Interval);
			}

			foreach (var item in torpedoObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall.Add(item);
				}
			}

			foreach (var item in abfall)
			{
				torpedoObjekte.Remove(item);
			}

			PruefeKollisionen();

			zeichenflaeche.Children.Clear();
			raumschiff.Zeichne(zeichenflaeche);

			foreach (var item in asteroidObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			foreach (var item in torpedoObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			foreach (var item in powerUpObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}
		}

		void PruefeKollisionen()
		{
			List<Asteroid> abfall_A = new List<Asteroid>();
			List<Torpedo> abfall_T = new List<Torpedo>();
			List<XpKugel> abfall_P = new List<XpKugel>();

			foreach (var asteroid in asteroidObjekte)
			{
				foreach (var torpedo in torpedoObjekte)
				{
					if (asteroid.EnthaeltPunkt(torpedo.MyX, torpedo.MyY))
					{
						if (asteroid.Treffer(torpedo.MySchaden))
						{
							abfall_A.Add(asteroid);
							score += 20;
							xp += 20;
						}

						abfall_T.Add(torpedo);

					}
				}

				if (asteroid.EnthaeltPunkt(raumschiff.MyX, raumschiff.MyY))
				{
					if (raumschiff.Damage())
					{
						abfall_A.Add(asteroid);

						if (upgrades > 1)
						{
							upgrades -= 2;
						}
						else if (upgrades > 0)
						{
							upgrades--;
						}
					}
					else
					{
						abfall_A.Add(asteroid);
					}
				}
			}

			foreach (var item in powerUpObjekte)
			{
				if (item.EnthaeltPunkt(raumschiff.MyX, raumschiff.MyY))
				{
					if (item is XpKlein)
					{
						xp += 50;
					}
					else if (item is XpMittel)
					{
						xp += 100;
					}
					else if (item is XpHoch)
					{
						xp += 200;
					}

					abfall_P.Add(item);
				}
			}

			foreach (var item in abfall_A)
			{
				asteroidObjekte.Remove(item);
			}
			foreach (var item in abfall_T)
			{
				torpedoObjekte.Remove(item);
			}
			foreach (var item in abfall_P)
			{
				powerUpObjekte.Remove(item);
			}
		}

		void UpdateText()
		{
			textBlock_time.Text = zeit.ToString("0.##");
			textBlock_score.Text = score.ToString("0.");
			textBlock_health.Text = raumschiff.MyHP.ToString();
			prograssBar_health.Value = raumschiff.MyHP;
			textBlock_lebenStein.Text = (durchlaeufe / 8 + 10).ToString();
			textBlock_xp.Text = xp.ToString();
			textBlock_bomben.Text = bomben.ToString();
			textBlock_speed.Text = schiffGeschwindikeit.ToString();
			textBlock_dmg.Text = schiffSchaden.ToString();
			textBlock_upgrade.Text = upgrades.ToString();
			textBlock_upgradePreis.Text = MyUpgradePreis.ToString();
			textBlock_dmgPreis.Text = MyDmgPreis.ToString();
			textBlock_speedPreis.Text = MySpeedPreis.ToString();
		}

		void Pause()
		{
			spielPausiert = true;
			timer.Stop();
		}

		public void Weiter()
		{
			spielPausiert = false;
			timer.Start();
		}

		private void Button_start_Click(object sender, RoutedEventArgs e)
		{
			spielLaeuft = true;
			button_start.IsEnabled = false;
			zeit = 0;
			score = 0;
			durchlaeufe = 1;
			schiessen = false;
			upgrades = 0;
			xp = 0;
			bomben = 3;
			prograssBar_health.Value = 100;
			schiffGeschwindikeit = 400;
			spawnZeit = 0;
			schiessWarte = 0;
			schiffSchaden = 1;
			asteroidObjekte = new List<Asteroid>();
			torpedoObjekte = new List<Torpedo>();
			powerUpObjekte = new List<XpKugel>();
			border.Background = Brushes.Black;
			raumschiff = new Raumschiff(zeichenflaeche);
			timer.Start();

			for (int i = 0; i < 4; i++)
			{
				asteroidObjekte.Add(new Asteroid(zeichenflaeche, durchlaeufe));
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (spielLaeuft == true)
			{
				switch (e.Key)
				{
					case Key.W:
						raumschiff.MyYvel = -schiffGeschwindikeit;
						break;
					case Key.S:
						raumschiff.MyYvel = schiffGeschwindikeit;
						break;
					case Key.A:
						raumschiff.MyXvel = -schiffGeschwindikeit;
						break;
					case Key.D:
						raumschiff.MyXvel = schiffGeschwindikeit;
						break;
					case Key.R:
						if (upgrades < maxUpgrades)
						{
							upgrades += 1;
						}
						break;
					case Key.F:
						if (spielPausiert == true)
						{
							Weiter();
						}
						else
						{
							Pause();
						}
						break;
					default:
						break;
				}
			}
			else if (e.Key == Key.Space)
			{
				button_start.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
			}
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (spielLaeuft == true)
			{
				switch (e.Key)
				{
					case Key.W:
						raumschiff.MyYvel = 0d;
						break;
					case Key.S:
						raumschiff.MyYvel = 0d;
						break;
					case Key.A:
						raumschiff.MyXvel = 0d;
						break;
					case Key.D:
						raumschiff.MyXvel = 0d;
						break;
					default:
						break;
				}
			}
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				schiessen = true;
			}
			else if (e.ChangedButton == MouseButton.Right && bomben > 0)
			{
				Bombe();
				bomben--;
			}
		}

		private void Window_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				schiessen = false;
			}
		}

		private void Bombe()
		{
			Point p = Mouse.GetPosition(this);

			for (int i = 0; i < 360; i += 10)
			{
				torpedoObjekte.Add(new Torpedo(p.X - 210, p.Y - 30, i, Color.FromArgb(255, 255, 0, 0), schiffSchaden));
			}
		}

		private void Button_speedPlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= MySpeedPreis)
			{
				xp -= MySpeedPreis;
				schiffGeschwindikeit += 20;
			}

			UpdateText();
		}

		private void Button_dmgPlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= MyDmgPreis)
			{
				xp -= MyDmgPreis;
				schiffSchaden += 0.1;
			}

			UpdateText();
		}

		private void Button_upgradePlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= MyUpgradePreis && upgrades < maxUpgrades)
			{
				xp -= MyUpgradePreis;
				upgrades++;
			}

			UpdateText();
		}
	}
}
