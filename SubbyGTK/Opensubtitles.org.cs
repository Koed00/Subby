using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubbyGTK.XmlRpc;

namespace SubbyGTK
{
    public class OpenSubtitlesClient
    {
        private const string Useragent = "Subby";
        private const string Apiurl = "http://api.opensubtitles.org/xml-rpc";
        private readonly string _token;

        public OpenSubtitlesClient()
        {
            _token = Login();
        }

        public string Login()
        {
            var client = new XmlRpcRequest {MethodName = "LogIn"};
            client.Params.Add("");
            client.Params.Add("");
            client.Params.Add("en");
            client.Params.Add(Useragent);
            var result = (Hashtable) client.Invoke(Apiurl);
            if (result["status"].ToString() == "200 OK")
            {
                return result["token"].ToString();
            }
            return string.Empty;
        }

        public bool Logout()
        {
            if (string.IsNullOrEmpty(_token))
                return false;
            var client = new XmlRpcRequest {MethodName = "LogOut"};
            client.Params.Add(_token);
            client.Params.Add(Useragent);
            var result = (Hashtable) client.Invoke(Apiurl);
            return (result["status"].ToString() == "200 OK");
        }

        public List<SearchResult> FileSearch(string filename, string lang)
        {
            var client = new XmlRpcRequest {MethodName = "SearchSubtitles"};
            client.Params.Add(_token);
            var query = new ArrayList();
            var qhash = new Hashtable();
            string moviehash = MovieHasher.GetHash(filename);
            long moviebytes = MovieHasher.GetByteSize(filename);
            qhash.Add("moviehash", moviehash);
            qhash.Add("moviebytesize", moviebytes);
			qhash.Add ("sublanguageid", lang);
            query.Add(qhash);
            client.Params.Add(query);
            var result = (Hashtable) client.Invoke(Apiurl);
            bool hasdata;
            if (bool.TryParse(result["data"].ToString(), out hasdata))
                return TitleSearch(filename, lang);
            return ParseSearchResult(result);
        }

        public ArrayList GetSubLanguages()
        {
            var client = new XmlRpcRequest {MethodName = "GetSubLanguages"};
            var result = (Hashtable) client.Invoke(Apiurl);
            return (ArrayList) result["data"];
        }

        public List<SearchResult> TitleSearch(string filename, string lang)
        {
            var client = new XmlRpcRequest {MethodName = "SearchSubtitles"};
            client.Params.Add(_token);
            var query = new ArrayList();
            var qhash = new Hashtable();
            string movietitle = Path.GetFileNameWithoutExtension(filename);
            qhash.Add("query", movietitle);
			qhash.Add ("sublanguageid", lang);
            query.Add(qhash);
            client.Params.Add(query);
            var result = (Hashtable) client.Invoke(Apiurl);
            return ParseSearchResult(result);
        }

