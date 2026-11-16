namespace Atc.Wpf.Displays;

/// <summary>
/// Decodes the I2C write transactions an ESP32 or Raspberry Pi sends to an SSD1306 or SH1106 module
/// into the command and data feed of an <see cref="IGraphicDisplayController"/>.
/// </summary>
/// <remarks>
/// <para>
/// Every transaction (without the address byte) starts with a control byte: bit 7 is Co, bit 6 is D/C (the other bits are ignored).
/// With Co = 0 the rest of the transaction is a stream of commands (D/C = 0) or data (D/C = 1);
/// with Co = 1 exactly one byte follows, then the next control byte. Consecutive commands are passed on in one
/// <see cref="IGraphicDisplayController.WriteCommands"/> call and consecutive data bytes in one
/// <see cref="IGraphicDisplayController.WriteData"/> call, in the order they were sent.
/// An empty transaction, or a control byte with nothing after it, is ignored. The decoder keeps no state between transactions.
/// </para>
/// <para>
/// The decoder adds no locking: it is not thread-safe beyond the guarantees of the controller it feeds, so feed it from one thread.
/// </para>
/// </remarks>
public sealed class Ssd1306I2cDecoder
{
    private const byte ContinuationBit = 0x80;
    private const byte DataBit = 0x40;

    private readonly IGraphicDisplayController controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ssd1306I2cDecoder"/> class.
    /// </summary>
    /// <param name="controller">The controller the decoded commands and data are applied to.</param>
    public Ssd1306I2cDecoder(IGraphicDisplayController controller)
    {
        ArgumentNullException.ThrowIfNull(controller);

        this.controller = controller;
    }

    /// <summary>
    /// Decodes one I2C write transaction.
    /// </summary>
    /// <param name="transfer">The bytes of the transaction, without the address byte, starting with a control byte.</param>
    public void Write(ReadOnlySpan<byte> transfer)
    {
        if (transfer.Length < 2)
        {
            return;
        }

        var rented = System.Buffers.ArrayPool<byte>.Shared.Rent(transfer.Length);
        try
        {
            var runLength = 0;
            var runIsData = false;
            var index = 0;

            while (index < transfer.Length)
            {
                var control = transfer[index++];
                var isData = (control & DataBit) != 0;
                int count;

                if ((control & ContinuationBit) == 0)
                {
                    count = transfer.Length - index;
                }
                else
                {
                    count = index < transfer.Length ? 1 : 0;
                }

                if (count == 0)
                {
                    break;
                }

                if (runLength > 0 && runIsData != isData)
                {
                    Flush(rented.AsSpan(0, runLength), runIsData);
                    runLength = 0;
                }

                runIsData = isData;
                transfer.Slice(index, count).CopyTo(rented.AsSpan(runLength));
                runLength += count;
                index += count;
            }

            if (runLength > 0)
            {
                Flush(rented.AsSpan(0, runLength), runIsData);
            }
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private void Flush(
        ReadOnlySpan<byte> run,
        bool isData)
    {
        if (isData)
        {
            controller.WriteData(run);
        }
        else
        {
            controller.WriteCommands(run);
        }
    }
}