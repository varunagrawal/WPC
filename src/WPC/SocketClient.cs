using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WPC
{
	public class SocketClient
	{
		// Cached Socket object that will be used by each call for the lifetime of this class
		StreamSocket _socket = null;

		// Flag to indicate if Socket is connected
		bool Connected = false;

		// Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
		// timeout period, the call is aborted.
		const int TIMEOUT_MILLISECONDS = 5000;

		// The maximum size of the data buffer to use with the asynchronous socket methods
		const int MAX_BUFFER_SIZE = 2048;

		/// <summary>
		/// Attempt a TCP socket connection to the given host over the given port
		/// </summary>
		/// <param name="hostName">The name of the host</param>
		/// <param name="portNumber">The port number to connect</param>
		/// <returns>A string representing the result of this connection attempt</returns>
		public async Task<bool> Connect(string hostName, string portNumber)
		{
			string result = string.Empty;

			// Create a remote Host. The hostName is passed in to this method.
			HostName remoteHost = new HostName(hostName);

			// Create a stream-based, TCP socket using the InterNetwork Address Family. 
			_socket = new StreamSocket();

			try
			{
				// Make an asynchronous Connect request over the socket
				await _socket.ConnectAsync(remoteHost, portNumber);

				Connected = true;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			return Connected;
		}

		/// <summary>
		/// Send the given data to the server using the established connection
		/// </summary>
		/// <param name="data">The data to send to the server</param>
		/// <returns>The result of the Send request</returns>
		public async Task<bool> Send(string data)
		{
			// We are re-using the _socket object initialized in the Connect method
			if (Connected)
			{
				try
				{
					data = data + Environment.NewLine;
					DataWriter writer = new DataWriter(_socket.OutputStream);
					UInt32 len = writer.MeasureString(data);
					writer.WriteString(data);

					await writer.StoreAsync();

					writer.DetachStream();
					writer.Dispose();

					return true;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);

					_socket.Dispose();
					Connected = false;

					return false;
				}
				
			}
			else
			{
				return false;
			}

			
		}

		/// <summary>
		/// Receive data from the server using the established socket connection
		/// </summary>
		/// <returns>The data received from the server</returns>
		public async Task<string> Receive()
		{
			string response = string.Empty;

			// We are receiving over an established socket connection
			if (Connected)
			{
				try
				{
					DataReader reader = new DataReader(_socket.InputStream);
					reader.InputStreamOptions = InputStreamOptions.Partial;

					while(true)
					{
						await reader.LoadAsync(1024);

						if(reader.UnconsumedBufferLength < 1024)
						{
							response += reader.ReadString(reader.UnconsumedBufferLength);
							break;
						}

						response += reader.ReadString(reader.UnconsumedBufferLength);
					}
				
					return response;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
					Connected = false;
					return null;
				}
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Closes the Socket connection and releases all associated resources
		/// </summary>
		public void Close()
		{
			if (Connected && _socket != null)
			{
				_socket.Dispose();
			}
		}

		public async Task<string> Command(string command)
		{
			await Connect(State.IP, State.Port);
			await Send(command);
			string response = await Receive();

			return response;
		}
	}
}
