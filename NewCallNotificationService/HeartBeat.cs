using System.Timers;
using NewCallNotificationService.DSTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using System.Reflection;

namespace NewCallNotificationService
{
    public class HeartBeat
    {
        StringBuilder sb = new StringBuilder();

        private readonly Timer _timer;
        private string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public HeartBeat()
        {
            _timer = new Timer(60000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {

            DS ds1 = new DS();
            StringBuilder sb = new StringBuilder();
            try { 
            NotificationCallSystemTypeIdTableAdapter typeIdTableAdapter = new NotificationCallSystemTypeIdTableAdapter();
            typeIdTableAdapter.Fill(ds1.NotificationCallSystemTypeId);
            DataTable data1 = (DataTable)typeIdTableAdapter.GetData();
            string str1 = "";
            string str2 = "";
                if (data1.Rows.Count > 0)
                {
                    for (int index1 = 0; index1 < data1.Rows.Count; ++index1)
                    {
                        int num1 = data1.Rows[index1].Field<int>(1);
                        string input = "";
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://notification.sng.com.pl/api/GetAlerts");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Accept = "application/json";
                        httpWebRequest.Method = "POST";
                        using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string str3 = "{\"Dzielnica\":\"Wrzeszcz\",\"Token\":\"testy\"}";
                            streamWriter.Write(str3);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
                            input = streamReader.ReadToEnd();
                        HeartBeat.Person[] personArray = new JavaScriptSerializer().Deserialize<HeartBeat.Person[]>(input);
                        for (int index2 = 0; index2 < personArray.Length; ++index2)
                        {
                            if (num1 == personArray[index2].Id && personArray[index2].Name == "Content")//&& || - coś może się niezgadzać na num1 == personArray
                            {
                                str1 = personArray[index2].value.ToString();
                            }
                            if (num1 == personArray[index2].Id && personArray[index2].Name == "Title")
                            {
                                str2 = personArray[index2].value.ToString();
                            }
                            int id = personArray[index2].Id;
                            string name = personArray[index2].Name;
                            string str3 = personArray[index2].value;
                        }
                        int num2 = data1.Rows[index1].Field<int>(0);
                        new QueriesTableAdapter().NotificationCallUpdateDocXMLSend(new int?(num2));
                        NotificationCallDzielnicIdTableAdapter dzielnicIdTableAdapter = new NotificationCallDzielnicIdTableAdapter();
                        dzielnicIdTableAdapter.Fill(ds1.NotificationCallDzielnicId, new int?(num2));
                        DS.NotificationCallDzielnicIdDataTable dzielnicIdDataTable = new DS.NotificationCallDzielnicIdDataTable();
                        DataTable data2 = (DataTable)dzielnicIdTableAdapter.GetData(new int?(num2));
                        List<string> stringList = new List<string>();
                        for (int index2 = 0; index2 < data2.Rows.Count; ++index2)
                        {
                            try
                            {
                                stringList.Add(data2.Rows[index2].Field<string>(0));
                                new QueriesTableAdapter().NotificationInsertTestTable(data2.Rows[index2].Field<string>(0));
                            }
                            catch (Exception ex)
                            {
                                sb.Append("\n" + "Błąd w NotificationInsertTestTable : " + ex.ToString() + "\n" + "\n");
                                File.AppendAllText(filePath + "log.txt", sb.ToString());
                                sb.Clear();

                            }
                        }

                        if (str1 == "" && str2 == "")
                        {
                            new PushToDaniel().SendNotification(null, "Puste stringi", "nadal jest problem z wysyłaniem", "awarie");
                        }
                        else if (str2 == "SNG Twoje wodociągi. Aktualnie brak informacji o awariach." || str1 == "SNG Twoje wodociągi. Aktualnie brak informacji o awariach.")
                        {
                            new PushToDaniel().SendNotification(null, "Brak Awarii", "wysyła brak awarii", "awarie");
                        }
                        else
                        {
                            foreach (string singledevice in stringList.ToArray())
                            {
                                new PushNotification().SendNotification(singledevice, "SNG Twoje wodociągi", str2 + ". " + str1, "awarie");
                                //wypychanie notyfikacji do firebase
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                sb.Append("\n"+ "błąd w klasie SendNotification: " + ex.ToString() + "\n"+ "\n");
                File.AppendAllText(filePath + "log.txt", sb.ToString());

                sb.Clear();
                
            }
            try
            {
                DS ds2 = new DS();
                NotifiGetNotifiToSendTableAdapter sendTableAdapter = new NotifiGetNotifiToSendTableAdapter();
                sendTableAdapter.Fill(ds2.NotifiGetNotifiToSend);
                DS.NotifiGetNotifiToSendDataTable notifiToSendDataTable = new DS.NotifiGetNotifiToSendDataTable();
                DS.NotifiGetNotifiToSendDataTable data3 = sendTableAdapter.GetData();

            if (data3.Rows.Count <= 0)
                return;

            try
            {
                for (int index1 = 0; index1 < data3.Rows.Count; ++index1)
                {
                    int num = data3.Rows[index1].Field<int>(0);
                    string str3 = data3.Rows[index1].Field<string>(1);
                    string tytul = data3.Rows[index1].Field<string>(2);
                    string message = data3.Rows[index1].Field<string>(3);
                    new QueriesTableAdapter().NotifiUpdateSendNotifiTable(new int?(num));
                    NotifiGetDeviceToSendActionsTableAdapter actionsTableAdapter = new NotifiGetDeviceToSendActionsTableAdapter();
                    actionsTableAdapter.Fill(ds2.NotifiGetDeviceToSendActions, str3);
                    DS.NotifiGetDeviceToSendActionsDataTable data2 = actionsTableAdapter.GetData(str3);
                    DataTable dataTable1 = new DataTable();
                    DataTable dataTable2 = (DataTable)data2;
                    List<string> stringList = new List<string>();
                    for (int index2 = 0; index2 < dataTable2.Rows.Count; ++index2)
                    {
                        try
                        {
                            stringList.Add(dataTable2.Rows[index2].Field<string>(0));
                            new QueriesTableAdapter().NotificationInsertTestTable(dataTable2.Rows[index2].Field<string>(0));
                        }
                        catch (Exception ex)
                        {
                                sb.Append("\n" + "błąd w NotifiGetDeviceToSendActionsTableAdapter:  " + ex.ToString() + "\n" + "\n");
                                File.AppendAllText(filePath + "log.txt", sb.ToString());
                                sb.Clear();
                            }
                    }
                    if (str3 == "awarie")
                    { }
                    else
                    {
                        sb.Append("Wysyła informacje: ");
                            File.AppendAllText(filePath + "log.txt", sb.ToString());                       
                            sb.Clear();
                            foreach (string singledevice in stringList.ToArray())
                            {
                                new PushNotification().SendNotification(singledevice, tytul, message, str3);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                    sb.Append("\n" + "błąd  w  wysylaniu Notyfikacji: " + ex.ToString() + "\n" + "\n");
                    File.AppendAllText(filePath + "log.txt", sb.ToString());
                    sb.Clear();
                }
            }
            catch (Exception ex)
            {
                sb.Append("\n" + "błąd 2: " + ex.ToString() + "\n" + "\n");
                sb.Clear();
            }


        }
        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string value { get; set; }
        }
    }
}