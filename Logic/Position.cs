﻿using Data;

namespace Logic
{
    internal record Position : IPosition
    {
        #region IPosition

        public double x { get; init; }
        public double y { get; init; }

        #endregion IPosition
        public Position(double posX, double posY)
        {
            x = posX;
            y = posY;
        }
    }
}
