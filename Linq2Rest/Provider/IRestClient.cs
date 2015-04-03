// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRestClient.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a REST client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	/// <summary>
	/// Defines the public interface for a REST client.
	/// </summary>
	[ContractClass(typeof(RestClientContracts))]
	public interface IRestClient : IDisposable
	{
		/// <summary>
		/// Gets the base <see cref="Uri"/> for the REST service.
		/// </summary>
		Uri ServiceBase { get; }

		/// <summary>
		/// Gets a service response.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>A string representation of the resource.</returns>
		Stream Get(Uri uri);

		/// <summary>
		/// Posts the passed data to the service.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <param name="input">The <see cref="Stream"/> representation to post.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		Stream Post(Uri uri, Stream input);

		/// <summary>
		/// Puts the passed data to the service.
		/// </summary>
		/// <param name="input">The <see cref="Stream"/> representation to put.</param>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		Stream Put(Uri uri, Stream input);

		/// <summary>
		/// Deletes the resource at the service.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		Stream Delete(Uri uri);
	}

	[ContractClassFor(typeof(IRestClient))]
	internal abstract class RestClientContracts : IRestClient
	{
		public Uri ServiceBase
		{
			get
			{
				Contract.Ensures(Contract.Result<Uri>() != null);
				Contract.Ensures(Contract.Result<Uri>().Scheme == Uri.UriSchemeHttp || Contract.Result<Uri>().Scheme == Uri.UriSchemeHttps);

				throw new NotImplementedException();
			}
		}

		public Stream Get(Uri uri)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentException>(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			Contract.Ensures(Contract.Result<Stream>() != null);

			throw new NotImplementedException();
		}

		public Stream Post(Uri uri, Stream input)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentException>(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			Contract.Requires<ArgumentNullException>(input != null);
			Contract.Ensures(Contract.Result<Stream>() != null);

			throw new NotImplementedException();
		}

		public Stream Put(Uri uri, Stream input)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentException>(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			Contract.Requires<ArgumentNullException>(input != null);
			Contract.Ensures(Contract.Result<Stream>() != null);

			throw new NotImplementedException();
		}

		public Stream Delete(Uri uri)
		{
			Contract.Requires<ArgumentNullException>(uri != null);
			Contract.Requires<ArgumentException>(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			Contract.Ensures(Contract.Result<Stream>() != null);

			throw new NotImplementedException();
		}

		public abstract void Dispose();
	}
}