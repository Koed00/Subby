using System;
using Nwc.XmlRpc;
using System.Collections;
using System.Collections.Generic;

namespace SubbyGTK
{
	public class OpenSubtitlesClient
	{
		private const string useragent = "OS Test User Agent";
		private const string apiurl = "http://api.opensubtitles.org/xml-rpc";
		private string token;

		public OpenSubtitlesClient ()
		{
			token = Login ();
		}

		public string Login ()
		{
			var client = new Nwc.XmlRpc.XmlRpcRequest ();
			client.MethodName = "LogIn";
			client.Params.Add ("");
			client.Params.Add ("");
			client.Params.Add ("en");
			client.Params.Add (useragent);
			var result = (Hashtable)client.Invoke (apiurl);
			if (result ["status"].ToString () == "200 OK") {
				return result ["token"].ToString ();
			}
			return string.Empty;
		}

		public bool Logout ()
		{
			if (string.IsNullOrEmpty (token))
				return false;
			var client = new Nwc.XmlRpc.XmlRpcRequest ();
			client.MethodName = "LogOut";
			client.Params.Add (token);
			client.Params.Add (useragent);
			var result = (Hashtable)client.Invoke (apiurl);
			return(result ["status"].ToString () == "200 OK");
		}
		public void FileSearch(string filename){
			var client = new Nwc.XmlRpc.XmlRpcRequest ();
			client.MethodName = "SearchSubtitles";
			client.Params.Add (token);
			var query = new ArrayList();
			var qhash = new Hashtable();
			qhash.Add ("query", "south park");
			qhash.Add ("season", "1");
			qhash.Add ("episode", "1");
			query.Add(qhash);
			client.Params.Add (query);
			var result = (Hashtable)client.Invoke (apiurl);


		}
	}
}

