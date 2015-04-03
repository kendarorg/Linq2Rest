// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestComplexSerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestComplexSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Tests.Fakes
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Json;
	using Linq2Rest.Provider;

	public class TestComplexSerializer : ISerializer<FakeItem>
	{
		private readonly DataContractJsonSerializer _innerListSerializer = new DataContractJsonSerializer(typeof(List<FakeItem>));
		private readonly DataContractJsonSerializer _innerSerializer = new DataContractJsonSerializer(typeof(FakeItem));

		public FakeItem Deserialize(Stream input)
		{
			return (FakeItem)_innerSerializer.ReadObject(input);
		}

		public IEnumerable<FakeItem> DeserializeList(Stream input)
		{
			return (List<FakeItem>)_innerListSerializer.ReadObject(input);
		}

		public Stream Serialize(FakeItem item)
		{
			var stream = new MemoryStream();
			_innerSerializer.WriteObject(stream, item);
			stream.Flush();
			stream.Position = 0;

			return stream;
		}
	}
}