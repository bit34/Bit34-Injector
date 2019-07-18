# Minic DI (Dependency Injection) Library

This is the a minimal dependency injection library written in C#. 


## What is a dependency injection library?

To be added...

## Why should I need one?

To be added...

## A quick tutorial

Let's say you have a `Settings` class and you want to access its single instance in some parts of your project.

```
public interface ISettings
{
    float SoundVolume{ get; }
}

public class Settings : ISettings
{
    public float SoundVolume{ get; set; }

    public Settings()
    {
        SoundVolume = 1.0f;
    }
}
```
---
(Using an interface is not neccesary, we will discuss later why it will come in handly to implement an interface with readonly access.)

---

First, you have to add `[Inject]` attribute to the fields or properties in all classes that you want to gain access to `Settings` class instance. They can be public or private.
```
public class SettingsWindow
{
    [Inject]
    private Settings settings;

    //  other stuff
}

public class SoundPlayer
{
    [Inject]
    private Settings settings;

    //  other stuff
}
```

Second, you have to create an Injector instance and add bindings.
```
Injector injector = new Injector();
```
There are two options for adding a binding:

- You can bind a type to an already instantiated *assignable* instance.

```
Settings settings = new Settings();
settings.SoundVolume = 0.5f;

injector.AddBinding<Settings>().ToValue(settings);
```

- Or you can bind a type to another *assignable* type that has a default constructor which will be instantiated on first usage.

```
injector.AddBinding<Settings>().ToType<Settings>();
```

Now you can inject into objects with `[Inject]` attributes.

```
SettingsWindow window = new SettingsWindow();
SoundPlayer player = new SoundPlayer();

injector.InjectInto(window);
injector.InjectInto(player);
```

# Error handling

To be added...
