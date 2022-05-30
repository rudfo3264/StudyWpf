using System.Windows;
//alt enter
//ctrl space
namespace WpfBasicApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello WPF!!","First WPF");
        }
    }
}
