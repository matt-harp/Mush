namespace Mush;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IPlayerLogic : ILogicBlock<PlayerLogic.State>;

[Meta]
[LogicBlock(typeof(State))]
public partial class PlayerLogic : LogicBlock<PlayerLogic.State>, IPlayerLogic {
    public override Transition GetInitialState() => To<State.Alive.Free.Grounded.Idle>();

    [Meta]
    public abstract partial record State : StateLogic<State>;

    public static class Input {
        public readonly record struct PhysicsTick(double Delta);

        public readonly record struct CameraMove(Vector2 Delta);

        public readonly record struct InputMove(Vector2 Delta);
    }

    public static class Output {
        public readonly record struct VelocityChanged(Vector3 Velocity);
    }

    public class Data {
    }
}
