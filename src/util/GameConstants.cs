namespace Mush;

using Godot;

public static class GameConstants {
    public static readonly float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
}
