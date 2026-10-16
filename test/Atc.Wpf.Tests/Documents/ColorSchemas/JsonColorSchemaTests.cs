namespace Atc.Wpf.Tests.Documents.ColorSchemas;

public sealed class JsonColorSchemaTests
{
    [StaTheory]
    [InlineData(ThemeMode.Light)]
    [InlineData(ThemeMode.Dark)]
    public void AllBrushes_AreFrozen(ThemeMode themeMode)
    {
        try
        {
            JsonColorSchema.SetThemeMode(themeMode);

            var brushes = typeof(JsonColorSchema)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => typeof(Brush).IsAssignableFrom(p.PropertyType))
                .Select(p => (p.Name, Brush: (Brush)p.GetValue(obj: null)!))
                .ToList();

            Assert.NotEmpty(brushes);
            Assert.All(brushes, b => Assert.True(b.Brush.IsFrozen, $"{b.Name} is not frozen"));
        }
        finally
        {
            JsonColorSchema.SetThemeMode(ThemeMode.Light);
        }
    }
}