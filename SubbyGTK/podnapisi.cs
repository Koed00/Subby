using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Nwc.XmlRpc;
using System.Collections;
namespace podnapisi
{
	public class Client
	{
		private readonly IStateName _proxy = XmlRpcProxyGen.Create<IStateName> ();
		private readonly string _session;

		public Client ()
		{
			_session = Login ();
		}

		private string Login ()
		{
			var init = podClient.Initiate ("Subby_0_1");
			var session = init ["session"].ToString();
			var nonce = init ["nonce"].ToString();
			string passhash = Sha256 (Md5Hash("7951leo") + nonce);
			XmlRpcStruct auth = _proxy.Authenticate (session, "koed00", passhash);
			if (auth ["status"].ToString () == Retconsts.OK)
				return session;
			return "";
		}

		public XmlRpcStruct Search (string filename)
		{
			byte[] moviehash = ComputeMovieHash (filename);
			var hashes = new string[1];
			hashes [0] = ToHexadecimal (moviehash);
			XmlRpcStruct result = _proxy.Search (_session, hashes);
			var results = (XmlRpcStruct)result ["results"];
			if (results.Count > 0)
				return results;

			return null;
		}

		public string GetFileName (int movieId)
		{
			string[] movieids = { movieId.ToString() };
			XmlRpcStruct fileresult = _proxy.Download(_session,movieids);
			if(fileresult["status"].ToString() !=Retconsts.OK) return null;
			var names=(XmlRpcStruct) fileresult["names"];
			//var filenames=(XmlRpcStruct) names["filename"
			return null;
		}
		public result XMLSearch(string filename)
		{
			string podnapurl = String.Format("http://www.podnapisi.net/en/ppodnapisi/search?sK={0}&sXML=1", filename);
			var request = (HttpWebRequest)WebRequest.Create(podnapurl);
			using (var response = request.GetResponse() as HttpWebResponse)
			{
				if (response != null)
				{
					using (var reader = new StreamReader(response.GetResponseStream()))
					{
						var settings = new XmlReaderSettings
						{
							ConformanceLevel = ConformanceLevel.Auto,
							DtdProcessing=DtdProcessing.Parse
						};
						using (XmlReader xmlTextReader = XmlReader.Create(reader, settings))
						{
							var xs = new XmlSerializer(typeof(result));
							return (result)xs.Deserialize(xmlTextReader);
						}
					}
				}
			}

			return null;
		}

		public static XmlDocument GetXMLDocument(string inURL)
		{
			XmlDocument myXMLDocument;

			try
			{
				var myHttpWebRequest = (HttpWebRequest) WebRequest.Create(inURL);
				myHttpWebRequest.Method = "GET";
				myHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";
				var myHttpWebResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();
				myXMLDocument = new XmlDocument();
				var myXMLReader = new XmlTextReader(myHttpWebResponse.GetResponseStream());
				myXMLDocument.Load(myXMLReader);
			}
			catch (Exception myException)
			{
				throw new Exception("Error Occurred in GetXMLDocument)", myException);
			}
			return myXMLDocument;
		}

		private static byte[] ComputeMovieHash(string filename)
		{
			byte[] result;
			using (Stream input = File.OpenRead(filename))
			{
				result = ComputeMovieHash(input);
			}
			return result;
		}

		private static byte[] ComputeMovieHash(Stream input)
		{
			long streamsize = input.Length;
			long lhash = streamsize;

			long i = 0;
			var buffer = new byte[sizeof (long)];
			while (i < 65536/sizeof (long) && (input.Read(buffer, 0, sizeof (long)) > 0))
			{
				i++;
				lhash += BitConverter.ToInt64(buffer, 0);
			}

			input.Position = Math.Max(0, streamsize - 65536);
			i = 0;
			while (i < 65536/sizeof (long) && (input.Read(buffer, 0, sizeof (long)) > 0))
			{
				i++;
				lhash += BitConverter.ToInt64(buffer, 0);
			}
			input.Close();
			byte[] result = BitConverter.GetBytes(lhash);
			Array.Reverse(result);
			return result;
		}

		private static string ToHexadecimal(byte[] bytes)
		{
			var hexBuilder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				hexBuilder.Append(bytes[i].ToString("x2"));
			}
			return hexBuilder.ToString();
		}

		private static string Md5Hash(string input)
		{
			MD5 md5 = MD5.Create();
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);
			var sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}
			return sb.ToString();
		}

		private static string Sha256(string password)
		{
			var crypt = new SHA256Managed();
			string hash = String.Empty;
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
			return crypto.Aggregate(hash, (current, bit) => current + bit.ToString("x2"));
		}
	}

	internal static class Retconsts
	{
		public const string OK = "200";
		public const string InvalidCredentials = "300";
		public const string NoAuthorisation = "301";
		public const string InvalidSession = "302";
		public const string MovieNotFound = "400";
		public const string InvalidFormat = "401";
		public const string InvalidLanguage = "402";
		public const string InvalidHash = "403";
		public const string InvalidArchive = "404";
	}

	public  class  podClient{
		private static string URL=@"http://ssp.podnapisi.net:8000/RPC2/";
		private static Nwc.XmlRpc.XmlRpcRequest client = new Nwc.XmlRpc.XmlRpcRequest();
		public static Hashtable Initiate(string appname){
			client.MethodName = "initiate";
			client.Params.Clear();
			client.Params.Add(appname);
			Nwc.XmlRpc.XmlRpcResponse response = client.Send(URL);
			if (response.IsFault)
				return null;
			return (Hashtable) response.Value;
		}

	}

	[XmlRpcUrl("http://ssp.podnapisi.net:8000/RPC2/")]
	public interface IStateName : IXmlRpcProxy
	{
		[XmlRpcMethod("initiate")]
		XmlRpcStruct Initiate(string appname);

		[XmlRpcMethod("authenticate")]
		XmlRpcStruct Authenticate(string session, string username, string password);

		[XmlRpcMethod("search")]
		XmlRpcStruct Search(string session, string[] hashes);
		
		[XmlRpcMethod("download")]
		XmlRpcStruct Download (string session, string[] subtitles);
	}
}

