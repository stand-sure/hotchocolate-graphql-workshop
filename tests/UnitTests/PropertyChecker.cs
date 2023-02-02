#nullable enable

namespace UnitTests;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using FluentAssertions;

using JetBrains.Annotations;

[PublicAPI]
internal static class PropertyChecker
{
    /// <summary>
    ///     Checks a property for invariance (the value gotten is the same as the one set).
    /// </summary>
    /// <param name="expression">
    ///     An expression of the form <code>foo.bar == val</code> is expected, where bar is a
    ///     property.
    /// </param>
    /// <remarks>This syntax is borrowed from the simplified property setup in Moq.</remarks>
    public static void CheckInvariance(Expression<Func<bool>> expression)
    {
        if (expression.Body is not BinaryExpression { Left: MemberExpression left, Right: ConstantExpression right })
        {
            if (expression.Body is MemberExpression memberExpression && memberExpression.Type == typeof(bool))
            {
                left = memberExpression;
                right = Expression.Constant(true);
            }
            else
            {
                throw new InvalidOperationException($"Expected a {nameof(Boolean)} {nameof(BinaryExpression)} in the form `foo.bar == val`.");
            }
        }

        object? expected = right.Value;

        BinaryExpression assignment = Expression.Assign(left, right);
        LambdaExpression assignmentLambda = Expression.Lambda(assignment);
        assignmentLambda.Compile().DynamicInvoke();

        UnaryExpression getter = Expression.Convert(left, typeof(object));
        LambdaExpression getterLambda = Expression.Lambda(getter);
        object? actual = getterLambda.Compile().DynamicInvoke();

        actual.Should().Be(expected);
    }

