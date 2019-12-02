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

		List<SpielObjekt> spielObjekte = new List<SpielObjekt>();
		DispatcherTimer timer = new DispatcherTimer();
		Raumschiff raumschiff = null;
		static Random zufall = new Random();
		bool spielLaeuft = false;
		double zeit;
		int durchlaeufe;
		int durchlaeufe2;
		double score;
		bool schiessen;
		int upgrades;
		const int maxUpgrades = 5;
		bool alternieren;
		bool alternieren2;
		double spawnZeit;
		int bomben;

		public int xp;
		public int schiffGeschwindikeit = 400;
		public double schiffSchaden = 1;


		void Update(object sender, EventArgs e)
		{
			if (spielLaeuft == true)
			{
				zeit += 0.2 * 0.2;
				spawnZeit += 0.2 * 0.2;
				textBlock_time.Text = zeit.ToString("0.##");
				textBlock_score.Text = score.ToString("0.");
				textBlock_health.Text = raumschiff.MyHP.ToString();
				prograssBar_health.Value = raumschiff.MyHP;
				textBlock_lebenStein.Text = (durchlaeufe / 8 + 10).ToString();
				textBlock_upgrades.Text = upgrades.ToString();
				textBlock_xp.Text = xp.ToString();
				textBlock_bomben.Text = bomben.ToString();

				if (raumschiff.MyHP > 0 && durchlaeufe2 < 10)
				{
					raumschiff.Rotieren(zeichenflaeche, zeichenflaeche.PointToScreen(Mouse.GetPosition(this)));
					List<SpielObjekt> abfall = new List<SpielObjekt>();

					foreach (SpielObjekt spielObjekt in spielObjekte)
					{
						if (spielObjekt.Animiere(zeichenflaeche, timer.Interval) && spielObjekt is PhotonenTorpedo)
						{
							abfall.Add(spielObjekt);
						}
					}

					PruefeKollisionen(abfall);

					zeichenflaeche.Children.Clear();

					foreach (SpielObjekt spielObjekt in spielObjekte)
					{
						spielObjekt.Zeichne(zeichenflaeche);
					}
					if (spawnZeit + durchlaeufe / 100 > 0.6)
					{
						spawnZeit = 0;

						spielObjekte.Add(new Asteroid(zeichenflaeche, durchlaeufe));
					}

					if (zeit / durchlaeufe >= 5d)
					{
						durchlaeufe += 1;
						durchlaeufe2 += 1;

						if (raumschiff.MyHP < 97d)
						{
							raumschiff.MyHP += 3;
						}
						else
						{
							raumschiff.MyHP = 100;
						}

						foreach (var item in spielObjekte.OfType<PowerUp>())
						{
							item.
						}

						switch (zufall.Next(0, 6))
						{
							case 1:
								spielObjekte.Add(new WaffenUp(zeichenflaeche));
								break;
							case 2:
								spielObjekte.Add(new SchildUp(zeichenflaeche));
								break;
							case 3:
								spielObjekte.Add(new BombUp(zeichenflaeche));
								break;
							case 4:
								break;
							default:
								spielObjekte.Add(new WaffenUp(zeichenflaeche));
								break;
						}
					}

					if (schiessen)
					{
						Color fabe1 = Color.FromArgb(255, 0, 255, 255);
						Color fabe2 = Color.FromArgb(255, 0, 140, 255);
						Color fabe3 = Color.FromArgb(255, 0, 105, 180);
						Color fabe4 = Color.FromArgb(255, 128, 105, 180);
						Color fabe5 = Color.FromArgb(255, 255, 105, 180);

						switch (upgrades)
						{

							case 0:
								spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
								break;
							case 1:
								if (alternieren)
								{
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, -3, fabe2));
									alternieren = false;
								}
								else
								{
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 3, fabe2));
									alternieren = true;
								}
								break;
							case 2:
								if (alternieren)
								{
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 6, fabe3));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, -6, fabe3));
									alternieren = false;
								}
								else
								{
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, 3, fabe2));
									spielObjekte.Add(new PhotonenTorpedo(raumschiff, -3, fabe2));
									alternieren = true;
								}
								break;
							case 3:
								if (alternieren)
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -9, fabe4));
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 6, fabe3));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -6, fabe3));
									}
									alternieren = false;
								}
								else
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 9, fabe4));
										alternieren2 = false;
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 3, fabe2));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -3, fabe2));
										alternieren2 = true;
									}
									alternieren = true;
								}
								break;
							case 4:
								if (alternieren)
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -9, fabe4));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 12, fabe5));
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 6, fabe3));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -6, fabe3));
									}
									alternieren = false;
								}
								else
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 9, fabe4));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -12, fabe5));
										alternieren2 = false;
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 3, fabe2));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -3, fabe2));
										alternieren2 = true;
									}
									alternieren = true;
								}
								break;
							case 5:
								if (alternieren)
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -9, fabe4));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 12, fabe5));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 170, fabe4));
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1, 1.2));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 6, fabe3));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -6, fabe3));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 15, fabe1));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -15, fabe1));
									}
									alternieren = false;
								}
								else
								{
									if (alternieren2)
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1, 1.4));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 9, fabe4));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -12, fabe5));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -170, fabe4));
										alternieren2 = false;
									}
									else
									{
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 0, fabe1, 1.6));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 3, fabe2));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -3, fabe2));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, 12, fabe5));
										spielObjekte.Add(new PhotonenTorpedo(raumschiff, -12, fabe5));
										alternieren2 = true;
									}
									alternieren = true;
								}
								break;
							default:
								break;
						}
					}
				}
				else if (raumschiff.MyHP <= 0)
				{
					zeichenflaeche.Children.Clear();
					raumschiff = null;
					spielObjekte.Clear();
					border.Background = Brushes.Red;
					button_start.IsEnabled = true;
					spielLaeuft = false;
					timer.Stop();
				}
				else if (durchlaeufe2 >= 10)
				{
					Pause();
				}

			}
		}

		void Pause()
		{
			timer.Stop();
			Upgrades upgrades = new Upgrades(this);
			upgrades.Show();
		}

		public void Weiter()
		{
			timer.Start();
			durchlaeufe2 = 0;
			zeit = 0;
		}

		void PruefeKollisionen(List<SpielObjekt> abfall)
		{
			foreach (Asteroid asteroid in spielObjekte.OfType<Asteroid>())
			{
				foreach (PhotonenTorpedo photonenTorpedo in spielObjekte.OfType<PhotonenTorpedo>())
				{
					if (asteroid.EnthaeltPunkt(photonenTorpedo.MyX, photonenTorpedo.MyY))
					{
						if (asteroid.Treffer(photonenTorpedo.MySchaden))
						{
							abfall.Add(asteroid);
							score += 20;
							xp += 20;
						}
						abfall.Add(photonenTorpedo);


					}
				}
				foreach (Raumschiff raumschiff in spielObjekte.OfType<Raumschiff>())
				{
					if (asteroid.EnthaeltPunkt(raumschiff.MyX, raumschiff.MyY))
					{
						if (raumschiff.Damage())
						{
							abfall.Add(asteroid);

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
							abfall.Add(asteroid);
						}
					}
				}
			}

			foreach (PowerUp powerUp in spielObjekte.OfType<PowerUp>())
			{
				if (powerUp.EnthaeltPunkt(raumschiff.MyX, raumschiff.MyY))
				{
					if (powerUp is WaffenUp)
					{
						if (upgrades < maxUpgrades)
						{
							upgrades++;
						}
						else if (upgrades >= maxUpgrades)
						{

						}
					}
					else if (powerUp is SchildUp)
					{
						raumschiff.StarteSchilde();
					}
					else if (powerUp is BombUp)
					{
						bomben = 3;
					}

					abfall.Add(powerUp);
				}
			}

			foreach (SpielObjekt spielObjekt in abfall)
			{
				spielObjekte.Remove(spielObjekt);
			}
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
			textBlock_health.Text = "100";
			textBlock_score.Text = "000";
			border.Background = Brushes.Black;
			raumschiff = new Raumschiff(zeichenflaeche);
			spielObjekte.Add(raumschiff);
			timer.Start();

			for (int i = 0; i < 4; i++)
			{
				spielObjekte.Add(new Asteroid(zeichenflaeche, durchlaeufe));
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
					case Key.F:
						if (upgrades < maxUpgrades)
						{
							upgrades += 1;
						}
						break;
					case Key.R:
						Pause();
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
			schiessen = false;
		}

		private void Bombe()
		{
			Point p = Mouse.GetPosition(this);

			for (int i = 0; i < 360; i += 10)
			{
				spielObjekte.Add(new PhotonenTorpedo(p.X - 210, p.Y - 30, i, Color.FromArgb(255, 255, 0, 0)));
			}
		}
	}
}
