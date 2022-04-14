## [2.0.0] 2022-04-14

### Changed

- The validation has become stricter. It not only tests with the property values that a class has immediatly after construction, but it now also has runs a test case with all nullable properties null and a test case for all not-null property values. The `TestCases` enum can be used to configure what cases to validate.
- The `params` keyword is removed for the assemblies and assemblyNames parameters.
