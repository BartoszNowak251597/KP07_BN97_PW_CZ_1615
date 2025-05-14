using Data;
using System;
using Timers = System.Timers;

namespace Logic
{
    internal class Ball : IBall
    {
        private readonly double width;
        private readonly double height;
        private readonly double diameter;

        private readonly Data.IBall dataBall;
        private Position position;
        private double velocityX;
        private double velocityY;

        public Ball(Data.IBall ball, double tableWidth, double tableHeight, double ballDiameter)
        {
            // U¿ycie prêdkoœci z warstwy data
            dataBall = ball;
            width = tableWidth;
            height = tableHeight;
            diameter = ballDiameter;

            // Inicjalizacja prêdkoœci zaczytu z Data.IBall.Velocity
            velocityX = dataBall.Velocity.x;
            velocityY = dataBall.Velocity.y;

            // Subskrypcja pozycji
            dataBall.NewPositionNotification += RaisePositionChangeEvent;
        }

        public event EventHandler<IPosition>? NewPositionNotification;



        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            // Aktualizacja pozycji na bazie wektora pozycji otrzymanego z Data
            position = new Position(e.x, e.y);

            // Obliczenie nowej pozycji z uwzglêdnieniem prêdkoœci
            position = position.UpdatePosition(velocityX, velocityY,
                                              width, height, diameter,
                                              out velocityX, out velocityY);
            // Powiadomienie warstwy prezentacji
            NewPositionNotification?.Invoke(this, position);
        }
    }
}
