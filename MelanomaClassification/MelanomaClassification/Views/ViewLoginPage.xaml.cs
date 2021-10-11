using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using Plugin.Media;
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
        private static List<string> FilePaths;

        public ICommand TestMakePredictionsRemote = new Command(() =>
        {
            WebClassifierService webServ = new WebClassifierService();

        });
        private static long totTime = 0;
        public ICommand TestLocal {
            get => new Command( () => { TestMakePredictions("local"); });
            set { }
        }
        public ICommand TestRemote
        {
            get => new Command( () => { TestMakePredictions("remote"); });
            set { }
        }
                
        public static void filePaths(){
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var MyAssemblyName = assembly.GetName().Name; //now this is for the future
                                                          // MyAssemblyName + ".Music."+filename
                                                          //anyway now we are building the whole list:
            var Files = new List<string>(); //todo change to your music class 
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if (res.Contains(".jpg") || res.Contains(".png"))
                {
                    Files.Add(res);
                }
            }
            FilePaths = Files; ;
        }
        public async static void TestMakePredictions(string mode, int size=50)
        {
            filePaths();
            IClassifierService classifier;
            if (mode == "local")
                classifier = DependencyService.Get<ILocalClassifierService>();
            else classifier = new WebClassifierService();

            await TestClassifier(classifier, "benign", (int)size);
            await TestClassifier(classifier, "malignant", (int)size);
            await TestClassifier(classifier, "unknown", (int)size);
            Debug.WriteLine($"Avaerage time elapsed {totTime / ((int)size) / 3000}, for a sample size of {(int)size * 3} in total "
                + $"\nTotal time elapased in seconds {totTime / 1000}");
            totTime = 0;

        
        }
      
        public ICommand OnTapLogin
        {
            get
            {
                return new Command(async () =>
                {
                    Console.WriteLine("logging in");
                    LoggingInIndicator.IsVisible = true;
                    //bool loggedIn = await UserService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);

                    LoggingInIndicator.IsVisible = false;
                    if (true)
                    {
                        Shell.Current.FindByName<ShellContent>("OnLogin").IsVisible = false;
                        if (Shell.Current.FindByName<TabBar>("AfterLogin") == null)
                        {
                            await DisplayAlert("Fatal Error", "After Login not found", "OK");
                        }
                        await Navigation.PushAsync(new ViewAccountPage());
                        Shell.Current.FindByName<TabBar>("AfterLogin").IsVisible = true;
                        pLoginPage.SetUsername(UsernameEntry.Text);
                        await DatabaseService.Init();
                        DatabaseService.LoadData();
                        ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Unsuccessful log in attempt, please try again", "OK");

                    }
                });
            }

        }
        private static Stopwatch stopwatch = new Stopwatch();

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

        private static async Task<bool> TestClassifier(IClassifierService classifier, string mode, int size = 50)
        {
            string appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.WriteLine("App folder = " + appFolder);
            //expected benign
            string folder = "";
            switch (mode)
            {
                case "benign":
                    folder = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\benign\ISIC-images\ISIC-images\BCN_2020_Challenge";
                    //folder = Path.Combine(appFolder, "Assets/Benign");
                    break;
                case "malign":
                    folder = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\malign\ISIC-images\BCN_2020_Challenge";
                   // folder = Path.Combine(appFolder, "Assets/Malign");
                    break;
                case "unknown":
                    folder = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\unknown\UNKNOWN\Brisbane ISIC Challenge 2020";
                    //folder = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\unknown\UNKNOWN\Brisbane ISIC Challenge 2020";
                    break;
            }
            List<string> imagePath = FilePaths;


            List<Stream> streams = new List<Stream>();
            var localClassifier = DependencyService.Get<ILocalClassifierService>();

            for (int i = 0; i < size; i++)
            {
                Stream tempStream = new StreamReader(imagePath[i]).BaseStream;
                stopwatch.Start();
                var predictions = await localClassifier.MakePredictions(tempStream);
                stopwatch.Stop();
                totTime += stopwatch.ElapsedMilliseconds;

                predictions.ForEach(predict =>
                {
                    Debug.WriteLine(predict.TagName);
                    Debug.WriteLine(predict.Probability.ToString("P"));
                });
                Debug.WriteLine("End of " + i);

            }
            return true;

        }

       
    }
}