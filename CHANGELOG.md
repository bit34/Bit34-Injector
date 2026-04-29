# Change Log

## [3.0.0] - 2026-04-26

### Added
 - `NoOpBinding<T>` returned from `AddBinding<T>` on error paths in non-throwing mode, so fluent chains stay safe even after a recorded `BindingAfterInjection` or `AlreadyAddedBindingForType`.
 - `IReadOnlyList<InjectionError> Errors` on `IInjectorTester`, replacing `ErrorCount` + `GetError(int)`. `HasErrors` kept as a useful shortcut.
 - XML doc comments across the public API; `<GenerateDocumentationFile>true</GenerateDocumentationFile>` so the `.xml` ships next to the assembly.
 - NuGet package metadata: `Description`, `PackageTags`, `PackageProjectUrl`, `RepositoryUrl`, `RepositoryType`, `LangVersion`, `Nullable`.

### Changed
 - `IInjector.GetAssignableInstances<T>` returns `IEnumerable<T>` instead of `IEnumerator<T>`; callers can `foreach` directly.
 - `IMemberInjector.InjectIntoField` / `InjectIntoProperty` renamed to `TryInjectIntoField` / `TryInjectIntoProperty` to match the standard .NET `Try*` convention for the swallow contract.
 - `IInjectionBinding.RestrictionList` exposed as `IReadOnlyList<IInjectionRestriction>` instead of mutable `List<>`.
 - `InjectionError.error` / `InjectionError.message` renamed to `Error` / `Message` (PascalCase).
 - `InjectionException.error` renamed to `Error`.
 - `ReflectionCache.reflectedType` (public) renamed to `_reflectedType` and made private.
 - `NamespaceRestriction._namespaceNameList` renamed to `_namespaceNames` (the underlying type is `string[]`, not `List<>`).
 - `LICENSE.txt` renamed to `LICENSE.md`.
 - `ReflectionCache` rewritten for performance: `LinkedList<>` → `List<>`, `FindMembers` → `GetFields`/`GetProperties`, `GetCustomAttributes` → `IsDefined`, `BindingFlags.DeclaredOnly` added so each level of the inheritance chain is visited exactly once. Properties path keeps a `HashSet<string>` name-dedup so virtual overrides aren't injected twice.
 - `InjectorContext.AddBinding<T>` no longer returns `null` on the error path in non-throwing mode (returns a `NoOpBinding<T>` instead).
 - `ReflectionCache.AddProperties` filters get-only properties (`CanWrite` check) instead of throwing inside `SetValue` at injection time.

### Removed
 - `IInjectorTester.ErrorCount` and `IInjectorTester.GetError(int)`. Use `Errors.Count` / `Errors[i]`.
 - `IInstanceProviderList` interface and `InjectionBinding<T>` class made `internal`. The two `AddValueProvider` / `AddTypedProvider` methods on `InjectorContext` converted to explicit interface implementation.

### Fixed
 - Trailing `]` typo in the `AlreadyAddedTypeWithDifferentProvider` error message format.
 - "Handler error" → "Handle error" comment typos (9 sites).
 - README sample code: variable name (`InjectorContext` → `injector`) and `manager.MoveNext` → `managers.MoveNext`.
 - `ReflactionCache.cs` filename → `ReflectionCache.cs` (matched the class name).
 - `Bit34-Injector.csproj` `RootNamespace` underscores → dots.
 - Spacing/punctuation pass: tabs → 4 spaces, trailing whitespace stripped, multi-blank lines collapsed, `if(` → `if (`, `==null` → `== null`, redundant `== true`/`== false` simplified, comma-no-space normalized, stray spaces inside parens removed.
 - README "Creating Injector" rewritten to document the non-throwing-mode contract (dev/tests only; check `HasErrors` after the bind phase).
 - README "Manually getting instances" example rewritten to use `foreach`.
 - README "Error Handling" section updated to point at the new `Errors` property.

### Notes
 - Breaking release — consumers of `2.x` should expect compile-time changes. The full set of breaking changes is summarized in the **Changed** and **Removed** sections above.
 - Single-thread assumption now stated explicitly in the README under Design Decisions.

## [2.0.0] - 2022-02-02

### Added
 - Changelog added (This file)

### Changed
 - Repository name chaned to ```Bit34-Injector```
 - Namespace changed to ```Com.Bit34Games.Injector```
 - ```Injector``` class renamed to ```InjectorContext```
 - ```IInjectorTester``` interface does not extend ```IInjector``` interface anymore, they are individually implemented
