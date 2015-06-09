using System;
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
				ObservableCollection<Song> library = GetLibraryCollection();

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

		private ObservableCollection<Song> GetLibraryCollection()
		{
			ObservableCollection<Song> library = new ObservableCollection<Song>();

			return library;
		}
	}
}
