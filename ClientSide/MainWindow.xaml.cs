using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClientSide;

public partial class MainWindow : Window
{
	UdpClient client;
	IPEndPoint remoteEP;

	List<byte> list = new List<byte>();


	public MainWindow()
	{
		InitializeComponent();

		client = new UdpClient();
		remoteEP = new(IPAddress.Parse("127.0.0.1"), 45678);
	}


	private async void Button_Click(object sender, RoutedEventArgs e)
	{
		var size = ushort.MaxValue - 29;
		byte[] buffer = new byte[size];

		await client.SendAsync(buffer, buffer.Length, remoteEP);

		int lengthSize = buffer.Length;

		int length = 0;
		while (true)
		{
			do
			{
				try
				{
					var receiveResult = await client.ReceiveAsync();
					buffer = receiveResult.Buffer;
					length = buffer.Length;
					list.AddRange(buffer);
				}
				catch (System.Exception)
				{
					MessageBox.Show("Server is not running...", "", MessageBoxButton.OK, MessageBoxImage.Warning);
				}

			} while (length == lengthSize);

			Image.Source = GetImage(list.ToArray());
			list.Clear();
		}
	}


	private static BitmapImage GetImage(byte[] imageInfo)
	{
		var image = new BitmapImage();

		using (var memoryStream = new MemoryStream(imageInfo))
		{
			memoryStream.Position = 0;

			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = memoryStream;
			image.EndInit();
		}

		image.Freeze();
		return image;
	}
}
