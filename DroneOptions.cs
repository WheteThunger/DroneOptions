using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Drone Options", "WhiteThunder", "0.1.2")]
    [Description("Allows changing toughness, speed and other properties of RC drones.")]
    internal class DroneOptions : CovalencePlugin
    {
        #region Fields

        private static DroneOptions _pluginInstance;
        private static Configuration _pluginConfig;

        private const string PermissionRulesetPrefix = "droneoptions.ruleset";

        private DroneProperties _vanillaDroneProperties;
        private ProtectionProperties _vanillaDroneProtection;
        private List<ProtectionProperties> _customProtectionProperties = new List<ProtectionProperties>();

        #endregion

        #region Hooks

        private void Init()
        {
            _pluginInstance = this;
            _pluginConfig.Init(this);

            Unsubscribe(nameof(OnEntitySpawned));
        }

        private void OnServerInitialized()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                var drone = entity as Drone;
                if (drone == null)
                    continue;

                OnEntitySpawned(drone);
            }

            Subscribe(nameof(OnEntitySpawned));
        }

        private void Unload()
        {
            foreach (var entity in BaseNetworkable.serverEntities)
            {
                var drone = entity as Drone;
                if (drone == null || !IsDroneEligible(drone))
                    continue;

                if (ApplyRulesetWasBlocked(drone))
                    continue;

                if (_vanillaDroneProtection != null && _customProtectionProperties.Contains(drone.baseProtection))
                    drone.baseProtection = _vanillaDroneProtection;

                if (_vanillaDroneProperties != null)
                    _vanillaDroneProperties?.ApplyToDrone(drone);

                Interface.CallHook("OnDroneOptionsChanged", drone);
            }

            foreach (var protectionProperties in _customProtectionProperties)
                UnityEngine.Object.Destroy(protectionProperties);

            _pluginConfig = null;
            _pluginInstance = null;
        }

        private void OnEntitySpawned(Drone drone)
        {
            if (!IsDroneEligible(drone))
                return;

            if (_vanillaDroneProtection == null && drone.baseProtection != null)
                _vanillaDroneProtection = drone.baseProtection;

            if (_vanillaDroneProperties == null)
                _vanillaDroneProperties = DroneProperties.FromDrone(drone);

            // Delay to give other plugins a moment to cache the drone id so they can block this.
            NextTick(() =>
            {
                if (drone == null)
                    return;

                var ruleset = GetDroneRuleset(drone.OwnerID);
                if (ruleset == null)
                    return;

                TryApplyRuleset(drone, ruleset);
            });
        }

        #endregion

        #region API

        private void ResetDroneOptions(Drone drone)
        {
            var ruleset = GetDroneRuleset(drone.OwnerID);
            if (ruleset == null)
                return;

            TryApplyRuleset(drone, ruleset);
        }

        #endregion

        #region Helper Methods

        private static bool ApplyRulesetWasBlocked(Drone drone)
        {
            object hookResult = Interface.CallHook("OnDroneOptionsChange", drone);
            return hookResult is bool && (bool)hookResult == false;
        }

        private static float Clamp(float x, float min, float max) => Math.Max(min, Math.Min(x, max));

        private static bool IsDroneEligible(Drone drone) => !(drone is DeliveryDrone);

        private static string GetRulesetPermission(string name) =>
            $"{PermissionRulesetPrefix}.{name}";

        private static bool TryApplyRuleset(Drone drone, DroneRuleset ruleset)
        {
            if (ApplyRulesetWasBlocked(drone))
                return false;

            ruleset.ApplyToDrone(drone);
            Interface.CallHook("OnDroneOptionsChanged", drone);
            return true;
        }

        private ProtectionProperties CreateProtectionProperties(Dictionary<string, float> damageMap)
        {
            var protectionProperties = ScriptableObject.CreateInstance<ProtectionProperties>();
            _customProtectionProperties.Add(protectionProperties);

            foreach (var entry in damageMap)
            {
                DamageType damageType;
                if (!Enum.TryParse<DamageType>(entry.Key, true, out damageType))
                {
                    _pluginInstance.LogError($"Invalid damage type: {entry.Key}");
                    continue;
                }
                protectionProperties.Add(damageType, 1 - Clamp(entry.Value, 0, 1));
            }

            return protectionProperties;
        }

        #endregion

        #region Configuration

        private DroneRuleset GetDroneRuleset(ulong ownerId)
        {
            if (ownerId == 0 || (_pluginConfig.Rulesets?.Length ?? 0) == 0)
                return _pluginConfig.DefaultRuleset;

            var ownerIdString = ownerId.ToString();
            for (var i = _pluginConfig.Rulesets.Length - 1; i >= 0; i--)
            {
                var ruleset = _pluginConfig.Rulesets[i];
                if (ruleset.Permission != null && permission.UserHasPermission(ownerIdString, ruleset.Permission))
                    return ruleset;
            }

            return _pluginConfig.DefaultRuleset;
        }

        private class Configuration : SerializableConfiguration
        {
            [JsonProperty("DefaultRuleset")]
            public DroneRuleset DefaultRuleset = new DroneRuleset();

            [JsonProperty("Rulesets")]
            public DroneRuleset[] Rulesets = new DroneRuleset[]
            {
                new DroneRuleset()
                {
                    Name = "balanced",
                    DamageScale = new Dictionary<string, float>()
                    {
                        [DamageType.Generic.ToString()] = 0.1f,
                        [DamageType.Heat.ToString()] = 0.2f,
                        [DamageType.Bullet.ToString()] = 0.2f,
                        [DamageType.Radiation.ToString()] = 0,
                        [DamageType.AntiVehicle.ToString()] = 0.25f,
                    },
                },
                new DroneRuleset()
                {
                    Name = "god",
                    DroneProperties = new DroneProperties()
                    {
                        KillInWater = false,
                        MovementAcceleration = 30,
                        AltitudeAcceleration = 20,
                        LeanWeight = 0,
                    },
                    DamageScale = new Dictionary<string, float>()
                    {
                        [DamageType.AntiVehicle.ToString()] = 0,
                        [DamageType.Arrow.ToString()] = 0,
                        [DamageType.Bite.ToString()] = 0,
                        [DamageType.Bleeding.ToString()] = 0,
                        [DamageType.Blunt.ToString()] = 0,
                        [DamageType.Bullet.ToString()] = 0,
                        [DamageType.Cold.ToString()] = 0,
                        [DamageType.ColdExposure.ToString()] = 0,
                        [DamageType.Collision.ToString()] = 0,
                        [DamageType.Decay.ToString()] = 0,
                        [DamageType.Drowned.ToString()] = 0,
                        [DamageType.ElectricShock.ToString()] = 0,
                        [DamageType.Explosion.ToString()] = 0,
                        [DamageType.Fall.ToString()] = 0,
                        [DamageType.Fun_Water.ToString()] = 0,
                        [DamageType.Generic.ToString()] = 0,
                        [DamageType.Heat.ToString()] = 0,
                        [DamageType.Hunger.ToString()] = 0,
                        [DamageType.Poison.ToString()] = 0,
                        [DamageType.Radiation.ToString()] = 0,
                        [DamageType.RadiationExposure.ToString()] = 0,
                        [DamageType.Slash.ToString()] = 0,
                        [DamageType.Stab.ToString()] = 0,
                        [DamageType.Suicide.ToString()] = 0,
                        [DamageType.Thirst.ToString()] = 0,
                    },
                },
            };

            public void Init(DroneOptions pluginInstance)
            {
                DefaultRuleset.Init(pluginInstance, requiresPermission: false);

                foreach (var ruleset in Rulesets)
                    ruleset.Init(pluginInstance, requiresPermission: true);
            }
        }

        private class DroneRuleset
        {
            [JsonProperty("Name", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Name;

            [JsonProperty("DroneProperties", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public DroneProperties DroneProperties;

            [JsonProperty("DamageScale", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Dictionary<string, float> DamageScale;

            [JsonIgnore]
            public ProtectionProperties ProtectionProperties;

            [JsonIgnore]
            public string Permission;

            public void Init(DroneOptions pluginInstance, bool requiresPermission)
            {
                if (requiresPermission)
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        return;

                    Permission = GetRulesetPermission(Name);
                    pluginInstance.permission.RegisterPermission(Permission, pluginInstance);
                }

                if (DamageScale != null)
                    ProtectionProperties = pluginInstance.CreateProtectionProperties(DamageScale);
            }

            public void ApplyToDrone(Drone drone)
            {
                if (ProtectionProperties != null)
                    drone.baseProtection = ProtectionProperties;

                if (DroneProperties != null)
                    DroneProperties.ApplyToDrone(drone);
            }
        }

        private class DroneProperties
        {
            public static DroneProperties FromDrone(Drone drone)
            {
                var properties = new DroneProperties()
                {
                    KillInWater = drone.killInWater,
                    MovementAcceleration = drone.movementAcceleration,
                    AltitudeAcceleration = drone.altitudeAcceleration,
                    LeanWeight = drone.leanWeight,
                };

                return properties;
            }

            [JsonProperty("KillInWater", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool? KillInWater;

            [JsonProperty("MovementAcceleration", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public float? MovementAcceleration;

            [JsonProperty("AltitudeAcceleration", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public float? AltitudeAcceleration;

            [JsonProperty("LeanWeight", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public float? LeanWeight;

            public void ApplyToDrone(Drone drone)
            {
                if (KillInWater != null)
                    drone.killInWater = (bool)KillInWater;

                if (MovementAcceleration != null)
                    drone.movementAcceleration = (float)MovementAcceleration;

                if (AltitudeAcceleration != null)
                    drone.altitudeAcceleration = (float)AltitudeAcceleration;

                if (LeanWeight != null)
                    drone.leanWeight = (float)LeanWeight;
            }
        }

        private Configuration GetDefaultConfig() => new Configuration();

        #endregion

        #region Configuration Boilerplate

        private class SerializableConfiguration
        {
            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonHelper.Deserialize(ToJson()) as Dictionary<string, object>;
        }

        private static class JsonHelper
        {
            public static object Deserialize(string json) => ToObject(JToken.Parse(json));

            private static object ToObject(JToken token)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        return token.Children<JProperty>()
                                    .ToDictionary(prop => prop.Name,
                                                  prop => ToObject(prop.Value));

                    case JTokenType.Array:
                        return token.Select(ToObject).ToList();

                    default:
                        return ((JValue)token).Value;
                }
            }
        }

        private bool MaybeUpdateConfig(SerializableConfiguration config)
        {
            var currentWithDefaults = config.ToDictionary();
            var currentRaw = Config.ToDictionary(x => x.Key, x => x.Value);
            return MaybeUpdateConfigDict(currentWithDefaults, currentRaw);
        }

        private bool MaybeUpdateConfigDict(Dictionary<string, object> currentWithDefaults, Dictionary<string, object> currentRaw)
        {
            bool changed = false;

            foreach (var key in currentWithDefaults.Keys)
            {
                object currentRawValue;
                if (currentRaw.TryGetValue(key, out currentRawValue))
                {
                    var defaultDictValue = currentWithDefaults[key] as Dictionary<string, object>;
                    var currentDictValue = currentRawValue as Dictionary<string, object>;

                    if (defaultDictValue != null)
                    {
                        if (currentDictValue == null)
                        {
                            currentRaw[key] = currentWithDefaults[key];
                            changed = true;
                        }
                        else if (MaybeUpdateConfigDict(defaultDictValue, currentDictValue))
                            changed = true;
                    }
                }
                else
                {
                    currentRaw[key] = currentWithDefaults[key];
                    changed = true;
                }
            }

            return changed;
        }

        protected override void LoadDefaultConfig() => _pluginConfig = GetDefaultConfig();

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _pluginConfig = Config.ReadObject<Configuration>();
                if (_pluginConfig == null)
                {
                    throw new JsonException();
                }

                if (MaybeUpdateConfig(_pluginConfig))
                {
                    LogWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }
            }
            catch
            {
                LogWarning($"Configuration file {Name}.json is invalid; using defaults");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig()
        {
            Log($"Configuration changes saved to {Name}.json");
            Config.WriteObject(_pluginConfig, true);
        }

        #endregion
    }
}
