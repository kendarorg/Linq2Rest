// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTrimMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringTrimMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Linq.Expressions;

    internal class StringTrimMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {


            return expression.Method.DeclaringType == typeof(string)
                   && expression.Method.Name == "Trim";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            var obj = expression.Object;



            return string.Format("trim({0})", expressionWriter(obj));
        }
    }
}