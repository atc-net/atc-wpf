namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelNetworkAddressBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelIPAddressBox_ValidatesIpAddresses()
    {
        UseEnglishUi();
        var sut = new LabelIPAddressBox { TriggerOnlyOnLostFocus = false };

        Assert.Equal(TextBoxValidationRuleType.IPAddress, sut.ValidationFormat);
        sut.Text = "fe80::1";
        Assert.True(sut.IsValid());
        sut.Text = "not-an-address";
        Assert.Equal("Invalid IP address", sut.ValidationText);
    }

    [StaFact]
    public void LabelIPRangeBox_ValidatesCidrRanges()
    {
        UseEnglishUi();
        var sut = new LabelIPRangeBox { TriggerOnlyOnLostFocus = false };

        Assert.Equal(TextBoxValidationRuleType.IPv4Cidr, sut.ValidationFormat);
        sut.Text = "192.168.1.0/24";
        Assert.True(sut.IsValid());
        sut.Text = "192.168.1.0";
        Assert.Equal("Invalid IPv4 range (CIDR), such as 192.168.1.0/24", sut.ValidationText);
    }

    [StaFact]
    public void LabelMacAddressBox_ValidatesMacAddresses()
    {
        UseEnglishUi();
        var sut = new LabelMacAddressBox { TriggerOnlyOnLostFocus = false };

        Assert.Equal(TextBoxValidationRuleType.MacAddress, sut.ValidationFormat);
        sut.Text = "00:11:22:33:44:55";
        Assert.True(sut.IsValid());
        sut.Text = "00:11:22";
        Assert.Equal("Invalid MAC address", sut.ValidationText);
    }

    [StaFact]
    public void ValidationFormat_SetInXaml_WinsOverThePreset()
    {
        var sut = new LabelIPAddressBox { ValidationFormat = TextBoxValidationRuleType.IPv4Address };

        Assert.Equal(TextBoxValidationRuleType.IPv4Address, sut.ValidationFormat);
    }

    private static void UseEnglishUi()
        => CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}