namespace Atc.Wpf.Forms.Tests.FontEditing;

public sealed class FontPickerStorageTests
{
    [Fact]
    public void Default_Current_is_an_InMemoryFontPickerStorage()
    {
        var original = FontPickerStorage.Current;

        try
        {
            // Reset to known default by re-assigning a fresh in-memory instance.
            FontPickerStorage.Current = new InMemoryFontPickerStorage();

            Assert.IsType<InMemoryFontPickerStorage>(FontPickerStorage.Current);
        }
        finally
        {
            FontPickerStorage.Current = original;
        }
    }

    [Fact]
    public void Setting_Current_to_null_throws_ArgumentNullException()
    {
        var original = FontPickerStorage.Current;

        try
        {
            Assert.Throws<ArgumentNullException>(() =>
                FontPickerStorage.Current = null!);
        }
        finally
        {
            FontPickerStorage.Current = original;
        }
    }

    [Fact]
    public void Custom_storage_replaces_the_Current_instance()
    {
        var original = FontPickerStorage.Current;
        var custom = new InMemoryFontPickerStorage();

        try
        {
            FontPickerStorage.Current = custom;

            Assert.Same(custom, FontPickerStorage.Current);
        }
        finally
        {
            FontPickerStorage.Current = original;
        }
    }
}