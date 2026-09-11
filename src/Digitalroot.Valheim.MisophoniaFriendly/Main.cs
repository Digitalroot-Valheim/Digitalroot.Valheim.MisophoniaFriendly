using BepInEx;
using BepInEx.Configuration;
using Digitalroot.Valheim.Common;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.MisophoniaFriendly
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInDependency(Jotunn.Main.ModGuid)]
  [NetworkCompatibility(CompatibilityLevel.NotEnforced, VersionStrictness.None)]
  public partial class Main : BaseUnityPlugin, ITraceableLogging
  {
    // ReSharper disable once MemberCanBePrivate.Global
    public static Main Instance;

    public readonly ConfigEntry<int> NexusId;

    public Main()
    {
      Instance = this;
      NexusId = Config.Bind("General", "NexusID", 1667, new ConfigDescription("Nexus mod ID for updates.", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = false, ReadOnly = true }));
      InitConfigs();
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
        MuteSounds();
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

    private void MuteSounds() => ToggleSounds(AudioActionType.Mute);

    private void UnMuteSounds() => ToggleSounds(AudioActionType.Unmute);

    private void ToggleSounds(AudioActionType actionType)
    {
      try
      {
        Log.Trace(Main.Instance, $"{Main.Namespace}.{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}.{MethodBase.GetCurrentMethod()?.Name}");

        IEnumerable<string> prefabNames;
        string msg;
        bool soundToggle;
        switch (actionType)
        {
          case AudioActionType.Mute:
            prefabNames = GetPrefabs(PrefabActionType.Mute);
            msg = "Muting";
            soundToggle = true;
            break;

          case AudioActionType.Unmute:
            prefabNames = GetPrefabs(PrefabActionType.Unmute);
            msg = "Unmuting";
            soundToggle = false;
            break;

          default:
            return;
        }

        foreach (var prefabName in prefabNames)
        {
          Log.Trace(Instance, $"{msg} {prefabName}");
          var sfx = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
          var audioSource = sfx.GetComponent<AudioSource>();
          audioSource.mute = soundToggle;
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    private IEnumerable<string> GetPrefabs(PrefabActionType actionType)
    {
      switch (actionType)
      {
        case PrefabActionType.All:
          yield return Common.Names.Vanilla.PrefabNames.SfxPukeMale;
          yield return Common.Names.Vanilla.PrefabNames.SfxPukeFemale;
          yield return Common.Names.Vanilla.PrefabNames.SfxCreatureConsume;
          yield return Common.Names.Vanilla.PrefabNames.SfxEat;
          yield return Common.Names.Vanilla.PrefabNames.SfxChickenEat;
          yield return Common.Names.Vanilla.PrefabNames.SfxHareIdleEating;
          yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeHit;
          yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeSwing;
          yield return Common.Names.Vanilla.PrefabNames.SfxRockHit;
          yield return Common.Names.Vanilla.PrefabNames.SfxRockDestroyed;
          break;

        case PrefabActionType.Mute:
          if (MuteSfxPukeMale.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPukeMale;
          if (MuteSfxPukeFemale.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPukeFemale;
          if (MuteSfxCreatureConsume.Value) yield return Common.Names.Vanilla.PrefabNames.SfxCreatureConsume;
          if (MuteSfxEat.Value) yield return Common.Names.Vanilla.PrefabNames.SfxEat;
          if (MuteSfxChickenEat.Value) yield return Common.Names.Vanilla.PrefabNames.SfxChickenEat;
          if (MuteSfxHareIdleEating.Value) yield return Common.Names.Vanilla.PrefabNames.SfxHareIdleEating;
          if (MuteSfxPickaxeHit.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeHit;
          if (MuteSfxPickaxeSwing.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeSwing;
          if (MuteSfxRockHit.Value) yield return Common.Names.Vanilla.PrefabNames.SfxRockHit;
          if (MuteSfxRockDestroyed.Value) yield return Common.Names.Vanilla.PrefabNames.SfxRockDestroyed;
          break;

        case PrefabActionType.Unmute:
          if (!MuteSfxPukeMale.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPukeMale;
          if (!MuteSfxPukeFemale.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPukeFemale;
          if (!MuteSfxCreatureConsume.Value) yield return Common.Names.Vanilla.PrefabNames.SfxCreatureConsume;
          if (!MuteSfxEat.Value) yield return Common.Names.Vanilla.PrefabNames.SfxEat;
          if (!MuteSfxChickenEat.Value) yield return Common.Names.Vanilla.PrefabNames.SfxChickenEat;
          if (!MuteSfxHareIdleEating.Value) yield return Common.Names.Vanilla.PrefabNames.SfxHareIdleEating;
          if (!MuteSfxPickaxeHit.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeHit;
          if (!MuteSfxPickaxeSwing.Value) yield return Common.Names.Vanilla.PrefabNames.SfxPickaxeSwing;
          if (!MuteSfxRockHit.Value) yield return Common.Names.Vanilla.PrefabNames.SfxRockHit;
          if (!MuteSfxRockDestroyed.Value) yield return Common.Names.Vanilla.PrefabNames.SfxRockDestroyed;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
      }
    }

    public ConfigEntry<bool> MuteSfxPukeMale;
    public ConfigEntry<bool> MuteSfxPukeFemale;
    public ConfigEntry<bool> MuteSfxCreatureConsume;
    public ConfigEntry<bool> MuteSfxEat;
    public ConfigEntry<bool> MuteSfxChickenEat;
    public ConfigEntry<bool> MuteSfxHareIdleEating;
    public ConfigEntry<bool> MuteSfxPickaxeHit;
    public ConfigEntry<bool> MuteSfxPickaxeSwing;
    public ConfigEntry<bool> MuteSfxRockHit;
    public ConfigEntry<bool> MuteSfxRockDestroyed;

    private void InitConfigs()
    {
      MuteSfxPukeMale = Config.Bind("Toggles", nameof(MuteSfxPukeMale), true, new ConfigDescription($"Mute Sfx Puke Male", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxPukeMale.SettingChanged += ModConfig_SettingChanged;
      MuteSfxPukeFemale = Config.Bind("Toggles", nameof(MuteSfxPukeFemale), true, new ConfigDescription($"Mute Sfx Puke Female", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxPukeFemale.SettingChanged += ModConfig_SettingChanged;
      MuteSfxCreatureConsume = Config.Bind("Toggles", nameof(MuteSfxCreatureConsume), true, new ConfigDescription($"Mute Sfx Creature Consume", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxCreatureConsume.SettingChanged += ModConfig_SettingChanged;
      MuteSfxEat = Config.Bind("Toggles", nameof(MuteSfxEat), true, new ConfigDescription($"Mute Sfx Eat", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxEat.SettingChanged += ModConfig_SettingChanged;
      MuteSfxChickenEat = Config.Bind("Toggles", nameof(MuteSfxChickenEat), true, new ConfigDescription($"Mute Sfx Chicken Eat", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxChickenEat.SettingChanged += ModConfig_SettingChanged;
      MuteSfxHareIdleEating = Config.Bind("Toggles", nameof(MuteSfxHareIdleEating), true, new ConfigDescription($"Mute Sfx Hare Idle Eating", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxHareIdleEating.SettingChanged += ModConfig_SettingChanged;
      MuteSfxPickaxeHit = Config.Bind("Toggles", nameof(MuteSfxPickaxeHit), true, new ConfigDescription($"Mute Sfx Pickaxe Hit", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxPickaxeHit.SettingChanged += ModConfig_SettingChanged;
      MuteSfxPickaxeSwing = Config.Bind("Toggles", nameof(MuteSfxPickaxeSwing), true, new ConfigDescription($"Mute Sfx Pickaxe Swing", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxPickaxeSwing.SettingChanged += ModConfig_SettingChanged;
      MuteSfxRockHit = Config.Bind("Toggles", nameof(MuteSfxRockHit), true, new ConfigDescription($"Mute Rock Hit", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxRockHit.SettingChanged += ModConfig_SettingChanged;
      MuteSfxRockDestroyed = Config.Bind("Toggles", nameof(MuteSfxRockDestroyed), true, new ConfigDescription($"Mute Sfx Rock Destroyed", null, new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = true, ReadOnly = false }));
      MuteSfxRockDestroyed.SettingChanged += ModConfig_SettingChanged;
    }

    private void ModConfig_SettingChanged(object sender, EventArgs e)
    {
      UnMuteSounds();
      MuteSounds();
    }

    private enum PrefabActionType
    {
      All
      , Mute
      , Unmute
    }

    private enum AudioActionType
    {
      Mute
      , Unmute
    }

    #region Implementation of ITraceableLogging

    /// <inheritdoc />
    public string Source => Namespace;

    /// <inheritdoc />
    public bool EnableTrace { get; }

    #endregion
  }
}
