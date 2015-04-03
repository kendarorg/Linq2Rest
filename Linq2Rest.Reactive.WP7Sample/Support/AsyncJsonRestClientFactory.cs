// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncJsonRestClientFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the factory to create a REST client using JSON requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WP8.Sample.Support
{
	using System;
	using System.IO;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines the factory to create a REST client using JSON requests.
	/// </summary>
	public class AsyncJsonRestClientFactory : IAsyncRestClientFactory
	{
		private Stream _input;
		private HttpMethod _method;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncJsonRestClientFactory"/> class.
		/// </summary>
		/// <param name="serviceBase">The base <see cref="Uri"/> for the REST service.</param>
		public AsyncJsonRestClientFactory(Uri serviceBase)
		{
			ServiceBase = serviceBase;
			_method = HttpMethod.Get;
		}

		/// <summary>
		/// Gets the base service address.
		/// </summary>
		public Uri ServiceBase { get; private set; }

		/// <summary>
		/// Creates an <see cref="IAsyncRestClient"/>.
		/// </summary>
		/// <param name="source">The <see cref="Uri"/> to download from.</param>
		/// <returns>An <see cref="IAsyncRestClient"/> instance.</returns>
		public IAsyncRestClient Create(Uri source)
		{
			return new AsyncJsonRestClient(source, _method, _input);
		}

		public void SetMethod(HttpMethod method)
		{
			_method = method;
		}

		public void SetInput(Stream input)
		{
			_input = input;
		}

		private class AsyncJsonRestClient : IAsyncRestClient
		{
			private readonly Stream _input;
			private readonly HttpMethod _method;
			private readonly Uri _uri;

			public AsyncJsonRestClient(Uri uri, HttpMethod method, Stream input)
			{
				_uri = uri;
				_method = method;
				_input = input;
			}

			public Task<Stream> Download()
			{
				var request = (HttpWebRequest)WebRequest.Create(_uri);
				request.Accept = "application/json";
				request.Method = _method.ToString().ToUpperInvariant();
				if (_method == HttpMethod.Put || _method == HttpMethod.Post)
				{
					return Task<Stream>.Factory
						.FromAsync(
							request.BeginGetRequestStream, 
							request.EndGetRequestStream, 
							request)
						.ContinueWith<HttpWebRequest>(WriteRequestStream)
						.ContinueWith<Task<WebResponse>>(GetResponse)
						.ContinueWith<Stream>(w => w.Result.Result.GetResponseStream())
						.ContinueWith(s => s.Result);
				}

				return Task<WebResponse>.Factory
						.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)
						.ContinueWith(x => x.Result.GetResponseStream());
			}

			private Task<WebResponse> GetResponse(Task<HttpWebRequest> r)
			{
				var request = r.Result;
				return Task<WebResponse>
					.Factory
					.FromAsync(
						request.BeginGetResponse, 
						request.EndGetResponse, 
						null);
			}

			private HttpWebRequest WriteRequestStream(Task<Stream> s)
			{
				var buffer = new byte[_input.Length];
				_input.Read(buffer, 0, buffer.Length);

				var stream = s.Result;
				stream.Write(buffer, 0, buffer.Length);
				return s.AsyncState as HttpWebRequest;
			}
		}
	}
}