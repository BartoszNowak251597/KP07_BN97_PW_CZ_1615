namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
        public abstract void UpdateBall(Guid id, IVector newPosition, IVector newVelocity);

        public virtual void EnableDiagnostics(string filePath) { }

        public abstract void LogWallCollision(Guid id, double x, double y, string wall);
        public abstract void LogBallCollision(Guid id1, double x1, double y1, Guid id2, double x2, double y2);


        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        #endregion private
    }

    public interface IVector
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        Guid Id { get; }
        IVector Velocity { get; set; }
        double Diameter { get; }
        double Weight { get; }
        void UpdateFromLogic(IVector newPosition, IVector newVelocity);
    }
}