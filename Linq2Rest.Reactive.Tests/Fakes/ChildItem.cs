// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildItem.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ChildItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Reactive.Tests.Fakes
{
	using System.Collections.Generic;

	public class ChildItem
	{
		public string Text { get; set; }

		public IList<ChildItem> Descendants { get; set; } 
	}
}