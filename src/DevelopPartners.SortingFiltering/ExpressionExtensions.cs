
using System.Linq.Expressions;
using System.Reflection;

namespace DeveloperPartners.SortingFiltering
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName(this Expression expr, bool withParent)
        {
            var memberExpr = expr as MemberExpression;

            if (memberExpr != null)
            {
                var name = memberExpr.Member.Name;

                if (withParent && !string.IsNullOrEmpty(name))
                {
                    var parent = GetMemberName(memberExpr.Expression, false);

                    if (!string.IsNullOrEmpty(parent))
                    {
                        return string.Format("{0}.{1}", parent, name);
                    }

                    return name;
                }

                return name;
            }

            return string.Empty;
        }

        public static string GetMemberName(this LambdaExpression expr, bool withParent)
        {
            return GetMemberName(expr.Body, withParent);
        }
    }
}