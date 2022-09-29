using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

class PushToDaniel
{ 
    public void SendNotification(string[] deviceId, string tytul, string message, string click_act)
    {

        // foreach (string singledevice in deviceId)
        {


            String serverKey = "AAAA_-XmlZA:APA91bF2IwC3vVd5nlDlrSwHWsd0tfg0KMDSgQnjOuZOEQuIBlWGiLB8GLejqZI2hcZGiTKtvZbIC5dRElYipvHFtqLTYbesxI6k-PuqLFobZUTcFFyKJwLCe6bdh8sY3WtxffKhvPuS";
            String senderId = "1099073754512";
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var objNotification = new
            {
                to = "eNoTjjL_MPQ:APA91bHEAyuyfRLi53A8ubmVI1jkUWxkbEpc5DJmmSsS7Zfnn0dJI5ObTvSY5SvZ5PHb4rXHFNMcIBcPHDIzJkw3IYpdikEWRS7icEhcyxHCsMs47GBGCP1gmNDl6H3D0mLQ_7I5CXyW", //singledevice,
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
                                //new NotificationBLL().InsertNotificationLog(dayNumber, notification, true);
                            }
                            else if (response.failure == 1)
                            {
                                // new NotificationBLL().InsertNotificationLog(dayNumber, notification, false);
                                // sbLogger.AppendLine(string.Format("Error sent from FCM server, after sending request : {0} , for following device info: {1}", responseFromFirebaseServer, jsonNotificationFormat));

                            }

                        }
                    }

                }
            }
        }


        //try
        //{
        //    string str1 = "AAAA_-XmlZA:APA91bF2IwC3vVd5nlDlrSwHWsd0tfg0KMDSgQnjOuZOEQuIBlWGiLB8GLejqZI2hcZGiTKtvZbIC5dRElYipvHFtqLTYbesxI6k-PuqLFobZUTcFFyKJwLCe6bdh8sY3WtxffKhvPuS";
        //    string str2 = "1099073754512";
        //    WebRequest webRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //    webRequest.Method = "post";
        //    webRequest.ContentType = "application/json";
        //    var data = new
        //    {
        //        registration_ids = deviceId,
        //        notification = new
        //        {
        //            title = tytul,
        //            body = message,
        //            click_action = click_act
        //        },
        //        priority = "high",
        //        sound = "Enabled"
        //    };
        //    byte[] bytes = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize((object)data));
        //    webRequest.Headers.Add(string.Format("Authorization: key={0}", (object)str1));
        //    webRequest.Headers.Add(string.Format("Sender: id={0}", (object)str2));
        //    webRequest.ContentLength = (long)bytes.Length;
        //    using (Stream requestStream = webRequest.GetRequestStream())
        //    {
        //        requestStream.Write(bytes, 0, bytes.Length);
        //        using (WebResponse response = webRequest.GetResponse())
        //        {
        //            using (Stream responseStream = response.GetResponseStream())
        //            {
        //                using (StreamReader streamReader = new StreamReader(responseStream))
        //                    streamReader.ReadToEnd();
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    ex.ToString();
        //}
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