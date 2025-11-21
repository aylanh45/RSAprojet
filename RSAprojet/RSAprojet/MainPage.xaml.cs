public partial class MainPage : ContentPage
{
    private readonly INfcService _nfc;

    public MainPage(INfcService nfc)
    {
        InitializeComponent();
        _nfc = nfc;

        _nfc.OnMessageReceived += (s, txt) =>
        {
            Dispatcher.Dispatch(async () =>
            {
                await DisplayAlert("Message NFC reçu", txt, "OK");
            });
        };
    }

    private void OnReceiveBtnClicked(object sender, EventArgs e)
    {
        _nfc.StartListening();
    }

    private void OnSendBtnClicked(object sender, EventArgs e)
    {
        _nfc.SendText("Contenu du fichier TXT");
    }
}