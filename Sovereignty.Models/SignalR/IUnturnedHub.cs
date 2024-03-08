namespace Sovereignty.Models.SignalR;

public interface IUnturnedHub
{
    Task SendMessage(string message);
    Task<string> ReceiveMessage();
}