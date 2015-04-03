// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncRestClientFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public enumeration of supported HTTP methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	/// <summary>
	/// Defines the public enumeration of supported HTTP methods.
	/// </summary>
	public enum HttpMethod
	{
		/// <summary>
		/// Represents the GET HTTP method.
		/// </summary>
		Get, 

		/// <summary>
		/// Represents the PUT HTTP method.
		/// </summary>
		Put, 

		/// <summary>
		/// Represents the POST HTTP method.
		/// </summary>
		Post, 

		/// <summary>
		/// Represents the DELETE HTTP method.
		/// </summary>
		Delete
	}

	/// <summary>
	/// Defines the public interface for the async REST client factory.
	/// </summary>
	[ContractClass(typeof(AsyncRestClientFactoryContracts))]
	public interface IAsyncRestClientFactory
	{
		/// <summary>
		/// Gets the base service address.
		/// </summary>
		Uri ServiceBase { get; }

		/// <summary>
		/// Creates an <see cref="IAsyncRestClient"/>.
		/// </summary>
		/// <param name="source">The <see cref="Uri"/> to download from.</param>
		/// <returns>An <see cref="IAsyncRestClient"/> instance.</returns>
		IAsyncRestClient Create(Uri source);

		/// <summary>
		/// Sets the HTTP method for the request.
		/// </summary>
		/// <param name="method">The <see cref="HttpMethod"/> to set.</param>
		void SetMethod(HttpMethod method);

		/// <summary>
		/// Sets the input data to send to the server.
		/// </summary>
		/// <param name="input">The input as a <see cref="Stream"/>.</param>
		void SetInput(Stream input);
	}

	[ContractClassFor(typeof(IAsyncRestClientFactory))]
	internal abstract class AsyncRestClientFactoryContracts : IAsyncRestClientFactory
	{
		public Uri ServiceBase
		{
			get
			{
				Contract.Ensures(Contract.Result<Uri>() != null);
#if !NETFX_CORE
				Contract.Ensures(Contract.Result<Uri>().Scheme == Uri.UriSchemeHttp || Contract.Result<Uri>().Scheme == Uri.UriSchemeHttps);
#endif
				throw new NotImplementedException();
			}
		}

		public IAsyncRestClient Create(Uri source)
		{
			Contract.Requires<ArgumentNullException>(source != null);
#if !NETFX_CORE
			Contract.Requires<ArgumentException>(source.Scheme == Uri.UriSchemeHttp || source.Scheme == Uri.UriSchemeHttps);
#endif
			throw new NotImplementedException();
		}

		public abstract void SetMethod(HttpMethod method);

		public void SetInput(Stream input)
		{
			Contract.Requires<ArgumentNullException>(input != null);

			throw new NotImplementedException();
		}
	}
}