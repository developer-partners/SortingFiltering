
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DeveloperPartners.SortingFiltering.EntityFrameworkCore.Helpers.QueryHelper
{
    class QueryExpressionDescriptor<TSource>
    {
        public QueryProperty QueryProperty { get; set; }
        public Expression Expression { get; set; }
        public List<QueryExpressionDescriptor<TSource>> Children { get; set; }

        private Expression GetChildrenExpression(IEnumerable<QueryExpressionDescriptor<TSource>> children)
        {
            Expression resultExpression = null;

            foreach (var child in children)
            {
                if (resultExpression != null)
                {
                    resultExpression = child.QueryProperty.LogicalOperator == LogicalOperator.And
                      ? Expression.AndAlso(resultExpression, child.Expression)
                      : Expression.OrElse(resultExpression, child.Expression);
                }
                else
                {
                    resultExpression = child.Expression;
                }

                if (!child.Children.IsNullOrEmpty())
                {
                    resultExpression = FlattenHierarchy(resultExpression, children);
                }
            }

            return resultExpression;
        }

        private Expression FlattenHierarchy(Expression expression, IEnumerable<QueryExpressionDescriptor<TSource>> children)
        {
            if (!children.IsNullOrEmpty())
            {
                var childrenExpression = GetChildrenExpression(children);

                if (childrenExpression != null)
                {
                    return Expression.AndAlso(expression, childrenExpression);
                }
            }

            return expression;
        }

        public Expression FlattenHierarchy()
        {
            return FlattenHierarchy(this.Expression, this.Children);
        }
    }
}