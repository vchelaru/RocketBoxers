namespace RocketBoxers.Entities
{
    public partial class Player : RedGrin.INetworkEntity
    {
        public long OwnerId { get; set; }
        public long EntityId { get; set; }
        public object GetState () 
        {
            var state = new RocketBoxers.NetStates.PlayerNetState();
            CustomGetState(state);
            return state;
        }
        public void UpdateFromState (object entityState, double stateTime) 
        {
            var state = entityState as RocketBoxers.NetStates.PlayerNetState;
            CustomUpdateFromState(state);
        }
    }
}
