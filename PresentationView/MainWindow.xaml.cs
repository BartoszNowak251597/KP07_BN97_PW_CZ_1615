

using System;
using System.Reactive;
using System.Windows;
using Presentation.ViewModel;


namespace PresentationView;

/// <summary>
/// View implementation
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        double tableWidth = TableBorder.ActualWidth;
        double tableHeight = TableBorder.ActualHeight;
        double diameter = 50.0;

        if (DataContext is MainWindowViewModel vm)
        {
            vm.InitializeTableSettings(tableWidth, tableHeight, diameter);
        }

    }

    /// <summary>
    /// Raises the <seealso cref="System.Windows.Window.Closed"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.Dispose();
        base.OnClosed(e);
    }
}