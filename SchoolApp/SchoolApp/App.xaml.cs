using Android.Content;
using SchoolApp.Pages;
using SchoolApp.Pages.EditPages;
using SchoolApp.Pages.MainPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace SchoolApp {
    public partial class App : Application {
        public static DataManager dataManager;
        public static ClassesManager classesManager;
        public static ScheduleManager scheduleManager;
        public static short initializeProgress;

        public App() {
            InitializeComponent();

            dataManager = new DataManager();

            classesManager = new ClassesManager();
            dataManager.LoadClasses();

            scheduleManager = new ScheduleManager();
            dataManager.LoadSchedules();

            initializeProgress = dataManager.LoadAuxiliary();

            //If user has set up everything
            if (initializeProgress == 2) {
                SchedulePage page = new SchedulePage();
                page.TransitionTo();
                MainPage = page;
            } else {
                MainPage = new WelcomePage();
            }

            //ClassSetupPage page = new ClassSetupPage();
            //page.TransitionTo();
            //MainPage = page;
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}