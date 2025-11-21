namespace RSAprojet.Services
{
    public interface INfcService
    {
        void StartListening();
        void StopListening();

        void SendText(string text);

        event EventHandler<string>? OnMessageReceived;
    }
}