using System;
using System.IO;
using System.Net;

namespace SharedMemory
{
	public class DownloadJob : Target
	{
		String url, dest;
		bool dl = true;

		public DownloadJob (long id, String url, String dest)
		{
			Id = id;
			Name = "Download Job for " + url;
			this.dest = dest;
			this.url = url;
		}

		public override TargetState run ()
		{
			try {
				WebClient client = new WebClient ();
				Console.WriteLine();
				client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write(new String(' ', Console.WindowWidth - 1));
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					String s = String.Format("{0,3}", e.ProgressPercentage);
					Console.Write(s + "%[");
					int perc = (int)((Console.WindowWidth - 6) * e.ProgressPercentage / 100);
					for(int i = 0; i < perc; i++) {
						Console.Write("=");
					}
					for(int i = 0; i < Console.WindowWidth - perc - 6; i++) {
						Console.Write(".");
					}
					Console.Write("]");
				};
				client.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => dl = false;
				client.DownloadFileAsync(new Uri(url), dest);
				while(dl) {
					System.Threading.Thread.Sleep(200);
				}
				Console.WriteLine();
				return TargetState.DONE;
			} catch (Exception e) {
				Console.Error.WriteLine(e);
				File.Delete(dest);
				return SharedMemory.TargetState.FAIL;
			}
		}
	}
}

