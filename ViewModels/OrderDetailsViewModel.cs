using CibusMed_App.Api.DTOs;
using CibusMed_App.Domains;
using CibusMed_App.Services;
using System.Windows;
using System.Windows.Input;
using CibusMed.WPF.Commands;

namespace CibusMed.WPF.ViewModels;

public class OrderDetailsViewModel : ViewModelBase
{
    private readonly OrderApiService _apiService;
    private OrderDetailsDto? _order;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    public OrderDetailsViewModel()
    {
        _apiService = new OrderApiService();
        UpdateStatusCommand = new AsyncRelayCommand(async parameter => await UpdateStatusAsync(parameter));
        CloseCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public OrderDetailsDto? Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
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

    public ICommand UpdateStatusCommand { get; }
    public ICommand CloseCommand { get; }

    public event EventHandler? CloseRequested;
    public event EventHandler? StatusUpdated;

    public async Task LoadOrderAsync(int orderId)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Order = await _apiService.GetOrderDetailsAsync(orderId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading order details: {ex.Message}";
            MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateStatusAsync(object? parameter)
    {
        if (Order == null || parameter is not OrderStatus newStatus)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = await _apiService.UpdateOrderStatusAsync(Order.Id, newStatus);

            if (result.Success)
            {
                Order = await _apiService.GetOrderDetailsAsync(Order.Id);
                StatusUpdated?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Order status updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                throw new Exception(result.Message);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating order status: {ex.Message}";
            MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
