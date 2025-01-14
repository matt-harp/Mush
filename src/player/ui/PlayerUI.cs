namespace Mush;

using Godot;
using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IPlayerUI : IControl {
}

[Meta(typeof(IAutoNode))]
public partial class PlayerUI : Control, IPlayerUI {
    public override void _Notification(int what) => this.Notify(what);

    [Dependency] private IInteractionLogic InteractionLogic => this.DependOn<IInteractionLogic>();

    [Node] private TextureRect Reticle { get; set; } = null!;

    public InteractionLogic.IBinding InteractionBinding { get; set; }

    public void OnResolved() {
        InteractionBinding = InteractionLogic.Bind();
        InteractionBinding
            .Handle((in InteractionLogic.Output.FocusBegan _) => {
                CreateTween().TweenProperty(Reticle, "self_modulate:a", 1, 0.5);
            })
            .Handle((in InteractionLogic.Output.FocusEnded _) => {
                CreateTween().TweenProperty(Reticle, "self_modulate:a", 0, 0.5);
            });
    }

    public void OnExitTree() {
        InteractionBinding.Dispose();
    }
}
