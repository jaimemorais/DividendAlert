using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DividendAlertApi.Services.Push
{
    public class PushService : IPushService
    {
        private readonly IConfiguration _config;

        public PushService(IConfiguration config)
        {
            _config = config;
        }


        private const string FIREBASE_CLOUD_MESSAGING_SEND_URI = "https://fcm.googleapis.com/fcm/send";

        public async Task SendPushAsync(string userFcmToken, string title, string body)
        {
            string jsonMessage =
                "{\"to\" : \"" + userFcmToken + "\", " +
                " \"data\": {\"title\": \" " + title + "\", \"body\": \"" + body + "\"}" +
                "}";


            var request = new HttpRequestMessage(HttpMethod.Post, FIREBASE_CLOUD_MESSAGING_SEND_URI);
            request.Headers.TryAddWithoutValidation("Authorization", "key=" + _config["FIREBASE_CLOUD_MESSAGING_KEY"]);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                result = await client.SendAsync(request);

                if (result.IsSuccessStatusCode)
                {
                    // "Push sent OK.";
                }
                else
                {
                    // TODO
                    // "Error sending Push. Firebase Cloud Messaging HttpStatusCode : " + (int)result.StatusCode;
                }
            }
        }

    }
}
