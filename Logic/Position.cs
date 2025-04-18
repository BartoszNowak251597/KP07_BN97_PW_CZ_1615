﻿using Data;

namespace Logic
{
    internal record Position : IPosition
    {
        public double x { get; init; }
        public double y { get; init; }

        public Position(double posX, double posY)
        {
            x = posX;
            y = posY;
        }

        public Position UpdatePosition(double velocityX, double velocityY, double width, double height, double diameter, out double updatedVx, out double updatedVy)
        {
            double newX = x + velocityX;
            double newY = y + velocityY;

            updatedVx = velocityX;
            updatedVy = velocityY;

            // Odbicie od lewej/prawej
            if (newX < 0)
            {
                newX = 0;
                updatedVx *= -1;
            }
            else if (newX + diameter > width)
            {
                newX = width - diameter;
                updatedVx *= -1;
            }

            // Odbicie od góry/dół
            if (newY < 0)
            {
                newY = 0;
                updatedVy *= -1;
            }
            else if (newY + diameter > height)
            {
                newY = height - diameter;
                updatedVy *= -1;
            }

            return new Position(newX, newY);
        }
    }
}

