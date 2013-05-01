using System;
using Nwc.XmlRpc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

		public List<SearchResult> FileSearch (string filename)
		{
			var client = new Nwc.XmlRpc.XmlRpcRequest ();
			client.MethodName = "SearchSubtitles";
			client.Params.Add (token);
			var query = new ArrayList ();
			var qhash = new Hashtable ();
			var moviehash = MovieHasher.GetHash (filename);
			var moviebytes = MovieHasher.GetByteSize (filename);
			qhash.Add ("moviehash", moviehash);
			qhash.Add ("moviebytesize", moviebytes);
			query.Add (qhash);
			client.Params.Add (query);
			var result = (Hashtable)client.Invoke (apiurl);
			bool hasdata = false;
			if (bool.TryParse (result ["data"].ToString(), out hasdata))
				return TitleSearch (filename);
			return ParseSearchResult (result);

		}

		public  List<SearchResult> TitleSearch (string filename)
		{
			var client = new Nwc.XmlRpc.XmlRpcRequest ();
			client.MethodName = "SearchSubtitles";
			client.Params.Add (token);
			var query = new ArrayList ();
			var qhash = new Hashtable ();
			var movietitle = Path.GetFileNameWithoutExtension (filename);
			qhash.Add ("query", movietitle);
			query.Add (qhash);
			client.Params.Add (query);
			var result = (Hashtable)client.Invoke (apiurl);
			return ParseSearchResult (result);
		}

		public class SearchResult
		{

			public string MatchedBy{ get; set; }
		}

		public  List<SearchResult> ParseSearchResult (Hashtable result)
		{

			var ret = new List<SearchResult> ();
			foreach (Hashtable res in (ArrayList)result["data"]) {
				var p = new SearchResult ();
				p.MatchedBy = res ["MatchedBy"].ToString ();
				ret.Add (p);
			}	

			return ret;
				
		}
	}
}
