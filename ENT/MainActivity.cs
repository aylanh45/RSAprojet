
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ENT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        NfcAdapter nfcAdapter;
        PendingIntent pendingIntent;
        IntentFilter[] intentFiltersArray;
        string[][] techListsArray;
        string localIp = "192.168.49.1"; // IP Wi-Fi Direct
        string token;
        private string selectedFilePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Affichage standard au démarrage
            ShowStandardView();

            // Initialisation NFC
            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            if (nfcAdapter == null)
            {
                Snackbar.Make(FindViewById(Resource.Id.fab), "NFC non disponible", Snackbar.LengthLong).Show();
                return;
            }

            pendingIntent = PendingIntent.GetActivity(this, 0,
                new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), PendingIntentFlags.Mutable);

            IntentFilter ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            intentFiltersArray = new IntentFilter[] { ndefDetected };
            techListsArray = new string[][] { new string[] { Java.Lang.Class.FromType(typeof(Android.Nfc.Tech.Ndef)).Name } };

            // Génère un token unique
            token = Guid.NewGuid().ToString();

            // Prépare le message NFC avec IP + token
            var payload = $"{localIp}\n{token}";
            var ndefMessage = new NdefMessage(new NdefRecord[]
            {
                NdefRecord.CreateMime("application/vnd.myapp", Encoding.UTF8.GetBytes(payload))
            });
            nfcAdapter.SetNdefPushMessage(ndefMessage, this);
        }

        private void ShowStandardView()
        {
            SetContentView(Resource.Layout.content_main);

            Button btnSendFile = FindViewById<Button>(Resource.Id.btnSendFile);
            if (btnSendFile != null)
            {
                btnSendFile.Click += (s, e) => SelectFile();
            }
        }

        private void ShowShareView(string fileName)
        {
            SetContentView(Resource.Layout.content_share);

            TextView txtInfo = FindViewById<TextView>(Resource.Id.txtShareInfo);
            if (txtInfo != null)
            {
                txtInfo.Text = $"Partage en cours : {Path.GetFileName(fileName)}";
            }
        }

        protected void SelectFile()
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            intent.SetType("*/*");
            intent.AddCategory(Intent.CategoryOpenable);
            StartActivityForResult(Intent.CreateChooser(intent, "Choisir un fichier"), 100);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 100 && resultCode == Result.Ok && data != null)
            {
                Android.Net.Uri uri = data.Data;
                selectedFilePath = GetPathFromUri(uri);

                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    ShowShareView(selectedFilePath);
                    _ = StartFileServer(selectedFilePath);
                }
            }
        }

        private string GetPathFromUri(Android.Net.Uri uri)
        {
            string path = null;
            var projection = new[] { Android.Provider.MediaStore.MediaColumns.Data };
            using (var cursor = ContentResolver.Query(uri, projection, null, null, null))
            {
                if (cursor != null && cursor.MoveToFirst())
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.MediaColumns.Data);
                    path = cursor.GetString(columnIndex);
                }
            }
            return path;
        }

        private async Task StartFileServer(string filePath)
        {
            var listener = new TcpListener(IPAddress.Parse(localIp), 9000);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();
            using (var stream = client.GetStream())
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                await fileStream.CopyToAsync(stream);
            }
        }

        private async void ConnectAndDownload(string ipAddress)
        {
            var client = new TcpClient();
            await client.ConnectAsync(ipAddress, 9000);
            using (var stream = client.GetStream())
            using (var fileStream = new FileStream("/storage/emulated/0/Download/received.txt", FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (nfcAdapter != null)
                nfcAdapter.EnableForegroundDispatch(this, pendingIntent, intentFiltersArray, techListsArray);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (nfcAdapter != null)
                nfcAdapter.DisableForegroundDispatch(this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent.Action == NfcAdapter.ActionNdefDiscovered)
            {
                var rawMsgs = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                if (rawMsgs != null && rawMsgs.Length > 0)
                {
                    var msg = (NdefMessage)rawMsgs[0];
                    var payload = Encoding.UTF8.GetString(msg.GetRecords()[0].GetPayload());
                    var parts = payload.Split('\n');
                    string ipAddress = parts[0];
                    string receivedToken = parts[1];
                    Snackbar.Make(FindViewById(Resource.Id.fab), $"Connect to {ipAddress} with token {receivedToken}", Snackbar.LengthLong).Show();
                    ConnectAndDownload(ipAddress);
                }
            }
        }
    }
}
