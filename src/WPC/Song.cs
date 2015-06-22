using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPC
{
	public class Song
	{
		public long ID { get; set; }
		public string Title { get; set; }

		public string Artist { get; set; }

		public string Album { get; set; }

		public long Time { get; set; }

		public string Filename { get; set; }

		public Song()
		{ }

		public Song(string _ID, string _Title, string _Artist)
		{
			ID = Int64.Parse(_ID);
			Title = _Title;
			Artist = _Artist;
		}
	}
}
