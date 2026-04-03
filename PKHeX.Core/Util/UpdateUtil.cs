using System;

namespace PKHeX.Core;

public static class UpdateUtil
{
    /// <summary>
    /// </summary>
    /// <remarks>Update checks against the upstream PKHeX repository are intentionally disabled in PKCompassHeX.</remarks>
    /// <returns>Always returns null; update checks are disabled for PKCompassHeX.</returns>
    public static Version? GetLatestPKHeXVersion()
    {
        return null;
    }
}
