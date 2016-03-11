using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Android.App;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Timers;

namespace ProvaXamarin
{
	public partial class MyPage : ContentPage
	{
		public string Message { get; set;}
		TcpClient a=new TcpClient();
		string TCPip;
		TcpClient client;
		UDPer udp;
		public MyPage ()
		{
			InitializeComponent ();
			Console.WriteLine ("MyPage Loaded");
			label.Text = "Wait for Broadcast message";
			button.IsEnabled = false;
			udp = new UDPer ();
			udp.StartListening (onReceive);
		}

		public void OnClick(Object sender,EventArgs args){
			Console.WriteLine ("Clicked");
			label.Text = "Connecting...";
			if(TCPip!=null && TCPip.Length!=0){
				button.IsEnabled = false;
				a.BeginConnect (TCPip, UDPer.TCP_PORT_NUMBER, ServerUpdate, a);
				label.Text = "Connected!";

			}
		}

		public void OnVolumeClick(Object sender,EventArgs args){
			if (sender == volUp) {
				Console.WriteLine ("Send Volume Up");
				client.Client.Send(Encoding.ASCII.GetBytes("vol+"));
			} else if (sender == volDown) {
				Console.WriteLine ("Send Volume Down");
				client.Client.Send(Encoding.ASCII.GetBytes("vol-"));
			}
		}

		public void onReceive(IAsyncResult ar){
			Console.WriteLine ("Broadcast Message!");
			IPEndPoint ip = new IPEndPoint (IPAddress.Any, UDPer.PORT_NUMBER);
			((UdpClient)ar.AsyncState).EndReceive (ar, ref ip);
			label.Text = "Found PC at " + ip.Address.ToString ();
			udp.Send (ip.Address.ToString(),"I found you!");
			TCPip = ip.Address.ToString ();
			button.IsEnabled = true;
		}

		public void ServerUpdate(IAsyncResult ar){
			Console.WriteLine ("Sending Things!");
			client = ar.AsyncState as TcpClient;
			client.Client.Send (Encoding.ASCII.GetBytes ("Hello Server!"));
			StartListeningCPU ();
		}

		public void onMessageReceive(IAsyncResult ar){
			try{
				byte[] bytes = ar.AsyncState as byte[];
				int num=client.Client.EndReceive(ar);
				byte[] message = new byte[num];
				Buffer.BlockCopy (bytes, 0, message, 0, num);
				Console.WriteLine("Server Say "+Encoding.ASCII.GetString(message));
				Device.BeginInvokeOnMainThread(()=> label.Text = "CPU Load="+Encoding.ASCII.GetString(message));
				StartListeningCPU ();
			}catch{
				Console.WriteLine ("Exception!");
				Device.BeginInvokeOnMainThread(()=>label.Text = "Exception!");
				Timer delay = new Timer (10000);
				delay.Elapsed+= (sender, e) => udp.StartListening(_ => {
					Console.WriteLine("fake receive");
					udp.StartListening(onReceive);
				});
				delay.Enabled = true;
			}
		}

		public void StartListeningCPU(){
			byte[] buffer = new byte[30000];
			client.Client.BeginReceive (buffer, 0, 30000, SocketFlags.None, onMessageReceive, buffer);
		}
	}
}

