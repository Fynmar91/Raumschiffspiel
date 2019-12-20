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
		string pfad = Environment.ExpandEnvironmentVariables("%AppData%\\Raumschiffspiel\\");
		string datei = Environment.ExpandEnvironmentVariables("%AppData%\\Raumschiffspiel\\highscore.xml");

		public void Serialisieren(HighscoreListe ml)
		{
			var xml = new XmlSerializer(typeof(HighscoreListe));
			Directory.CreateDirectory(pfad);
			var stream = new FileStream(datei, FileMode.OpenOrCreate);
			xml.Serialize(stream, ml);
			stream.Close();
		}

		public HighscoreListe Deserialisieren()
		{
			HighscoreListe fromFile = new HighscoreListe();
			FileStream stream = null;
			var xml = new XmlSerializer(typeof(HighscoreListe));

			if (File.Exists(datei))
			{
				stream = new FileStream(datei, FileMode.Open);
				fromFile = xml.Deserialize(stream) as HighscoreListe;
				stream.Close();
				return fromFile;
			}
			return fromFile;
		}
	}
}
