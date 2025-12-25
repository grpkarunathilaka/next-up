using Microsoft.AspNetCore.SignalR;

namespace TodoApi.Hubs;

public class TodoHub : Hub
{
    public async Task NotifyTodoCreated(string todoJson)
    {
        await Clients.Others.SendAsync("TodoCreated", todoJson);
    }

    public async Task NotifyTodoUpdated(string todoJson)
    {
        await Clients.Others.SendAsync("TodoUpdated", todoJson);
    }

    public async Task NotifyTodoDeleted(string todoId)
    {
        await Clients.Others.SendAsync("TodoDeleted", todoId);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}