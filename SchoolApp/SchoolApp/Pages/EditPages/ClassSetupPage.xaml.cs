using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolApp.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassSetupPage : ContentPage, ITransitionablePage {
        ITransitionablePage nextPage;
        Action onDone;
        public ClassSetupPage(ITransitionablePage to) {
            InitializeComponent();
            nextPage = to;
        }
        public ClassSetupPage(Action onDone = null) {
            InitializeComponent();
            nextPage = null;
            this.onDone += onDone;
        }
        public ClassSetupPage(ITransitionablePage to, Action onDone) {
            InitializeComponent();
            nextPage = to;
            this.onDone += onDone;
        }

        public void TransitionTo() {
            AnimateFadeIns();
            UpdateScrollViewContents();
        }

        private void UpdateScrollViewContents() {
            foreach (var item in App.classesManager.classes) {
                AddClass(item);
            }
        }

        public Page GetPage() {
            return this;
        }

        async Task AnimateFadeIns() {
            foreach (var item in root_stackLayout.Children) {
                await item.FadeTo(1d, 200, Easing.CubicInOut);
            }
        }
        async Task AnimateFadeOuts() {
            foreach (var item in root_stackLayout.Children) {
                await item.FadeTo(0, 200, Easing.CubicInOut);
            }
        }

        private async void Button_Clicked_Done(object sender, EventArgs e) {
            if (App.classesManager.classes.Count == 0) {
                await DisplayAlert("No classes.", "Please add at least one class.", "Ok");
                return;
            }
            //App.classesManager.InstantiateClasses(classes);
            App.dataManager.SaveClasses();
            await AnimateFadeOuts();

            if (nextPage == null) {
                App.Current.MainPage.Navigation.PopModalAsync(false);
            } else {
                nextPage.TransitionTo();
                App.Current.MainPage.Navigation.PushModalAsync(nextPage.GetPage(), false);
            }
            onDone?.Invoke();
        }
        private void Button_Clicked_Add(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(entry.Text)) {
                DisplayAlert("No Class name.", "Please enter a name for your Class.", "Ok");
                return;
            }
            foreach (var item in App.classesManager.classes) {
                if (entry.Text.Equals(item.name)) return;
            }

            Class sclass = new Class(entry.Text.Trim());
            App.classesManager.classes.Add(sclass);
            AddClass(sclass);

            //Scroll to bottom
            var lastChild = scrollViewContents.Children.LastOrDefault();
            if (lastChild != null) {
                scrollView.ScrollToAsync(lastChild, ScrollToPosition.End, true);
            }

            entry.Text = "";
        }
        private void Button_Clicked_Undo(object sender, EventArgs e) {
            if (App.classesManager.classes.Count == 0) return;
            RemoveLastClass();
        }
        private async void Button_Clicked_Clear(object sender, EventArgs e) {
            if (App.classesManager.classes.Count == 0) return;
            if (await DisplayAlert("Are you sure?", "Clearing will delete all classes.", "Yes", "Cancel")) {
                RemoveAllClasses();
            }
        }

        void AddClass(Class sclass) {
            if (string.IsNullOrWhiteSpace(sclass.name)) return;

            UrgencyColors colors = UrgencyColors.urgencyColors[sclass.urgencyLevel];

            Button button = new Button() {
                Text = "X",
                TextTransform = TextTransform.None,
                CornerRadius = 100,
                HorizontalOptions = LayoutOptions.End,
                Margin = 0,
                BackgroundColor = Color.FromHex("#1a1a1a"),
                BorderWidth = 3,
                BorderColor = colors.borderColor,
                TextColor = colors.textColor,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                WidthRequest = 45,
                HeightRequest = 45
            };

            Frame frame = new Frame() {
                BackgroundColor = colors.borderColor,
                CornerRadius = 20,
                Margin = 10,
                Padding = new Thickness(0, 0, 1, 0),
                VerticalOptions = LayoutOptions.Start,
                HasShadow = false,
                Content = new Frame() {
                    BackgroundColor = colors.backgroundColor,
                    CornerRadius = 17,
                    Margin = 3,
                    HasShadow = false,
                    Content = new StackLayout() {
                        Orientation = StackOrientation.Horizontal,
                        Padding = -8,
                        Children = {
                            new Label() {
                               Text = sclass.name,
                               TextColor = Color.White,
                               FontSize = 20,
                               HorizontalTextAlignment = TextAlignment.Center,
                               HorizontalOptions = LayoutOptions.FillAndExpand,
                               VerticalOptions = LayoutOptions.Center
                            },
                            button
                        }
                    }
                }
            };
            button.Clicked += (s, e) => DeleteButtonClicked(sclass, frame);
            scrollViewContents.Children.Add(frame);
        }

        private async void DeleteButtonClicked(Class sclass, Frame classFrame) {
            if (sclass.reminders.Count > 0) {
                if (!await DisplayAlert("Are you sure?", "Deleting this Class will delete all of it's Reminders.", "Yes", "Cancel")) {
                    return;
                }
            }
            App.classesManager.DeleteClass(sclass);
            scrollViewContents.Children.Remove(classFrame);
        }

        void RemoveLastClass() {
            App.classesManager.DeleteClass(App.classesManager.classes.Last());
            scrollViewContents.Children.Remove(scrollViewContents.Children.Last());
        }
        void RemoveAllClasses() {
            App.classesManager.DeleteAllClasses();
            scrollViewContents.Children.Clear();
        }

        protected override bool OnBackButtonPressed() {
            if (App.initializeProgress == 2) {
                //App.classesManager.InstantiateClasses(classes);
                App.dataManager.SaveClasses();

                BackPressed();
                return base.OnBackButtonPressed();
            }
            return true;
        }

        async void BackPressed() {
            await AnimateFadeOuts();
            App.Current.MainPage.Navigation.PopModalAsync(false);
            App.dataManager.SaveClasses();
            onDone?.Invoke();
        }
    }
}