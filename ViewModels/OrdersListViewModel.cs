using CibusMed.WPF.Commands;
using CibusMed_App.Api.DTOs;
using CibusMed_App.Domains;
using CibusMed_App.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CibusMed.WPF.ViewModels;

public class OrdersListViewModel : ViewModelBase
{
    private readonly OrderApiService _apiService;
    private ObservableCollection<OrderListDto> _orders;
    private OrderListDto? _selectedOrder;
    private bool _isBusy;
    private string _errorMessage = string.Empty;
    private OrderStatus? _selectedStatus;
    private DateTime? _fromDate;
    private DateTime? _toDate;
    private string _searchText = string.Empty;

    public OrdersListViewModel()
    {
        _apiService = new OrderApiService();
        _orders = new ObservableCollection<OrderListDto>();

        LoadOrdersCommand = new AsyncRelayCommand(async _ => await LoadOrdersAsync());
        OpenOrderDetailsCommand = new RelayCommand(OpenOrderDetails, _ => SelectedOrder != null);
        ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

        // Load orders on initialization
        _ = LoadOrdersAsync();
    }

    public ObservableCollection<OrderListDto> Orders
    {
        get => _orders;
        set => SetProperty(ref _orders, value);
    }

    public OrderListDto? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (SetProperty(ref _selectedOrder, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public OrderStatus? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (SetProperty(ref _selectedStatus, value))
            {
                _ = LoadOrdersAsync();
            }
        }
    }

    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            if (SetProperty(ref _fromDate, value))
            {
                _ = LoadOrdersAsync();
            }
        }
    }

    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            if (SetProperty(ref _toDate, value))
            {
                _ = LoadOrdersAsync();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ICommand LoadOrdersCommand { get; }
    public ICommand OpenOrderDetailsCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public event EventHandler<int>? OrderDetailsRequested;

    private async Task LoadOrdersAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var orders = await _apiService.GetOrdersAsync(
                SelectedStatus,
                FromDate,
                ToDate,
                string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading orders: {ex.Message}";
            MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OpenOrderDetails(object? parameter)
    {
        if (SelectedOrder != null)
        {
            OrderDetailsRequested?.Invoke(this, SelectedOrder.Id);
        }
    }

    private void ClearFilters()
    {
        SelectedStatus = null;
        FromDate = null;
        ToDate = null;
        SearchText = string.Empty;
    }
}