namespace MHServerEmu.Games.Entities
{
    public class Missile : Agent
    {
        private Bounds _entityCollideBounds;

        public Missile(Game game) : base(game)
        {
            SetFlag(EntityFlags.IsNeverAffectedByPowers, true);
            _entityCollideBounds = new();
        }

        public override void SetEntityCollideBounds(ref Bounds bounds)
        {
            _entityCollideBounds = bounds;
        }
    }
}
