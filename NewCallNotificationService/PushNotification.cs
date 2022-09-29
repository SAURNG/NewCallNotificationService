using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

class PushNotification
{
    private string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


    public void SendNotification(string singledevice, string tytul, string message, string click_act)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Wysłano wiadomość:" + tytul + " " + message);
        //AppendAllText(@"\NewCallNotificationService\\log.txt", sb.ToString());
        File.AppendAllText(filePath + "log.txt", sb.ToString());

        sb.Clear();

        try
        {

            String serverKey = "AAAA_-XmlZA:APA91bF2IwC3vVd5nlDlrSwHWsd0tfg0KMDSgQnjOuZOEQuIBlWGiLB8GLejqZI2hcZGiTKtvZbIC5dRElYipvHFtqLTYbesxI6k-PuqLFobZUTcFFyKJwLCe6bdh8sY3WtxffKhvPuS";
            String senderId = "1099073754512";
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var objNotification = new
            {
                to = singledevice,
                message_id = "mid-" + DateTime.Now.ToString(),
                data = new
                {
                    title = tytul,
                    body = message,
                    click_action = click_act
                },
                notification = new
                {
                    title = tytul,
                    body = message,
                    click_action = click_act
                },
                priority = "high",
                sound = "Enabled"

            };
            string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
            tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            tRequest.ContentLength = byteArray.Length;
            tRequest.ContentType = "application/json";
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);

                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String responseFromFirebaseServer = tReader.ReadToEnd();

                            FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);
                            if (response.success == 1)
                            {
                            }
                            else if (response.failure == 1)
                            {
                            }

                        }
                    }

                }
            }
        }catch(Exception ex)
        {
            sb.Append("\n" + "Występowanie błedu w klasie PushNotification: " + "\n"+
                "token: " + singledevice
                +"\n" + ex.ToString() + "\n" + "\n");
            File.AppendAllText(filePath + "log.txt", sb.ToString());
            sb.Clear();
        }
    }
    public class FCMResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<FCMResult> results { get; set; }
    }
    public class FCMResult
    {
        public string message_id { get; set; }
    }
}