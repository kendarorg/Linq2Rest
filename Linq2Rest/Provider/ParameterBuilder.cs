// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterBuilder.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;

#if !NETFX_CORE && !SILVERLIGHT
	using System.Web;
#endif

	internal class ParameterBuilder
	{
		private readonly Uri _serviceBase;

		public ParameterBuilder(Uri serviceBase, Type sourceType)
		{
			Contract.Requires(serviceBase != null);
			Contract.Requires(sourceType != null);
#if !NETFX_CORE
			Contract.Requires(serviceBase.Scheme == Uri.UriSchemeHttp || serviceBase.Scheme == Uri.UriSchemeHttps);
#endif
			Contract.Ensures(((System.Collections.ICollection)this.OrderByParameter).Count == 0);
			Contract.Ensures(serviceBase == this._serviceBase);

			_serviceBase = serviceBase;
			SourceType = sourceType;
			OrderByParameter = new List<string>();
		}

		public string FilterParameter { get; set; }

		public IList<string> OrderByParameter { get; private set; }

		public string SelectParameter { get; set; }

		public string SkipParameter { get; set; }

		public string TakeParameter { get; set; }

		public string ExpandParameter { get; set; }

		public Type SourceType { get; private set; }

		public Uri GetFullUri()
		{
			Contract.Ensures(Contract.Result<Uri>() != null);
#if !NETFX_CORE
			Contract.Ensures(Contract.Result<Uri>().Scheme == Uri.UriSchemeHttp || Contract.Result<Uri>().Scheme == Uri.UriSchemeHttps);
#endif

			var parameters = new List<string>();
			if (!string.IsNullOrWhiteSpace(FilterParameter))
			{
#if !SILVERLIGHT && !NETFX_CORE
				parameters.Add(BuildParameter(StringConstants.FilterParameter, HttpUtility.UrlEncode(FilterParameter)));
#else
				parameters.Add(BuildParameter(StringConstants.FilterParameter, FilterParameter));
#endif
			}

			if (!string.IsNullOrWhiteSpace(SelectParameter))
			{
				parameters.Add(BuildParameter(StringConstants.SelectParameter, SelectParameter));
			}

			if (!string.IsNullOrWhiteSpace(SkipParameter))
			{
				parameters.Add(BuildParameter(StringConstants.SkipParameter, SkipParameter));
			}

			if (!string.IsNullOrWhiteSpace(TakeParameter))
			{
				parameters.Add(BuildParameter(StringConstants.TopParameter, TakeParameter));
			}

			if (OrderByParameter.Any())
			{
				parameters.Add(BuildParameter(StringConstants.OrderByParameter, string.Join(",", OrderByParameter)));
			}

			if (!string.IsNullOrWhiteSpace(ExpandParameter))
			{
				parameters.Add(BuildParameter(StringConstants.ExpandParameter, ExpandParameter));
			}

			var builder = new UriBuilder(_serviceBase);
			builder.Query = (string.IsNullOrEmpty(builder.Query) ? string.Empty : builder.Query.Substring(1) + "&") + string.Join("&", parameters);

			var resultUri = builder.Uri;

#if !NETFX_CORE
			Contract.Assume(_serviceBase.Scheme == Uri.UriSchemeHttp || _serviceBase.Scheme == Uri.UriSchemeHttps);
			Contract.Assume(resultUri.Scheme == Uri.UriSchemeHttp || resultUri.Scheme == Uri.UriSchemeHttps);
#endif

			return resultUri;
		}

		private static string BuildParameter(string name, string value)
		{
			Contract.Ensures(Contract.Result<string>() != null);
			Contract.Ensures(0 <= Contract.Result<string>().Length); 

			return name + "=" + value;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_serviceBase != null);
#if !NETFX_CORE
			Contract.Invariant(_serviceBase.Scheme == Uri.UriSchemeHttp || _serviceBase.Scheme == Uri.UriSchemeHttps);
#endif
			Contract.Invariant(OrderByParameter != null);
		}
	}
}