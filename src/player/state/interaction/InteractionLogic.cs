namespace Mush;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public enum EInteractionEndType {
    /// <summary>
    /// <para>When focus is lost with the interactable i.e. another object occludes it or when looking away</para>
    /// </summary>
    LostFocus,

    /// <summary>
    /// <para>When the user cancels the interaction via releasing input</para>
    /// </summary>
    Canceled,

    /// <summary>
    /// <para>When either the interactor or interactable can no longer interact/be interacted with</para>
    /// </summary>
    BecameUnavailable,

    /// <summary>
    /// <para>When an interaction finishes successfully</para>
    /// </summary>
    Success
}

public interface IInteractionLogic : ILogicBlock<InteractionLogic.State>;

[Meta]
[LogicBlock(typeof(State))]
public partial class InteractionLogic : LogicBlock<InteractionLogic.State>, IInteractionLogic {
    public override Transition GetInitialState() => To<State.Active.Idle>();

    [Meta]
    public abstract partial record State : StateLogic<State>;

    public partial record Data {
        public IInteractable? FocusedInteractable { get; set; }
    }

    public static class Input {
        public readonly record struct PhysicsTick(double Delta);

        public readonly record struct BeginInteract;

        public readonly record struct EndInteract;
    }

    public static class Output {
        public readonly record struct FocusBegan(IInteractable Interactable);

        public readonly record struct FocusEnded(IInteractable Interactable);

        public readonly record struct InteractBegan(IInteractable Interactable);

        public readonly record struct InteractProgressChanged(IInteractable Interactable, float Progress);

        public readonly record struct InteractEnded(IInteractable Interactable, EInteractionEndType EndType);

        public readonly record struct AvailabilityChanged(bool IsAvailable);
    }
}
