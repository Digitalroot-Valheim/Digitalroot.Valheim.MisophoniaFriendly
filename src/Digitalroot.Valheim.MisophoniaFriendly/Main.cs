using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using System;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.MisophoniaFriendly
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  [NetworkCompatibility(CompatibilityLevel.NotEnforced)]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    public static Main Instance;

    public readonly ConfigEntry<int> NexusId;

    public Main()
    {
      Instance = this;
      NexusId  = Config.Bind("General", "NexusID",   1667, new ConfigDescription("Nexus mod ID for updates.", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = false, ReadOnly = true }));
      #if DEBUG
      EnableTrace = true;
      Log.RegisterSource(Instance);
      #else
      EnableTrace = false;
      #endif
      Log.RegisterSource(Instance);
      Log.Trace(Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");
        if (Common.Utils.IsHeadless()) return;
        PrefabManager.OnVanillaPrefabsAvailable += PrefabManagerOnVanillaPrefabsAvailable;
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    [HarmonyWrapSafe]
    private void PrefabManagerOnVanillaPrefabsAvailable()
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

        var prefabNames = new[]
        {
          Common.Names.Vanilla.PrefabNames.SfxPukeMale
          , Common.Names.Vanilla.PrefabNames.SfxPukeFemale
          , Common.Names.Vanilla.PrefabNames.SfxCreatureConsume
          , Common.Names.Vanilla.PrefabNames.SfxEat
          , Common.Names.Vanilla.PrefabNames.SfxChickenEat
          , Common.Names.Vanilla.PrefabNames.SfxHareIdleEating
        };

        foreach (var prefabName in prefabNames)
        {
          var sfx = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
          var audioSource = sfx.GetComponent<AudioSource>();
          audioSource.mute = true;
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
      finally
      {
        PrefabManager.OnVanillaPrefabsAvailable += PrefabManagerOnVanillaPrefabsAvailable;
      }
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
