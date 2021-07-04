## Features

- Allows changing speed, toughness and other properties of RC drones
- Allows multiple profiles based on permissions
- Allows other plugins to dynamically change a drone's profile based on attachments or other circumstances

Note: By default, this plugin changes the toughness of all drones since they aren't very balanced in vanilla. Of course, you can edit the default profile, or remove it if you only want to use permission-based profiles.

## Permissions

The following permissions come with this plugin's **default configuration**. Granting one to a player determines the properties of drones they deploy.

- `droneoptions.basedrone.god` -- Invincible drones with super speed, no altitude loss while moving, and able to go underwater.

You can add more drone profiles in the plugin configuration, and the plugin will automatically generate permissions of the format `droneoptions.basedrone.<name>` when reloaded. If a player has permission to multiple profiles, only the last one will apply, based on the order in the config.

## Configuration

Default configuration:

```json
{
  "OptionsByDroneType": {
    "BaseDrone": {
      "DefaultProfile": {
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
    "DroneStorage": {
      "DefaultProfile": {
        "DroneProperties": {
          "MovementAcceleration": 7.5,
          "AltitudeAcceleration": 7.5
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
        "PermissionSuffix": "droneturrets",
        "DroneProperties": {
          "MovementAcceleration": 5.0,
          "AltitudeAcceleration": 5.0
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
        "PermissionSuffix": "megadrones",
        "DroneProperties": {
          "KillInWater": false,
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
    }
  }
}
```

- `OptionsByDroneType` -- Each drone type has a separate configuration nested under this section. Drone types are registered automatically by plugins. Developers of drone-related plugins may ask you to manually add a config section for their plugin.
  - `BaseDrone` -- Applies to all drones, except for drones that other pluginss declare to be special.
  - `DroneStorage` -- Applies to drones that have storage on them from the [Drone Storage](https://umod.org/plugins/drone-storage) plugin.
  - `Drone Turrets` -- Applies to drones that have turrets on them from the [Drone Turrets](https://umod.org/plugins/drone-turrets) plugin.
  - `Mega Drones` -- Applies to drones created by the Mega Drones plugin.

Each drone type has the following options.
- `DefaultProfile` -- Applies to all drones of this type, except for drones owned by players with permission to a profile under `ProfilesRequiringPermission` for this drone type.
- `ProfilesRequiringPermission` -- List of profiles that require permission. Each profile will generate a permission of the format `droneoptions.<type>.<suffix>` (e.g., `droneoptions.basedrone.god`). Granting that permission to a player will cause any drones they deploy to have that profile instead of `DefaultProfile`. Granting multiple profiles to a player will cause only the last one to apply, based on the order in the config.

Each profile has the following options.
- `PermissionSuffix` -- This determins the generated permission.
- `DroneProperties`
  - `KillInWater` (default: `true`) -- While `true`, the drone will be destroyed when it enters water. While `false` the drone can enter water without issue.
    - Tip: While controlling a drone that is underwater, you can actually see better if wearing a diving mask.
  - `MovementAcceleration` (default: `10.0`) -- This determines the drone's horizontal movement speed.
  - `AltitudeAcceleration` (default: `10.0`) -- This determines the drone's vertical movement speed (how quickly it can go up and down).
  - `LeanWeight` (vanilla: `0.25`) -- This determines how much the drone leans while moving, as well as how much altitude is lost while moving.
    - Set to `0.0` for no lean or altitude loss.
      - Useful when using the [Drone Lights](https://umod.org/plugins/drone-lights) plugin since it prevents the beam from unintentionally moving.
      - Useful when flying in locations where the altitude does not change, such as in the underground train tunnels.
- `DamageScale` (each `0.0` to `1.0`) -- These options determine how much damage the drone will take, per damage type. If this option is excluded, drones will use vanilla damage scaling.
  - Set a damage type to `1.0` to take full damage. This is the default for any damage type not specified.
  - Set a damage type to `0.0` to block all damage of that type.
  - Note: Vanilla Drone collision uses `Generic` damage, not `Collision` damage. Using the Better Drone Collision plugin fixes that.

## FAQ

#### How do I get a drone?

As of this writing, RC drones are a deployable item named `drone`, but they do not appear naturally in any loot table, nor are they craftable. However, since they are simply an item, you can use plugins to add them to loot tables, kits, GUI shops, etc. Admins can also get them with the command `inventory.give drone 1`, or spawn one in directly with `spawn drone.deployed`.

#### How do I remote-control a drone?

If a player has building privilege, they can pull out a hammer and set the ID of the drone. They can then enter that ID at a computer station and select it to start controlling the drone. Controls are `W`/`A`/`S`/`D` to move, `shift` (sprint) to go up, `ctrl` (duck) to go down, and mouse to steer.

Note: If you are unable to steer the drone, that is likely because you have a plugin drawing a UI that is grabbing the mouse cursor. The Movable CCTV was previously guilty of this and was patched in March 2021.

## Recommended compatible plugins

- [Drone Hover](https://umod.org/plugins/drone-hover) -- Allows RC drones to hover in place while not being controlled.
- [Drone Lights](https://umod.org/plugins/drone-lights) -- Adds controllable search lights to RC drones.
- [Drone Storage](https://umod.org/plugins/drone-storage) -- Allows players to deploy a small stash to RC drones.
- [Drone Turrets](https://umod.org/plugins/drone-turrets) -- Allows players to deploy auto turrets to RC drones.
- [Drone Effects](https://umod.org/plugins/drone-effects) -- Adds collision effects and propeller animations to RC drones.
- [Auto Flip Drones](https://umod.org/plugins/auto-flip-drones) -- Auto flips upside-down RC drones when a player takes control.
- [RC Identifier Fix](https://umod.org/plugins/rc-identifier-fix) -- Auto updates RC identifiers saved in computer stations to refer to the correct entity.

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
- If all plugins return `null`, this plugin will select a profile of type `"BaseDrone"`, based on the drone owner's permission
- Recommended to conditionally return your plugin's `Name` or `null`

#### OnDroneOptionsChange

```csharp
bool? OnDroneOptionsChange(Drone drone)
```

- Called when this plugin is about to alter a drone
- Returning `false` will prevent the the drone from being altered
- Returning `null` will result in the default behavior

#### OnDroneOptionsChanged

```csharp
void OnDroneOptionsChanged(Drone drone)
```

- Called after this plugin has altered a drone
- No return behavior
