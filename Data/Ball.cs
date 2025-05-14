namespace Data
{
    internal class Ball : IBall
    {
        #region ctor
        public Guid Id { get; } = Guid.NewGuid();
        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }
        public IVector CurrentPosition => Position;

        public double Radius { get; } = 10;
        public void UpdateFromLogic(IVector newPosition, IVector newVelocity)
        {
            Position = new Vector(newPosition.x, newPosition.y);
            Velocity = newVelocity;
        }


        #endregion IBall

        #region private

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}
