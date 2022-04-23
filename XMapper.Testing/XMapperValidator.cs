using System.Reflection;

namespace XMapper.Testing;

/// <summary>
/// <para>This class requires a logging method from your unit testing library or you can explicitly ignore it with an empty Action like `output => { }`.</para>
/// <para>Console.WriteLine, Debug.WriteLine and Trace.WriteLine may not be supported because of parallellization.</para>
/// </summary>
public class XMapperValidator
{
    private readonly Action<string> Log;
    private readonly List<string> _ignoredMapperStorageLocation = new();

    /// <summary>
    /// <para>This class requires a logging method from your unit testing library or you can explicitly ignore it with an empty Action like `output => { }`.</para>
    /// <para>Console.WriteLine, Debug.WriteLine and Trace.WriteLine may not be supported because of parallellization.</para>
    /// </summary>
    public XMapperValidator(Action<string> logMethod)
    {
        Log = logMethod;
    }

    /// <summary>
    /// Non-static fields and properties for XMapper instances must be explicitly ignored. This is a safety check: maybe you thought the mapper would be tested but you forgot the static keyword.
    /// </summary>
    public XMapperValidator Ignore(string mapperStorageLocationFullname)
    {
        _ignoredMapperStorageLocation.Add(mapperStorageLocationFullname);
        return this;
    }

    /// <summary>
    /// In case you don't want to validate all XMapper instances via <see cref="AllAreValidInAssemblies(string[], TestCases)"/>, you can validate a single XMapper instance via this method.
    /// </summary>
    /// <param name="mapper">The mapper you'd like to validate.</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    public void IsValid<TSource, TTarget>(XMapper<TSource, TTarget> mapper, TestCases testCases) where TSource : class, new() where TTarget : class, new()
    {
        if (testCases.HasFlag(TestCases.AppDefaults))
        {
            Log($"Test case: {TestCases.AppDefaults}");
            mapper.Map(new TSource());
        }

        if (testCases.HasFlag(TestCases.NotNullDefaults))
        {
            Log($"Test case: {TestCases.NotNullDefaults}");
            mapper.Map(GetWithoutNulls<TSource>());
        }

        if (testCases.HasFlag(TestCases.TargetNullDefaults))
        {
            Log($"Test case: {TestCases.TargetNullDefaults}");
            mapper.Map(GetWithoutNulls<TSource>(), GetWithNulls<TTarget>());
        }

        if (testCases.HasFlag(TestCases.NullDefaults))
        {
            Log($"Test case: {TestCases.NullDefaults}");
            mapper.Map(GetWithNulls<TSource>());
        }
    }

    private TClass GetWithNulls<TClass>() where TClass : class, new()
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

    private TClass GetWithoutNulls<TClass>() where TClass : class, new()
    {
        var objectWithoutNulls = new TClass();
        foreach (var propertyInfo in typeof(TClass).GetRuntimeProperties())
        {
            if (propertyInfo.GetValue(objectWithoutNulls) != null)
            {
                continue;
            }

            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType!);
            if (underlyingType != null)
            {
                propertyInfo.SetValue(objectWithoutNulls, Activator.CreateInstance(underlyingType));
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(objectWithoutNulls, "");
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                propertyInfo.SetValue(objectWithoutNulls, Array.CreateInstance(propertyInfo.PropertyType.GetElementType()!, 0));
            }
            else
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
    public void AllAreValidInAssemblies(string[] assemblyNames, TestCases testCases)
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
    public void AllAreValidInAssemblies(Assembly[] assemblies, TestCases testCases)
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
    public void AllAreValidInAssembly(string assemblyName, TestCases testCases)
    {
        AllAreValidInAssemblies(new[] { assemblyName }, testCases);
    }

    /// <summary>
    /// In case you don't want to use <see cref="AllAreValidInAssembly(string, TestCases)"/>, you can load and pass an <paramref name="assembly"/> to this method.
    /// </summary>
    /// <param name="assembly">All the mappers inside this assembly are validated.</param>
    /// <param name="testCases">Specify how strict the test should be. <see cref="TestCases.All"/> is strictest.</param>
    public void AllAreValidInAssembly(Assembly assembly, TestCases testCases)
    {
        Log("Start collecting XMapper instance storage locations (static fields/properties).");
        var types = assembly.GetTypes();
        var fields = types.SelectMany(x => x.GetRuntimeFields().Where(x => x.FieldType.Name == "XMapper`2"))
            .Select(x => new XMapperStorageInfo { Fullname = x.DeclaringType + "." + x.Name, GetXMapperInstance = () => x.GetValue(null), IsStatic = x.IsStatic })
            .ToArray();
        var properties = types.SelectMany(x => x.GetRuntimeProperties().Where(x => x.PropertyType.Name == "XMapper`2"))
            .Select(x => new XMapperStorageInfo { Fullname = x.DeclaringType + "." + x.Name, GetXMapperInstance = () => x.GetValue(null), IsStatic = x.GetMethod != null && x.GetMethod.IsStatic })
            .ToArray();

        Log($"Found {fields.Length + properties.Length} XMapper instance storage locations to test.");

        foreach (var xMapperInfo in fields.Concat(properties))
        {
            if (_ignoredMapperStorageLocation.Contains(xMapperInfo.Fullname))
            {
                Log($"Skipping ignored mapper '{xMapperInfo.Fullname}'");
                continue;
            }

            if (!xMapperInfo.IsStatic)
            {
                throw new Exception($"'{xMapperInfo.Fullname}' is not static and also not ignored via '{nameof(XMapperValidator)}.{nameof(Ignore)}'. Mark the field/property static to include the mapper in the test or explicitly ignore it.");
            }

            try
            {
                Log($"Validating '{xMapperInfo.Fullname}'.");

                var mapper = (dynamic)xMapperInfo.GetXMapperInstance();

                if (mapper == null)
                {
                    throw new Exception("The static field's value was null, so there was no XMapper instance to test.");
                }

                IsValid(mapper, testCases);
            }
            catch (Exception ex)
            {
                var message = "Invalid mapper: " + xMapperInfo.Fullname + Environment.NewLine
                    + "Full exception:" + Environment.NewLine +
                    ex.ToString();
                throw new Exception(message);
            }
        }
        Log("Finished validating all XMapper instances.");
    }

    internal class XMapperStorageInfo
    {
#nullable disable
        public string Fullname { get; set; }
        public bool IsStatic { get; set; }
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
    /// Uses <see cref="NotNullDefaults"/> for source, but sets target's nullable properties to null. This test case signals invalid custom mappings of reference types.
    /// </summary>
    TargetNullDefaults = 8,

    /// <summary>
    /// Strict test. Runs all test cases.
    /// </summary>
    All = AppDefaults | NotNullDefaults | NullDefaults | TargetNullDefaults,
}
