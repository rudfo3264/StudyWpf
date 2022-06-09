using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class DataBaseViewModel : Screen
    {
        private string brokerUrl;
        private bool isConnected;
        public string BrokerUrl
        {
            get { return brokerUrl; }
            set 
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);
            }
        }
        private string topic;

        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

      

        private string connString;
        
        private string dbLog;
       
        public string DbLog
        {
            get { return dbLog; }
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }

        public string ConnString
        {
            get { return connString; }
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }
       

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "127.0.0.1";
            Topic = Commons.PUB_TOPIC = "home/device/fakedata/";
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            if (Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }
        }
        /// <summary>
        /// DB연결 + MQTT Broker 접속
        /// </summary>
        public void ConnectDb()
        {
            if (isConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);

                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                        new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        UpdateText(">>> MQTT Broker Connected");

                        Commons.IS_CONNECT = true;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else//접속 끄기
            {
                try
                {
                    if (Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected...");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }

        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender,MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            UpdateText(message);
        }
    }
}