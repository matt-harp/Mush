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

            public abstract partial record Grounded : Alive {
                public partial record Idle : Grounded;

                public partial record Moving : Grounded {
                }

                public override Transition On(in Input.PhysicsTick input) {
                    base.On(input);

                    var player = Get<IPlayer>();
                    var data = Get<Data>();

                    return ToSelf();
                }
            }

            public partial record Falling : Alive;

            public partial record Falling : Alive;
        }
    }
}
