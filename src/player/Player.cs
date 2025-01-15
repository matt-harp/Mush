namespace Mush;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Util;

public interface IPlayer : ICharacterBody3D, IProvide<IPlayerLogic>, IProvide<IInteractionLogic> {
    IPlayerLogic PlayerLogic { get; }
    IInteractionLogic InteractionLogic { get; }
}

[Meta(typeof(IAutoNode))]
public partial class Player : CharacterBody3D, IPlayer {
    public override void _Notification(int what) => this.Notify(what);

    [Node] private ICamera3D Camera { get; set; } = null!;
    [Node] private IRayCast3D InteractionCast { get; set; } = null!;

    #region Providers

    IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;
    IInteractionLogic IProvide<IInteractionLogic>.Value() => InteractionLogic;

    #endregion

    #region State

    public IPlayerLogic PlayerLogic { get; set; } = null!;
    public IInteractionLogic InteractionLogic { get; set; } = null!;
    public LogicBlock<PlayerLogic.State>.IBinding PlayerBinding { get; set; } = null!;
    public LogicBlock<InteractionLogic.State>.IBinding InteractionBinding { get; set; } = null!;

    #endregion

    public void OnReady() => SetPhysicsProcess(true);

    public void Setup() {
        PlayerLogic = new PlayerLogic();
        InteractionLogic = new InteractionLogic();

        PlayerLogic.Set<IPlayer>(this);
        PlayerLogic.Set(new PlayerLogic.Data());
        PlayerLogic.Set(Camera);
        PlayerLogic.Set(InteractionLogic);

        InteractionLogic.Set(InteractionCast);
        InteractionLogic.Set(new InteractionLogic.Data());
    }

    public void OnResolved() {
        PlayerBinding = PlayerLogic.Bind();
        InteractionBinding = InteractionLogic.Bind();

        PlayerBinding
            .Handle((in PlayerLogic.Output.VelocityChanged output) => {
                Velocity = output.Velocity;
            });

        this.Provide();

        PlayerLogic.Start();
    }

    //todo temp
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector2 _mouseInput;

    public void OnPhysicsProcess(double delta) {
        PlayerLogic.Input(new PlayerLogic.Input.PhysicsTick(delta));
        DebugDraw2D.DebugEnabled = true;
        DebugDraw2D.SetText("fps", Engine.GetFramesPerSecond());
        DebugDraw2D.SetText("state", InteractionLogic.Value.GetType().ToString());
        if (Input.IsActionJustPressed(GameInputs.Interact)) {
            InteractionLogic.Input(new InteractionLogic.Input.BeginInteract());
        }
        else if (Input.IsActionJustReleased(GameInputs.Interact)) {
            InteractionLogic.Input(new InteractionLogic.Input.EndInteract());
        }

        //todo temp
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
        if (Input.IsKeyPressed(Key.Escape)) {
            GetTree().Quit();
        }

        MoveAndSlide();

        // var velocity = Velocity;
        // var _movement = Vector3.Zero;
        // var _inputDirection = Input.GetVector(GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveForward,
        //     GameInputs.MoveBackward);
        //
        // // Add the gravity
        // if (!IsOnFloor()) velocity.Y -= gravity * (float)GetPhysicsProcessDeltaTime();
        //
        // // Convert input to movementDirection
        // _movement = (Transform.Basis * new Vector3(_inputDirection.X, 0, _inputDirection.Y)).Normalized();
        //
        // velocity.X = _movement.X * 5;
        // velocity.Z = _movement.Z * 5;
        // Velocity = velocity;
        // MoveAndSlide();
        //
        // if (_mouseInput == Vector2.Zero) {
        //     return;
        // }
        //
        // RotateY(Mathf.DegToRad(-_mouseInput.X * 8 * (float)GetPhysicsProcessDeltaTime()));
        // var rotation = Camera.RotationDegrees;
        // rotation.X += -_mouseInput.Y * 8 * (float)GetPhysicsProcessDeltaTime();
        // rotation.X = Mathf.Clamp(rotation.X, -90, 90);
        // Camera.RotationDegrees = rotation;
        //
        // _mouseInput = Vector2.Zero;
    }

    //todo temp
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseEvent) {
            _mouseInput = mouseEvent.ScreenRelative;
        }
    }

    public void OnExitTree() {
        PlayerLogic.Stop();
        InteractionLogic.Stop();

        PlayerBinding.Dispose();
        InteractionBinding.Dispose();
    }
}
