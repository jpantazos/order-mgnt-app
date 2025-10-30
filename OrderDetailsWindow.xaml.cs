using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CibusMed.WPF.ViewModels;

namespace CibusMed.WPF
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>

    public partial class OrderDetailsWindow : Window
    {
        private readonly OrderDetailsViewModel _viewModel;

        public event EventHandler? StatusUpdated;

        public OrderDetailsWindow(int orderId)
        {
            InitializeComponent();
            _viewModel = new OrderDetailsViewModel();
            _viewModel.CloseRequested += (s, e) => Close();
            _viewModel.StatusUpdated += (s, e) => StatusUpdated?.Invoke(this, EventArgs.Empty);
            DataContext = _viewModel;

            Loaded += async (s, e) => await _viewModel.LoadOrderAsync(orderId);
        }
    }
}
