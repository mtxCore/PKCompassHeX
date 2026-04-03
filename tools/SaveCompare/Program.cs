using PKHeX.Core;

/// <summary>
/// Compares decrypted SCBlock structures between a Compass and Vanilla SV save file.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        string compassPath = args.Length > 0 ? args[0] : "../../saves/Compass/main";
        string vanillaPath = args.Length > 1 ? args[1] : "../../saves/Vanilla/main";

        Console.WriteLine("=== PKCompassHeX Save File Comparator ===");
        Console.WriteLine();

        var compassData = File.ReadAllBytes(compassPath);
        var vanillaData = File.ReadAllBytes(vanillaPath);

        Console.WriteLine($"Compass file: {compassPath} ({compassData.Length} bytes, 0x{compassData.Length:X})");
        Console.WriteLine($"Vanilla file: {vanillaPath} ({vanillaData.Length} bytes, 0x{vanillaData.Length:X})");
        Console.WriteLine($"Size difference: {compassData.Length - vanillaData.Length} bytes");
        Console.WriteLine();

        bool compassHashValid = SwishCrypto.GetIsHashValid(compassData);
        bool vanillaHashValid = SwishCrypto.GetIsHashValid(vanillaData);
        Console.WriteLine($"Compass hash valid: {compassHashValid}");
        Console.WriteLine($"Vanilla hash valid: {vanillaHashValid}");
        Console.WriteLine();

        if (!compassHashValid || !vanillaHashValid)
        {
            Console.WriteLine("ERROR: One or both save files have invalid hashes. Cannot decrypt.");
            return;
        }

        Console.WriteLine("Decrypting save files...");
        var compassBlocks = SwishCrypto.Decrypt(compassData);
        var vanillaBlocks = SwishCrypto.Decrypt(vanillaData);

        Console.WriteLine($"Compass blocks: {compassBlocks.Count}");
        Console.WriteLine($"Vanilla blocks: {vanillaBlocks.Count}");
        Console.WriteLine();

        var compassDict = new Dictionary<uint, SCBlock>();
        var vanillaDict = new Dictionary<uint, SCBlock>();

        foreach (var block in compassBlocks)
            compassDict[block.Key] = block;
        foreach (var block in vanillaBlocks)
            vanillaDict[block.Key] = block;

        var compassOnlyKeys = compassDict.Keys.Except(vanillaDict.Keys).OrderBy(k => k).ToList();
        var vanillaOnlyKeys = vanillaDict.Keys.Except(compassDict.Keys).OrderBy(k => k).ToList();
        var sharedKeys = compassDict.Keys.Intersect(vanillaDict.Keys).OrderBy(k => k).ToList();

        Console.WriteLine($"=== Block Summary ===");
        Console.WriteLine($"Blocks only in Compass: {compassOnlyKeys.Count}");
        Console.WriteLine($"Blocks only in Vanilla: {vanillaOnlyKeys.Count}");
        Console.WriteLine($"Shared blocks: {sharedKeys.Count}");
        Console.WriteLine();

        // List Compass-only blocks
        if (compassOnlyKeys.Count > 0)
        {
            Console.WriteLine("=== Blocks ONLY in Compass ===");
            foreach (var key in compassOnlyKeys)
            {
                var block = compassDict[key];
                Console.WriteLine($"  Key=0x{key:X8} Type={block.Type} DataLength={block.Data.Length}");
                if (block.Data.Length > 0 && block.Data.Length <= 64)
                {
                    Console.WriteLine($"    Data: {Convert.ToHexString(block.Data.ToArray())}");
                }
                else if (block.Data.Length > 64)
                {
                    Console.WriteLine($"    Data (first 64): {Convert.ToHexString(block.Data[..64].ToArray())}...");
                }
            }
            Console.WriteLine();
        }

        // List S/V only blocks
        if (vanillaOnlyKeys.Count > 0)
        {
            Console.WriteLine("=== Blocks ONLY in Vanilla ===");
            foreach (var key in vanillaOnlyKeys)
            {
                var block = vanillaDict[key];
                Console.WriteLine($"  Key=0x{key:X8} Type={block.Type} DataLength={block.Data.Length}");
                if (block.Data.Length > 0 && block.Data.Length <= 64)
                {
                    Console.WriteLine($"    Data: {Convert.ToHexString(block.Data.ToArray())}");
                }
                else if (block.Data.Length > 64)
                {
                    Console.WriteLine($"    Data (first 64): {Convert.ToHexString(block.Data[..64].ToArray())}...");
                }
            }
            Console.WriteLine();
        }

        // Compare shared blocks for size/type differences 
        Console.WriteLine("=== Shared Blocks with differences (type/size) ===");
        int structDiffCount = 0;
        foreach (var key in sharedKeys)
        {
            var cb = compassDict[key];
            var vb = vanillaDict[key];

            bool typeDiff = cb.Type != vb.Type;
            bool sizeDiff = cb.Data.Length != vb.Data.Length;

            if (typeDiff || sizeDiff)
            {
                structDiffCount++;
                Console.WriteLine($"  Key=0x{key:X8}:");
                if (typeDiff)
                    Console.WriteLine($"    Type: Compass={cb.Type} vs Vanilla={vb.Type}");
                if (sizeDiff)
                    Console.WriteLine($"    Size: Compass={cb.Data.Length} vs Vanilla={vb.Data.Length}");
            }
        }
        if (structDiffCount == 0)
            Console.WriteLine("  (none)");
        Console.WriteLine();

        int dataDiffCount = 0;
        int dataMatchCount = 0;
        foreach (var key in sharedKeys)
        {
            var cb = compassDict[key];
            var vb = vanillaDict[key];

            if (cb.Type != vb.Type || cb.Data.Length != vb.Data.Length)
                continue;

            if (cb.Data.Length == 0)
            {
                // Bool blocks
                if (cb.Type != vb.Type)
                    dataDiffCount++;
                else
                    dataMatchCount++;
                continue;
            }

            if (!cb.Data.SequenceEqual(vb.Data))
                dataDiffCount++;
            else
                dataMatchCount++;
        }
        Console.WriteLine($"=== Data Comparison (same structure) ===");
        Console.WriteLine($"Blocks with different data: {dataDiffCount}");
        Console.WriteLine($"Blocks with identical data: {dataMatchCount}");
        Console.WriteLine();
        Console.WriteLine("=== Key Block Sizes ===");
        uint[] importantKeys = [
            0x0D66012C, // KBox
            0x3AA1A9AD, // KParty
            0x21C9BD44, // KItem
            0xE3E89BD1, // KMyStatus
            0x19722C89, // KBoxLayout
            0xEDAFF794, // KPlayTime
            0x0DEAAEBD, // KZukan
            0x0520A1B0, // KBCATRaidEnemyArray
            0xCAAC8800, // KTeraRaidPaldea
            0x100B93DA, // KTeraRaidDLC
            0x2482AD60, // KFieldItems
            0x173304D8, // KOverworld
            0x29B4AED2, // KSandwiches
            0x14C5A101, // KPictureProfileCurrent
        ];

        foreach (var key in importantKeys)
        {
            string cInfo = compassDict.TryGetValue(key, out var cb)
                ? $"Type={cb.Type}, Size={cb.Data.Length}"
                : "NOT PRESENT";
            string vInfo = vanillaDict.TryGetValue(key, out var vb)
                ? $"Type={vb.Type}, Size={vb.Data.Length}"
                : "NOT PRESENT";

            string label = key switch
            {
                0x0D66012C => "KBox",
                0x3AA1A9AD => "KParty",
                0x21C9BD44 => "KItem",
                0xE3E89BD1 => "KMyStatus",
                0x19722C89 => "KBoxLayout",
                0xEDAFF794 => "KPlayTime",
                0x0DEAAEBD => "KZukan",
                0x0520A1B0 => "KBCATRaidEnemy",
                0xCAAC8800 => "KTeraRaidPaldea",
                0x100B93DA => "KTeraRaidDLC",
                0x2482AD60 => "KFieldItems",
                0x173304D8 => "KOverworld",
                0x29B4AED2 => "KSandwiches",
                0x14C5A101 => "KPictureProfile",
                _ => "Unknown",
            };

            Console.WriteLine($"  {label} (0x{key:X8}):");
            Console.WriteLine($"    Compass: {cInfo}");
            Console.WriteLine($"    Vanilla: {vInfo}");
            if (cb is not null && vb is not null && cb.Data.Length != vb.Data.Length)
                Console.WriteLine($"    *** SIZE DIFFERS BY {cb.Data.Length - vb.Data.Length} bytes ***");
        }

        // Try to load as SAV9SV 
        Console.WriteLine();
        Console.WriteLine("=== Attempting SAV9SV load ===");
        try
        {
            var compassSav = new SAV9SV(compassData);
            Console.WriteLine($"Compass loaded successfully!");
            Console.WriteLine($"  OT: {compassSav.OT}");
            Console.WriteLine($"  Version: {compassSav.Version}");
            Console.WriteLine($"  SaveRevision: {compassSav.SaveRevision} ({compassSav.SaveRevisionString})");
            Console.WriteLine($"  Money: {compassSav.Money}");
            Console.WriteLine($"  PlayedHours: {compassSav.PlayedHours}:{compassSav.PlayedMinutes:D2}:{compassSav.PlayedSeconds:D2}");
            Console.WriteLine($"  Party count: {compassSav.PartyCount}");
            Console.WriteLine($"  Box count: {compassSav.BoxCount}");

            // List party Pokemon
            Console.WriteLine("  Party Pokemon:");
            for (int i = 0; i < compassSav.PartyCount; i++)
            {
                var pk = compassSav.GetPartySlotAtIndex(i);
                Console.WriteLine($"    Slot {i}: {(Species)pk.Species} Lv.{pk.CurrentLevel} ({pk.Nickname})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Compass load FAILED: {ex.Message}");
        }

        try
        {
            var vanillaSav = new SAV9SV(vanillaData);
            Console.WriteLine($"\nVanilla loaded successfully!");
            Console.WriteLine($"  OT: {vanillaSav.OT}");
            Console.WriteLine($"  Version: {vanillaSav.Version}");
            Console.WriteLine($"  SaveRevision: {vanillaSav.SaveRevision} ({vanillaSav.SaveRevisionString})");
            Console.WriteLine($"  Money: {vanillaSav.Money}");
            Console.WriteLine($"  PlayedHours: {vanillaSav.PlayedHours}:{vanillaSav.PlayedMinutes:D2}:{vanillaSav.PlayedSeconds:D2}");
            Console.WriteLine($"  Party count: {vanillaSav.PartyCount}");
            Console.WriteLine($"  Box count: {vanillaSav.BoxCount}");

            // party Pokemon
            Console.WriteLine("  Party Pokemon:");
            for (int i = 0; i < vanillaSav.PartyCount; i++)
            {
                var pk = vanillaSav.GetPartySlotAtIndex(i);
                Console.WriteLine($"    Slot {i}: {(Species)pk.Species} Lv.{pk.CurrentLevel} ({pk.Nickname})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vanilla load FAILED: {ex.Message}");
        }
    }
}
