namespace Logic
{
    public abstract class LogicAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static LogicAbstractAPI GetLogicLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region Layer API

        public abstract void SetLogicParameters(double width, double height);

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #endregion Layer API

        #region private

        private static Lazy<LogicAbstractAPI> modelInstance = new Lazy<LogicAbstractAPI>(() => new LogicImplementation());

        #endregion private
    }
    public record Dimensions(double BallDimension, double TableHeight, double TableWidth);

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
        Guid Id { get; }
        double Radius { get; }
        double Mass { get; }

    }
}
