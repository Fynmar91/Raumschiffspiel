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
		string sPath = @"C:";

		public void Serialisieren(HighscoreListe ml, string path)
		{
			var xml = new XmlSerializer(typeof(HighscoreListe));
			Directory.CreateDirectory(path + @"\Raumschiffspiel");
			var stream = new FileStream(sPath + @"\Raumschiffspiel\highscore.xml", FileMode.Create);
			xml.Serialize(stream, ml);
			stream.Close();
		}

		public HighscoreListe Deserialisieren(string path)
		{
			HighscoreListe fromFile = new HighscoreListe();
			FileStream stream = null;
			var xml = new XmlSerializer(typeof(HighscoreListe));

			if (File.Exists(sPath + @"\Raumschiffspiel\highscore.xml"))
			{
				stream = new FileStream(sPath + @"\Raumschiffspiel\highscore.xml", FileMode.Open);
				fromFile = xml.Deserialize(stream) as HighscoreListe;
				stream.Close();
				return fromFile;
			}
			return fromFile;
		}
	}
}
