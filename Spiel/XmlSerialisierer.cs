using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spiel
{
	public class XmlSerialisierer
	{
		//string sPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		public void Serialisieren(HighscoreListe ml)
		{
			var xml = new XmlSerializer(typeof(HighscoreListe));
			Directory.CreateDirectory(@"C:\Raumschiffspiel");
			var stream = new FileStream(@"C:\Raumschiffspiel\highscore.xml", FileMode.OpenOrCreate);
			xml.Serialize(stream, ml);
			stream.Close();
		}

		public HighscoreListe Deserialisieren()
		{
			HighscoreListe fromFile = new HighscoreListe();
			FileStream stream = null;
			var xml = new XmlSerializer(typeof(HighscoreListe));

			if (File.Exists(@"C:\Raumschiffspiel\highscore.xml"))
			{
				stream = new FileStream(@"C:\Raumschiffspiel\highscore.xml", FileMode.Open);
				fromFile = xml.Deserialize(stream) as HighscoreListe;
				stream.Close();
				return fromFile;
			}
			return fromFile;
		}
	}
}
