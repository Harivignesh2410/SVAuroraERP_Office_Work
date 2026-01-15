namespace SVAuroraERP.WebUI.Custom
{
    //Added on 2025.02.24
    public class NotificationHub: Hub
    {
        // This method will be called to send notifications to connected clients
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}