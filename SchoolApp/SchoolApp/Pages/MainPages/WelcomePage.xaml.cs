using SchoolApp.Pages.MainPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolApp.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage {
        public WelcomePage() {
            InitializeComponent();
            AnimateFadeIns();
        }

        async void AnimateFadeIns() {
            await Task.Delay(2000);
            await label_subTitle.FadeTo(1d, 500, Easing.CubicInOut);
            await Task.Delay(1000);
            await label_title.FadeTo(1d, 500, Easing.CubicInOut);
            await Task.Delay(500);
            await button.FadeTo(1d, 300, Easing.CubicInOut);
        }

        bool alreadyPressed = false;
        private async void Button_Clicked(object sender, EventArgs e) {
            if (alreadyPressed) {
                return;
            } else {
                alreadyPressed = true;
            }
            await button.FadeTo(0d, 300, Easing.CubicInOut);
            await label_title.FadeTo(0d, 200, Easing.CubicInOut);
            await label_subTitle.FadeTo(0d, 100, Easing.CubicInOut);
            if (App.initializeProgress == 0) {
                ClassSetupPage page = new ClassSetupPage(new ScheduleSetupPage(new Schedule("Default Schedule"), new SchedulePage(), DoneSettingUpSchedules), DoneSettingUpClasses);
                page.TransitionTo();
                await App.Current.MainPage.Navigation.PushModalAsync(page, false);
            } else {
                ScheduleSetupPage page = new ScheduleSetupPage(new Schedule("Default Schedule"), new SchedulePage(), DoneSettingUpSchedules);
                page.TransitionTo();
                await App.Current.MainPage.Navigation.PushModalAsync(page, false);
            }
        }
        private void DoneSettingUpClasses() {
            App.dataManager.SetAuxiliary(1);
        }
        private void DoneSettingUpSchedules() {
            App.dataManager.SetAuxiliary(2);
        }
    }
}