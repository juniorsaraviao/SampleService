namespace SampleService
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            StringConstants.Message = "Enter App.xaml.cs\n";

            MainPage = new MainPage();
        }
    }
}
