using OpenMod.API.Ioc;

namespace V.Panel.Models.SignalR;

[Service]
public interface IUnturnedClient
{
    Task SendMessage(string message);
    Task<string> ReceiveMessage();
}