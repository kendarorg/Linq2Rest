// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataSerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WinRT.Sample.Support
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Json;
	using Linq2Rest.Provider;

	public class ODataSerializer<T> : ISerializer<T>
	{
		private readonly DataContractJsonSerializer _innerSerializer = new DataContractJsonSerializer(typeof(ODataResponse<T>));

		public T Deserialize(Stream input)
		{
			var response = (ODataResponse<T>)_innerSerializer.ReadObject(input);
			return response.Result.Results.FirstOrDefault();
		}

		public IEnumerable<T> DeserializeList(Stream input)
		{
			var response = (ODataResponse<T>)_innerSerializer.ReadObject(input);
			return response.Result.Results;
		}

		public Stream Serialize(T item)
		{
			var stream = new MemoryStream();
			_innerSerializer.WriteObject(stream, item);
			stream.Flush();
			stream.Position = 0;

			return stream;
		}
	}
}