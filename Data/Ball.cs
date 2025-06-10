using System;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal class Ball : IBall, IDisposable
    {
        #region ctor

        public Guid Id { get; } = Guid.NewGuid();

        private readonly BallLogger? logger;
        private readonly Timer? logTimer;

        internal Ball(Vector pos, Vector vel, double diameter, double weight, BallLogger? logger = null)
        {
            Position = pos;
            Velocity = vel;
            Diameter = diameter;
            Weight = weight;
            this.logger = logger;

            if (logger != null)
            {
                logTimer = new Timer(_ =>
                {
                    logger.Log(Id, Position.x, Position.y, Velocity.x, Velocity.y);
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }
        public double Diameter { get; }
        public double Weight { get; }
        public IVector CurrentPosition => Position;

        public void UpdateFromLogic(IVector newPosition, IVector newVelocity)
        {
            Position = new Vector(newPosition.x, newPosition.y);
            Velocity = newVelocity;
        }

        #endregion IBall

        #region private

        private Vector Position;

        private async Task RaiseNewPositionChangeNotificationAsync()
        {
            if (NewPositionNotification != null)
            {
                var handlers = NewPositionNotification.GetInvocationList();
                foreach (EventHandler<IVector> handler in handlers)
                {
                    await Task.Run(() => handler(this, Position));
                }
            }
        }

        internal async void Move(Vector delta)
        {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);

            await RaiseNewPositionChangeNotificationAsync();
        }

        #endregion private

        #region IDisposable

        public void Dispose()
        {
            logTimer?.Dispose();
        }

        #endregion
    }
}
