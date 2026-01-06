// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls.Internal.Helpers;

internal static class EndpointBoxValidationHelper
{
    public static TextBoxValidationRuleType MapNetworkProtocolToValidationType(
        NetworkProtocolType protocol)
        => protocol switch
        {
            NetworkProtocolType.Ftp => TextBoxValidationRuleType.Ftp,
            NetworkProtocolType.Ftps => TextBoxValidationRuleType.Ftps,
            NetworkProtocolType.Http => TextBoxValidationRuleType.Http,
            NetworkProtocolType.Https => TextBoxValidationRuleType.Https,
            NetworkProtocolType.OpcTcp => TextBoxValidationRuleType.OpcTcp,
            NetworkProtocolType.Tcp => TextBoxValidationRuleType.Tcp,
            NetworkProtocolType.Udp => TextBoxValidationRuleType.Udp,
            _ => TextBoxValidationRuleType.None,
        };

    public static TextBoxValidationRuleType MapNetworkValidationRuleToValidationType(
        NetworkValidationRule validationRule)
        => validationRule switch
        {
            NetworkValidationRule.IPAddress => TextBoxValidationRuleType.IPAddress,
            NetworkValidationRule.IPv4Address => TextBoxValidationRuleType.IPv4Address,
            NetworkValidationRule.IPv6Address => TextBoxValidationRuleType.IPv6Address,
            _ => TextBoxValidationRuleType.None,
        };

    public static (bool IsValid, string ErrorMessage) Validate(
        NetworkProtocolType networkProtocol,
        string? value,
        bool allowNull = false)
    {
        if (allowNull &&
            string.IsNullOrEmpty(value))
        {
            return (IsValid: true, ErrorMessage: string.Empty);
        }

        var mappedRule = networkProtocol switch
        {
            NetworkProtocolType.None => TextBoxValidationRuleType.None,
            NetworkProtocolType.Ftp => TextBoxValidationRuleType.Ftp,
            NetworkProtocolType.Ftps => TextBoxValidationRuleType.Ftps,
            NetworkProtocolType.Http => TextBoxValidationRuleType.Http,
            NetworkProtocolType.Https => TextBoxValidationRuleType.Https,
            NetworkProtocolType.OpcTcp => TextBoxValidationRuleType.OpcTcp,
            NetworkProtocolType.Tcp => TextBoxValidationRuleType.Tcp,
            NetworkProtocolType.Udp => TextBoxValidationRuleType.Udp,
            _ => throw new SwitchCaseDefaultException(networkProtocol),
        };

        return TextBoxValidationHelper.Validate(
            mappedRule,
            value,
            allowNull);
    }
}