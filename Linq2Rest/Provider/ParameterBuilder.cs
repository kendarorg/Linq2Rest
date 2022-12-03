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
			
			
#if !NETFX_CORE
			
#endif
			
			

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
			
#if !NETFX_CORE
			
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
			
			
#endif

			return resultUri;
		}

		private static string BuildParameter(string name, string value)
		{
			
			 

			return name + "=" + value;
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			
#if !NETFX_CORE
			
#endif
			
		}
	}
}