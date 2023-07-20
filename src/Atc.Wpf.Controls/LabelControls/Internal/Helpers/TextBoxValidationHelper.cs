// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls.Internal.Helpers;

internal static class TextBoxValidationHelper
{
    public static (bool IsValid, string ErrorMessage) Validate(
        TextBoxValidationRuleType validationRule,
        string? value,
        bool allowNull = false)
    {
        if (allowNull &&
            string.IsNullOrEmpty(value))
        {
            return (IsValid: true, ErrorMessage: string.Empty);
        }

        var isValid = true;
        var errorMessage = string.Empty;

        switch (validationRule)
        {
            case TextBoxValidationRuleType.None:
                break;
            case TextBoxValidationRuleType.Email:
                if (!value!.IsEmailAddress())
                {
                    isValid = false;
                    errorMessage = Validations.InvalidEmailAddress;
                }

                break;
            case TextBoxValidationRuleType.Ftp:
            case TextBoxValidationRuleType.Ftps:
            case TextBoxValidationRuleType.FtpOrFtps:
                ValidateFtpOrFtps(value, ref isValid, ref errorMessage, allowFtp: true, allowFtps: true);
                break;
            case TextBoxValidationRuleType.Http:
                ValidateHttpOrHttps(value, ref isValid, ref errorMessage, allowHttp: true, allowHttps: false);
                break;
            case TextBoxValidationRuleType.Https:
                ValidateHttpOrHttps(value, ref isValid, ref errorMessage, allowHttp: false, allowHttps: true);
                break;
            case TextBoxValidationRuleType.HttpOrHttps:
                ValidateHttpOrHttps(value, ref isValid, ref errorMessage, allowHttp: true, allowHttps: true);
                break;
            case TextBoxValidationRuleType.IPAddress:
                ValidateIpAddress(value, ref isValid, ref errorMessage, allowV4: true, allowV6: true);
                break;
            case TextBoxValidationRuleType.IPv4Address:
                ValidateIpAddress(value, ref isValid, ref errorMessage, allowV4: true, allowV6: false);
                break;
            case TextBoxValidationRuleType.IPv6Address:
                ValidateIpAddress(value, ref isValid, ref errorMessage, allowV4: false, allowV6: true);
                break;
            case TextBoxValidationRuleType.OpcTcp:
                ValidateOpcTcp(value, ref isValid, ref errorMessage);
                break;
            case TextBoxValidationRuleType.Tcp:
                ValidateTcp(value, ref isValid, ref errorMessage);
                break;
            case TextBoxValidationRuleType.Udp:
                ValidateUdp(value, ref isValid, ref errorMessage);
                break;
            default:
                throw new SwitchCaseDefaultException(validationRule);
        }

        return (isValid, errorMessage);
    }

    private static void ValidateFtpOrFtps(
        string? value,
        ref bool isValid,
        ref string errorMessage,
        bool allowFtp,
        bool allowFtps)
    {
        var validation = new UriAttribute(
            required: true,
            allowHttp: false,
            allowHttps: false,
            allowFtp: allowFtp,
            allowFtps: allowFtps,
            allowFile: false,
            allowOpcTcp: false);
        if (validation.IsValid(value))
        {
            return;
        }

        isValid = false;
        errorMessage = Validations.InvalidFtpUrl;
    }

    private static void ValidateHttpOrHttps(
        string? value,
        ref bool isValid,
        ref string errorMessage,
        bool allowHttp,
        bool allowHttps)
    {
        var validation = new UriAttribute(
            required: true,
            allowHttp: allowHttp,
            allowHttps: allowHttps,
            allowFtp: false,
            allowFtps: false,
            allowFile: false,
            allowOpcTcp: false);
        if (validation.IsValid(value))
        {
            return;
        }

        isValid = false;
        errorMessage = Validations.InvalidHttpUrl;
    }

    private static void ValidateIpAddress(
        string? value,
        ref bool isValid,
        ref string errorMessage,
        bool allowV4,
        bool allowV6)
    {
        if (IPAddress.TryParse(value, out var ipAddress))
        {
            if (allowV4 && allowV6)
            {
                return;
            }

            if (allowV4 && ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                return;
            }

            if (allowV6 && ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return;
            }
        }

        isValid = false;
        errorMessage = Validations.InvalidIpAddres;
    }

    private static void ValidateOpcTcp(
        string? value,
        ref bool isValid,
        ref string errorMessage)
    {
        var validation = new UriAttribute(
            required: true,
            allowHttp: false,
            allowHttps: false,
            allowFtp: false,
            allowFtps: false,
            allowFile: false,
            allowOpcTcp: true);
        if (validation.IsValid(value))
        {
            return;
        }

        isValid = false;
        errorMessage = Validations.InvalidOpcUaUrl;
    }

    private static void ValidateTcp(
        string? value,
        ref bool isValid,
        ref string errorMessage)
    {
        var validation = new UriAttribute(
            required: true,
            allowHttp: false,
            allowHttps: false,
            allowFtp: false,
            allowFtps: false,
            allowFile: false,
            allowOpcTcp: true);
        if (validation.IsValid(value))
        {
            return;
        }

        isValid = false;
        errorMessage = Validations.InvalidTcpUrl;
    }

    private static void ValidateUdp(
        string? value,
        ref bool isValid,
        ref string errorMessage)
    {
        var validation = new UriAttribute(
            required: true,
            allowHttp: false,
            allowHttps: false,
            allowFtp: false,
            allowFtps: false,
            allowFile: false,
            allowOpcTcp: true);
        if (validation.IsValid(value))
        {
            return;
        }

        isValid = false;
        errorMessage = Validations.InvalidTcpUrl;
    }
}