using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMvvmApp.Models;

namespace WpfMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //View에서 사용하기 위해 만든 멤버변수
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate;

        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate;

        private string outAdult;
        private string outBirthday;

        //실제 사용하는 속성
        public string InFirstName
        {
            get { return inFirstName; }
            set
            {
                inFirstName = value;
                RaisePropertyChanged("InFirstName");    //값이 바뀜 공지
            }
        }
        public string InLastName
        {
            get { return inLastName; }
            set
            {
                inLastName = value;
                RaisePropertyChanged("InLastName");
            }
        }
        public string InEmail
        {
            get { return inEmail; }
            set
            {
                inEmail = value;
                RaisePropertyChanged("InEmail");
            }
        }
        public DateTime InDate
        {
            get { return inDate; }
            set
            {
                inDate = value;
                RaisePropertyChanged("InDate");
            }
        }
        public string OutFirstName
        {
            get { return outFirstName; }
            set
            {
                outFirstName = value;
                RaisePropertyChanged("OutFirstName");
            }
        }

        public string OutLastName
        {
            get { return outLastName; }
            set
            {
                outLastName = value;
                RaisePropertyChanged("outLastName");
            }
        }
        public string OutEmail
        {
            get { return outEmail; }
            set
            {
                outEmail = value;
                RaisePropertyChanged("outEmail");
            }
        }
        public string OutDate
        {
            get { return outDate; }
            set
            {
                outDate = value;
                RaisePropertyChanged("outDate");
            }
        }
        public string OutAdult
        {
            get { return outAdult; }
            set
            {
                outAdult = value;
                RaisePropertyChanged("outAdult");

            }
        }
        public string OutBirthday
        {
            get { return outBirthday; }
            set
            {
                outBirthday = value;
                RaisePropertyChanged("outBirthday");

            }
        }

        //값이 전부 적용되서 버튼을 활성화하기 위한 명령
        private ICommand proceedCommand;
        public ICommand ProceedCommand
        {
            get { return proceedCommand ?? (
                  proceedCommand = new RelayCommand<object>(
                o => Proceed(), o => !string.IsNullOrEmpty(inFirstName) &&
                !string.IsNullOrEmpty(inLastName) &&
                !string.IsNullOrEmpty(inEmail) &&
                !string.IsNullOrEmpty(inDate.ToString())
                ));
            }
        }
        // 버튼 클릭시 일어나는 실제 명령의 실체
        private async void Proceed()
        {
            try
            {
                Person person = new Person(InFirstName, InLastName, InEmail, InDate);

                await Task.Run(() => OutFirstName = person.FirstName);
                await Task.Run(() => OutLastName = person.LastName);
                await Task.Run(() => OutEmail = person.Email);
                await Task.Run(() => OutDate = person.Date.ToString("yyyy-MM-dd"));
                await Task.Run(() => OutAdult = person.IsAdult.ToString());
                await Task.Run(() => OutBirthday = person.IsBirthday.ToString());

                //
            }
            catch (Exception)
            {
                MessageBox.Show("예외 발생");
            }
        }
        public MainViewModel()
        {
            this.InDate = DateTime.Parse("1990-01-01");
        }
    }
}