        public List<SearchResult> ParseSearchResult(Hashtable result)
        {
            return (from Hashtable res in (ArrayList) result["data"]
                    select new SearchResult
                               {
                                   MatchedBy = (res["MatchedBy"] ?? string.Empty).ToString(),
                                   IDSubMovieFile = (res["IDSubMovieFile"] ?? string.Empty).ToString(),
                                   MovieHash = (res["MovieHash"] ?? string.Empty).ToString(),
                                   MovieByteSize = (res["MovieByteSize"] ?? string.Empty).ToString(),
                                   IDSubtitleFile = (res["IDSubtitleFile"] ?? string.Empty).ToString(),
                                   SubActualCD = (res["SubActualCD"] ?? string.Empty).ToString(),
                                   SubSize = (res["SubSize"] ?? string.Empty).ToString(),
                                   SubHash = (res["SubHash"] ?? string.Empty).ToString(),
                                   IDSubtitle = (res["IDSubtitle"] ?? string.Empty).ToString(),
                                   SubFileName = (res["SubFileName"] ?? string.Empty).ToString(),
                                   UserID = (res["UserID"] ?? string.Empty).ToString(),
                                   SubLanguageID = (res["SubLanguageID"] ?? string.Empty).ToString(),
                                   SubFormat = (res["SubFormat"] ?? string.Empty).ToString(),
                                   SubSumCD = (res["SubSumCD"] ?? string.Empty).ToString(),
                                   SubAuthorComment = (res["SubAuthorComment"] ?? string.Empty).ToString(),
                                   SubAddDate = DateTime.Parse(res["SubAddDate"].ToString()),
                                   SubBad = (res["SubBad"] ?? string.Empty).ToString(),
                                   SubRating = (res["SubRating"] ?? string.Empty).ToString(),
                                   SubDownloadsCnt = (res["SubDownloadsCnt"] ?? string.Empty).ToString(),
                                   MovieReleaseName = (res["MovieReleaseName"] ?? string.Empty).ToString(),
                                   IDMovie = (res["IDMovie"] ?? string.Empty).ToString(),
                                   IDMovieImdb = (res["IDMovieImdb"] ?? string.Empty).ToString(),
                                   MovieName = (res["MovieName"] ?? string.Empty).ToString(),
                                   MovieNameEng = (res["MovieNameEng"] ?? string.Empty).ToString(),
                                   MovieYear = (res["MovieYear"] ?? string.Empty).ToString(),
                                   MovieImdbRating = (res["MovieImdbRating"] ?? string.Empty).ToString(),
                                   SubFeatured = (res["SubFeatured"] ?? string.Empty).ToString(),
                                   UserNickName = (res["UserNickName"] ?? string.Empty).ToString(),
                                   ISO639 = (res["ISO639"] ?? string.Empty).ToString(),
                                   LanguageName = (res["LanguageName"] ?? string.Empty).ToString(),
                                   SubComments = (res["SubComments"] ?? string.Empty).ToString(),
                                   SubHearingImpaired = (res["SubHearingImpaired"] ?? string.Empty).ToString(),
                                   UserRank = (res["UserRank"] ?? string.Empty).ToString(),
                                   SeriesSeason = (res["SeriesSeason"] ?? string.Empty).ToString(),
                                   SeriesEpisode = (res["SeriesEpisode"] ?? string.Empty).ToString(),
                                   MovieKind = (res["MovieKind"] ?? string.Empty).ToString(),
                                   SubDownloadLink = (res["SubDownloadLink"] ?? string.Empty).ToString(),
                                   ZipDownloadLink = (res["ZipDownloadLink"] ?? string.Empty).ToString()
                               }).ToList();
        }

        #region Nested type: SearchResult

        public class SearchResult
        {
            public string MatchedBy { get; set; }

            public string IDSubMovieFile { get; set; }

            public string MovieHash { get; set; }

            public string MovieByteSize { get; set; }

            public string IDSubtitleFile { get; set; }

            public string SubFileName { get; set; }

            public string SubActualCD { get; set; }

            public string SubSize { get; set; }

            public string SubHash { get; set; }

            public string IDSubtitle { get; set; }

            public string UserID { get; set; }

            public string SubLanguageID { get; set; }

            public string SubFormat { get; set; }

            public string SubSumCD { get; set; }

            public string SubAuthorComment { get; set; }

            public DateTime SubAddDate { get; set; }

            public string SubBad { get; set; }

            public string SubRating { get; set; }

            public string SubDownloadsCnt { get; set; }

            public string MovieReleaseName { get; set; }

            public string IDMovie { get; set; }

            public string IDMovieImdb { get; set; }

            public string MovieName { get; set; }

            public string MovieNameEng { get; set; }

            public string MovieYear { get; set; }

            public string MovieImdbRating { get; set; }

            public string SubFeatured { get; set; }

            public string UserNickName { get; set; }

            public string ISO639 { get; set; }

            public string LanguageName { get; set; }

            public string SubComments { get; set; }

            public string SubHearingImpaired { get; set; }

            public string UserRank { get; set; }

            public string SeriesSeason { get; set; }

            public string SeriesEpisode { get; set; }

            public string MovieKind { get; set; }

            public string SubDownloadLink { get; set; }

            public string ZipDownloadLink { get; set; }
        }

        #endregion
    }
}