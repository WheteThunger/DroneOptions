## Features

- Allows changing speed, toughness and other properties of RC drones
- Allows multiple settings profiles based on permissions
- Allows other plugins to dynamically change a drone's profile based on attachments or other circumstances

Note: By default, this plugin changes the settings of all drones since they aren't very balanced in vanilla. If you aren't happy with the plugin's defaults, you can of course configure them to your liking.

## How it works

There are multiple types of drones.
- `BaseDrone` -- Normal drones with no attachments
- `DroneBoombox` -- Drones that have an attached boombox from the [Drone Boombox](https://umod.org/plugins/drone-boombox) plugin
- `DroneStorage` -- Drones that have an attached stash container from the [Drone Storage](https://umod.org/plugins/drone-storage) plugin
- `DroneTurrets` -- Drones that have an attached turret from the [Drone Turrets](https://umod.org/plugins/drone-turrets) plugin
- `MegaDrones` -- Drones created by the [Mega Drones](https://umod.org/plugins/mega-drones) plugin
- `RidableDrones` -- Drones that have an attached chair from the [Ridable Drones](https://umod.org/plugins/ridable-drones) plugin

Each drone type has a default profile which determines the speed, toughness and other properties for drones of that type. Each drone type can also have unlimited permission-based profiles which will override the default depending on the drone owner's permissions.

## Configuration

Default configuration:

```json
{
  "SettingsByDroneType": {
    "BaseDrone": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": true,
          "DisableWhenHurtChance": 25.0,
          "MovementAcceleration": 10.0,
          "AltitudeAcceleration": 10.0,
          "LeanWeight": 0.025
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.2,
          "Bullet": 0.2,
          "AntiVehicle": 0.25
        }
      },
      "ProfilesRequiringPermission": [
        {
          "PermissionSuffix": "god",
          "DroneProperties": {
            "KillInWater": false,
            "DisableWhenHurtChance": 0.0,
            "MovementAcceleration": 30.0,
            "AltitudeAcceleration": 20.0,
            "LeanWeight": 0.0
          },
          "DamageScale": {
            "AntiVehicle": 0.0,
            "Arrow": 0.0,
            "Bite": 0.0,
            "Bleeding": 0.0,
            "Blunt": 0.0,
            "Bullet": 0.0,
            "Cold": 0.0,
            "ColdExposure": 0.0,
            "Collision": 0.0,
            "Decay": 0.0,
            "Drowned": 0.0,
            "ElectricShock": 0.0,
            "Explosion": 0.0,
            "Fall": 0.0,
            "Fun_Water": 0.0,
            "Generic": 0.0,
            "Heat": 0.0,
            "Hunger": 0.0,
            "Poison": 0.0,
            "Radiation": 0.0,
            "RadiationExposure": 0.0,
            "Slash": 0.0,
            "Stab": 0.0,
            "Suicide": 0.0,
            "Thirst": 0.0
          }
        }
      ]
    },
    "DroneBoombox": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": true,
          "DisableWhenHurtChance": 25.0,
          "MovementAcceleration": 7.5,
          "AltitudeAcceleration": 7.5,
          "LeanWeight": 0.025
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.1,
          "Bullet": 0.1,
          "AntiVehicle": 0.1
        }
      },
      "ProfilesRequiringPermission": []
    },
    "DroneStorage": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": true,
          "DisableWhenHurtChance": 25.0,
          "MovementAcceleration": 7.5,
          "AltitudeAcceleration": 7.5,
          "LeanWeight": 0.025
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.1,
          "Bullet": 0.1,
          "AntiVehicle": 0.1
        }
      },
      "ProfilesRequiringPermission": []
    },
    "DroneTurrets": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": true,
          "DisableWhenHurtChance": 25.0,
          "MovementAcceleration": 5.0,
          "AltitudeAcceleration": 5.0,
          "LeanWeight": 0.025
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.1,
          "Bullet": 0.1,
          "AntiVehicle": 0.1,
          "Explosion": 0.75,
          "Blunt": 0.75
        }
      },
      "ProfilesRequiringPermission": []
    },
    "MegaDrones": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": false,
          "DisableWhenHurtChance": 0.0,
          "MovementAcceleration": 20.0,
          "AltitudeAcceleration": 20.0,
          "LeanWeight": 0.1
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.05,
          "Bullet": 0.05,
          "AntiVehicle": 0.1,
          "Explosion": 0.1,
          "Blunt": 0.25
        }
      },
      "ProfilesRequiringPermission": []
    },
    "RidableDrones": {
      "DefaultProfile": {
        "DroneProperties": {
          "KillInWater": true,
          "DisableWhenHurtChance": 25.0,
          "MovementAcceleration": 7.5,
          "AltitudeAcceleration": 7.5,
          "LeanWeight": 0.025
        },
        "DamageScale": {
          "Generic": 0.1,
          "Heat": 0.1,
          "Bullet": 0.1,
          "AntiVehicle": 0.1
        }
      },
      "ProfilesRequiringPermission": []
    }
  }
}
```

Each drone type has the following options.
- `DefaultProfile` -- Applies to all drones of this type, except for drones owned by players with permission to a profile under `ProfilesRequiringPermission` for this drone type.
- `ProfilesRequiringPermission` -- List of profiles that require permission. Each profile will generate a permission of the format `dronesettings.<type>.<suffix>` (e.g., `dronesettings.basedrone.god`). Granting that permission to a player will cause any drones they deploy to have that profile instead of `DefaultProfile`. Granting multiple profiles to a player will cause only the last one to apply, based on the order in the config.

Each profile has the following options.
- `PermissionSuffix` -- This determines the generated permission of format `dronesettings.<type>.<suffix>`.
- `DroneProperties`
  - `KillInWater` (default: `true`) -- While `true`, the drone will be destroyed when it enters water. While `false` the drone can enter water without issue.
    - Tip: While controlling a drone that is underwater, for some reason, you can see better if wearing a diving mask.
  - `DisableWhenHurtChance` (default: `25.0`) -- This determines the chance that the drone control will be briefly disabled when the drone is damaged.
  - `MovementAcceleration` (default: `10.0`) -- This determines the drone's horizontal movement speed (forward, backward, sideways).
  - `AltitudeAcceleration` (default: `10.0`) -- This determines the drone's vertical movement speed (up, down).
  - `LeanWeight` (vanilla: `0.25`) -- This determines how much the drone leans while moving, as well as how much altitude is lost while moving.
    - Set to `0.0` for no lean or altitude loss.
      - Useful when using the [Drone Lights](https://umod.org/plugins/drone-lights) plugin since it prevents the beam from unintentionally moving as the drone leans.
      - Useful when flying in locations where the altitude does not change, such as in the underground train tunnels.
- `DamageScale` (each `0.0` to `1.0`) -- These options determine how much damage the drone will take, per damage type. If this option is excluded, drones will use vanilla damage scaling.
  - Set a damage type to `1.0` to take full damage. This is the default for any damage type not specified.
  - Set a damage type to `0.0` to block all damage of that type.
  - Note: Vanilla Drone collision uses `Generic` damage, not `Collision` damage. Using the Better Drone Collision plugin fixes that.

## Recommended compatible plugins

Drone balance:
- [Drone Settings](https://umod.org/plugins/drone-settings) (This plugin) -- Allows changing speed, toughness and other properties of RC drones.
- [Targetable Drones](https://umod.org/plugins/targetable-drones) -- Allows RC drones to be targeted by Auto Turrets and SAM Sites.
- [Limited Drone Range](https://umod.org/plugins/limited-drone-range) -- Limits how far RC drones can be controlled from computer stations.

Drone fixes and improvements:
- [Better Drone Collision](https://umod.org/plugins/better-drone-collision) -- Overhauls RC drone collision damage so it's more intuitive.
- [Auto Flip Drones](https://umod.org/plugins/auto-flip-drones) -- Auto flips upside-down RC drones when a player takes control.
- [Drone Hover](https://umod.org/plugins/drone-hover) -- Allows RC drones to hover in place while not being controlled.

Drone attachments:
- [Drone Lights](https://umod.org/plugins/drone-lights) -- Adds controllable search lights to RC drones.
- [Drone Turrets](https://umod.org/plugins/drone-turrets) -- Allows players to deploy auto turrets to RC drones.
- [Drone Storage](https://umod.org/plugins/drone-storage) -- Allows players to deploy a small stash to RC drones.
- [Ridable Drones](https://umod.org/plugins/ridable-drones) -- Allows players to ride RC drones by standing on them or mounting a chair.

## Developer API

#### API_RefreshDroneProfile

```csharp
void API_RefreshDroneProfile(Drone drone)
```

- Attempts to apply the drone's most appropriate profile
- Useful for changing a drone's profile after an attachment has been added or removed
- This calls the `OnDroneTypeDetermine` hook call so that other plugins can respond with the profile name they think is appropriate

## Developer Hooks

#### OnDroneTypeDetermine

```csharp
string OnDroneTypeDetermine(Drone drone)
```

- Called when this plugin is determining which profile to apply to a particular drone
- Returning a string indicates that the drone is special and should use the specified profile type
- If all plugins return `null`, this plugin will select a profile of type `"BaseDrone"`
- Recommended to conditionally return your plugin's `Name` or `null`

#### OnDroneSettingsChange

```csharp
object OnDroneSettingsChange(Drone drone)
```

- Called when this plugin is about to alter a drone
- Returning `false` will prevent the the drone from being altered
- Returning `null` will result in the default behavior

#### OnDroneSettingsChanged

```csharp
void OnDroneSettingsChanged(Drone drone)
```

- Called after this plugin has altered a drone
- No return behavior
