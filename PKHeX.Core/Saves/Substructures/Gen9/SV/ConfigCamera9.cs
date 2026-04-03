using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ConfigCamera9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Raw)
{
    // Structure: u32
    /* CameraSupport:1 | On = 0, Off = 1
     * CameraInterpolation:1 | Slow = 0, Normal = 1
     * CameraDistance:2 | Close = 0, Normal = 1, Far = 2
     * ControlsWhileFlying:1 | Regular = 0, Inverted = 1  (bit 4, inferred from layout)
     *
     * Remaining 27 bits unused?
     */

    public int ConfigValue
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, value);
    }

    private const int DefaultValue = 0x00000002;

    public void Reset() => ConfigValue = DefaultValue;

    public ConfigOption9 CameraSupport { get => (ConfigOption9)((ConfigValue >> 0) & 1); set => ConfigValue = (ConfigValue & ~(1 << 0)) | ((((int)value) & 1) << 0); }
    public CameraInterpolation9 CameraInterpolation { get => (CameraInterpolation9)((ConfigValue >> 1) & 1); set => ConfigValue = (ConfigValue & ~(1 << 1)) | ((((int)value) & 1) << 1); }
    public CameraDistance9 CameraDistance { get => (CameraDistance9)(ConfigValue >> 2); set => ConfigValue = (ConfigValue & ~(0b11 << 2)) | (((int)value & 0b11) << 2); }

    /// <summary>
    /// Controls while Flying camera inversion. Regular = 0, Inverted = 1.
    /// NOTE: Bit 4 position is inferred from UI layout order, not directly observed via save comparison.
    /// </summary>
    public ConfigOption9 ControlsWhileFlying { get => (ConfigOption9)((ConfigValue >> 4) & 1); set => ConfigValue = (ConfigValue & ~(1 << 4)) | ((((int)value) & 1) << 4); }
}

public enum CameraInterpolation9 : byte
{
    Slow = 0,
    Normal = 1,
}

public enum CameraDistance9 : byte
{
    Close = 0,
    Normal = 1,
    Far = 2,
}
