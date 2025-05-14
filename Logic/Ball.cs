using Data;
using System;
using Timers = System.Timers;

namespace Logic
{
    internal class Ball : IBall
    {
        private readonly double width, height, diameter;
        private readonly Guid ballId;
        private readonly DataAbstractAPI dataLayer;
        private Position position;
        private double velocityX, velocityY;

        public Ball(Guid id, IVector initialVelocity, DataAbstractAPI dataAPI, double tableWidth, double tableHeight, double ballDiameter)
        {
            ballId = id;
            dataLayer = dataAPI;
            width = tableWidth;
            height = tableHeight;
            diameter = ballDiameter;
            velocityX = initialVelocity.x;
            velocityY = initialVelocity.y;
        }

        public event EventHandler<IPosition>? NewPositionNotification;

        public void OnNewPosition(Data.IVector dataPos)
        {
            position = new Position(dataPos.x, dataPos.y);
            position = position.UpdatePosition(velocityX, velocityY, width, height, diameter, out velocityX, out velocityY);
            NewPositionNotification?.Invoke(this, position);

            dataLayer.UpdateBall(ballId,
                new Data.Vector(position.x, position.y),
                new Data.Vector(velocityX, velocityY));
        }
    }

}
