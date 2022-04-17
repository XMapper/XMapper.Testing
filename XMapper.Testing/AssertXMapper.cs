using System.Reflection;

namespace XMapper.Testing;

/// <summary>
/// See the public methods of this class to see how you can validate your XMappers efficiently.
/// </summary>
public static class AssertXMapper
{
    /// <summary>
    /// In case you don't want to validate all XMapper instances via <see cref="AllAreValidInAssemblies(string[], TestCases)"/>, you can validate a single XMapper instance via this method.
    /// </summary>
    /// <param name="mapper">The mapper you'd like to validate.</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    public static void IsValid<TSource, TTarget>(this XMapper<TSource, TTarget> mapper, TestCases testCases) where TSource : class, new() where TTarget : class, new()
    {
        if (testCases.HasFlag(TestCases.AppDefaults))
        {
            mapper.Map(new TSource());
        }

        if (testCases.HasFlag(TestCases.NotNullDefaults))
        {
            mapper.Map(GetWithoutNulls<TSource>());
        }

        if (testCases.HasFlag(TestCases.TargetNullDefaults))
        {
            mapper.Map(GetWithoutNulls<TSource>(), GetWithNulls<TTarget>());
        }

        if (testCases.HasFlag(TestCases.NullDefaults))
        {
            mapper.Map(GetWithNulls<TSource>());
        }
    }

    private static TClass GetWithNulls<TClass>() where TClass : class, new()
    {
        var objectWithNulls = new TClass();
        foreach (var propertyInfo in typeof(TClass).GetRuntimeProperties())
        {
            if (Nullable.GetUnderlyingType(propertyInfo.PropertyType!) != null
                || new NullabilityInfoContext().Create(propertyInfo).WriteState == NullabilityState.Nullable)
            {
                propertyInfo.SetValue(objectWithNulls, null);
            }
        }

        return objectWithNulls;
    }

    private static TClass GetWithoutNulls<TClass>() where TClass : class, new()
    {
        var objectWithoutNulls = new TClass();
        foreach (var propertyInfo in typeof(TClass).GetRuntimeProperties())
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType!);
            if (underlyingType != null)
            {
                propertyInfo.SetValue(objectWithoutNulls, Activator.CreateInstance(underlyingType));
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(objectWithoutNulls, "");
            }
            else if (propertyInfo.PropertyType is object)
            {
                propertyInfo.SetValue(objectWithoutNulls, Activator.CreateInstance(propertyInfo.PropertyType));
            }
        }

        return objectWithoutNulls;
    }

    /// <summary>
    /// Uses AppDomain.CurrentDomain.GetAssemblies() and only uses the assemblies you listed by name to validate all XMapper instances that are stored in a public static field or property.
    /// </summary>
    /// <param name="assemblyNames">What is the C# project's display name?</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    /// <exception cref="Exception">Throws at the first assembly that is not found or at the first invalid XMapper found.</exception>
    public static void AllAreValidInAssemblies(string[] assemblyNames, TestCases testCases)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblyName in assemblyNames)
        {
            var assembly = assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName)
                ?? throw new Exception($"An assembly with the name '{assemblyName}' is not part of AppDomain.CurrentDomain.GetAssemblies()." + Environment.NewLine
                + "Those assemblies are: " + string.Join(", ", assemblies.Select(x => x.FullName)));
            AllAreValidInAssembly(assembly, testCases);
        }
    }

    /// <summary>
    /// In case you don't want to use <see cref="AllAreValidInAssemblies(string[], TestCases)"/>, you can load and pass assemblies to this method.
    /// </summary>
    public static void AllAreValidInAssemblies(Assembly[] assemblies, TestCases testCases)
    {
        foreach (var assembly in assemblies)
        {
            AllAreValidInAssembly(assembly, testCases);
        }
    }

    /// <summary>
    /// Uses AppDomain.CurrentDomain.GetAssemblies() and only uses the assembly you listed by name to validate all XMapper instances that are stored in a public static field or property.
    /// </summary>
    /// <param name="assemblyName">What is the C# project's display name?</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    /// <exception cref="Exception">Throws if the assembly cannot be found or at the first invalid XMapper found.</exception>
    public static void AllAreValidInAssembly(string assemblyName, TestCases testCases)
    {
        AllAreValidInAssemblies(new[] { assemblyName }, testCases);
    }

    /// <summary>
    /// In case you don't want to use <see cref="AllAreValidInAssembly(string, TestCases)"/>, you can load and pass an <paramref name="assembly"/> to this method.
    /// </summary>
    /// <param name="assembly">All the mappers inside this assembly are validated.</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    public static void AllAreValidInAssembly(Assembly assembly, TestCases testCases)
    {
        var types = assembly.GetTypes();
        var staticFields = types.SelectMany(x => x.GetRuntimeFields().Where(x => x.IsStatic)).Where(x => x.FieldType.Name == "XMapper`2").ToArray();
        var staticFieldXMapperInfos = staticFields.Select(x => new StaticMemberXMapperInfo { Fullname = x.DeclaringType + "." + x.Name, GetXMapperInstance = () => x.GetValue(null) });
        var staticProperties = types.SelectMany(x => x.GetRuntimeProperties().Where(x => x.GetMethod!.IsStatic)).Where(x => x.PropertyType.Name == "XMapper`2").ToArray();
        var staticPropertyXMapperInfos = staticProperties.Select(x => new StaticMemberXMapperInfo { Fullname = x.DeclaringType + "." + x.Name, GetXMapperInstance = () => x.GetValue(null) });
        foreach (var xMapperInfo in staticFieldXMapperInfos.Concat(staticPropertyXMapperInfos))
        {
            try
            {
                var mapper = (dynamic)xMapperInfo.GetXMapperInstance();

                AssertXMapper.IsValid(mapper, testCases);
            }
            catch (Exception ex)
            {
                var message = "Invalid mapper: " + xMapperInfo.Fullname + Environment.NewLine
                    + "Full exception:" + Environment.NewLine +
                    ex.ToString();
                throw new Exception(message);
            }
        }
    }

    internal class StaticMemberXMapperInfo
    {
#nullable disable
        public string Fullname { get; set; }
        public Func<object> GetXMapperInstance { get; set; }
#nullable restore
    }
}

/// <summary>
/// What property values should be used for the members of the source and target object? If multiple options are chosen, multiple test runs will be done. The option <see cref="All"/> is strictest and safest.
/// </summary>
[Flags]
public enum TestCases
{
    /// <summary>
    /// This test case uses a newly instantiated source object without changing any of its property values. It is not recommended to only use this test case.
    /// </summary>
    AppDefaults = 1,
    /// <summary>
    /// This test case uses a newly instantiated source object and sets all property values to their non-null C# type default. If a string is mistakenly mapped to an int, this test case will signal an error.
    /// </summary>
    NotNullDefaults = 2,
    /// <summary>
    /// This test case uses a newly instantiated source object and sets nullable properties to null. If you want to map from nullable properties to non-nullable properties, this test case signals incorrect custom mapping.
    /// </summary>
    NullDefaults = 4,
    /// <summary>
    /// Uses <see cref="NotNullDefaults"/> for source, but sets  target's nullable properties to null. This test case signals invalid custom mappings of reference types.
    /// </summary>
    TargetNullDefaults = 8,
    /// <summary>
    /// Strict test. Runs all cases.
    /// </summary>
    All = AppDefaults | NotNullDefaults | NullDefaults | TargetNullDefaults,
}
