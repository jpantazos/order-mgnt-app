using CibusMed_App.Api.DTOs;
using CibusMed_App.Domains;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace CibusMed_App.Services;

public class OrderApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5136")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<OrderListDto>> GetOrdersAsync(
        OrderStatus? status = null,
        DateTime? from = null,
        DateTime? to = null,
        string? searchQuery = null)
    {
        var queryParams = new List<string>();

        if (status.HasValue)
            queryParams.Add($"status={status.Value}");

        if (from.HasValue)
            queryParams.Add($"from={from.Value:O}");

        if (to.HasValue)
            queryParams.Add($"to={to.Value:O}");

        if (!string.IsNullOrWhiteSpace(searchQuery))
            queryParams.Add($"q={Uri.EscapeDataString(searchQuery)}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _httpClient.GetAsync($"/api/orders{query}");

        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<OrderListDto>>(_jsonOptions);
        return orders ?? new List<OrderListDto>();
    }

    public async Task<OrderDetailsDto?> GetOrderDetailsAsync(int orderId)
    {
        var response = await _httpClient.GetAsync($"/api/orders/{orderId}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDetailsDto>(_jsonOptions);
    }

    public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        try
        {
            var content = JsonContent.Create(new { NewStatus = newStatus });
            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/status", content);

            if (response.IsSuccessStatusCode)
            {
                return (true, "Status updated successfully");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
