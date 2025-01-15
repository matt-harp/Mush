namespace Mush;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
    public partial record State {
        [Meta]
        public abstract partial record Alive : State, IGet<Input.PhysicsTick> {
            protected Alive() {
                this.OnEnter(() => Get<IInteractionLogic>().Start());
                this.OnExit(() => Get<IInteractionLogic>().Stop());
            }

            public virtual Transition On(in Input.PhysicsTick input) {
                Get<IInteractionLogic>().Input(new InteractionLogic.Input.PhysicsTick(input.Delta));
                return ToSelf();
            }

            public abstract partial record Free : Alive {
                public abstract partial record Grounded : Free {
                    public partial record Idle : Grounded;

                    public partial record Moving : Grounded {
                    }
                    public override Transition On(in Input.PhysicsTick input) {
                        base.On(input);



                        return ToSelf();
                    }
                }

                // apply gravity
                public override Transition On(in Input.PhysicsTick input) {
                    base.On(input);

                    var player = Get<IPlayer>();
                    var velocity = player.Velocity;

                    var delta = (float)input.Delta;
                    velocity.Y += -GameConstants.Gravity * delta;

                    Output(new Output.VelocityChanged(velocity));

                    return ToSelf();
                }

                public partial record Falling : Free;
            }

            public abstract partial record Frozen : Alive;
        }
    }
}
