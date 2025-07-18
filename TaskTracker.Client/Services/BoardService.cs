using System.Net.Http.Json;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Services;

public class BoardService : IBoardService
{
    private readonly HttpClient _http;

    public BoardService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BoardDto>> GetBoardsByUserAsync(Guid userId)
    {
        var response = await _http.GetAsync($"api/board/by-user/{userId}");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch boards. Status: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<List<BoardDto>>() ?? new();
    }

    public async Task<BoardDto?> GetBoardByIdAsync(Guid boardId)
    {
        var response = await _http.GetAsync($"api/board/{boardId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoardDto>();
    }

    public async Task<List<ColumnDto>> GetColumnsAsync(Guid boardId)
    {
        var response = await _http.GetAsync($"api/column/by-board/{boardId}");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch columns. Status: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<List<ColumnDto>>() ?? new();
    }

    public async Task<List<CardDto>> GetCardsAsync(Guid columnId)
    {
        var response = await _http.GetAsync($"api/card/by-column/{columnId}");
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch cards. Status: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<List<CardDto>>() ?? new();
    }
}
