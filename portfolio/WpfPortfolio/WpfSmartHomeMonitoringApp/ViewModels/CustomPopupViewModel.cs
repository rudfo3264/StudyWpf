using Caliburn.Micro;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    class CustomPopupViewModel  : Conductor<object>
    {
        private string brokerIp;
        private string topic;

        public string BrokerIp
        {
            get { return brokerIp; }
            set
            {
                brokerIp = value;
                NotifyOfPropertyChange(() => BrokerIp);
            }
        }
        public string Topic
        {
            get
            {
                return topic;
            }

            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        public CustomPopupViewModel(string Title)
        {
            this.DisplayName = Title;

            BrokerIp = "127.0.0.1";
            Topic = "home/device/#";
        }

        public void AcceptClose()
        {
            Commons.BROKERHOST = BrokerIp;
            Commons.PUB_TOPIC = Topic;
            //  창닫기
            TryCloseAsync(true);
        }
    }
}