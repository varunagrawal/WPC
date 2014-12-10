using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WPC
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Playlist : Page
	{
		public Playlist()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			SocketClient client = new SocketClient();

			await client.Connect(State.IP, State.Port);
			await client.Send("playlistinfo");
			string response = await client.Receive();

			FormatPlaylistInfo(response);
		}

		public ObservableCollection<Song> FormatPlaylistInfo(string value)
		{
			ObservableCollection<Song> playlist = new ObservableCollection<Song>();

			value = value.Substring("OK MPD 0.17.0\n".Length);

			while (true)
			{
				int last = value.IndexOf("\nfile");
				string singlesong;
				bool lastSong = false;

				if (last >= 0)
				{
					singlesong = value.Substring(0, last);
					value = value.Substring(last + 1);
				}
				else
				{
					singlesong = value.Substring(0);
					lastSong = true;

				}

				string[] raw = singlesong.Split('\n');
				
				Song s = new Song();

				for (int i = 0; i < raw.Length; i++)
				{
					string[] keyVal = raw[i].Split(':');

					string property;
					string val;

					if (keyVal.Length > 1)
					{
						property = raw[i].Split(':')[0];
						val = raw[i].Split(':')[1].Trim();
					}
					else
						continue;
					
					switch (property)
					{
						case "Time":
							s.Time = Int64.Parse(val);
							break;
						case "Artist":
							s.Artist = val;
							break;
						case "Title":
							s.Title = val;
							break;
						case "Album":
							s.Album = val;
							break;
						case "Id":
							s.ID = Int64.Parse(val);
							playlist.Add(s);
							break;
					}
				}

				if (lastSong)
					break;
			
			}

			
			return playlist;
		}
	}
}
