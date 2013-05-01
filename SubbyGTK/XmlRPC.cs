using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace SubbyGTK
{
	public class XmlRPC
	{
		public XmlRPC ()
		{
		}
		public static string XmlRpcExecMethod(string uri, string methodName, List<Object> parameters)
		{
			var req = (HttpWebRequest)WebRequest.Create(uri);
			req.Method = "POST";
			var genParams = "";
			if (parameters != null && parameters.Count > 0)
			{
				foreach (var param in parameters)
				{
					if (param == null) continue;
					if (param.GetType().Equals(typeof (string)))
					{
						genParams += string.Format(@"<param><value><string>{0}</string></value></param>", param);
					}
					if (param.GetType().Equals(typeof(bool)))
					{
						genParams += string.Format(@"<param><value><boolean>{0}</boolean></value></param>", (bool)param ? 1 : 0);
					}
					if (param.GetType().Equals(typeof(double)))
					{
						genParams += string.Format(@"<param><value><double>{0}</double></value></param>", param);
					}
					if (param.GetType().Equals(typeof(int)) || param.GetType().Equals(typeof(short)))
					{
						genParams += string.Format(@"<param><value><int>{0}</int></value></param>", param);
					}
					if (param.GetType().Equals(typeof (DateTime)))
					{
						genParams +=
							string.Format(
								@"<param><value><dateTime.iso8601>{0:yyyy}{0:MM}{0:dd}T{0:hh}:{0:mm}:{0:ss}</dateTime.iso8601></value></param>",
								param);
					}
				}
			}
			var command = 
				@"<?xml version=""1.0""?><methodCall><methodName>" + methodName + 
					@"</methodName><params>" + genParams + @"</params></methodCall>";
			var bytes = Encoding.ASCII.GetBytes(command);
			req.ContentLength = bytes.Length;
			using (var stream = req.GetRequestStream())
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			using (var stream = new StreamReader(req.GetResponse().GetResponseStream()))
			{
				return stream.ReadToEnd();
			}
		}
	}
}

