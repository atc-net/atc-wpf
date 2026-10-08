namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class EndpointBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void HostAndPort_ChangedWhileEditing_DoNotRaiseLostFocusEvents()
    {
        var sut = new EndpointBox { Host = "old.example.com" };
        var raised = new List<string>();
        sut.HostLostFocus += (_, _) => raised.Add(nameof(sut.HostLostFocus));
        sut.PortLostFocus += (_, _) => raised.Add(nameof(sut.PortLostFocus));
        sut.ValueLostFocus += (_, _) => raised.Add(nameof(sut.ValueLostFocus));
        GotFocus(sut.HostTextBox);

        sut.Host = "new.example.com";
        sut.Port = 8443;

        Assert.Empty(raised);
    }

    [StaFact]
    public void Host_Changed_StillRaisesHostChangedOnEveryChange()
    {
        var sut = new EndpointBox { Host = "a.example.com" };
        var raised = 0;
        sut.AddHandler(
            EndpointBox.HostChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, _) => raised++));

        sut.Host = "b.example.com";
        sut.Host = "c.example.com";

        Assert.Equal(2, raised);
    }

    [StaFact]
    public void HostEditor_LosesFocusAfterAChange_RaisesHostAndValueLostFocusWithOldAndNewValue()
    {
        var sut = new EndpointBox { NetworkProtocol = NetworkProtocolType.Http, Host = "old.example.com" };
        ValueChangedEventArgs<string?>? host = null;
        ValueChangedEventArgs<Uri?>? value = null;
        sut.HostLostFocus += (_, e) => host = e;
        sut.ValueLostFocus += (_, e) => value = e;
        GotFocus(sut.HostTextBox);
        sut.Host = "new";
        sut.Host = "new.example.com";

        LostFocus(sut.HostTextBox);

        Assert.NotNull(host);
        Assert.Equal("old.example.com", host.OldValue);
        Assert.Equal("new.example.com", host.NewValue);
        Assert.NotNull(value);
        Assert.Equal(new Uri("http://old.example.com"), value.OldValue);
        Assert.Equal(new Uri("http://new.example.com"), value.NewValue);
    }

    [StaFact]
    public void HostEditor_LosesFocusWithoutAChange_RaisesNothing()
    {
        var sut = new EndpointBox { Host = "same.example.com" };
        var raised = new List<string>();
        sut.HostLostFocus += (_, _) => raised.Add(nameof(sut.HostLostFocus));
        sut.PortLostFocus += (_, _) => raised.Add(nameof(sut.PortLostFocus));
        sut.ValueLostFocus += (_, _) => raised.Add(nameof(sut.ValueLostFocus));
        GotFocus(sut.HostTextBox);

        LostFocus(sut.HostTextBox);

        Assert.Empty(raised);
    }

    [StaFact]
    public void PortEditor_LosesFocusAfterAChange_RaisesPortLostFocusOnly()
    {
        var sut = new EndpointBox { Host = "example.com", Port = 8080 };
        ValueChangedEventArgs<int?>? port = null;
        var hostRaised = false;
        sut.PortLostFocus += (_, e) => port = e;
        sut.HostLostFocus += (_, _) => hostRaised = true;
        GotFocus(sut.PortIntegerBox);
        sut.Port = 9090;

        LostFocus(sut.PortIntegerBox);

        Assert.NotNull(port);
        Assert.Equal(8080, port.OldValue);
        Assert.Equal(9090, port.NewValue);
        Assert.False(hostRaised);
    }

    [StaFact]
    public void LabelEndpointBox_HostChanged_DoesNotRaiseHostLostFocus()
    {
        var sut = new LabelEndpointBox { Host = "old.example.com" };
        var raised = false;
        sut.HostLostFocus += (_, _) => raised = true;

        sut.Host = "new.example.com";

        Assert.False(raised);
    }

    [StaFact]
    public void LabelEndpointBox_InnerHostEditorLosesFocusAfterAChange_RaisesHostLostFocus()
    {
        var sut = new LabelEndpointBox { LabelText = "Server" };
        var inner = sut.InnerEndpointBox;
        inner.Host = "old.example.com";
        ValueChangedEventArgs<string?>? raised = null;
        sut.HostLostFocus += (_, e) => raised = e;
        GotFocus(inner.HostTextBox);
        inner.Host = "new.example.com";

        LostFocus(inner.HostTextBox);

        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Equal("old.example.com", raised.OldValue);
        Assert.Equal("new.example.com", raised.NewValue);
    }

    private static void GotFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.GotFocusEvent, editor));

    private static void LostFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent, editor));
}