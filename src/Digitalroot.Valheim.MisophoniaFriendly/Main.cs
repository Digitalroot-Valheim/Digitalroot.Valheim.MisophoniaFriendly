using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.MisophoniaFriendly
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  public class Main : BaseUnityPlugin, ITraceableLogging
  {
    public const string Version = "1.0.1";
    public const string Name = "Digitalroot Misophonia Friendly";
    public const string Guid = "digitalroot.mods.misophoniafriendly";
    public const string Namespace = "Digitalroot.Valheim" + nameof(MisophoniaFriendly);
    private Harmony _harmony;
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
      Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");
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
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod().DeclaringType?.Name}.{MethodBase.GetCurrentMethod().Name}");

        var prefabNames = new[]
        {
          Common.Names.Vanilla.PrefabNames.SfxPukeMale
          , Common.Names.Vanilla.PrefabNames.SfxPukeFemale
          , Common.Names.Vanilla.PrefabNames.SfxCreatureConsume
          , Common.Names.Vanilla.PrefabNames.SfxEat
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
