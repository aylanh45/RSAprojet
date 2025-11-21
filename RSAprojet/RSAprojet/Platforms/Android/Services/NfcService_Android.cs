using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using System.Text;
using RSAprojet.Services;

namespace RSAprojet.Platforms.Android.Services
{
    public class NfcService_Android : Java.Lang.Object, INfcService, NfcAdapter.IReaderCallback
    {
        private readonly Activity _activity;
        private readonly NfcAdapter? _nfcAdapter;

        public event EventHandler<string>? OnMessageReceived;

        public NfcService_Android()
        {
            _activity = Platform.CurrentActivity!;
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(_activity);
        }

        public void StartListening()
        {
            if (_nfcAdapter == null) return;

            var options = new Bundle();
            _nfcAdapter.EnableReaderMode(
                _activity,
                this,
                NfcReaderFlag.NfcA | NfcReaderFlag.NfcB | NfcReaderFlag.NfcF | NfcReaderFlag.NfcV,
                options
            );
        }

        public void StopListening()
        {
            _nfcAdapter?.DisableReaderMode(_activity);
        }

        public void OnTagDiscovered(Tag tag)
        {
            var ndef = Ndef.Get(tag);
            if (ndef == null) return;

            ndef.Connect();
            var msg = ndef.NdefMessage;
            ndef.Close();

            var record = msg.Records[0];
            var text = Encoding.UTF8.GetString(record.GetPayload());

            OnMessageReceived?.Invoke(this, text);
        }

        public void SendText(string text)
        {
            if (_nfcAdapter == null) return;

            var payload = Encoding.UTF8.GetBytes(text);
            var record = new NdefRecord(
                NdefRecord.TnfWellKnown,
                Encoding.ASCII.GetBytes("T"),
                new byte[0],
                payload
            );

            var message = new NdefMessage(new[] { record });
            NfcAdapter.SetNdefPushMessage(message, _activity);
        }
    }
}