// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyAnyMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyAnyMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class EmptyAnyMethodWriter : IMethodCallWriter
    {
        private static readonly MethodInfo AnyMethod = typeof(Enumerable)
#if !NETFX_CORE
.GetMethods()
#else
.GetRuntimeMethods()
#endif
.FirstOrDefault(m => m.Name == "Any" && m.GetParameters()
                                                     .Length == 2);

        public bool CanHandle(MethodCallExpression expression)
        {


            return expression.Method.Name == "Any" && expression.Arguments.Count == 1;
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {


#if !NETFX_CORE
            var argumentType = expression.Arguments[0].Type;
#else
			var argumentType = expression.Arguments[0].Type.GetTypeInfo();
#endif
            var parameterType = argumentType.IsGenericType
#if !NETFX_CORE
 ? argumentType.GetGenericArguments()[0]
#else
									? argumentType.GetGenericParameterConstraints()[0]
#endif
 : typeof(object);
            var anyMethod = AnyMethod.MakeGenericMethod(parameterType);

            var parameter = Expression.Parameter(parameterType);

            var lambda = Expression.Lambda(Expression.Constant(true), parameter);
            var rewritten = Expression.Call(expression.Object, anyMethod, expression.Arguments[0], lambda);
            return expressionWriter(rewritten);
        }
    }
}