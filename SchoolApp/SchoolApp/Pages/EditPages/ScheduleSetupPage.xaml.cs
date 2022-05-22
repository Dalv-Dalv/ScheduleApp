using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolApp.Pages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleSetupPage : ContentPage, ITransitionablePage {
        int day = 0;
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        Schedule schedule;
        ITransitionablePage nextPage;
        Action onDone;

        public ScheduleSetupPage(Schedule schedule, ITransitionablePage to) {
            InitializeComponent();
            nextPage = to;
            this.schedule = schedule;
        }
        public ScheduleSetupPage(Schedule schedule, Action onDone) {
            InitializeComponent();
            nextPage = null;
            this.schedule = schedule;
            this.onDone += onDone;
        }
        public ScheduleSetupPage(Schedule schedule, ITransitionablePage to, Action onDone) {
            InitializeComponent();
            nextPage = to;
            this.schedule = schedule;
            this.onDone += onDone;
        }
        public ScheduleSetupPage(Schedule schedule, ITransitionablePage to, int startDay) {
            InitializeComponent();
            nextPage = to;
            this.schedule = schedule;
            day = startDay;
        }
        public ScheduleSetupPage(Schedule schedule, Action onDone, int startDay) {
            InitializeComponent();
            nextPage = null;
            this.schedule = schedule;
            this.onDone += onDone;
            day = startDay;
        }

        public void TransitionTo() {
            nameEntry.Text = schedule.name;
            label_day.Text = days[day];

            classPicker.Items.Clear();
            foreach (var cls in App.classesManager.classes) {
                classPicker.Items.Add(cls.name);
            }

            AnimateFadeIns();

            UpdateScrollViewContents();
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

        bool scrollviewWasEmptyBefore = true;
        private void UpdateScrollViewContents() {
            if (schedule.entriesByDay[day].Count == 0) {
                if (scrollviewWasEmptyBefore) return;

                scrollViewContents.Children.Clear();

                scrollviewWasEmptyBefore = true;

                scrollViewContents.Children.Add(new Label() {
                    Text = "No entries.",
                    FontSize = 15,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Color.FromHex("#4D4D4D")
                });
                return;
            } else {
                scrollviewWasEmptyBefore = false;
            }

            scrollViewContents.Children.Clear();


            foreach (var entry in schedule.entriesByDay[day]) {
                AddScrollViewEntry(entry);
            }
        }

        private void AddScrollViewEntry(ScheduleEntry entry) {
            UrgencyColors colors = UrgencyColors.urgencyColors[App.classesManager.classes[entry.classIndex].urgencyLevel];

            string time = entry.startTime == entry.endTime ? entry.startTime.ToString(@"hh\:mm") : $"{entry.startTime.ToString(@"hh\:mm")} - {entry.endTime.ToString(@"hh\:mm")}";

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
                Padding = new Thickness(0, 0, 1, 1),
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
                            new StackLayout() {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Children = {
                                    new Label() {
                                        Text = App.classesManager.classes[entry.classIndex].name,
                                        TextColor = Color.White,
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 20,
                                        HorizontalTextAlignment = TextAlignment.Center
                                    },
                                    new Label() {
                                        Text = time,
                                        TextColor = colors.textColor,
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 20,
                                        HorizontalTextAlignment = TextAlignment.Center
                                    }
                                }
                            },
                            button
                        }
                    }
                }
            };

            scrollViewContents.Children.Add(frame);
            button.Clicked += (s, e) => DeleteEntry(entry, frame);
        }

        private void DeleteEntry(ScheduleEntry entry, Frame frame) {
            schedule.RemoveEntry(day, entry);
            scrollViewContents.Children.Remove(frame);
        }

        private async void Button_Clicked_Done(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(nameEntry.Text)) {
                DisplayAlert("No name.", "Please input a name for your Schedule.", "Ok");
                return;
            }
            schedule.name = nameEntry.Text.Trim();

            App.dataManager.SaveSchedules();

            await AnimateFadeOuts();

            if (nextPage == null) {
                App.Current.MainPage.Navigation.PopModalAsync(false);
            } else {
                nextPage.TransitionTo();
                App.Current.MainPage.Navigation.PushModalAsync(nextPage.GetPage(), false);
            }
            onDone?.Invoke();
        }

        private void Button_Clicked_PrevDay(object sender, EventArgs e) {
            if (day == 0) {
                day = 6;
            } else {
                day--;
            }
            label_day.Text = days[day];
            UpdateScrollViewContents();
        }

        private void Button_Clicked_NextDay(object sender, EventArgs e) {
            day = (day + 1) % 7;
            label_day.Text = days[day];
            UpdateScrollViewContents();
        }

        private async void Button_Clicked_Add(object sender, EventArgs e) {
            if (classPicker.SelectedIndex == -1) {
                DisplayAlert("No Class selected.", "Please select a class to add.", "Ok");
                return;
            }
            if (tp_start.Time == tp_end.Time) {
                if (!(await DisplayAlert("Are you sure?", "Start and end Times are the same.", "Yes", "Cancel"))) return;
            }
            if (tp_start.Time > tp_end.Time) {
                schedule.AddEntry(day, new ScheduleEntry(classPicker.SelectedIndex, tp_end.Time, tp_start.Time));
            } else {
                schedule.AddEntry(day, new ScheduleEntry(classPicker.SelectedIndex, tp_start.Time, tp_end.Time));
            }
            UpdateScrollViewContents();
            var lastChild = scrollViewContents.Children.LastOrDefault();
            if (lastChild != null) {
                scrollView.ScrollToAsync(lastChild, ScrollToPosition.End, true);
            }
        }

        private async void Button_Clicked_Clear(object sender, EventArgs e) {
            if (schedule.entriesByDay[day].Count == 0) return;
            if (await DisplayAlert("Are you sure?", "Clearing will delete all entries, but only for the current day.", "Yes", "Cancel")) {
                schedule.entriesByDay[day].Clear();
                UpdateScrollViewContents();
            }
        }

        protected override bool OnBackButtonPressed() {
            if (App.initializeProgress == 2) {
                schedule.name = nameEntry.Text.Trim();

                App.dataManager.SaveSchedules();

                BackPressed();
                return base.OnBackButtonPressed();
            }
            return true;
        }

        async void BackPressed() {
            await AnimateFadeOuts();
            App.Current.MainPage.Navigation.PopModalAsync(false);
            onDone?.Invoke();
        }
    }
}