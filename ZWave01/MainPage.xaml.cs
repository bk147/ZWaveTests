using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Foundation;
//using Windows.Foundation.Collections;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;

using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Web.Http.Filters;

using Windows.Data.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ZWave01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpBaseProtocolFilter filter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;

        public MainPage()
        {
            this.InitializeComponent();

            filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            httpClient = new HttpClient(filter);

            string username = "admin";
            string password = "admin";
            var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(username + ":" + password, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            string base64token = Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(buffer);
            HttpCredentialsHeaderValue authHeader = new HttpCredentialsHeaderValue("Basic", base64token);

//            var ah = new HttpCredentialsHeaderValue("Authentication", "Basic YWRtaW46YWRtaW4=")
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

//            httpClient.DefaultRequestHeaders.Remove("Accept-Encoding");
//            httpClient.DefaultRequestHeaders.Remove("AcceptEncoding");
//            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "");

            cts = new CancellationTokenSource();
        }

        internal static async Task DisplayTextResultAsync(
            HttpResponseMessage response,
            TextBox output,
            CancellationToken token)
        {
            output.Text = "";
            string responseBodyAsText;
            output.Text += SerializeHeaders(response);
            responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(token);

            token.ThrowIfCancellationRequested();

            // Insert new lines.
            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);

            output.Text += responseBodyAsText;
        }

        internal static string SerializeHeaders(HttpResponseMessage response)
        {
            StringBuilder output = new StringBuilder();

            // We cast the StatusCode to an int so we display the numeric value (e.g., "200") rather than the
            // name of the enum (e.g., "OK") which would often be redundant with the ReasonPhrase.
            output.Append(((int)response.StatusCode) + " " + response.ReasonPhrase + "\r\n");

            SerializeHeaderCollection(response.Headers, output);
            SerializeHeaderCollection(response.Content.Headers, output);
            output.Append("\r\n");
            return output.ToString();
        }

        internal static void SerializeHeaderCollection(
            IEnumerable<KeyValuePair<string, string>> headers,
            StringBuilder output)
        {
            foreach (var header in headers)
            {
                output.Append(header.Key + ": " + header.Value + "\r\n");
            }
        }

        internal static bool TryGetUri(string uriString, out Uri uri)
        {
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
            // since the user may provide URIs for servers located on the internet or intranet. If apps only
            // communicate with servers on the internet, only the "Internet (Client)" capability should be set.
            // Similarly if an app is only intended to communicate on the intranet, only the "Home and Work
            // Networking" capability should be set.
            if (!Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out uri))
            {
                return false;
            }

            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                return false;
            }

            return true;
        }

        private async void Light00_Click(object sender, RoutedEventArgs e)
        {
            Uri resourceUri;

            TryGetUri("http://raspberrypi0:8083/ZAutomation/api/v1/devices/LightScene_3/command/on", out resourceUri);
//            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
            await DisplayTextResultAsync(response, OutputField, cts.Token);
        }

        private async void Light20_Click(object sender, RoutedEventArgs e)
        {
            Uri resourceUri;

            TryGetUri("http://raspberrypi0:8083/ZAutomation/api/v1/devices/LightScene_4/command/on", out resourceUri);
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
            await DisplayTextResultAsync(response, OutputField, cts.Token);
        }

        private async void Light30_Click(object sender, RoutedEventArgs e)
        {
            Uri resourceUri;

            TryGetUri("http://raspberrypi0:8083/ZAutomation/api/v1/devices/LightScene_5/command/on", out resourceUri);
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
            await DisplayTextResultAsync(response, OutputField, cts.Token);
        }

        private async void Devices_Click(object sender, RoutedEventArgs e)
        {
            Uri resourceUri;

            TryGetUri("http://raspberrypi0:8083/ZAutomation/api/v1/devices", out resourceUri);
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri).AsTask(cts.Token);
            await DisplayTextResultAsync(response, OutputField, cts.Token);
        }
    }
}
