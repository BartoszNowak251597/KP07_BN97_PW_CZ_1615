﻿using System;
using System.Windows;
using System.Collections.ObjectModel;
using Presentation.Model;
using Presentation.ViewModel.MVVMLight;
using Presentation.Model;
using ModelIBall = Presentation.Model.IBall;

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
            RemoveBallsCommand = new RelayCommand(RemoveBalls, CanRemoveBalls);
            Balls.CollectionChanged += (s, e) => RemoveBallsCommand.RaiseCanExecuteChanged();
        }

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            //Observer.Dispose();
        }


        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        public RelayCommand RemoveBallsCommand { get; }

        // Remove all balls method
        private void RemoveBalls()
        {
            Balls.Clear();
        }

        // Check if removal is allowed
        private bool CanRemoveBalls()
        {
            return Balls.Any();
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
