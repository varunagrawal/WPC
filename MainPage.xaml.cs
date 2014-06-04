using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WinMPC.Resources;

namespace WinMPC
{
    public partial class MainPage : PhoneApplicationPage
    {
        string MPD_IP = "127.0.0.1"; 
        int MPD_PORT = 6600; // The Media Player Daemon (MPD) protocol uses port 6600

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// </summary>
        private string SendCommand(string command)
        {
            // Clear the log 
            ClearLog();

            // Make sure we can perform this action with valid data
            if (ValidateRemoteHost())
            {
                MPD_IP = txtRemoteHost.Text;

                // Instantiate the SocketClient object
                SocketClient client = new SocketClient();

                // Attempt connection to the MPD server
                Log(String.Format("Connecting to server '{0}' over port {1} ...", txtRemoteHost.Text, MPD_PORT), true);
                string result = client.Connect(MPD_IP, MPD_PORT);
                Log(result, false);

                // Attempt to send command to MPD
                Log(String.Format("Sending '{0}' to server ...", command), true);
                result = client.Send(command + "\n");
                Log(result, false);

                // Receive response from the MPD server
                Log("Requesting Receive ...", true);
                result = client.Receive();
                Log(result, false);

                // Close the socket conenction explicitly
                client.Close();

                return result;
            }

            return "MPD Server IP Missing";
        }


        #region Playback Controls
        private void btnPlaySong_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("play");
        }

        private void btnStopSong_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("stop");
        }

        private void btnPauseSong_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("pause");
        }

        private void btnBackSong_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("previous");
        }

        private void btnNextSong_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("next");
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            MPD_IP = txtRemoteHost.Text;

            MusicPD.GetStatus(MPD_IP, MPD_PORT);

            if (MusicPD.Random == true)
            {
                SendCommand("random 0");
            }
            else
            {
                SendCommand("random 1");
            }
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            MPD_IP = txtRemoteHost.Text;

            MusicPD.GetStatus(MPD_IP, MPD_PORT);

            if (MusicPD.Repeat == true)
            {
                SendCommand("repeat 0");
            }
            else
            {
                SendCommand("repeat 1");
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
            // The txtRemoteHost must contain some text
            if (String.IsNullOrWhiteSpace(txtRemoteHost.Text))
            {
                MessageBox.Show("Please enter a host name");
                return false;
            }

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

        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            MPD_IP = txtRemoteHost.Text;
        }

        

    }
}