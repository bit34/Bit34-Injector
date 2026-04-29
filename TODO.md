# Bit34-Injector — TODO


## Open

### `#1` Namespace restriction — fix three latent bugs in `Check`
File: `src/Com/Bit34Games/Injector/Binding/NamespaceRestriction.cs`

The `Check` method has three issues. Apply all three together — the rewrite below covers all of them.

- [ ] **Null-guard `containerNamespace`.** `container.GetType().Namespace` returns `null` for types declared in the global namespace; the current code then NREs inside `IndexOf`. Treat global-namespace containers as "no match" and return `false`.
- [ ] **Use ordinal comparison.** Replace the default culture-sensitive `IndexOf` with `StringComparison.Ordinal`. C# identifiers are ordinal — culture-sensitive matching can misbehave on non-invariant locales (notably Turkish `i`/`İ`, which is real risk for a Turkey-based studio).
- [ ] **Express intent with `Equals` / `StartsWith`.** Drop the `IndexOf(...) == 0` workaround. Use explicit `Equals` for exact match and `StartsWith` + `'.'` boundary check for nested namespaces. Current expression is correct but order-of-evaluation fragile (the `||` short-circuit is the only thing keeping the indexer in bounds).

#### Suggested replacement

```csharp
public bool Check(object container, Type typeToInject, IInstanceProvider provider)
{
    string containerNamespace = container.GetType().Namespace;
    if (containerNamespace == null) { return false; }

    for (int i = 0; i < _namespaceNameList.Length; i++)
    {
        string namespaceName = _namespaceNameList[i];

        //  Exact match ("Game.UI" matches "Game.UI")
        if (containerNamespace.Equals(namespaceName, StringComparison.Ordinal)) { return true; }

        //  Nested match ("Game.UI" matches "Game.UI.MainMenu" but NOT "Game.UISystem")
        if (containerNamespace.Length > namespaceName.Length
            && containerNamespace[namespaceName.Length] == '.'
            && containerNamespace.StartsWith(namespaceName, StringComparison.Ordinal))
        {
            return true;
        }
    }
    return false;
}
```

- [ ] **Add a test for the global-namespace container case.** In `unity/dev/test`, add a payload class declared with no `namespace` block, bind a dependency on it with `RestrictToNamespace`, and verify the restriction returns `false` / is reported as `InjectionRestricted` instead of throwing an NRE.

---

### `#2` Stop building a full `StackTrace` on every recorded error
File: `src/Com/Bit34Games/Injector/InjectorContext.cs` (`GetCallerInfo`)

`new StackTrace(true)` reads PDBs and walks every frame — multi-millisecond cost. In throwing mode this fires once and the program stops; in non-throwing mode (dev/tests) errors accumulate silently and the cost adds up. Make stack capture either opt-in or much cheaper.

Two options — pick one:

- [ ] **Cheaper capture.** Replace `new StackTrace(true).GetFrame(1 + callerLevel)` with `new StackFrame(1 + callerLevel, true)`. Same information, no full trace built. This is the minimal-diff fix.

  ```csharp
  private string GetCallerInfo(int callerLevel = 0)
  {
      StackFrame sf = new StackFrame(1 + callerLevel, true);
      return string.Format("\tFilename:{0}\n\tMethod:{1}\n\tLine:{2}",
                           sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber());
  }
  ```

- [ ] **Opt-in capture.** Add a constructor flag (e.g. `bool captureCallerInfo = true`) that disables `GetCallerInfo` entirely when off. Useful for production non-throwing mode where the file/line info is overkill and the cost matters.

The first option alone is enough for almost all cases; the second is a tighter knob if you ever profile injection in a hot loop.
