using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace PingToLaowuLaowu.Scripts;

[ModInitializer(nameof(Init))]
public static class Entry
{
    public const string ModId = "PingToLaowuLaowu";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        new Harmony("Mornux.PingToLaowuLaowu").PatchAll(assembly);
        Logger.Info("Mod initialized!");
    }
}
