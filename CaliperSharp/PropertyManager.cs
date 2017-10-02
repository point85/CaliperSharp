using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.point85.uom
{
	public class PropertyManager
	{
		private Dictionary<string, string> Properties;

		private string Filename;

		public PropertyManager(string fileName)
		{
			Reload(fileName);
		}

		public string GetString(string field, string defValue)
		{
			return (GetString(field) == null) ? (defValue) : (GetString(field));
		}
		public string GetString(string field)
		{
			return (Properties.ContainsKey(field)) ? (Properties[field]) : (null);
		}

		public void SetString(string field, string value)
		{
			field = this.TrimUnwantedChars(field);
			value = this.TrimUnwantedChars(value);

			if (!Properties.ContainsKey(field))
				Properties.Add(field, value.ToString());
			else
				Properties[field] = value.ToString();
		}

		public string TrimUnwantedChars(string toTrim)
		{
			toTrim = toTrim.Replace(";", string.Empty);
			toTrim = toTrim.Replace("#", string.Empty);
			toTrim = toTrim.Replace("'", string.Empty);
			toTrim = toTrim.Replace("=", string.Empty);
			return toTrim;
		}

		public void Save()
		{
			Save(this.Filename);
		}

		public void Save(string Filename)
		{
			this.Filename = Filename;

			if (!System.IO.File.Exists(Filename))
				System.IO.File.Create(Filename);

			System.IO.StreamWriter file = new System.IO.StreamWriter(Filename);

			foreach (string prop in Properties.Keys.ToArray())
				if (!string.IsNullOrWhiteSpace(Properties[prop]))
					file.WriteLine(prop + "=" + Properties[prop]);

			file.Close();
		}

		public void Reload()
		{
			Reload(this.Filename);
		}

		public void Reload(string Filename)
		{
			this.Filename = Filename;
			Properties = new Dictionary<string, string>();

			if (System.IO.File.Exists(Filename))
				loadFromFile(Filename);
			else
				System.IO.File.Create(Filename);
		}

		private void loadFromFile(string file)
		{
			foreach (string line in System.IO.File.ReadAllLines(file))
			{
				if ((!string.IsNullOrEmpty(line)) &&
					(!line.StartsWith(";")) &&
					(!line.StartsWith("#")) &&
					(!line.StartsWith("'")) &&
					(line.Contains('=')))
				{
					int index = line.IndexOf('=');
					string key = line.Substring(0, index).Trim();
					string value = line.Substring(index + 1).Trim();

					if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
						(value.StartsWith("'") && value.EndsWith("'")))
					{
						value = value.Substring(1, value.Length - 2);
					}

					try
					{
						//ignore duplicates
						Properties.Add(key, value);
					}
					catch { }
				}
			}
		}
	}
}
