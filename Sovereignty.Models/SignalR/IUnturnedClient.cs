using OpenMod.API.Ioc;

namespace Sovereignty.Models.SignalR;

[Service]
public interface IUnturnedClient
{
    Task SendMessage(string message);
    Task<string> ReceiveMessage();
}