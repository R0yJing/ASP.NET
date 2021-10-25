
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
        public ViewLoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            pLoginPage = new PresenterLoginPage(this);
            RegisterLink.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => { await Navigation.PushAsync(new ViewRegistrationPage()); })
            });
#if DEBUG
            DisplayAlert("Debug mode", "", "OK");
#endif
        }

        private bool CheckEntries()
        {
            if (UsernameEntry.Text == null || PasswordEntry.Text == null)
                return false;
            if (UsernameEntry.Text.Trim() == "" || PasswordEntry.Text.Trim() == "")
            {
                return false;
            }
            return true;
        }
        public ICommand OnTapLogin
        {
            get
            {
                return new Command(async () =>
                {
                    Console.WriteLine("logging in");
                    
                    bool loggedIn = false;

                    if (CheckEntries())
                    {
                        LoggingInIndicator.IsVisible = true;
                        mainScreen.IsVisible = false;
                        loggedIn = await UserService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
                    }
                    string preferredClassifier = "local";//await DatabaseService.GetUserPreferredClassifier();
                    
                    if (loggedIn)
                    {
                        UsernameEntry.Text = "";
                        PasswordEntry.Text = "";
                        if (preferredClassifier == "local")

                            ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
                        else ClassifierServiceFactory.SetClassifier(new WebClassifierService());

                        mainScreen.IsVisible = true;
                        LoggingInIndicator.IsVisible = false;
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
            get => new Command<string>(CalculateROC);
            set { }
        }

        public ICommand Cmd_TestSpeedRemote
        {
            get => new Command(() => CalculatePredictionSpeed("Remote"));
            set { }

        }
        public ICommand Cmd_TestSpeedLocal
        {
            get => new Command(() => CalculatePredictionSpeed("Local"));
            set { }

        }

        public static string[] GetFiles()
        {
            var allData = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            //var sortedByLabel = allData.Select(str => Assembly.GetExecutingAssembly().GetManifestResourceStream(str));
            return allData;
          
        }

        private void AddToConfusionMatrix(double limit, List<int> confusionMatrixAboveThreshold, List<int> confusionMatrixBelowThreshold )
        {
            //add one to the confusion matrices whose probability threshold the current prediction probability (limit)
            //has exceeded

            for (int i = 0; i <= (int)(limit * 10); i += 1)
            {
                
                confusionMatrixAboveThreshold[i]++;
               
            }
            for (int i = (int)(limit * 10) + 1; i <= 9; i++)
            {
                confusionMatrixBelowThreshold[i]++;
            }
        }

#if DEBUG
        public async void CalculatePredictionSpeed(string mode)
        {
            IClassifierService cls;

            if (mode == "Local")
                cls = DependencyService.Get<ILocalClassifierService>();
            else cls = new WebClassifierService();

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            string[] benigns = names.Where(name => name.ToLower().Contains("benign")).ToArray();
            var maligns = names.Where(name => name.ToLower().Contains("malign")).ToArray();
            var unknown = names.Where(name => name.ToLower().Contains("unknown")).ToArray();

            Stopwatch timer = new Stopwatch();
            var files = new string[9];
            long timeMillis = 0;
            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(benigns[i]))
                {
                    timer.Start();
                    await cls.MakePredictions(reader);

                    timer.Stop();
                    timeMillis += timer.ElapsedMilliseconds;

                    Debug.WriteLine("" + (double)timer.ElapsedMilliseconds / 1000);
                    timer.Reset();
                }

            }
            Debug.WriteLine("--------");

            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(maligns[i]))
                {
                    timer.Start();
                    await cls.MakePredictions(reader);

                    timer.Stop();

                    timeMillis += timer.ElapsedMilliseconds;

                    Debug.WriteLine("" + (double)timer.ElapsedMilliseconds / 1000);
                    timer.Reset();
                }

            }
            Debug.WriteLine("--------");
            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(unknown[i]))
                {
                    timer.Start();
                    await cls.MakePredictions(reader);

                    timer.Stop();

                    timeMillis += timer.ElapsedMilliseconds;

                    Debug.WriteLine("" + ((double)timer.ElapsedMilliseconds / 1000));
                    timer.Reset();
                }
            }
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!");
            Debug.WriteLine("average = " + ((double)timeMillis / 1000 / 27));
        }

        public async void CalculateROC(string tag="benign")
            {

            //benign
            //malign
            //unknown
            const int NUM_MALIGNS = 100;
            int NUM_ALLFILES;
            var classifier = DependencyService.Get<ILocalClassifierService>();


                var allFiles = GetFiles();
                NUM_ALLFILES = allFiles.Length;
               
                int count_exceptions_malign = 0;
                int count_exceptions_benign = 0;
                int count_exceptions_unknown = 0;
                List<int> TP_Maligns = new List<int>(11), FN_Maligns = new List<int>(11), FP_Maligns = new List<int>(11), TN_Maligns = new List<int>(11);
                foreach (var list in new List<int>[] { TP_Maligns, FN_Maligns, FP_Maligns, TN_Maligns })
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        list.Add(0);
                    }
                }

            int gcCounter = 0;
            for (int i=0; i < NUM_MALIGNS; i++)
            {
                string fileName = allFiles[i];
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    var wrapper = await ClassifierServiceWrapper.MakePredictions(stream);
                    var predictionByTag = wrapper.Where(pred => pred.TagName.ToLower().Contains(tag)).ElementAt(0);
                    var prob = predictionByTag.Probability;
                    AddToConfusionMatrix(prob, TP_Maligns, FN_Maligns);
                }
                //GC.Collect();
              

            }
            gcCounter = 0;
            for (int i = NUM_MALIGNS; i < NUM_ALLFILES; i++)
            {
                string fileName = allFiles[i];
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    var wrapper = await classifier.MakePredictions(stream);
                    var predictionByTag = wrapper.Where(pred => pred.TagName.ToLower().Contains(tag)).ElementAt(0);
                    var prob = predictionByTag.Probability;
                    AddToConfusionMatrix(prob, FP_Maligns, TN_Maligns);
                }
                //GC.Collect();

            }

            for (int i = 0; i <= 9; i++)
            {
                    Debug.WriteLine(tag + " ROC");
                    Debug.WriteLine("number of exceptions = " + (count_exceptions_benign + count_exceptions_malign + count_exceptions_unknown));
                    Debug.WriteLine("malign exceptions malign = " + count_exceptions_malign);
                    Debug.WriteLine("malign exceptions benign= " + count_exceptions_malign);
                    Debug.WriteLine("malign exceptions unknown= " + count_exceptions_malign);
                    Debug.WriteLine("Threshold = " + (((double)i) / 9) + " \nTPR = " + ((double)TP_Maligns[i] / (TP_Maligns[i] + FN_Maligns[i])));
                    Debug.WriteLine("FPR = " + ((double)FP_Maligns[i] / (FP_Maligns[i] + TN_Maligns[i])));
                    Debug.WriteLine("");    
            }


            

        }
        
    }
#endif
}