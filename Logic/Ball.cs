using Data;
using System;

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

        public Ball(Guid id, IVector initialPosition, IVector initialVelocity, DataAbstractAPI dataAPI, double tableWidth, double tableHeight, double ballDiameter, double weight)
        {
            ballId = id;
            position = new Position(initialPosition.x, initialPosition.y);
            dataLayer = dataAPI;
            width = tableWidth-7;
            height = tableHeight-7;
            diameter = ballDiameter;
            velocityX = initialVelocity.x;
            velocityY = initialVelocity.y;
            mass = weight;
        }

        public event EventHandler<IPosition>? NewPositionNotification;

        public void OnNewPosition(IVector dataPos)
        {
            position = new Position(dataPos.x, dataPos.y);
            position = position.UpdatePosition(velocityX, velocityY, width, height, diameter, out velocityX, out velocityY);
            NewPositionNotification?.Invoke(this, position);

            dataLayer.UpdateBall(ballId,
                new Vector(position.x, position.y),
                new Vector(velocityX, velocityY));
        }

        public void ForceMove(double dx, double dy)
        {
            position = new Position(position.x + dx, position.y + dy);
        }


        public Position Position => position;
        public double Radius => diameter / 2;
        public double VelocityX => velocityX;
        public double VelocityY => velocityY;

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