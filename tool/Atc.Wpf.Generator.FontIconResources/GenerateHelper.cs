// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Generator.FontIconResources;

[SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public static class GenerateHelper
{
    public static IList<LogKeyValueItem> GenerateFontAwesomeBrand(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\fontawesome.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\fa-brands-400.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontAwesomeBrandType.cs")),
            "fa");
    }

    public static IList<LogKeyValueItem> GenerateFontAwesomeRegular(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\fontawesome.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\fa-regular-400.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontAwesomeRegularType.cs")),
            "fa");
    }

    public static IList<LogKeyValueItem> GenerateFontAwesomeSolid(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\fontawesome.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\fa-solid-900.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontAwesomeSolidType.cs")),
            "fa");
    }

    public static IList<LogKeyValueItem> GenerateBootstrap(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\bootstrap.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\glyphicons-halflings-regular.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontBootstrapType.cs")),
            "glyphicon");
    }

    public static IList<LogKeyValueItem> GenerateIco(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\icofont.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\icofont.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"IcoFontType.cs")),
            "icofont");
    }

    public static IList<LogKeyValueItem> GenerateMaterialDesign(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\materialdesignicons.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\materialdesignicons-webfont.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontMaterialDesignType.cs")),
            "mdi");
    }

    public static IList<LogKeyValueItem> GenerateWeather(
        DirectoryInfo resourcesFolder,
        DirectoryInfo outputEnumFolder)
    {
        ArgumentNullException.ThrowIfNull(resourcesFolder);
        ArgumentNullException.ThrowIfNull(outputEnumFolder);

        return GoCssParseAndGenerate(
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"css\\weather-icons.css")),
            new FileInfo(Path.Combine(resourcesFolder.FullName, @"fonts\weathericons-regular-webfont.ttf")),
            new FileInfo(Path.Combine(outputEnumFolder.FullName, @"FontWeatherType.cs")),
            "wi");
    }

    [SuppressMessage("Performance", "MA0065:Default ValueType.Equals or HashCode is used for struct equality", Justification = "OK.")]
    public static IList<LogKeyValueItem> GoCssParseAndGenerate(
        FileInfo cssFileInfo,
        FileInfo fontFileInfo,
        FileInfo outputFileInfo,
        string prefix)
    {
        ArgumentNullException.ThrowIfNull(cssFileInfo);
        ArgumentNullException.ThrowIfNull(fontFileInfo);
        ArgumentNullException.ThrowIfNull(outputFileInfo);
        ArgumentNullException.ThrowIfNull(prefix);

        if (!cssFileInfo.Exists)
        {
            throw new FileNotFoundException(cssFileInfo.FullName);
        }

        if (!fontFileInfo.Exists)
        {
            throw new FileNotFoundException(fontFileInfo.FullName);
        }

        if (!outputFileInfo.Exists)
        {
            Directory.CreateDirectory(outputFileInfo.Directory!.FullName);
        }

        var logItems = new List<LogKeyValueItem>();

        if (!prefix.StartsWith('.'))
        {
            prefix = "." + prefix;
        }

        var fontFamilies = Fonts.GetFontFamilies(fontFileInfo.FullName);
        if (fontFamilies.Count != 1)
        {
            throw new Exception("We expect 1 loaded font file");
        }

        var fontFamily = fontFamilies.First();
        var typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        string text;
        using (var reader = File.OpenText(cssFileInfo.FullName))
        {
            text = reader.ReadToEnd();
        }

        var allCompacts = GetCategorizedCharacterStringsWithoutWhiteSpaces(text);
        var collectedKeyValues = GetAllFontClassesWithContentValue(allCompacts, typeface, prefix);
        if (collectedKeyValues.Count <= 0)
        {
            return logItems;
        }

        using (var sw = new StreamWriter(outputFileInfo.FullName, append: false))
        {
            var enumTypeName = Path.GetFileNameWithoutExtension(outputFileInfo.FullName);
            sw.WriteLine("//------------------------------------------------------------------------------");
            sw.WriteLine("// <auto-generated>");
            sw.WriteLine("//     This code was generated by a tool: Atc.Wpf.Generator.FontIconResources");
            sw.WriteLine("//");
            sw.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            sw.WriteLine("//     the code is regenerated.");
            sw.WriteLine("// </auto-generated>");
            sw.WriteLine("//------------------------------------------------------------------------------");
            sw.WriteLine();
            sw.WriteLine("// ReSharper disable once CheckNamespace");
            sw.WriteLine("namespace Atc.Wpf.FontIcons;");
            sw.WriteLine();
            sw.WriteLine("/// <summary>");
            sw.WriteLine($"/// Enumeration: {enumTypeName}");
            sw.WriteLine("/// </summary>");
            sw.WriteLine($"public enum {enumTypeName}");
            sw.WriteLine("{");
            sw.WriteLine("    /// <summary>");
            sw.WriteLine("    /// Default None");
            sw.WriteLine("    /// </summary>");
            sw.WriteLine("    None = 0x0,");
            var lastPair = collectedKeyValues.Last();
            foreach (var pair in collectedKeyValues)
            {
                var enumDescription = CreateEnumDescription(pair.Key);
                var enumName = CreateEnumName(prefix, pair.Key);
                var enumValue = CreateEnumValue(pair.Value);
                var useReSharper = IsInconsistentNaming(enumName);
                sw.WriteLine();
                sw.WriteLine("    /// <summary>");
                sw.WriteLine($"    /// {enumName} - {enumValue} - \"{enumDescription}\"");
                sw.WriteLine("    /// </summary>");
                sw.WriteLine($"    [Description(\"{enumDescription}\")]");
                if (useReSharper)
                {
                    sw.WriteLine("    // ReSharper disable InconsistentNaming");
                }

                sw.WriteLine(pair.Equals(lastPair)
                    ? $"    {enumName} = {enumValue}"
                    : $"    {enumName} = {enumValue},");
                if (useReSharper)
                {
                    sw.WriteLine("    // ReSharper restore InconsistentNaming");
                }
            }

            sw.Write("}");
        }

        return logItems;
    }

    private static bool IsInconsistentNaming(
        string enumName)
    {
        if (enumName.StartsWith('_'))
        {
            return true;
        }

        return enumName.Length > 2 && char.IsUpper(enumName, 1);
    }

    private static string CreateEnumDescription(
        string key)
        => key
            .Replace(":before", string.Empty, StringComparison.Ordinal)
            .ToLower(GlobalizationConstants.EnglishCultureInfo);

    [SuppressMessage("Usage", "MA0011:IFormatProvider is missing", Justification = "OK.")]
    private static string CreateEnumName(
        string removePrefix,
        string key)
    {
        var prefix = removePrefix.Replace(".", string.Empty, StringComparison.Ordinal);
        var s = key
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .Replace(":before", string.Empty, StringComparison.Ordinal)
            .ToLower(GlobalizationConstants.EnglishCultureInfo);
        if (s.StartsWith(prefix, StringComparison.CurrentCulture))
        {
            s = s[prefix.Length..];
        }

        var sb = new StringBuilder();
        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == '-')
            {
                if (i >= s.Length - 2)
                {
                    continue;
                }

                sb.Append(s[i + 1].ToString().ToUpperInvariant());
                i++;
            }
            else
            {
                sb.Append(s[i]);
            }
        }

        s = sb.ToString();
        return char.IsDigit(s, 0)
            ? "_" + s
            : $"{char.ToUpper(s[0], GlobalizationConstants.EnglishCultureInfo)}{s[1..]}";
    }

    private static string CreateEnumValue(
        string key)
        => "0x" + key
            .Replace("\"", string.Empty, StringComparison.Ordinal)
            .Replace("\\", string.Empty, StringComparison.Ordinal);

    private static List<CategorisedCharacterString> GetCategorizedCharacterStringsWithoutWhiteSpaces(
        string text)
    {
        IEnumerable<CategorisedCharacterString> all = CSSParser.Parser.ParseCSS(text);
        return all.Where(x =>
            x.CharacterCategorisation != CharacterCategorisationOptions.Whitespace &&
            x.Value is not null).ToList();
    }

    private static Dictionary<string, string> GetAllFontClassesWithContentValue(
        IReadOnlyCollection<CategorisedCharacterString> allCompacts,
        Typeface typeface,
        string prefix)
    {
        var tmpKeyValueList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var allEndsWithBefore = allCompacts.Where(x =>
            x.Value.StartsWith(prefix, StringComparison.CurrentCulture) &&
            x.Value.EndsWith(":before", StringComparison.CurrentCulture));
        foreach (var before in allEndsWithBefore)
        {
            var allFromBefore = allCompacts.Where(x => x.IndexInSource > before.IndexInSource);
            var scopeList = new List<CategorisedCharacterString>();
            scopeList.AddRange(allFromBefore.TakeWhile(x => x.CharacterCategorisation != CharacterCategorisationOptions.CloseBrace));
            var properties = scopeList.Where(x => x.CharacterCategorisation == CharacterCategorisationOptions.SelectorOrStyleProperty);
            foreach (var s
                     in
                     from property
                         in properties
                     where property.Value.Equals("content", StringComparison.OrdinalIgnoreCase)
                     select scopeList.Find(x =>
                         x.IndexInSource > property.IndexInSource &&
                         x.CharacterCategorisation == CharacterCategorisationOptions.Value &&
                         x.Value.Length > 0)
                     into s
                     where !tmpKeyValueList.ContainsKey(before.Value)
                     select s)
            {
                tmpKeyValueList.Add(before.Value, s!.Value);
            }
        }

        typeface.TryGetGlyphTypeface(out var glyph);
        var characterMap = glyph.CharacterToGlyphMap;

        var tmpKeyValueFontList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in tmpKeyValueList
                     .ToDictionary(x => x.Key.Replace(":before", string.Empty, StringComparison.Ordinal), x => x.Value, StringComparer.OrdinalIgnoreCase))
        {
            var s = item.Value.Replace("\"", string.Empty, StringComparison.Ordinal).Replace("\\", string.Empty, StringComparison.Ordinal);
            var ik = int.Parse(s, NumberStyles.HexNumber, GlobalizationConstants.EnglishCultureInfo);
            if (characterMap.ContainsKey(ik))
            {
                tmpKeyValueFontList.Add(item.Key, item.Value);
            }
        }

        return tmpKeyValueFontList
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
    }
}