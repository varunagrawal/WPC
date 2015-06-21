using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WPC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		string MPD_IP = State.IP;
		string MPD_PORT = State.Port;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
			if(MPD_IP == null)
			{
				this.Frame.Navigate(typeof(Settings));
			}

			bool except = false;
				
			try
			{
				await MusicPD.GetStatus(MPD_IP, MPD_PORT);
			}
			catch(Exception)
			{
				except = true;
			}

			if(except)
			{
				MessageDialog md = new MessageDialog("Error! Please make sure MPD is running and IP is specified.");
				await md.ShowAsync();
				Frame.Navigate(typeof(Settings));
			}
			
			VolumeSlider.Value = Double.Parse(MusicPD.Volume.ToString());
			
        }

		private async Task<string> SendCommand(string command)
		{
			// Clear the log 
			ClearLog();

			// Make sure we can perform this action with valid data
			if (ValidateRemoteHost())
			{
				// Instantiate the SocketClient object
				SocketClient client = new SocketClient();

				// Attempt connection to the MPD server
				//Log(String.Format("Connecting to server '{0}' over port {1} ...", MPD_IP, MPD_PORT), true);
				await client.Connect(MPD_IP, MPD_PORT);
				//Log(result, false);

				// Attempt to send command to MPD
				//Log(String.Format("Sending '{0}' to server ...", command), true);
				bool result = await client.Send(command + "\n");
				//Log("Message Sent", false);

				// Receive response from the MPD server
				//Log("Requesting Receive ...", true);
				string response = await client.Receive();
				//Log(response, false);

				// Close the socket conenction explicitly
				client.Close();

				return response;
			}

			return "MPD Server IP Missing";
		}

		#region Playback Controls
		private async void btnPlaySong_Click(object sender, RoutedEventArgs e)
		{
			await SendCommand("play");
			txtOutput.Text = await SendCommand("currentsong");
		}

		private async void btnStopSong_Click(object sender, RoutedEventArgs e)
		{
			await SendCommand("stop");
		}

		private async void btnPauseSong_Click(object sender, RoutedEventArgs e)
		{
			await SendCommand("pause");
		}

		private async void btnBackSong_Click(object sender, RoutedEventArgs e)
		{
			await SendCommand("previous");
			txtOutput.Text = await SendCommand("currentsong");
		}

		private async void btnNextSong_Click(object sender, RoutedEventArgs e)
		{
			await SendCommand("next");
			txtOutput.Text = await SendCommand("currentsong");
		}

		private async void btnShuffle_Click(object sender, RoutedEventArgs e)
		{
			await MusicPD.GetStatus(MPD_IP, MPD_PORT);

			if (MusicPD.Random == true)
			{
				await SendCommand("random 0");
			}
			else
			{
				await SendCommand("random 1");
			}
		}

		private async void btnRepeat_Click(object sender, RoutedEventArgs e)
		{
			await MusicPD.GetStatus(MPD_IP, MPD_PORT);

			if (MusicPD.Repeat == true)
			{
				await SendCommand("repeat 0");
			}
			else
			{
				await SendCommand("repeat 1");
			}
		}
		#endregion

		#region UI Validation
		/// <summary>
		/// Validates the txtRemoteHost TextBox
		/// </summary>
		/// <returns>True if the txtRemoteHost contains valid data,
		/// otherwise False
		/// </returns>
		private bool ValidateRemoteHost()
		{
			return true;
		}
		#endregion

		#region Logging
		/// <summary>
		/// Log text to the txtOutput TextBox
		/// </summary>
		/// <param name="message">The message to write to the txtOutput TextBox</param>
		/// <param name="isOutgoing">True if the message is an outgoing (client to server)
		/// message, False otherwise.
		/// </param>
		/// <remarks>We differentiate between a message from the client and server 
		/// by prepending each line  with ">>" and "<<" respectively.</remarks>
		private void Log(string message, bool isOutgoing)
		{
			string direction = (isOutgoing) ? ">> " : "<< ";
			txtOutput.Text += Environment.NewLine + direction + message;
		}

		/// <summary>
		/// Clears the txtOutput TextBox
		/// </summary>
		private void ClearLog()
		{
			txtOutput.Text = String.Empty;
		}
		#endregion

		
		private void Playlist_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(Playlist));
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(Settings));
		}

		private async void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			var Volume = e.NewValue;
			await SendCommand(string.Format("setvol {0}", Volume));
		}

		private void Collection_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(MusicCollection));
		}

    }
}
