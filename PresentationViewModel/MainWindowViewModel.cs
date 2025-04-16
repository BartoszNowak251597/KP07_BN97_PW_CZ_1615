﻿using System;
using System.Windows;
using System.Collections.ObjectModel;
using Presentation.Model;
using Presentation.ViewModel.MVVMLight;
using Presentation.Model;
using ModelIBall = Presentation.Model.IBall;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
            GenerateCommand = new RelayCommand(GenerateFromCommand);
        }
        public void InitializeTableSettings(double width, double height, double diameter)
        {
            ModelLayer.SetTableSettings(width, height, diameter);
        }
        #endregion ctor

        #region public API

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        private int _ballCount = 12;
        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                RaisePropertyChanged(nameof(BallCount));
            }
        }

        public ICommand GenerateCommand { get; }

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
        }

        private void GenerateFromCommand()
        {
            Balls.Clear(); 
            Start(BallCount);
        }

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}
