using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.IO.Ports;

namespace ProjectTest1
{
	class Program
	{
		public static void Main()
		{
			string a = GetPacket("---a---b---", "---", "--", false);
			var task = Task.Run(() => MainAsync());
			var result = task.GetAwaiter().GetResult();
			Console.ReadLine();

			Console.ReadLine();
			Console.ReadLine();
			// hahaha my world
			
			//SerialPort sp = new SerialPort();

			//sp.ReadBufferSize;
			//sp.Read()
		}

		static async Task<string> MainAsync()
		{
			using (var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(10000) })
			{
				client.BaseAddress = new Uri("https://naver.com");
				TimeSpan t = client.Timeout;
				var values = new[]
				{
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login"),
					new KeyValuePair<string, string>("", "login")
				};
				try
				{
					var result = await client.GetAsync("");
					//var result = await client.PostAsync("/api/Membership/exists", new FormUrlEncodedContent(values));
					string resultContent = await result.Content.ReadAsStringAsync();
					Console.WriteLine(resultContent);
					return resultContent;
				}
				catch (TaskCanceledException ex)
				{
					return null;
				}
				catch (Exception ex)
				{
					return null;
				}
			}
		}

		static string GetPacket(string _origin, string _prev, string _post, bool _last = true)
		{
			List<int> prevList = new List<int>(), postList = new List<int>();
			int oldIdx, newIdx;
			int prevIdx = 0, postIdx = 0;
			string packet = string.Empty;

			try
			{
				oldIdx = -1;
				while ((newIdx = _origin.Substring(oldIdx + 1).IndexOf(_prev)) >= 0)
				{
					oldIdx += newIdx + 1;
					prevList.Add(oldIdx + _prev.Length);
				}
			}
			catch (Exception) { }

			try
			{
				oldIdx = -1;
				while ((newIdx = _origin.Substring(oldIdx + 1).IndexOf(_post)) >= 0)
				{
					oldIdx += newIdx + 1;
					postList.Insert(0, oldIdx);
				}
			}
			catch (Exception) { }

			if (prevList.Count == 0 || postList.Count == 0 || prevList[prevIdx] >= postList[postIdx])
				return packet;
		
			while (true)
			{
				if (_last)
				{	// origin의 끝부분을 검색합니다.
					prevIdx++;
					if (prevList.Count > prevIdx && prevList[prevIdx] < postList[postIdx]) continue;
					prevIdx--;

					postIdx++;
					if (postList.Count > postIdx && prevList[prevIdx] < postList[postIdx]) continue;
					postIdx--;
				}
				else
				{	// origin의 시작부분을 검색합니다.
					postIdx++;
					if (postList.Count > postIdx && prevList[prevIdx] < postList[postIdx]) continue;
					postIdx--;
					
					prevIdx++;
					if (prevList.Count > prevIdx && prevList[prevIdx] < postList[postIdx]) continue;
					prevIdx--;
				}

				return _origin.Substring(prevList[prevIdx], postList[postIdx] - prevList[prevIdx]).Trim();
			}
		}

		static string[] GetPackets(string _origin, string _prev, string _post)
		{
			int oldIdx, newIdx, prevLen = _prev.Length;
			List<string> packet = new List<string>();

			oldIdx = -1;
			while ((newIdx = _origin.Substring(oldIdx + 1).IndexOf(_prev)) >= 0)
			{
				oldIdx += newIdx + 1;
				int prevIdx = _origin.Substring(oldIdx + 1).IndexOf(_prev);
				int postIdx = _origin.Substring(oldIdx + prevLen).IndexOf(_post);

				if ((prevIdx < 0 && postIdx >= 0) || (prevIdx + oldIdx + 1 >= postIdx + oldIdx + prevLen))
				{
					try
					{
						string s = _origin.Substring(oldIdx + prevLen, postIdx).Trim(); // 정확히는 length에 postIdx + oldIdx + prevLen - (oldIdx + prevLen)가 들어갑니다.
						if (!string.IsNullOrEmpty(s)) packet.Add(s);
					}
					catch (Exception) { }
				}
			}
			return packet.ToArray();
		}
	}
}
