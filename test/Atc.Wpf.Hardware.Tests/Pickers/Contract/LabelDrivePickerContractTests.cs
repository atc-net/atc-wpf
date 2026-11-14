namespace Atc.Wpf.Hardware.Tests.Pickers.Contract;

public sealed class LabelDrivePickerContractTests : LabelPickerContractTests<LabelDrivePicker, DrivePicker, DiskDriveInfo>
{
    protected override DiskDriveInfo CreateItem(int id)
        => new DiskDriveInfo($"{(char)('C' + id)}:", $"Drive {id}", DriveType.Fixed, isReady: true, totalSize: null, availableFreeSpace: null);

    protected override void SetState(
        DiskDriveInfo item,
        DeviceState state)
        => item.State = state;
}