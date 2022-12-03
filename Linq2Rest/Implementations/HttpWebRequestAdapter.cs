// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpWebRequestAdapter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Takes a System.Net.HttpWebRequest and wraps it in an IHttpRequest Implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Net;
	using Linq2Rest.Provider;

	/// <summary>
	/// Takes a System.Net.HttpWebRequest and wraps it in an IHttpRequest Implementation.
	/// </summary>
	internal class HttpWebRequestAdapter : IHttpRequest
	{
		public HttpWebRequestAdapter(HttpWebRequest httpWebRequest)
		{
			HttpWebRequest = httpWebRequest;
		}

		/// <summary>
		/// The HttpWebRequest we are adapting to IHttpRequest.
		/// </summary>
		public HttpWebRequest HttpWebRequest { get; private set; }

		/// <summary>
		/// Creates a basic HttpWebRequest that can then be built off of depending on what other functionality is needed.
		/// </summary>
		/// <param name="uri">The uri to send the request to.</param>
		/// <param name="method">The Http Request Method.</param>
		/// <param name="requestMimeType">The MIME type of the data we are sending.</param>
		/// <param name="responseMimeType">The MIME we accept in response.</param>
		/// <returns>Returns an HttpWebRequest initialized with the given parameters.</returns>
		public static HttpWebRequest CreateHttpWebRequest(Uri uri, HttpMethod method, string responseMimeType, string requestMimeType)
		{
			
			
			

			requestMimeType = requestMimeType ?? responseMimeType;

			var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

			httpWebRequest.Method = method.ToString().ToUpperInvariant();

			if (method == HttpMethod.Post || method == HttpMethod.Put)
			{
				httpWebRequest.ContentType = requestMimeType;
			}

			httpWebRequest.Accept = responseMimeType;

			return httpWebRequest;
		}

		public Stream GetRequestStream()
		{
			return HttpWebRequest.GetRequestStream();
		}

		public Stream GetResponseStream()
		{
			var response = HttpWebRequest.GetResponse();
			var stream = response.GetResponseStream();
			return stream;
		}
	}
}
