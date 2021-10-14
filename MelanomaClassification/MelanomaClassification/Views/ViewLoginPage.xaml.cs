using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewLoginPage : ContentPage
    {
        private PresenterLoginPage pLoginPage;
     
        public ICommand OnTapLogin
        {
            get
            {
                return new Command(async () =>
                {
                    Console.WriteLine("logging in");
                    LoggingInIndicator.IsVisible = true;
#if DEBUG
                    ModelAccountPage.Username = "t@mail.com";
                    bool loggedIn = true;
#else
                    bool loggedIn = await UserService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
#endif
                    string preferredClassifier = "local";//await DatabaseService.GetUserPreferredClassifier();
                    if (preferredClassifier == "local")

                        ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
                    else ClassifierServiceFactory.SetClassifier(new WebClassifierService());

                    LoggingInIndicator.IsVisible = false;
                    if (true)
                    {
                        DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                        Shell.Current.FindByName<ShellContent>("OnLogin").IsVisible = false;
                        if (Shell.Current.FindByName<TabBar>("AfterLogin") == null)
                        {
                            await DisplayAlert("Fatal Error", "After Login not found", "OK");
                        }
                        DatabaseService.LoadData();
                        Shell.Current.FindByName<TabBar>("AfterLogin").IsVisible = true;

                        
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Unsuccessful log in attempt, please try again", "OK");

                    }
                });
            }

        }
        public ICommand Cmd_TestLocally
        {
            get => new Command(TestLocally);
            set { }
        }
        public static List<string> GetFiles()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var list = assembly.GetManifestResourceNames();
            string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //string projectDirectory = Directory.GetParent(rootDir).Parent.Parent.FullName;

            string[] files = Directory.GetFiles(rootDir, "*.jpg", SearchOption.AllDirectories);
            try
            {
                var malignFiles = Directory.GetFiles(Path.Combine(rootDir, "Assets/Malign"));

            }catch(Exception e)
            {

            }
            /*List<string> fileList = new List<string>();

            foreach (var item in list)
            {
                if (item.Contains("ISIC"))
                {
                    fileList.Add(item);
                }
            }*/
            return files.ToList();
        }


        public static void TestLocally()
        {
            var files = GetFiles();
            var classifier = DependencyService.Get<ILocalClassifierService>();
            foreach (var file in files)
            {
                Debug.WriteLine(file);
            }

            string filename = Path.GetFileName(files[0]);
            string path = Path.Combine("../Assets/Benign", filename);

            Stream s = new StreamReader(path).BaseStream;
            if (s != null)
            {
                var prediction = classifier.MakePredictions(s);

            }
        }
        public ViewLoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            pLoginPage = new PresenterLoginPage(this);
            RegisterLink.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => { await Navigation.PushAsync(new ViewRegistrationPage()); })
            });
        }
    }
}