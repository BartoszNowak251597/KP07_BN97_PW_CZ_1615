using System.Diagnostics;
using UnderneathLayerAPI = Data.DataAbstractAPI;

namespace Logic
{
    internal class LogicImplementation : LogicAbstractAPI
    {
        #region ctor

        public LogicImplementation() : this(null)
        { }

        internal LogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region LogicAbstractAPI

        public override void SetLogicParameters(double width, double height, double diameter)
        {
            this.tableWidth = width;
            this.tableHeight = height;
            this.diameter = diameter;
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                var logicBall = new Ball(dataBall.Id, dataBall.Velocity, layerBellow, tableWidth, tableHeight, diameter);
                dataBall.NewPositionNotification += (s, pos) => logicBall.OnNewPosition(pos);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;
        private double tableWidth;
        private double tableHeight;
        private double diameter;

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}