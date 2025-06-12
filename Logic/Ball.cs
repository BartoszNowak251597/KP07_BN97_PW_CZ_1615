using Data;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Logic
{
    internal class Ball : IBall
    {
        private readonly double width, height, diameter;
        private readonly Guid ballId;
        private readonly DataAbstractAPI dataLayer;
        private Position position;
        private double velocityX, velocityY;
        private double mass;
        private readonly object lockObj = new();

        private Stopwatch stopwatch;

        public Ball(Guid id, IVector initialPosition, IVector initialVelocity, DataAbstractAPI dataAPI, double tableWidth, double tableHeight, double ballDiameter, double weight)
        {
            ballId = id;
            position = new Position(initialPosition.x, initialPosition.y);
            dataLayer = dataAPI;
            width = tableWidth - 7;
            height = tableHeight - 7;
            diameter = ballDiameter;
            velocityX = initialVelocity.x;
            velocityY = initialVelocity.y;
            mass = weight;
            stopwatch = Stopwatch.StartNew();
        }

        public event EventHandler<IPosition>? NewPositionNotification;

        public async void OnNewPosition(IVector dataPos)
        {
            double deltaTime;
            lock (lockObj)
            {
                deltaTime = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                position = new Position(dataPos.x, dataPos.y);
                position = position.UpdatePosition(velocityX, velocityY,width, height, diameter,out velocityX, out velocityY,deltaTime,ballId,dataLayer );

            }

            if (NewPositionNotification != null)
            {
                var handlers = NewPositionNotification.GetInvocationList();
                foreach (EventHandler<IPosition> handler in handlers)
                {
                    await Task.Run(() => handler(this, Position));
                }
            }

            await Task.Run(() =>
            {
                lock (lockObj)
                {
                    dataLayer.UpdateBall(ballId,
                        new Vector(position.x, position.y),
                        new Vector(velocityX, velocityY));
                }
            });
        }

        public void ForceMove(double dx, double dy)
        {
            lock (lockObj)
            {
                position = new Position(position.x + dx, position.y + dy);
            }
        }

        public Position Position
        {
            get { lock (lockObj) { return position; } }
        }

        public double Radius => diameter / 2;
        public double VelocityX
        {
            get { lock (lockObj) { return velocityX; } }
        }

        public double VelocityY
        {
            get { lock (lockObj) { return velocityY; } }
        }

        public double Mass => mass;

        public Guid Id => ballId;

        public void UpdateFromCollision(double newVelX, double newVelY)
        {
            velocityX = newVelX;
            velocityY = newVelY;
            dataLayer.UpdateBall(ballId,
                new Vector(position.x, position.y),
                new Vector(velocityX, velocityY));
        }
    }
}