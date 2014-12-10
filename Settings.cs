using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPC
{
	public class State
	{
		public static string IP
		{
			get 
			{
				string val = ReadSetting("IP");
				if (string.IsNullOrEmpty(val))
					return "127.0.0.1";
				else
					return val;
			}
			set
			{
				WriteSetting("IP", value);
			}
		}

		public static string Port
		{
			get
			{
				string val = ReadSetting("Port");
				if (string.IsNullOrEmpty(val))
					return "6600"; // The Media Player Daemon (MPD) protocol uses port 6600
				else
					return val;
			}
			set
			{
				WriteSetting("Port", value);
			}
		}

		private static string ReadSetting(string key)
		{
			var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

			if (settings.Values.ContainsKey(key))
				return (string)settings.Values[key];
			else
				return string.Empty;
		}

		private static void WriteSetting(string key, string value)
		{
			var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			settings.Values.Add(key, value);
		}
	}
}
