using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		List<GegnerSchiff> gegnerObjekte = new List<GegnerSchiff>();
		List<Torpedo> torpedoObjekte = new List<Torpedo>();
		List<Torpedo> gegnerTorpedoObjekte = new List<Torpedo>();
		List<XpKugel> powerUpObjekte = new List<XpKugel>();
		List<Rakete> raketeObjekte = new List<Rakete>();

		Raumschiff raumschiff = null;

		DispatcherTimer timer = new DispatcherTimer();
		static Random zufall = new Random();

		bool spielLaeuft;
		bool spielPausiert;
		double zeit;
		double spawnZeit;
		int durchlaeufe;
		int level;

		bool schiessen;
		int schiessWarte;
		double score;
		int bomben;

		const int maxUpgrades = 6;
		const int maxRaketen = 6;

		int xp;
		int schiffWaffen;
		int schiffGeschwindikeit;
		double schiffSchaden;
		int schiffRaketen;
		bool umlenkung;

		public int MyWaffenPreis
		{
			get
			{
				return (schiffWaffen + 1) * 200 + 600;
			}
		}

		public int MySpeedPreis
		{
			get
			{
				return Convert.ToInt32(((double)schiffGeschwindikeit / (double)400) * (double)800 - 400);
			}
		}

		public int MyDmgPreis
		{
			get
			{
				return Convert.ToInt32(schiffSchaden * 800 - 400);
			}
		}

		public int MyRaketenPreis
		{
			get
			{
				return Convert.ToInt32(schiffRaketen * 100 + 800);
			}
		}

		void Update(object sender, EventArgs e)
		{
			if (spielLaeuft == true)
			{
				zeit += 0.02;
				spawnZeit -= 1;

				if (raumschiff.MyHP > 0)
				{
					if (zeit / durchlaeufe >= 10)
					{
						durchlaeufe += 1;
					}

					AsteridGenerator();
					PowerUpGenerator();
					HpRegen();
					AutoUpgrade();
					Schiessen();
					Animieren();
					PruefeKollisionen();
					Zeichnen();
					UpdateText();
				}
				else if (raumschiff.MyHP <= 0)
				{
					Stop();
				}

				foreach (var item in gegnerObjekte)
				{
					item.Schiessen(gegnerTorpedoObjekte);
				}
			}
		}

		void AsteridGenerator()
		{
			if (spawnZeit <= 0)
			{
				spawnZeit = 20;

				if (asteroidObjekte.Count() < 16)
				{
					asteroidObjekte.Add(new Asteroid(zeichenflaeche, durchlaeufe));
					gegnerObjekte.Add(new GegnerSchiff(zeichenflaeche, durchlaeufe));
				}
			}

			if (zufall.NextDouble() * zufall.NextDouble() * zufall.NextDouble() > 0.8)
			{
				asteroidObjekte.Add(new BossAsteroid(zeichenflaeche, level));
				level++;
			}
		}

		void PowerUpGenerator()
		{
			if (zufall.NextDouble() * zufall.NextDouble() * zufall.NextDouble() > 0.75)
			{
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
		}


		void HpRegen()
		{
			if (zeit / durchlaeufe >= 10d)
			{
				if (raumschiff.MyHP < 95d)
				{
					raumschiff.MyHP += 5;
				}
				else
				{
					raumschiff.MyHP = 100;
				}
			}
		}

		void AutoUpgrade()
		{
			if (xp >= MyWaffenPreis && schiffWaffen < maxUpgrades && checkBox_autoUpgrade.IsChecked == true)
			{
				xp -= MyWaffenPreis;
				schiffWaffen++;
				UpdateText();
			}
		}

		void Schiessen()
		{
			schiessWarte++;

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

				for (int i = 0; i < schiffWaffen + 1; i++)
				{
					if (i == 0 || (i == 1 && schiessWarte % 5 == 0) || (i == 2 && schiessWarte % 6 == 0)
							|| (i == 3 && schiessWarte % 7 == 0) || (i == 4 && schiessWarte % 7 == 0) || (i == 5 && schiessWarte % 7 == 0) || (i == 6 && schiessWarte % 7 == 0))
					{
						torpedoObjekte.Add(new Torpedo(raumschiff, (5 * i + 2), farbe[i], schiffSchaden, 24 - i * 4, 1500, 3));
						torpedoObjekte.Add(new Torpedo(raumschiff, -(5 * i + 2), farbe[i], schiffSchaden, 24 - i * 4, 1500, 3));
					}

					if (schiffWaffen > 2 && i == 0)
					{
						torpedoObjekte.Add(new Torpedo(raumschiff, 180 - 2, farbe[6], schiffSchaden, 6, 1500, 3));
						torpedoObjekte.Add(new Torpedo(raumschiff, 180 + 2, farbe[6], schiffSchaden, 6, 1500, 3));
					}

					if (schiffWaffen > 4 && i == 0)
					{
						torpedoObjekte.Add(new Torpedo(raumschiff, 120, farbe[1], schiffSchaden, 10, 1500, 3));
						torpedoObjekte.Add(new Torpedo(raumschiff, -120, farbe[1], schiffSchaden, 10, 1500, 3));
					}

					if (schiffRaketen > 0 && schiessWarte % (20 - schiffRaketen / 2) == 0)
					{
						raketeObjekte.Add(new Rakete(raumschiff, farbe[i], schiffSchaden));
					}
				}
			}
		}

		void Animieren()
		{
			raumschiff.Animiere(zeichenflaeche, timer.Interval);

			List<Asteroid> abfall_A = new List<Asteroid>();
			foreach (var item in asteroidObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall_A.Add(item);
				}
			}
			foreach (var item in abfall_A)
			{
				asteroidObjekte.Remove(item);
			}

			List<GegnerSchiff> abfall_G = new List<GegnerSchiff>();
			foreach (var item in gegnerObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall_G.Add(item);
				}
			}
			foreach (var item in abfall_G)
			{
				gegnerObjekte.Remove(item);
			}

			List<Torpedo> abfall_T = new List<Torpedo>();
			foreach (var item in torpedoObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval) && umlenkung == false)
				{
					abfall_T.Add(item);
				}
			}
			foreach (var item in abfall_T)
			{
				torpedoObjekte.Remove(item);
			}

			abfall_T = new List<Torpedo>();
			foreach (var item in gegnerTorpedoObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall_T.Add(item);
				}
			}
			foreach (var item in abfall_T)
			{
				gegnerTorpedoObjekte.Remove(item);
			}

			List<Rakete> abfall_R = new List<Rakete>();
			foreach (var item in raketeObjekte)
			{
				item.Ziel(FindeZiel(item.MyX, item.MyY));

				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall_R.Add(item);
				}
			}
			foreach (var item in abfall_R)
			{
				raketeObjekte.Remove(item);
			}

			List<XpKugel> abfall_P = new List<XpKugel>();
			foreach (var item in powerUpObjekte)
			{
				if (item.Animiere(zeichenflaeche, timer.Interval))
				{
					abfall_P.Add(item);
				}
			}
			foreach (var item in abfall_P)
			{
				powerUpObjekte.Remove(item);
			}
		}

		void PruefeKollisionen()
		{
			List<Asteroid> abfall_A = new List<Asteroid>();
			List<GegnerSchiff> abfall_G = new List<GegnerSchiff>();
			List<Torpedo> abfall_T = new List<Torpedo>();
			List<Torpedo> abfall_GT = new List<Torpedo>();
			List<XpKugel> abfall_P = new List<XpKugel>();
			List<Rakete> abfall_R = new List<Rakete>();

			foreach (var asteroid in asteroidObjekte)
			{
				foreach (var torpedo in torpedoObjekte)
				{
					if (asteroid.EnthaeltPunkt(torpedo.MyKollision))
					{
						if (asteroid.Treffer(torpedo.MySchaden))
						{
							abfall_A.Add(asteroid);

							if (asteroid is BossAsteroid)
							{
								score += 400;
								xp += 400;
							}
							else
							{
								score += 40;
								xp += 40;
							}
							break;
						}

						abfall_T.Add(torpedo);
					}
				}

				foreach (var rakete in raketeObjekte)
				{
					if (asteroid.EnthaeltPunkt(rakete.MyKollision))
					{
						if (asteroid.Treffer(rakete.MySchaden))
						{
							abfall_A.Add(asteroid);

							if (asteroid is BossAsteroid)
							{
								score += 400;
								xp += 400;
							}
							else
							{
								score += 40;
								xp += 40;
							}
							break;
						}

						abfall_R.Add(rakete);
					}
				}

				if (asteroid.EnthaeltPunkt(raumschiff.MyKollision))
				{
					raumschiff.Damage(asteroid.MyMass);
					abfall_A.Add(asteroid);
				}
			}

			foreach (var gegner in gegnerObjekte)
			{
				foreach (var torpedo in torpedoObjekte)
				{
					if (gegner.EnthaeltPunkt(torpedo.MyKollision))
					{
						if (gegner.Treffer(torpedo.MySchaden))
						{
							abfall_G.Add(gegner);

							score += 40;
							xp += 40;

							break;
						}

						abfall_T.Add(torpedo);
					}
				}

				foreach (var rakete in raketeObjekte)
				{
					if (gegner.EnthaeltPunkt(rakete.MyKollision))
					{
						if (gegner.Treffer(rakete.MySchaden))
						{
							abfall_G.Add(gegner);


							score += 40;
							xp += 40;

							break;
						}

						abfall_R.Add(rakete);
					}
				}

				if (gegner.EnthaeltPunkt(raumschiff.MyKollision))
				{
					raumschiff.Damage(gegner.MyMass);
					abfall_G.Add(gegner);
				}
			}

			foreach (var item in gegnerTorpedoObjekte)
			{
				if (item.EnthaeltPunkt(raumschiff.MyKollision))
				{
					raumschiff.Damage(item.MySchaden);
					abfall_GT.Add(item);
				}
			}

			foreach (var item in powerUpObjekte)
			{
				if (item.EnthaeltPunkt(raumschiff.MyKollision))
				{
					if (item is XpKlein)
					{
						xp += 200;
					}
					else if (item is XpMittel)
					{
						xp += 300;
						raumschiff.StarteSchilde();
					}
					else if (item is XpHoch)
					{
						xp += 400;
						bomben += 3;
					}

					abfall_P.Add(item);
				}
			}

			foreach (var item in abfall_A)
			{
				asteroidObjekte.Remove(item);
			}
			foreach (var item in abfall_G)
			{
				gegnerObjekte.Remove(item);
			}
			foreach (var item in abfall_T)
			{
				torpedoObjekte.Remove(item);
			}
			foreach (var item in abfall_GT)
			{
				gegnerTorpedoObjekte.Remove(item);
			}
			foreach (var item in abfall_P)
			{
				powerUpObjekte.Remove(item);
			}
			foreach (var item in abfall_R)
			{
				raketeObjekte.Remove(item);
			}
		}

		void Zeichnen()
		{
			zeichenflaeche.Children.Clear();

			raumschiff.Rotieren(zeichenflaeche, zeichenflaeche.PointToScreen(Mouse.GetPosition(this)));
			raumschiff.Zeichne(zeichenflaeche);

			foreach (var item in asteroidObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			foreach (var item in gegnerObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			foreach (var item in powerUpObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			foreach (var item in raketeObjekte)
			{
				item.Zeichne(zeichenflaeche);
			}

			List<Torpedo> abfall_T = new List<Torpedo>();
			foreach (var item in torpedoObjekte)
			{
				if (item.Zeichne(zeichenflaeche))
				{
					abfall_T.Add(item);
				}
			}
			foreach (var item in abfall_T)
			{
				torpedoObjekte.Remove(item);
			}

			abfall_T = new List<Torpedo>();
			foreach (var item in gegnerTorpedoObjekte)
			{
				if (item.Zeichne(zeichenflaeche))
				{
					abfall_T.Add(item);
				}
			}
			foreach (var item in abfall_T)
			{
				gegnerTorpedoObjekte.Remove(item);
			}
		}

		void UpdateText()
		{
			textBlock_time.Text = zeit.ToString("0.##");
			textBlock_score.Text = score.ToString("0.");
			textBlock_health.Text = raumschiff.MyHP.ToString();
			prograssBar_health.Value = raumschiff.MyHP;
			textBlock_lebenStein.Text = (durchlaeufe / 4 + 20).ToString();
			textBlock_xp.Text = xp.ToString();
			textBlock_bomben.Text = bomben.ToString();
			textBlock_speed.Text = schiffGeschwindikeit.ToString();
			textBlock_dmg.Text = schiffSchaden.ToString();
			textBlock_upgrade.Text = schiffWaffen.ToString();
			textBlock_upgradePreis.Text = MyWaffenPreis.ToString();
			textBlock_dmgPreis.Text = MyDmgPreis.ToString();
			textBlock_speedPreis.Text = MySpeedPreis.ToString();
			textBlock_UmlenkungPreis.Text = "1000";
			textBlock_Umlenkung.Text = Convert.ToInt32(umlenkung).ToString();
			textBlock_Raketen.Text = schiffRaketen.ToString();
			textBlock_RaketenPreis.Text = MyRaketenPreis.ToString();


			textBlock1.Text = asteroidObjekte.Count().ToString();
			textBlock1_Copy.Text = torpedoObjekte.Count().ToString();
			textBlock1_Copy1.Text = powerUpObjekte.Count().ToString();
			textBlock1_Copy2.Text = raketeObjekte.Count().ToString();
		}

		void Pause()
		{
			spielPausiert = true;
			timer.Stop();
		}

		void Weiter()
		{
			spielPausiert = false;
			timer.Start();
		}

		void Stop()
		{
			zeichenflaeche.Children.Clear();
			asteroidObjekte.Clear();
			torpedoObjekte.Clear();
			powerUpObjekte.Clear();
			raketeObjekte.Clear();
			raumschiff = null;
			border.Background = Brushes.Red;
			button_start.IsEnabled = true;
			spielLaeuft = false;
			timer.Stop();
		}

		void Bombe(Point p)
		{
			for (int i = 0; i < 360; i += 10)
			{
				torpedoObjekte.Add(new Torpedo(p.X - 210, p.Y - 30, i, Color.FromArgb(255, 255, 0, 0), schiffSchaden * 10 + 50, 20, 1500, 3));
			}
		}

		Vector FindeZiel(double x, double y)
		{
			double naechstes;
			Point punktNaechstes = new Point(x, y);
			Point punkt = new Point(x, y);

			if (asteroidObjekte.Count > 0)
			{
				Point punkt2 = new Point(asteroidObjekte[0].MyX, asteroidObjekte[0].MyY);
				naechstes = Point.Subtract(punkt2, punkt).Length;

				foreach (var item in asteroidObjekte)
				{
					punkt2 = new Point(item.MyX, item.MyY);
					double abstand = Point.Subtract(punkt2, punkt).Length;

					if (abstand < naechstes)
					{
						punktNaechstes = punkt2;
						naechstes = abstand;
					}
				}
			}
			else
			{

			}

			Vector vector = Point.Subtract(punkt, punktNaechstes);
			return vector;
		}

		private void Button_start_Click(object sender, RoutedEventArgs e)
		{
			spielLaeuft = true;
			spielPausiert = false;
			zeit = 0;
			spawnZeit = 0;
			durchlaeufe = 1;
			level = 1;

			schiessen = false;
			schiessWarte = 0;
			score = 0;
			bomben = 3;

			xp = 0;
			schiffWaffen = 0;
			schiffGeschwindikeit = 600;
			schiffSchaden = 1;
			schiffRaketen = 0;
			umlenkung = false;

			button_start.IsEnabled = false;
			prograssBar_health.Value = 100;
			border.Background = Brushes.Black;
			raumschiff = new Raumschiff(zeichenflaeche);
			timer.Start();
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
						if (schiffWaffen < maxUpgrades)
						{
							schiffWaffen += 1;
						}
						break;
					case Key.Space:
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
			else if (e.Key == Key.Enter)
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
				Keyboard.Focus(zeichenflaeche);
			}
			else if (e.ChangedButton == MouseButton.Right && bomben > 0)
			{
				Bombe(Mouse.GetPosition(this));
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

		private void Button_speedPlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= MySpeedPreis)
			{
				xp -= MySpeedPreis;
				schiffGeschwindikeit += 40;
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
			if (xp >= MyWaffenPreis && schiffWaffen < maxUpgrades)
			{
				xp -= MyWaffenPreis;
				schiffWaffen++;
			}

			UpdateText();
		}

		private void Button_UmlenkungPlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= 1000 && umlenkung == false)
			{
				xp -= 1000;
				umlenkung = true;
			}

			UpdateText();
		}

		private void Button_RaketenPlus_Click(object sender, RoutedEventArgs e)
		{
			if (xp >= MyRaketenPreis && schiffRaketen < maxRaketen)
			{
				xp -= MyRaketenPreis;
				schiffRaketen++;
			}

			UpdateText();
		}
	}
}
