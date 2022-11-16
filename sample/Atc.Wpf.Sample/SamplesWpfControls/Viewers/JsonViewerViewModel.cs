namespace Atc.Wpf.Sample.SamplesWpfControls.Viewers;

public class JsonViewerViewModel : ViewModelBase
{
    private string jsonData = string.Empty;

    public JsonViewerViewModel()
    {
        JsonData = "{\r\n  \"array\": [\r\n    1,\r\n    2,\r\n    3\r\n  ],\r\n  \"boolean\": true,\r\n  \"color\": \"gold\",\r\n  \"null\": null,\r\n  \"number\": 123,\r\n  \"object\": {\r\n    \"a\": \"b\",\r\n    \"c\": \"d\"\r\n  },\r\n  \"string\": \"Hello World\"\r\n}";
    }

    public string JsonData
    {
        get => jsonData;
        set
        {
            jsonData = value;
            RaisePropertyChanged();
        }
    }
}