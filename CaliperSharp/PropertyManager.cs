/*
MIT License

Copyright (c) 2016 Kent Randall

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Linq;

namespace Point85.Caliper.UnitOfMeasure
{
	/// <summary>
	/// Class for reading a java-style .properties file.
	/// </summary>
	public class PropertyManager
	{
		private Dictionary<string, string> Properties;

		private string Filename;

		/// <summary>
		/// Construct a manager for this file
		/// </summary>
		/// <param name="fileName">Name of .properties file</param>
		public PropertyManager(string fileName)
		{
			Reload(fileName);
		}

		/// <summary>
		/// Get the string for the key field
		/// </summary>
		/// <param name="field">Key value</param>
		/// <param name="defValue">Default value if not found</param>
		/// <returns>Value</returns>
		public string GetString(string field, string defValue)
		{
			return (GetString(field) == null) ? (defValue) : (GetString(field));
		}

		/// <summary>
		/// Get the string for the key field
		/// </summary>
		/// <param name="field">Key value</param>
		/// <returns>Value</returns>
		public string GetString(string field)
		{
			return (Properties.ContainsKey(field)) ? (Properties[field]) : (null);
		}

		/// <summary>
		/// Set the field value
		/// </summary>
		/// <param name="field">Key</param>
		/// <param name="value">Value</param>
		public void SetString(string field, string value)
		{
			field = this.TrimUnwantedChars(field);
			value = this.TrimUnwantedChars(value);

			if (!Properties.ContainsKey(field))
				Properties.Add(field, value.ToString());
			else
				Properties[field] = value.ToString();
		}

		private string TrimUnwantedChars(string toTrim)
		{
			toTrim = toTrim.Replace(";", string.Empty);
			toTrim = toTrim.Replace("#", string.Empty);
			toTrim = toTrim.Replace("'", string.Empty);
			toTrim = toTrim.Replace("=", string.Empty);
			return toTrim;
		}

		/// <summary>
		/// Save properties to the original file
		/// </summary>
		public void Save()
		{
			Save(this.Filename);
		}

		/// <summary>
		/// Save properties to the specified file
		/// </summary>
		/// <param name="Filename">Name of .properties file</param>
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

		/// <summary>
		/// Re-read the original .properties file
		/// </summary>
		public void Reload()
		{
			Reload(this.Filename);
		}

		/// <summary>
		/// Re-read from the specfied file
		/// </summary>
		/// <param name="Filename">Name of .properties file</param>
		public void Reload(string Filename)
		{
			this.Filename = Filename;
			Properties = new Dictionary<string, string>();

			if (System.IO.File.Exists(Filename))
				LoadFromFile(Filename);
			else
				System.IO.File.Create(Filename);
		}

		private void LoadFromFile(string file)
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
