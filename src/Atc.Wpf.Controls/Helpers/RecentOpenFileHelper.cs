namespace Atc.Wpf.Controls.Helpers;

public static class RecentOpenFileHelper
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    public static IList<RecentOpenFileViewModel> Load(
        DirectoryInfo applicationDataDirectory)
    {
        ArgumentNullException.ThrowIfNull(applicationDataDirectory);

        var recentOpenFilesFile = System.IO.Path.Combine(applicationDataDirectory.FullName, AtcFileNameConstants.RecentOpenFilesFileName);
        if (!File.Exists(recentOpenFilesFile))
        {
            return [];
        }

        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.PropertyNamingPolicy = null;

        var recentFiles = new List<RecentOpenFileViewModel>();

        try
        {
            var json = File.ReadAllText(recentOpenFilesFile);

            var recentOpenFilesOption = JsonSerializer.Deserialize<RecentOpenFilesOption>(
                json,
                jsonSerializerOptions) ?? throw new IOException($"Invalid format in {recentOpenFilesFile}");

            recentFiles.AddRange(
                from recentOpenFile
                in recentOpenFilesOption.RecentOpenFiles.OrderByDescending(x => x.TimeStamp)
                where File.Exists(recentOpenFile.FilePath)
                select new RecentOpenFileViewModel(
                    applicationDataDirectory,
                    recentOpenFile.TimeStamp,
                    recentOpenFile.FilePath));
        }
        catch
        {
            // Skip
        }

        return recentFiles;
    }

    public static void Save(
        DirectoryInfo applicationDataDirectory,
        ObservableCollectionEx<RecentOpenFileViewModel> recentOpenFiles)
    {
        ArgumentNullException.ThrowIfNull(applicationDataDirectory);

        var recentOpenFilesOption = new RecentOpenFilesOption();
        foreach (var vm in recentOpenFiles.OrderByDescending(x => x.TimeStamp))
        {
            var item = new RecentOpenFileOption
            {
                TimeStamp = vm.TimeStamp,
                FilePath = vm.File,
            };

            if (recentOpenFilesOption.RecentOpenFiles.FirstOrDefault(x => x.FilePath == item.FilePath) is not null)
            {
                continue;
            }

            if (!File.Exists(item.FilePath))
            {
                continue;
            }

            recentOpenFilesOption.RecentOpenFiles.Add(item);
        }

        var recentOpenFilesFilePath = System.IO.Path.Combine(applicationDataDirectory.FullName, AtcFileNameConstants.RecentOpenFilesFileName);
        if (!Directory.Exists(applicationDataDirectory.FullName))
        {
            Directory.CreateDirectory(applicationDataDirectory.FullName);
        }

        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.PropertyNamingPolicy = null;

        var json = JsonSerializer.Serialize(recentOpenFilesOption, jsonSerializerOptions);
        File.WriteAllText(recentOpenFilesFilePath, json);
    }
}