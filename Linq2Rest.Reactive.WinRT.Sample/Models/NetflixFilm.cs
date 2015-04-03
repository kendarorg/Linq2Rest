// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetflixFilm.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NetflixFilm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.WinRT.Sample.Models
{
	using System.Runtime.Serialization;

	[DataContract]
	internal sealed class NetflixFilm
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember(IsRequired = false)]
		public int? ReleaseYear { get; set; }

		[DataMember(IsRequired = false)]
		public double? AverageRating { get; set; }
	}
}
