using Data;
using System;
using Timers = System.Timers;

namespace Logic
{
    internal class Ball : IBall
    {
        private double width;
        private double height;
        private double diameter;

        private Position position;
        private double velocityX;
        private double velocityY;
        private readonly Timers.Timer movementTimer;

        public Ball(Data.IBall ball, double tableWidth, double tableHeight, double ballDiameter)
        {
            width = tableWidth;
            height = tableHeight;
            diameter = ballDiameter;

            // Start position and random velocity
            Random rand = new Random();
            velocityX = (rand.NextDouble()) * 4;
            velocityY = (rand.NextDouble()) * 4;

            ball.NewPositionNotification += RaisePositionChangeEvent;

        }

        public event EventHandler<IPosition>? NewPositionNotification;

       

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            var position = new Position(e.x, e.y);
            position = position.UpdatePosition(velocityX, velocityY, width, height, diameter, out velocityX, out velocityY);
            NewPositionNotification?.Invoke(this, position);
        }
    }
}
