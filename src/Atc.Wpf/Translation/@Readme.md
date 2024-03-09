# Translation Component / Extension

Localizing your application in .NET involves adapting it to different languages and regions. This process is streamlined by leveraging the CurrentUICulture property of the Thread class, which determines the UI culture for any thread. The .NET framework uses this setting to select the appropriate resources for that culture. Resources are typically stored in .resx files, which contain translations for various strings used within your application. When you build your application, these .resx files are compiled into satellite assemblies - separate assemblies that are specific to each culture.

## Satellite Assemblies and .resx Files

Satellite assemblies are essential for managing localized resources. They allow you to deploy applications with support for multiple languages by simply including the appropriate satellite assemblies, without modifying the main application assembly. Each satellite assembly contains resources for a specific culture, identified by its locale (e.g., "en-US", "fr-FR"). This approach keeps your application modular and simplifies the addition of new languages.

## CurrentUICulture in Localization

The CurrentUICulture property plays a pivotal role in resource selection. By setting this property, you direct the .NET runtime to retrieve the correct localized resources. This property should match the user's preferred language, which can be determined automatically or allowed to be selected manually in the application's settings.

```csharp
Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
```

This code snippet sets the current UI culture to Spanish (Spain), instructing the .NET runtime to use resources from the satellite assembly associated with this culture.

## XAML and C# Localization Usage

Effortlessly localize your application by binding text to a resource assembly containing the necessary translations, adhering to the standard .NET Globalization and Localization approach. This process is enhanced by a XAML markup extension for efficient key-based resource retrieval.

## XAML Usage

To be able to use resource translation lookups in XAML, 
Atc has a `ResxExtension`, which is a markup extension.

### Setup

First, declare the namespace in your root element:

```xml
<UserControl
    xmlns:atcTranslation="https://github.com/atc-net/atc-wpf/tree/main/schemas/translations">
```

### Basic Binding

Bind directly to a resource key in a .resx file, specifying its full namespace:

```xml
<TextBlock
    Text="{atcTranslation:Resx ResxName=Atc.Wpf.Sample.Resource.Word, Key=About}" />
```

### Advanced Binding

Enhance the basic binding by adding prefixes and suffixes to the translated text:

```xml
<TextBlock
    Text="{atcTranslation:Resx ResxName=Atc.Wpf.Sample.Resource.Word, Key=About, Prefix='foo', Suffix='bar'}" />
```

## C# Usage

### Retrieve Translated Value

Access a translated value by key from a `.resx` file, using its full namespace:

```csharp
var value = Wpf.Sample.Resource.Word.About
```

### Apply Prefix and Suffix

Concatenate a prefix and suffix to the translated value for dynamic text manipulation:

```csharp
var value = $"foo {Wpf.Sample.Resource.Word.About} bar";
```

By understanding and utilizing CurrentUICulture, satellite assemblies, and .resx files, 
developers can create highly adaptable .NET applications that cater to a global audience.
