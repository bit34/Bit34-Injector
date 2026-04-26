# **Bit34 Injector - Dependency Injection Library**

# **Table of contents**
- [What is it?](#what-is-it)
- [Who is it for?](#who-is-it-for)
- [What does dependency injection do anyway?](#what-does-dependency-injection-do-anyway)
- [Basic example](#basic-example)
- [Documentation](#documentation)
    - [Design decisions](#design-decisions)
    - [Creating Injector](#creating-injector)
    - [Adding Bindings](#adding-bindings)
    - [Injection](#injection)
    - [Error Handling](#error-handling)

## **What is it?**
This is a C# dependency injection (DI) library with a small set of features.

## **Who is it for?**
It is primarily developed to be used in conjunction with our other libraries. Its simple nature also makes it an easy point to start learning DI.

## **What does dependency injection do anyway?**
If you are new to DI just jump into [this Unity3D tutorial project](https://github.com/bit34/bit34-injector-unityexamples) to learn its usage and advantages.

## **Basic example**
```
using System;
using Com.Bit34Games.Injector;

//  A class that we need to access its reference in other classes
class UserData
{
    public string name;

    public UserData()
    {
        this.name = "Guest";
    }
}

//  A target class that will be injected into
class MyWindow
{
    [Inject] private UserData _userData;

    public void Show()
    {
        Console.WriteLine("Hello " + _userData.name);
    }
}

class Program
{
    static void Main(string[] args)
    {
        //  Create injector
        IInjector injector = new InjectorContext(true);

        //  Add bindings
        UserData myUser = new UserData();
        myUser.name = "It is me";
        injector.AddBinding<UserData>().ToValue(myUser);

        //  Inject into targets
        MyWindow window = new MyWindow();
        injector.InjectInto(window);

        //  All set
        window.Show();
    }
}
```

## **Documentation**
---
### **Design Decisions**

- Only singleton bindings; every binded type will share same instance.
- Bind first, use later; after making first injection you can not make any more bindings and it will cause an error.

---
### **Creating Injector**

Constructor parameter ```shouldThrowException``` allows you to choose error handling behavior.

When set to ```true``` (recommended for almost all use cases, including production), ```InjectorContext``` throws an ```InjectionException``` immediately when an error occurs. Errors fail loudly at the call site so they are easy to find and fix.

When set to ```false```, ```InjectorContext``` records errors into an internal list instead of throwing. **This mode is for development and tests only.** Its purpose is to let you:

- collect every binding mistake in one pass during development, instead of fix-one-rerun-fix-one cycles, and
- assert on structured ```InjectionErrorType``` values from the test suite without wrapping every call in ```try/catch```.

In non-throwing mode the API contract is:

- ```AddBinding<T>()``` returns a no-op setter on error. Chained ```.ToValue(...)```, ```.ToType<...>()```, and ```.RestrictToNamespace(...)``` calls are safe but do nothing.
- ```GetInstance<T>()``` returns ```default(T)``` (i.e. ```null``` for reference types) on error. Callers must check ```HasErrors``` before using the result.
- After your bind phase, check ```HasErrors``` and inspect ```ErrorCount``` / ```GetError(i)``` to see what went wrong.

Do not ship non-throwing mode to production unless you have a wrapper that checks ```HasErrors``` between operations.

---
### **Adding Bindings**

Binding is implemented as a fluent api to make it easy to write and read.

To add a new binding you start by calling ```AddBinding``` generic method with the ***binding type*** that you want to inject in classes.

```
injector.AddBinding<MyClass>()
```

It returns an object with two methods.

```ToValue``` method allows you to bind an already instantiated ***provider object*** to ***binding type***.

```
MyClass myClass = new MyClass();
injector.AddBinding<MyClass>().ToValue(myClass);
```

```ToType``` method allows you to bind a ***provider type*** that will be instantiated the first time it is needed.

```
injector.AddBinding<MyClass>().ToType<MyClass>();
```

*PS: ***Provider type*** must have a parameterless constructor or a constructor that has default values for all off its parameters.*

#### **Assignable and multiple bindings**

You can bind a ***binding type*** to assignable ***provider type*** or ***value*** (interface implementations to interfaces, child classes to parent classes).

Lets say you have a ```Setting``` class that implements read only ```ISettings``` interface.

```
interface ISettings
{
    float Volume { get; }
}

class Settings : ISettings
{
    public float Volume { get; set; }
}
```

You can use interface as ***binding type*** since its implementations are assignable to it.  

```
injector.AddBinding<ISettings>().ToType<Settings>();
```

Actually you can bind multiple ***binding types*** to same value or ***provider type*** if they are assignable. They will share the same instance of ```Settings``` class.

```
injector.AddBinding<Settings>().ToType<Settings>();
injector.AddBinding<ISettings>().ToType<Settings>();
```

Some uses of this practice are;
- Using read only and mutable interfaces of a type to prevent accedental writes (e.g. injecting ```Settings``` in ```SettingsWindow```, injecting readonly ```ISettings``` interface everywhere else)
- Binding platform specific implementations and to a common interface to provide abstraction (e.g. injecting IPlatform in everywhere, binding it to WindowsPlatform or LinuxPlatform implementations depending on your target)
- Using debugging friendly implementations of interfaces to find or isolate errors faster (e.g. an implementaion that adds verbose logging)
- Provide a dummy implementations of a class before its actual implementation is available (e.g. injecting IHighScoreServer, using MockHighScoreServer until it is provided )

#### **Binding Restrictions**

TBA

---
### **Injection**

After you complete your injections all you have to do is to using ```InjectInto``` to target objects. This will set all fields and properties that has ```Inject``` attribute.

```
injector.InjectInto(window);
injector.InjectInto(settingsPanel);
injector.InjectInto(playerManager);
```

### **Manually getting instances**

Other than injections ```InjectorContext``` has methods to access instances directly. Those are usefully for initilization methods after bindings are completed.

```T GetInstance<T>()``` methods gets the instance of that ***bindind type***

```
injector.GetInstance<ISaveManager>().LoadSaves();
```

```IEnumerator<T> GetAssignableInstances<T>()``` method checks all ***provider types*** and values and collect if they are assignable to given type;

```
IEnumerator<IManager> managers = injector.GetAssignableInstances<IManager>();

while(managers.MoveNext())
{
    managers.Current.Initialize();
}
```

### **Error Handling**

When `InjectorContext` is set to NOT to throw errors in constructor you can use ```ErrorCount``` property and ```GetError()``` method any time to inspect stored errors in ```InjectorContext```.
