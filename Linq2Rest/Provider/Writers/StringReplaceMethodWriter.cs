// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringReplaceMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringReplaceMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Linq.Expressions;

    internal class StringReplaceMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {


            return expression.Method.DeclaringType == typeof(string)
                   && expression.Method.Name == "Replace";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {



            var firstArgument = expression.Arguments[0];
            var secondArgument = expression.Arguments[1];
            var obj = expression.Object;





            return string.Format(
                "replace({0}, {1}, {2})",
                expressionWriter(obj),
                expressionWriter(firstArgument),
                expressionWriter(secondArgument));
        }
    }
}