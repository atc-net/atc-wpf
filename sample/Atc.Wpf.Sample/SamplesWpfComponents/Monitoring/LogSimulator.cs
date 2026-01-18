#pragma warning disable CA5394
#pragma warning disable SCS0005
namespace Atc.Wpf.Sample.SamplesWpfComponents.Monitoring;

public static class LogSimulator
{
    private static readonly Dictionary<string, LogCategoryType> LogMessages = new(StringComparer.Ordinal)
    {
        { "User 'john.doe' logged in successfully", LogCategoryType.Information },
        { "User 'jane.doe' failed login attempt", LogCategoryType.Warning },
        { "API call to /api/products returned 200 OK", LogCategoryType.Information },
        { "API call to /api/orders returned 500 Internal Server Error", LogCategoryType.Error },
        { "User 'admin' changed system settings", LogCategoryType.Critical },
        { "Scheduled task 'DataBackup' completed successfully", LogCategoryType.Information },
        { "API call to /api/users returned 404 Not Found", LogCategoryType.Warning },
        { "User 'john.doe' updated profile", LogCategoryType.Information },
        { "Database connection lost unexpectedly", LogCategoryType.Critical },
        { "Service 'EmailService' started", LogCategoryType.Service },
        { "Service 'PaymentGateway' encountered timeout", LogCategoryType.Error },
        { "User 'jane.doe' logged out", LogCategoryType.Information },
        { "Security alert: Multiple failed login attempts detected", LogCategoryType.Security },
        { "Audit log: User 'john.doe' exported data", LogCategoryType.Audit },
        { "UI action: Button 'Submit' clicked", LogCategoryType.UI },
        { "Debug info: Variable 'x' value is 42", LogCategoryType.Debug },
        { "Trace: Entered method 'CalculateTotal'", LogCategoryType.Trace },
        { "User 'guest' attempted unauthorized access", LogCategoryType.Security },
        { "Audit log: User 'admin' reset password for 'jane.doe'", LogCategoryType.Audit },
        { "API call to /api/inventory updated stock count", LogCategoryType.Information },
        { "User 'john.doe' uploaded file 'report.pdf'", LogCategoryType.Information },
        { "Service 'NotificationService' stopped unexpectedly", LogCategoryType.Error },
        { "API call to /api/login returned auth token", LogCategoryType.Information },
        { "User 'alice' registered a new account", LogCategoryType.Information },
        { "UI error: Dialog displayed due to network issue", LogCategoryType.Error },
        { "Debug info: Memory usage at 70%", LogCategoryType.Debug },
        { "Trace: Exiting function 'ProcessOrder'", LogCategoryType.Trace },
        { "User 'bob' changed profile picture", LogCategoryType.Information },
        { "API call to /api/orders experienced delay", LogCategoryType.Warning },
        { "Service 'CacheService' cleared cache successfully", LogCategoryType.Information },
        { "Audit log: User 'alice' downloaded sensitive report", LogCategoryType.Audit },
        { "User 'charlie' updated billing information", LogCategoryType.Information },
        { "Security warning: Unauthorized API access attempt detected", LogCategoryType.Security },
        { "UI action: Dashboard loaded", LogCategoryType.Information },
        { "Critical error: System encountered unrecoverable issue", LogCategoryType.Critical },
        { "User 'dave' subscribed to newsletter", LogCategoryType.Information },
        { "API call to /api/comments returned 201 Created", LogCategoryType.Information },
        { "Debug info: Received unexpected null value", LogCategoryType.Debug },
        { "Service 'LoggingService' initialized", LogCategoryType.Service },
        { "Trace: Starting transaction processing", LogCategoryType.Trace },
        { "User 'eve' reset account settings", LogCategoryType.Information },
        { "API call to /api/search returned ambiguous results", LogCategoryType.Warning },
        { "Audit log: User 'bob' performed data deletion", LogCategoryType.Audit },
        { "User 'frank' encountered authentication error", LogCategoryType.Error },
        { "Security alert: Possible SQL injection attempt detected", LogCategoryType.Security },
        { "UI action: Dropdown list populated", LogCategoryType.Information },
        { "API call to /api/reports generated unexpected data", LogCategoryType.Warning },
        { "Service 'BackupService' completed incremental backup", LogCategoryType.Information },
        { "Critical alert: System overload detected in Analytics module", LogCategoryType.Critical },
        { "User 'grace' successfully updated preferences", LogCategoryType.Information },
    };

    private static readonly string[] Areas =
    [
        "Authentication",
        "UserManagement",
        "Inventory",
        "Payment",
        "System",
        "Logging",
        "Notification",
        "Database",
        "UI",
        "API"
    ];

    private static readonly Random Random = new();

    public static void SendRandomLogMessage()
    {
        var area = Areas[Random.Next(Areas.Length)];
        var randomIndex = Random.Next(LogMessages.Count);
        var (message, logCategory) = LogMessages.ElementAt(randomIndex);

        Messenger.Default.Send(
            new ApplicationEventEntry(
                logCategory,
                area,
                message));
    }
}