using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace ProvaXamarin
{
	public class UDPer
	{
		public const int PORT_NUMBER = 15000;
		public const int TCP_PORT_NUMBER = 15010;
		private readonly UdpClient udp = new UdpClient(PORT_NUMBER);
		public UDPer ()
		{			
		}

		public void Stop()
		{
			try
			{
				udp.Close();
				Console.WriteLine("Stopped listening");
			}
			catch { /* don't care */ }
		}
			
		public void StartListening(AsyncCallback callback)
		{
			Console.WriteLine("Started listening");
			udp.BeginReceive((ar) => Device.BeginInvokeOnMainThread(() => callback(ar)), udp);
		}

		public void Send(string serverip,string message)
		{
			UdpClient client = new UdpClient();
			IPEndPoint ip = new IPEndPoint(IPAddress.Parse(serverip), PORT_NUMBER);
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			client.Send(bytes, bytes.Length, ip);
			client.Close();
			Console.WriteLine("[UDP] Sent: {0} ", message);
		}
	}
}

