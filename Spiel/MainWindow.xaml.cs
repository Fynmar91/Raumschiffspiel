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

		DispatcherTimer timer = new DispatcherTimer();
		Raumschiff raumschiff = null;
		List<SpielObjekt> spielObjekte = new List<SpielObjekt>();
		bool spielLaeuft = false;
		double zeit;
		int durchlaeufe;
		int leben;
		double score;
		bool schiessen;
		int upgrades;
		const int maxUpgrades = 5; 
		int xp;
		bool alternieren;
		bool alternieren2;
		double spawnZeit;


		void Update(object sender, EventArgs e)
		{
			if (spielLaeuft == true)
			{
				zeit += 0.2 * 0.2;
				spawnZeit += 0.2 * 0.2;
				textBlock_time.Text = zeit.ToString("0.##");
				textBlock_score.Text = score.ToString("0.");
				textBlock_health.Text = leben.ToString();
				prograssBar_health.Value = leben;
				textBlock_lebenStein.Text = (durchlaeufe / 8 + 10).ToString();
				textBlock_upgrades.Text = upgrades.ToString();
				textBlock_xp.Text = xp.ToString();

				if (leben > 0)
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

						if (leben < 97d)
						{
							leben += 3;
						}
						else
						{
							leben = 100;
						}

						if (xp >= 400 && upgrades < maxUpgrades)
						{
							upgrades++;
							xp = 0;
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
				else
				{
					zeichenflaeche.Children.Clear();
					raumschiff = null;
					spielObjekte.Clear();
					border.Background = Brushes.Red;
					button_start.IsEnabled = true;
					spielLaeuft = false;
					timer.Stop();
				}
				
			}
		}

		void PruefeKollisionen(List<SpielObjekt> abfall)
		{
			foreach (Asteroid asteroid in spielObjekte.OfType<Asteroid>())
			{
				foreach (PhotonenTorpedo photonenTorpedo in spielObjekte.OfType<PhotonenTorpedo>())
				{
					if (asteroid.EnthaeltPunkt(photonenTorpedo.MyX, photonenTorpedo.MyY))
					{
						if (asteroid.Damage())
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
						leben -= 33;
						abfall.Add(asteroid);
						if (upgrades > 1)
						{
							upgrades -= 2;
						}
						else if (upgrades > 0)
						{
							upgrades--;
						}
						raumschiff.MyX = 0.5d * zeichenflaeche.ActualWidth;
						raumschiff.MyY = 0.5d * zeichenflaeche.ActualHeight;
					}
				}
			}
			foreach (SpielObjekt spielObjekt in abfall)
			{
				spielObjekte.Remove(spielObjekt);
			}
		}

		void Auserhalb()
		{
			List<SpielObjekt> auserhalb = new List<SpielObjekt>();

			foreach (PhotonenTorpedo photonenTorpedo in spielObjekte.OfType<PhotonenTorpedo>())
			{
				if (photonenTorpedo.MyX < 0d)
				{
					auserhalb.Add(photonenTorpedo);
				}
				else if (photonenTorpedo.MyX > zeichenflaeche.ActualWidth)
				{
					auserhalb.Add(photonenTorpedo);
				}

				if (!auserhalb.Contains(photonenTorpedo))
				{
					if (photonenTorpedo.MyY < 0d)
					{
						auserhalb.Add(photonenTorpedo);
					}
					else if (photonenTorpedo.MyY > zeichenflaeche.ActualHeight)
					{
						auserhalb.Add(photonenTorpedo);
					}
				}
			}
			foreach (SpielObjekt spielObjekt in auserhalb)
			{
				spielObjekte.Remove(spielObjekt);
			}
		}

		private void Button_start_Click(object sender, RoutedEventArgs e)
		{
			spielLaeuft = true;
			button_start.IsEnabled = false;
			zeit = 0;
			leben = 100;
			score = 0;
			durchlaeufe = 1;
			schiessen = false;
			upgrades = 0;
			xp = 0;
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
						raumschiff.MyYvel = -400d;
						break;
					case Key.S:
						raumschiff.MyYvel = 400d;
						break;
					case Key.A:
						raumschiff.MyXvel = -400d;
						break;
					case Key.D:
						raumschiff.MyXvel = 400d;
						break;
					case Key.F:
						if (upgrades < maxUpgrades)
						{
							upgrades += 1;
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
			schiessen = true;
		}

		private void Window_MouseUp(object sender, MouseButtonEventArgs e)
		{
			schiessen = false;
		}
	}
}
