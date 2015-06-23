﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WPC
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MusicCollection : Page
	{
		public MusicCollection()
		{
			this.InitializeComponent();
		}

		private void lvLibrary_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}

		private async void SyncLibrary_Click(object sender, RoutedEventArgs e)
		{
			if(await AcceptSync())
			{
				ObservableCollection<Song> library = await GetLibraryCollection();

				lvLibrary.ItemsSource = library;
			}
		}

		private async Task<bool> AcceptSync()
		{
			bool accept = false;

			MessageDialog md = new MessageDialog("Syncing library is a long process. Do you want to continue?");
			md.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(x => accept = true)));
			md.Commands.Add(new UICommand("No", new UICommandInvokedHandler(x => accept = false)));

			await md.ShowAsync();

			return accept;
		}

		private async Task<ObservableCollection<Song>> GetLibraryCollection()
		{
			SocketClient client = new SocketClient();
			string response = await client.Command("listall");

			ObservableCollection<Song> library = await FormatLibrary(response);

			return library;
		}

		private async Task<ObservableCollection<Song>> FormatLibrary(string value)
		{
			ObservableCollection<Song> library = new ObservableCollection<Song>();

			List<string> values = new List<string>(value.Split('\n'));

			foreach(string v in values)
			{
				if(v.Contains("file"))
				{
					Song s = await GetSongDetails(v.Substring(6));
					library.Add(s);
				}
			}

			

			return library;
		}

		private async Task<Song> GetSongDetails(string file)
		{
			SocketClient client = new SocketClient();
			string response = await client.Command(string.Format(@"find file ""{0}""", file));

			string[] details = response.Split('\n');

			Song s = new Song();

			foreach(string t in details)
			{
				if(t.Contains("file"))
				{
					s.Filename = t.Substring("file: ".Length);
				}
				else if (t.Contains("Time")) 
				{
					s.Time = long.Parse(t.Substring("Time: ".Length));
				}
				else if (t.Contains("Artist")) 
				{
					s.Artist = t.Substring("Artist: ".Length);
				}
				else if (t.Contains("Title")) 
				{
					s.Title = t.Substring("Title: ".Length);
				}
				else if (t.Contains("Album")) 
				{
					s.Album = t.Substring("Album: ".Length);
				}
				else if (t.Contains("Track")) 
				{
					s.Track = t.Substring("Track: ".Length);
				}
				else if (t.Contains("Date")) 
				{
					s.Date = t.Substring("Date: ".Length);
				}
				else if (t.Contains("Genre")) 
				{
					s.Genre = t.Substring("Genre: ".Length);
				}
			}

			return s;
		}
	}
}
