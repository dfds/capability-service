using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests
{
    public class TestCapability
    {
        [Fact]
        public void expected_domain_event_is_raised_when_creating_a_capability()
        {
            var capability = Capability.Create("foo");

            Assert.Equal(
                expected: new[] {new CapabilityCreated(capability.Id, "foo")},
                actual: capability.DomainEvents,
                comparer: new DomainEventComparer()
            );
        }
    }

    public class DomainEventComparer : IEqualityComparer<DomainEvent>
    {
        public bool Equals(DomainEvent x, DomainEvent y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            var id = x.GetPropertyInfo(e => e.EventId);

            var properties = x.GetType()
                              .GetProperties()
                              .Where(p => p.Name != id.Name)
                              .ToArray();

            return properties.All(p => Comparer.Default.Compare(p.GetValue(x), p.GetValue(y)) == 0);
        }

        public int GetHashCode(DomainEvent obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class PropertySelector
    {
        public static PropertyInfo GetPropertyInfo<T>(this T obj, Expression<Func<T, object>> selector)
        {
            if (selector.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Selector must be lambda expression", "selector");
            }

            var lambda = (LambdaExpression)selector;

            var memberExpression = ExtractMemberExpression(lambda.Body);

            if (memberExpression == null)
            {
                throw new ArgumentException("Selector must be member access expression", "selector");
            }

            if (memberExpression.Member.DeclaringType == null)
            {
                throw new InvalidOperationException("Property does not have declaring type");
            }

            return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression)expression);
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression)expression).Operand;
                return ExtractMemberExpression(operand);
            }

            return null;
        }
    }
}