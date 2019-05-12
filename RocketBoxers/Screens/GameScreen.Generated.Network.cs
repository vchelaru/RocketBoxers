namespace RocketBoxers.Screens
{
    public partial class GameScreen : RedGrin.INetworkArena
    {
        public RedGrin.INetworkEntity RequestCreateEntity (long ownerId, object entityData) 
        {
            RedGrin.INetworkEntity entity = null;
            if (entityData is RocketBoxers.NetStates.PlayerNetState)
            {
                entity = new RocketBoxers.Entities.Player();
                entity.OwnerId = ownerId;
                entity.UpdateFromState(entityData, 0);
            }
            CustomRequestCreateNetworkEntity(ref entity, entityData);
            return entity;
        }
        public void RequestDestroyEntity (RedGrin.INetworkEntity entity) 
        {
            (entity as FlatRedBall.Graphics.IDestroyable).Destroy();
            CustomRequestDestroyNetworkEntity(entity);
        }
        private void Claim (RedGrin.INetworkEntity entity) 
        {
            if (RedGrin.NetworkManager.Self.Role == RedGrin.NetworkRole.Server)
            {
                entity.OwnerId = RedGrin.NetworkManager.Self.NetworkId;
                var claim = new RocketBoxers.Messages.ClaimEntity();
                entity.OwnerId = RedGrin.NetworkManager.Self.NetworkId;
                claim.EntityName = ((FlatRedBall.Utilities.INameable)entity).Name;
                RedGrin.NetworkManager.Self.AddToEntities(entity);
                claim.EntityId = entity.EntityId;
                RedGrin.NetworkManager.Self.RequestGenericMessage(claim);
            }
            else
            {
                RedGrin.NetworkManager.Self.AddToEntities(entity);
            }
        }
        private void HandleClaimEntity (RocketBoxers.Messages.ClaimEntity claim) 
        {
            switch(claim.EntityName)
            {
                case  "Player1":
                    Player1.OwnerId = claim.OwnerId;
                    Player1.EntityId = claim.EntityId;
                    break;
            }
        }
    }
}
