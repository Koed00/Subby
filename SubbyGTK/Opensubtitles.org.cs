using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SubbyGTK
{
	public class OpenSubtitlesClient
	{
		private const string useragent = "Subby";
		private const string apiurl = "http://api.opensubtitles.org/xml-rpc";
		private string token;

		public OpenSubtitlesClient ()
		{
			token = Login ();
		}

		public string Login ()
		{
			var client = new XmlRpc.XmlRpcRequest ();
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
			var client = new XmlRpc.XmlRpcRequest ();
			client.MethodName = "LogOut";
			client.Params.Add (token);
			client.Params.Add (useragent);
			var result = (Hashtable)client.Invoke (apiurl);
			return(result ["status"].ToString () == "200 OK");
		}

		public List<SearchResult> FileSearch (string filename)
		{
			var client = new XmlRpc.XmlRpcRequest ();
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

		public ArrayList GetSubLanguages(){
			var client = new XmlRpc.XmlRpcRequest ();
			client.MethodName = "GetSubLanguages";
			var result = (Hashtable)client.Invoke (apiurl);
			return (ArrayList)result["data"];
		}
		public  List<SearchResult> TitleSearch (string filename)
		{
			var client = new XmlRpc.XmlRpcRequest ();
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

			public string IDSubMovieFile{ get; set; }

			public string MovieHash{ get; set; }

			public string MovieByteSize{ get; set; }

			public string IDSubtitleFile{ get; set; }

			public string SubFileName{ get; set; }

			public string SubActualCD{ get; set; }

			public string SubSize{ get; set; }

			public string SubHash{ get; set; }

			public string IDSubtitle{ get; set; }

			public string UserID{ get; set; }

			public string SubLanguageID{ get; set; }

			public string SubFormat{ get; set; }

			public string SubSumCD{ get; set; }

			public string SubAuthorComment{ get; set; }

			public DateTime SubAddDate{ get; set; }

			public string SubBad{ get; set; }

			public string SubRating{ get; set; }

			public string SubDownloadsCnt{ get; set; }

			public string MovieReleaseName{ get; set; }

			public string IDMovie{ get; set; }

			public string IDMovieImdb{ get; set; }

			public string MovieName{ get; set; }

			public string MovieNameEng{ get; set; }

			public string MovieYear{ get; set; }

			public string MovieImdbRating{ get; set; }

			public string SubFeatured{ get; set; }

			public string UserNickName{ get; set; }

			public string ISO639{ get; set; }

			public string LanguageName{ get; set; }

			public string SubComments{ get; set; }

			public string SubHearingImpaired{ get; set; }

			public string UserRank{ get; set; }

			public string SeriesSeason{ get; set; }

			public string SeriesEpisode{ get; set; }

			public string MovieKind{ get; set; }

			public string SubDownloadLink{ get; set; }

			public string ZipDownloadLink{ get; set; }
		}

		public  List<SearchResult> ParseSearchResult (Hashtable result)
		{

			var ret = new List<SearchResult> ();
			foreach (Hashtable res in (ArrayList)result["data"]) {
				var p = new SearchResult ();
				p.MatchedBy = (res ["MatchedBy"] ?? string.Empty).ToString ();
				p.IDSubMovieFile = (res ["IDSubMovieFile"] ?? string.Empty).ToString ();
				p.MovieHash = (res ["MovieHash"] ?? string.Empty).ToString ();
				p.MovieByteSize = (res ["MovieByteSize"] ?? string.Empty).ToString ();
				p.IDSubtitleFile = (res ["IDSubtitleFile"] ?? string.Empty).ToString ();
				p.SubActualCD = (res ["SubActualCD"] ?? string.Empty).ToString ();
				p.SubSize = (res ["SubSize"] ?? string.Empty).ToString ();
				p.SubHash = (res ["SubHash"] ?? string.Empty).ToString ();
				p.IDSubtitle = (res ["IDSubtitle"] ?? string.Empty).ToString ();
				p.SubFileName = (res ["SubFileName"] ?? string.Empty).ToString ();
				p.UserID = (res ["UserID"] ?? string.Empty).ToString ();
				p.SubLanguageID = (res ["SubLanguageID"] ?? string.Empty).ToString ();
				p.SubFormat = (res ["SubFormat"] ?? string.Empty).ToString ();
				p.SubSumCD = (res ["SubSumCD"] ?? string.Empty).ToString ();
				p.SubAuthorComment = (res ["SubAuthorComment"] ?? string.Empty).ToString ();
				p.SubAddDate = DateTime.Parse (res ["SubAddDate"].ToString());
				p.SubBad = (res ["SubBad"] ?? string.Empty).ToString ();
				p.SubRating = (res ["SubRating"] ?? string.Empty).ToString ();
				p.SubDownloadsCnt = (res ["SubDownloadsCnt"] ?? string.Empty).ToString ();
				p.MovieReleaseName = (res ["MovieReleaseName"] ?? string.Empty).ToString ();
				p.IDMovie = (res ["IDMovie"] ?? string.Empty).ToString ();
				p.IDMovieImdb = (res ["IDMovieImdb"] ?? string.Empty).ToString ();
				p.MovieName = (res ["MovieName"] ?? string.Empty).ToString ();
				p.MovieNameEng = (res ["MovieNameEng"] ?? string.Empty).ToString ();
				p.MovieYear = (res ["MovieYear"] ?? string.Empty).ToString ();
				p.MovieImdbRating = (res ["MovieImdbRating"] ?? string.Empty).ToString ();
				p.SubFeatured = (res ["SubFeatured"] ?? string.Empty).ToString ();
				p.UserNickName = (res ["UserNickName"] ?? string.Empty).ToString ();
				p.ISO639 = (res ["ISO639"] ?? string.Empty).ToString ();
				p.LanguageName = (res ["LanguageName"] ?? string.Empty).ToString ();
				p.SubComments = (res ["SubComments"] ?? string.Empty).ToString ();
				p.SubHearingImpaired = (res ["SubHearingImpaired"] ?? string.Empty).ToString ();
				p.UserRank = (res ["UserRank"] ?? string.Empty).ToString ();
				p.SeriesSeason = (res ["SeriesSeason"] ?? string.Empty).ToString ();
				p.SeriesEpisode = (res ["SeriesEpisode"] ?? string.Empty).ToString ();
				p.MovieKind = (res ["MovieKind"] ?? string.Empty).ToString ();
				p.SubDownloadLink = (res ["SubDownloadLink"] ?? string.Empty).ToString ();
				p.ZipDownloadLink = (res ["ZipDownloadLink"] ?? string.Empty).ToString ();
				ret.Add (p);
			}	

			return ret;
				
		}
	}
}
