namespace Mush;

using Godot;
using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

public interface IInteractable : INode {
    bool IsActive { get; }
    bool IsAvailable { get; }
    float InteractionTime { get; }
    void OnInteract();
}

[Meta(typeof(IAutoNode))]
public partial class Interactable : Node3D, IInteractable {
    public override void _Notification(int what) => this.Notify(what);

    #region IInteractable

    public bool IsActive { get; } = true;
    public bool IsAvailable { get; } = true;
    public float InteractionTime { get; } = 1;
    public void OnInteract() => QueueFree();

    #endregion
}
