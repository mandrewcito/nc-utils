using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace NetCoreUtils.Requests
{
    public static class RequestsHandler
    {

	    private static T MakeRequest<T>(string uri,System.Net.Http.HttpMethod httpMethod, byte[] body, Dictionary<string, string> headers,
		    string contentType)
	    {
		    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

		    #region set headers
		    #endregion

			request.ContentType = contentType;
		    request.Method = httpMethod.Method;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			#region set Body content 
			if (body != null)
			{
				request.ContentLength = body.Length;
				using (var requestBody = request.GetRequestStream())
				{
					requestBody.Write(body, 0, body.Length);
				}
			}
			#endregion


			using (var response = (HttpWebResponse)request.GetResponse())
		    using (var stream = response.GetResponseStream())
		    using (var reader = new StreamReader(stream))
		    {
			    return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
		    }
		}

	    private static string MakeQuerystring(Dictionary<string, string> parameters)
	    {
			return parameters != null
				? "?" + string.Join("&", parameters.Select(o => $"{o.Key}={o.Value}"))
				: string.Empty;
		}

	    public static T Get<T>(string url, Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, string contentType = "application/json")
	    {
		    var querystring = MakeQuerystring(parameters);
			var uri = url + querystring;

		    return MakeRequest<T>(uri,HttpMethod.Get, null, headers, contentType);
		}

	    public static T Delete<T>(string url, Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, string contentType = "application/json")
	    {
			var querystring = MakeQuerystring(parameters);
		    var uri = url + querystring;
			return MakeRequest<T>(uri, HttpMethod.Delete,null,headers,contentType);
	    }

	    public static T Post<T,TK>(string uri, TK body, Dictionary<string, string> headers = null, string contentType = "application/json")
	    {
		    var bodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
		    return MakeRequest<T>(uri, HttpMethod.Post, bodyBytes,headers,contentType);
	    }

	    public static T Put<T, TK>(string uri, TK body, Dictionary<string, string> headers = null, string contentType = "application/json")
	    {
		    var bodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
			return MakeRequest<T>(uri, HttpMethod.Put, bodyBytes, headers, contentType);
		}

	}
}
