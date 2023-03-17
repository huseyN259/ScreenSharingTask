using System.Drawing;
using System.Net.Sockets;


var server = new UdpClient(45678);


while (true)
{
	UdpReceiveResult result = await server.ReceiveAsync();


	new Task(async () =>
	{
		var remoteEP = result.RemoteEndPoint;

		while (true)
		{
			await Task.Delay(10);

			var image = TakeScreenshot();
			var bytes = ImageToByte(image);
			var size = ushort.MaxValue - 29;
			var chunk = bytes.Chunk(size);
			foreach (var buffer in chunk)
				await server.SendAsync(buffer, buffer.Length, remoteEP);
		}

	}).Start();
}


#region Methods
Image TakeScreenshot()
{
	Bitmap bitmap = new Bitmap(1920, 1080);

	Graphics graphics = Graphics.FromImage(bitmap);
	graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

	return bitmap;
}


byte[] ImageToByte(Image image)
{
	using (var stream = new MemoryStream())
	{
		image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

		return stream.ToArray();
	}
}
#endregion