using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPC
{
	class MusicPD
	{
		public enum PlaybackState
		{
			Play,
			Pause,
			Stop
		};

		public static int Volume { get; set; }
		public static bool Repeat { get; set; }
		public static bool Random { get; set; }    // A.k.a. Shuffle
		public static bool Single { get; set; }
		public static bool Consume { get; set; }
		public static int Playlist { get; set; }
		public static int PlaylistLength { get; set; }
		public static int Xfade { get; set; }
		public static double MixRampdB { get; set; }
		public static double MixRampDelay { get; set; }
		public static PlaybackState State { get; set; }
		public static int Song { get; set; }
		public static int SongId { get; set; }
		public static int PlayTime { get; set; }
		public static int TotalTime { get; set; }
		public static double Elapsed { get; set; }
		public static int BitRate { get; set; }
		public static int AudioSampleRate { get; set; }
		public static int AudioBits { get; set; }
		public static int AudioChannel { get; set; }
		public static int NextSong { get; set; }
		public static int NextSongId { get; set; }

		public static async Task<string> GetStatus(string host, string port)
		{
			string result = string.Empty;

			try
			{
				// Instantiate the SocketClient object
				SocketClient client = new SocketClient();

				// Attempt connection to the MPD server
				bool connecttion = await client.Connect(host, port);

				// Attempt to send command to MPD
				bool sent = await client.Send("status\n");

				// Receive response from the MPD server
				string response = await client.Receive();
				string[] data = response.Split('\n');

				SetStatus(data);

				// Close the socket conenction explicitly
				client.Close();

				return result;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Set the MPD status on the client, such as volume, repeat, shuffle etc.
		/// </summary>
		/// <param name="data"></param>
		public static void SetStatus(string[] data)
		{
			foreach (string str in data)
			{
				string[] s = str.Split(new string[] { ": " }, StringSplitOptions.None);

				if (s[0] == "volume")
				{
					Volume = Int32.Parse(s[1].Trim());
				}
				else if (s[0] == "repeat")
				{
					Repeat = (s[1] == "1" ? true : false);
				}
				else if (s[0] == "random")
				{
					Random = (s[1] == "1" ? true : false);
				}
				else if (s[0] == "single")
				{
					Single = (s[1] == "1" ? true : false);
				}
				else if (s[0] == "consume")
				{
					Consume = (s[1] == "1" ? true : false);
				}
				else if (s[0] == "playlist")
				{
					Playlist = Int32.Parse(s[1]);
				}
				else if (s[0] == "playlistlength")
				{
					PlaylistLength = Int32.Parse(s[1]);
				}
				else if (s[0] == "xfade")
				{
					int Xfade = Int32.Parse(s[1]);
				}
				else if (s[0] == "mixrampdb")
				{
					MixRampdB = Double.Parse(s[1]);
				}
				else if (s[0] == "mixrampdelay")
				{
					MixRampDelay = Double.Parse(s[1]);
				}
				else if (s[0] == "state")
				{
					if (s[1] == "play")
						State = PlaybackState.Play;
					else if (s[1] == "pause")
						State = PlaybackState.Pause;
					else if (s[1] == "stop")
						State = PlaybackState.Stop;

				}
				else if (s[0] == "song")
				{
					Song = Int32.Parse(s[1]);
				}
				else if (s[0] == "songid")
				{
					SongId = Int32.Parse(s[1]);
				}
				else if (s[0] == "time")
				{
					string[] t = s[1].Split(':');
					PlayTime = Int32.Parse(t[0]);
					TotalTime = Int32.Parse(t[1]);
				}
				else if (s[0] == "elapsed")
				{
					Elapsed = Double.Parse(s[1]);
				}
				else if (s[0] == "bitrate")
				{
					BitRate = Int32.Parse(s[1]);
				}
				else if (s[0] == "audio")
				{
					string[] audio = s[1].Split(':');
					AudioSampleRate = Int32.Parse(audio[0]);
					AudioBits = Int32.Parse(audio[1]);
					AudioChannel = Int32.Parse(audio[2]);
				}
				else if (s[0] == "nextsong")
				{
					NextSong = Int32.Parse(s[1]);
				}
				else if (s[0] == "nextsongid")
				{
					NextSongId = Int32.Parse(s[1]);
				}

			}
		}
	}
}
