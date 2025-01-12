namespace Mush;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class InteractionLogic {
    public partial record State {
        public abstract partial record Active : State {
            protected bool HasValidInteraction(out IInteractable? interactable) {
                interactable = null;
                var raycast = Get<IRayCast3D>();
                return raycast.IsColliding() && raycast.GetCollider() is IInteractable obj &&
                       (interactable = obj).IsActive;
            }

            /// <summary>
            /// <para>Unfocused state</para>
            /// </summary>
            [Meta]
            public partial record Idle : Active, IGet<Input.PhysicsTick> {
                public Idle() {
                    OnAttach(() => {
                        var data = Get<Data>();
                        data.FocusedInteractable = null;
                    });
                }

                public virtual Transition On(in Input.PhysicsTick input) {
                    if (!HasValidInteraction(out var interactable)) {
                        return ToSelf();
                    }

                    var data = Get<Data>();
                    data.FocusedInteractable = interactable;
                    return interactable!.IsAvailable ? To<Focused.Available>() : To<Focused.Unavailable>();
                }
            }

            [Meta]
            public abstract partial record Focused : Active, IGet<Input.PhysicsTick> {
                public Focused() {
                    this.OnEnter(() => Output(new Output.FocusBegan(FocusedInteractable)));
                    this.OnExit(() => Output(new Output.FocusEnded(FocusedInteractable)));
                }
                protected IInteractable FocusedInteractable => Get<Data>().FocusedInteractable!;
                protected bool _wasPrevAvailable;

                public virtual Transition On(in Input.PhysicsTick input) {
                    if (!HasValidInteraction(out var interactable)) {
                        return To<Idle>();
                    }

                    if (interactable != FocusedInteractable) {
                        return To<Idle>();
                    }

                    if (interactable.IsAvailable != _wasPrevAvailable) {
                        Output(new Output.AvailabilityChanged(interactable.IsAvailable));
                        _wasPrevAvailable = interactable.IsAvailable;
                    }

                    return interactable.IsAvailable ? To<Available>() : To<Unavailable>();
                }

                public partial record Available : Focused, IGet<Input.BeginInteract> {
                    public Transition On(in Input.BeginInteract input) => To<Interacting>();

                    public partial record Interacting : Available, IGet<Input.EndInteract> {
                        private float _elapsed;

                        public Interacting() {
                            OnAttach(() => {
                                _elapsed = 0;
                                Output(new Output.InteractBegan(FocusedInteractable));
                            });
                        }

                        public override Transition On(in Input.PhysicsTick input) {
                            var goingTo = base.On(input);
                            if (goingTo.State is Idle) {
                                Output(new Output.InteractEnded(FocusedInteractable, EInteractionEndType.LostFocus));
                                return goingTo;
                            }

                            if (goingTo.State is Unavailable) {
                                Output(new Output.InteractEnded(FocusedInteractable,
                                    EInteractionEndType.BecameUnavailable));
                                return goingTo;
                            }

                            _elapsed += (float)input.Delta;
                            if (_elapsed >= FocusedInteractable.InteractionTime) {
                                Output(new Output.InteractEnded(FocusedInteractable, EInteractionEndType.Success));
                                FocusedInteractable.OnInteract();
                                return To<Available>();
                            }

                            Output(new Output.InteractProgressChanged(FocusedInteractable,
                                _elapsed / FocusedInteractable.InteractionTime));

                            return ToSelf();
                        }

                        public Transition On(in Input.EndInteract input) {
                            Output(new Output.InteractEnded(FocusedInteractable, EInteractionEndType.Canceled));
                            return To<Idle>();
                        }
                    }
                }

                public partial record Unavailable : Focused;
            }
        }
    }
}
