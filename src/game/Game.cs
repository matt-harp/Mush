namespace Mush;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IGame : INode3D, IProvide<IGameRepo> {
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node3D, IGame {
    public override void _Notification(int what) => this.Notify(what);

    #region Provider

    IGameRepo IProvide<IGameRepo>.Value() => GameRepo;

    #endregion

    #region State

    public IGameRepo GameRepo { get; set; } = null!;

    #endregion

    public void Setup() {
        GameRepo = new GameRepo();
    }

    public void OnExitTree() {
        GameRepo.Dispose();
    }
}
