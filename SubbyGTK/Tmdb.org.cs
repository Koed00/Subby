using System;
namespace SubbyGTK
{
	public class tmdb
	{
		private const string baseurl="http://api.themoviedb.org";
		private const string token="c945c6b61eec48ae2fccea0dec867774";
		private string _session;
		public tmdb()
		{
	
			_session=GetSession();
		
		}
			public string GetSession(){}
		}
}

