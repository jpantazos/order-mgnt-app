using System.Windows;
using System.Windows.Input;
using CibusMed.WPF.ViewModels;

namespace CibusMed.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OrdersListViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new OrdersListViewModel();
            _viewModel.OrderDetailsRequested += OnOrderDetailsRequested;
            DataContext = _viewModel;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedOrder != null)
            {
                OpenOrderDetails(_viewModel.SelectedOrder.Id);
            }
        }

        private void OnOrderDetailsRequested(object? sender, int orderId)
        {
            OpenOrderDetails(orderId);
        }

        private void OpenOrderDetails(int orderId)
        {
            var detailsWindow = new OrderDetailsWindow(orderId);
            detailsWindow.Owner = this;
            detailsWindow.StatusUpdated += (s, e) => _viewModel.LoadOrdersCommand.Execute(null);
            detailsWindow.ShowDialog();
        }
    }
}