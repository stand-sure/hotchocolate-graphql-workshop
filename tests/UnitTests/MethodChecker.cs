namespace UnitTests;

using System.Linq.Expressions;
using System.Reflection;

using FluentAssertions;

using JetBrains.Annotations;

[PublicAPI]
internal static class MethodChecker
{
    public static MethodInfo VerifyMethod<TReturn>(Expression<Func<TReturn>> expression)
    {
        var methodCallExpression = expression.Body as MethodCallExpression;

        methodCallExpression.Should().NotBeNull();

        return methodCallExpression!.Method;
    }

    public static IEnumerable<TAttribute> VerifyMethodAttribute<TAttribute>(Expression<Func<object?>> expression)
        where TAttribute : Attribute
    {
        var method = VerifyMethod(expression);

        IEnumerable<TAttribute> attributes = method.GetCustomAttributes<TAttribute>();

        attributes.Should().NotBeEmpty();

        return attributes;
    }

    public static ParameterInfo? VerifyParameter<TMethodReturn>(Expression<Func<TMethodReturn>> expression, string parameterName)
    {
        var method = VerifyMethod(expression);

        var parameter = method.GetParameters().SingleOrDefault(p => p.Name == parameterName);

        parameter.Should().NotBeNull();

        return parameter;
    }

    public static ParameterInfo? VerifyParameter<TMethodReturn>(Expression<Func<TMethodReturn>> expression, Type parameterType)
    {
        var method = VerifyMethod(expression);

        var parameter = method.GetParameters().SingleOrDefault(p => p.ParameterType == parameterType);

        parameter.Should().NotBeNull();

        return parameter;
    }

    public static IEnumerable<TAttribute> VerifyParameterAttribute<TAttribute>(
        Expression<Func<object?>> expression,
        string parameterName)
        where TAttribute : Attribute
    {
        var parameter = VerifyParameter(expression, parameterName);

        IEnumerable<TAttribute> attributes = parameter!.GetCustomAttributes<TAttribute>().ToList();

        attributes.Should().NotBeEmpty();

        return attributes;
    }

    public static IEnumerable<TAttribute>? VerifyParameterAttribute<TAttribute>(
        Expression<Func<object?>> expression,
        Type parameterType)
        where TAttribute : Attribute
    {
        var parameter = VerifyParameter(expression, parameterType);

        IEnumerable<TAttribute>? attributes = parameter?.GetCustomAttributes<TAttribute>().ToList();

        attributes.Should().NotBeEmpty();

        return attributes;
    }
}