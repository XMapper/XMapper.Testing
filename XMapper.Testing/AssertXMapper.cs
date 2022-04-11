using System.Reflection;

namespace XMapper.Testing;

/// <summary>
/// See the public methods of this class to see how you can validate your XMappers efficiently.
/// </summary>
public static class AssertXMapper
{
    /// <summary>
    /// In case you don't want to validate all XMapper instances via <see cref="AllAreValidInAssemblies(string[])"/>, you can validate a single XMapper instance via this method.
    /// </summary>
    public static void IsValid<TSource, TTarget>(this XMapper<TSource, TTarget> mapper) where TSource : class, new() where TTarget : class, new()
    {
        mapper.Map(new TSource());
    }

    /// <summary>
    /// Uses AppDomain.CurrentDomain.GetAssemblies() and only uses the assemblies you listed by name to validate all XMapper instances that are stored in a public static field or property.
    /// </summary>
    /// <param name="assemblyNames">What is the C# project's display name?</param>
    /// <exception cref="Exception">Throws at the first assembly that is not found or at the first invalid XMapper found.</exception>
    public static void AllAreValidInAssemblies(string[] assemblyNames)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblyName in assemblyNames)
        {
            var assembly = assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName)
                ?? throw new Exception($"An assembly with the name '{assemblyName}' is not part of AppDomain.CurrentDomain.GetAssemblies()." + Environment.NewLine
                + "Those assemblies are: " + string.Join(", ", assemblies.Select(x => x.FullName)));
            AllAreValidInAssembly(assembly);
        }
    }

    /// <summary>
    /// In case you don't want to use <see cref="AllAreValidInAssemblies(string[])"/>, you can load and pass assemblies to this method.
    /// </summary>
    public static void AllAreValidInAssemblies(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AllAreValidInAssembly(assembly);
        }
    }

    /// <summary>
    /// Uses AppDomain.CurrentDomain.GetAssemblies() and only uses the assembly you listed by name to validate all XMapper instances that are stored in a public static field or property.
    /// </summary>
    /// <param name="assemblyName">What is the C# project's display name?</param>
    /// <exception cref="Exception">Throws if the assembly cannot be found or at the first invalid XMapper found.</exception>
    public static void AllAreValidInAssembly(string assemblyName)
    {
        AllAreValidInAssemblies(new[] { assemblyName });
    }

    /// <summary>
    /// In case you don't want to use <see cref="AllAreValidInAssembly(string)"/>, you can load and pass an assembly to this method.
    /// </summary>
    public static void AllAreValidInAssembly(Assembly assembly)
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

                AssertXMapper.IsValid(mapper);
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
