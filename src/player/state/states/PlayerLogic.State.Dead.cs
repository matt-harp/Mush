namespace Mush;

public partial class PlayerLogic
{
    public partial record State
    {
        public partial record Dead : State;
    }
}
