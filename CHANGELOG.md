    
    IMPORTANT:
    Major and minor version are a copy of the `XMapper` package's major and minor version.
    Only the revision number of `XMapper.Testing` is independent of `XMapper`.

## [3.0.4] 2022-04-24

- New test case added: `NoPublicFields`. XMapper only maps properties, no fields. This test case reminds you when you forget to type `{ get; set; }`. (Of course you can turn this test case off if you really want to use public fields - knowing they will not be mapped automatically.)


## [3.0.3] 2022-04-23

- Bump XMapper version.


## [3.0.2] 2022-04-20

- At validating all mappers in an assembly, the test fails if an XMapper field/propety is not static: you should explicitly ignore it (or make it static).
- Bump XMapper version.


## [3.0.1] 2022-04-19

### Breaking

- The static `AssertXMapper` has been replaced by a non-static `XMapperValidator` that requires an `Action<string> logMethod` that you may explicitly ignore by passing it `output => { }`.

### Fixes

- If an assembly has properties that do not have a GetMethod, no NullReferenceException will be thrown anymore.
- `TestCases.NotNullDefaults` has become more stable: for example if TSource has a property of type Array, no exception will be thrown anymore.



## [3.0.0] 2022-04-17

### Breaking

- Rename `TestCases.TargetReferenceTypeMembersNull` to `TestCases.TargetNullDefaults`.
- Use `NullabilityInfoContext` to verify whether a reference type is nullable and do not test the case that a not-nullable reference type is null.
- Use .NET 6 instead of .NET 5 (for `NullabilityInfoContext`).



## [2.0.2] 2022-04-16

- Make `TestCases.TargetReferenceTypeMembersNull` independent of `TestCases.NotNullDefaults`.



## [2.0.1] 2022-04-16

- Bump version of XMapper.
- Add one more test case for nullability of non-enumerable reference type members of the target.



## [2.0.0] 2022-04-14

### Breaking

- The validation has become stricter. It not only tests with the property values that a class has immediatly after construction, but it now also has a test case with all nullable properties null and a test case for all not-null property values. The `TestCases` enum can be used to configure what cases to validate.
- The `params` keyword is removed for the `assemblies` and `assemblyNames` parameters.
