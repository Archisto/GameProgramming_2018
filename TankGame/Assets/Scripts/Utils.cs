using System;
using System.Linq.Expressions;

namespace TankGame.Utils
{
    public static class Utils
    {
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            if (propertyLambda == null)
            {
                throw new ArgumentNullException("propertyLambda");
            }

            var memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException
                    ("You must pass a lambda of the form: " +
                     "'() => Class.Property' or '() => object.Property'");
            }

            return memberExpression.Member.Name;
        }
    }
}
