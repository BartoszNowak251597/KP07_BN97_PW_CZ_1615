﻿using System; 
using System.ComponentModel; 
using System.Diagnostics; 
using System.Reactive; 
using System.Reactive.Linq; 
using System.Reflection;
using UnderneathLayerAPI = Logic.LogicAbstractAPI;

namespace Presentation.Model
{
    internal class ModelImplementation : ModelAbstractApi
    {
        internal ModelImplementation() : this(null)
        { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
        }

        #region ModelAbstractApi

        public override void EnableDiagnostics(string filePath)
        {
            layerBellow.EnableDiagnostics(filePath);
        }


        public override void SetTableSettings(double width, double height)
        {
            layerBellow.SetLogicParameters(width, height);
        }
        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Model));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        #endregion ModelAbstractApi

        #region API

        public event EventHandler<BallChaneEventArgs> BallChanged;

        #endregion API

        #region private

        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
        private readonly UnderneathLayerAPI layerBellow = null;
        private double diameter;

        private void StartHandler(Logic.IPosition position, Logic.IBall ball)
        {
            ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = ball.Radius*2 };
            BallChanged.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
        {
            returnNumberOfBalls(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
        {
            returnBallChangedIsNull(BallChanged == null);
        }

        #endregion TestingInfrastructure
    }

    public class BallChaneEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }
}