    /// <summary>
    ///     Checks a property for invariance (the value gotten is the same as the one set).
    /// </summary>
    /// <typeparam name="T">The type of the value(s).</typeparam>
    /// <param name="expression">A property/member expression.</param>
    /// <param name="values">The values to use for testing.</param>
    /// <remarks>
    ///     For built-in value types, the <paramref name="values" /> can be omitted if min, max, zero are acceptable.
    ///     <seealso cref="Nullable{T}"/> is supported.
    ///     <seealso cref="Guid"/> is supported.
    ///     <seealso cref="string"/> is tested with null, whitespace, aGUID, and a 100-character. Do not use this method if truncation is important.
    /// </remarks>
    public static void CheckInvariance<T>(Expression<Func<T>> expression, IEnumerable<T>? values = null)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
        }

        Type? type = typeof(T);
        var isNullable = false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
            isNullable = true;
        }

        values ??= Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => new[] { false, true }.Cast<T>(),
            TypeCode.Char => new[] { char.MinValue, '\0', char.MaxValue }.Cast<T>(),
            TypeCode.SByte => new[] { sbyte.MinValue, 0, sbyte.MaxValue }.Cast<T>(),
            TypeCode.Byte => new[] { byte.MinValue, 0, byte.MaxValue }.Cast<T>(),
            TypeCode.Int16 => new[] { short.MinValue, 0, short.MaxValue }.Cast<T>(),
            TypeCode.UInt16 => new[] { ushort.MinValue, 0, ushort.MaxValue }.Cast<T>(),
            TypeCode.Int32 => new[] { int.MinValue, 0, int.MaxValue }.Cast<T>(),
            TypeCode.UInt32 => new[] { uint.MinValue, 0U, uint.MaxValue }.Cast<T>(),
            TypeCode.Int64 => new[] { long.MinValue, 0L, long.MaxValue }.Cast<T>(),
            TypeCode.UInt64 => new[] { ulong.MinValue, 0UL, ulong.MaxValue }.Cast<T>(),
            TypeCode.Single => new[] { float.MinValue, 0F, float.MaxValue }.Cast<T>(),
            TypeCode.Double => new[] { double.MinValue, 0D, double.MaxValue }.Cast<T>(),
            TypeCode.Decimal => new[] { decimal.MinValue, 0M, decimal.MaxValue }.Cast<T>(),
            TypeCode.DateTime => new[] { DateTime.MinValue, DateTime.MaxValue, default }.Cast<T>(),
            TypeCode.String => new[] { null, string.Empty, " ", new string('a', 100), Guid.NewGuid().ToString() }.Cast<T>(),
            ////TypeCode.Object => (type == typeof(Guid)
            ////    ? new object[] {Guid.Empty, Guid.NewGuid()}
            ////    : type == typeof(DateTimeOffset)
            ////        ? new object[] {DateTimeOffset.MinValue, default(DateTimeOffset), DateTimeOffset.MaxValue}
            ////        : new object[] {null}).Cast<T>(),
            TypeCode.Object => GetInvarianceTestValues(type).Cast<T>(),
            _ => throw new ArgumentOutOfRangeException(nameof(values),
                $"Default values for type {typeof(T).Name} are not known. Please supply ${nameof(values)}."),
        };

        IEnumerable<object?> values2 = values.Cast<object>();

        if (isNullable)
        {
            values2 = values2.AsEnumerable().Concat(new object?[] { null });
        }

        foreach (T? value in values2)
        {
            ConstantExpression constantExpression = Expression.Constant(value, typeof(T));
            BinaryExpression binaryExpression = Expression.Equal(memberExpression, constantExpression);
            Expression<Func<bool>> lambda = Expression.Lambda<Func<bool>>(binaryExpression);
            PropertyChecker.CheckInvariance(lambda);
        }
    }

    private static IEnumerable<object?> GetInvarianceTestValues(Type? type)
    {
        IEnumerable<object?> retVal;

        if (type == typeof(Guid))
        {
            retVal = new object?[] { Guid.Empty, Guid.NewGuid() };
        }
        else if (type == typeof(DateTimeOffset))
        {
            retVal = new object?[] { DateTimeOffset.MinValue, default(DateTimeOffset), DateTimeOffset.MaxValue };
        }
        else
        {
            retVal = new object?[] { null };
        }

        return retVal;
    }

    /// <summary>
    /// Checks that a property has the correct characteristics: read/write, return type, default value.
    /// </summary>
    /// <typeparam name="T">The EXPECTED type of the property.</typeparam>
    /// <param name="expression">The property access expression.</param>
    /// <param name="readOnly">If <c>true</c>, then the property is not writable. Default is that properties are read-write.</param>
    /// <param name="expectedDefaultValue">The expected value from when the property is not explicitly set.</param>
    public static void CheckProperty<T>(Expression<Func<object?>> expression, bool? readOnly = null, object? expectedDefaultValue = default)
    {
        if (expression.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
        {
            if (expression.Body is not MemberExpression tmp)
            {
                throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
            }

            memberExpression = tmp;
        }

        var property = (PropertyInfo)memberExpression.Member;
        property.Should().BeReadable();
        property.PropertyType.Should().Be<T>();

        if (readOnly != true)
        {
            property.Should().BeWritable();
        }
        else
        {
            property.Should().NotBeWritable();
        }

        UnaryExpression convert = Expression.Convert(memberExpression, typeof(T));
        LambdaExpression lambda = Expression.Lambda<Func<T>>(convert);
        Delegate compile = lambda.Compile();
        object? actualDefault = compile.DynamicInvoke();
        expectedDefaultValue ??= PropertyChecker.GetDefault(typeof(T));
        actualDefault.Should().Be(expectedDefaultValue);
    }

    public static void CheckRequired(Expression<Func<object>> expression, bool allowEmptyStrings = false)
    {
        if (expression.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
        {
            if (expression.Body is not MemberExpression tmp)
            {
                throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
            }

            memberExpression = tmp;
        }

        var property = (PropertyInfo)memberExpression.Member;

        var attribute = property.GetCustomAttribute<RequiredAttribute>();
        attribute.Should().NotBeNull();
        attribute!.AllowEmptyStrings.Should().Be(allowEmptyStrings);
    }

    public static void CheckMaxLength(Expression<Func<object>> expression, int maxLength)
    {
        if (expression.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
        {
            if (expression.Body is not MemberExpression tmp)
            {
                throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
            }

            memberExpression = tmp;
        }

        var property = (PropertyInfo)memberExpression.Member;

        var attribute = property.GetCustomAttribute<MaxLengthAttribute>();
        attribute.Should().NotBeNull();
        attribute!.Length.Should().Be(maxLength);
    }

    public static void CheckRange<TValue>(Expression<Func<object>> expression, TValue minValue, TValue maxValue)
    {
        if (expression.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
        {
            if (expression.Body is not MemberExpression tmp)
            {
                throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
            }

            memberExpression = tmp;
        }

        var property = (PropertyInfo)memberExpression.Member;

        var attribute = property.GetCustomAttribute<RangeAttribute>();
        attribute.Should().NotBeNull();
        attribute!.Minimum.Should().Be(minValue);
        attribute.Maximum.Should().Be(maxValue);
    }

    public static TAttribute CheckAttribute<TAttribute>(Expression<Func<object?>> expression) where TAttribute : Attribute
    {
        if (expression.Body is not UnaryExpression { Operand: MemberExpression memberExpression })
        {
            if (expression.Body is not MemberExpression tmp)
            {
                throw new InvalidOperationException($"Expected a {nameof(MemberExpression)}.");
            }

            memberExpression = tmp;
        }

        var property = (PropertyInfo)memberExpression.Member;

        var attribute = property.GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull();

        return attribute!;
    }

    private static object? GetDefault(Type t)
    {
        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}