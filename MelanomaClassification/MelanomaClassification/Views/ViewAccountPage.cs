using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MelanomaClassification.Views
{
    public class ViewAccountPage : ContentPage, PresenterAccountPage.IViewAccountPage
    {
        private class Option
        {
            public string Text { get; set; }
        }
        private PresenterAccountPage pAccountPage;

        private Button DeleteAcctBtn = new Button { Text = "Delete this account" };
        private Button ChangePswdBtn = new Button { Text = "Change passsword" };
        private Button LogOffBtn = new Button { Text = "Log off" };

        private ObservableCollection<Option> options = new ObservableCollection<Option>
        {
            new Option { Text = "Update personal details"},
            new Option {Text = "Update password"},
            new Option {Text = "Sign off"},
            new Option {Text = "Delete account"}
        };

        private DataTemplate dataTemp = new DataTemplate(() =>
        {
           
            Label optionLabel = new Label();
            optionLabel.SetBinding(Label.TextProperty, "Text");
            ViewCell viewCell = new ViewCell
            {
                View = optionLabel
                
            };
            return viewCell;
        });
        ListView listView = new ListView();
        // ObservableCollection<SourceItem> source = new ObservableCollection<SourceStream>();
        private Switch API_Switch = new Switch { IsToggled = true };
        private Switch UseRemote_Switch = new Switch { IsToggled = true };

        public ViewAccountPage()
        {
            pAccountPage = new PresenterAccountPage(this);

            listView.ItemSelected += ListView_ItemSelected;
            listView.ItemsSource = options;
            listView.ItemTemplate = dataTemp;
            API_Switch.Toggled += API_Switch_Toggled;
            UseRemote_Switch.Toggled += EnableStoringInRemote;
            Content = new StackLayout
            {

                Children = {
                    listView,
                    ChangePswdBtn,
                    DeleteAcctBtn,
                    API_Switch

                }
            };
        }



        private void EnableStoringInRemote(object sender, ToggledEventArgs e)
        {
            pAccountPage.SetEnableStoringRemote(e.Value);

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Option selection = e.SelectedItem as Option;
            switch (selection.Text)
            {
                case ("Update personal details"):
                    Console.WriteLine("updating details");
                    pAccountPage.ChangeDetails();
                    break;
                case ("Update password"):
                    pAccountPage.UpdatePassword();
                    Console.WriteLine("updating pass");
                    break;
                case ("Sign off"):
                    Console.WriteLine("soff");
                    pAccountPage.LogOff();
                    
                    break;
                case ("Delete account"):
                    Console.WriteLine("del acc");
                    pAccountPage.DelAcct();
                    break;

            }
        }
        
        public async void LogoutApp()
        {
            await Navigation.PopToRootAsync();
            //pAccountPage.StoreData();
            
        }
        private void API_Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {

                ClassifierServiceFactory.SetClassifier(new WebClassifierService());
            }
            else ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
        }
    }

    
    
}