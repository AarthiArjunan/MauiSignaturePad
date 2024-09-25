

namespace SignaturePadImage
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
           
        }

        private async void CounterBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NextPage());
        }
    }



}
