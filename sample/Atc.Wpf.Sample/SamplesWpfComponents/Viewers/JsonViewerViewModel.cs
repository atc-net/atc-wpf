namespace Atc.Wpf.Sample.SamplesWpfComponents.Viewers;

public sealed class JsonViewerViewModel : ViewModelBase
{
    private string jsonData = string.Empty;

    public JsonViewerViewModel()
    {
        JsonData = "{\r\n  \"array\": [\r\n    1,\r\n    2,\r\n    3\r\n  ],\r\n  \"boolean\": true,\r\n  \"color\": \"gold\",\r\n  \"null\": null,\r\n  \"number\": 123,\r\n  \"object\": {\r\n    \"a\": \"b\",\r\n    \"c\": \"d\"\r\n  },\r\n  \"string\": \"Hello World\",\r\n  \"date\": \"2021-11-04T00:00:00\",\r\n  \"guid\": \"c0a29e8d-5a6e-4f70-ba54-1b72c7bf9f4f\",\r\n  \"url\": \"http://google.com/\"\r\n}";
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