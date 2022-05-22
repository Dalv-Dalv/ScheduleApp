using SchoolApp.Pages.EditPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolApp.Pages.MainPages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchedulePage : ContentPage, ITransitionablePage {
        int day = 0;
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        Schedule currentSchedule;
        ClassesPage classesPage;
        bool classDetailOpen;

        public SchedulePage() {
            InitializeComponent();
        }

        public Page GetPage() {
            return this;
        }

        public void TransitionTo() {
            AnimateFadeIns();

            schedulePicker.Items.Clear();

            foreach (var item in App.scheduleManager.schedules) {
                schedulePicker.Items.Add(item.name);
            }

            schedulePicker.SelectedIndexChanged += SchedulePicker_SelectedIndexChanged;

            schedulePicker.SelectedIndex = 0;

            day = /*today*/ ((int)DateTime.Today.DayOfWeek + 6) % 7;
            label_day.Text = days[day];

            try {
                UpdateScrollViewContents();

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private void SchedulePicker_SelectedIndexChanged(object sender, EventArgs e) {
            currentSchedule = App.scheduleManager.schedules[schedulePicker.SelectedIndex];
            UpdateScrollViewContents();
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
            if (currentSchedule.entriesByDay[day].Count == 0) {
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


            foreach (var entry in currentSchedule.entriesByDay[day]) {
                AddScrollViewEntry(entry);
            }
        }
        private void AddScrollViewEntry(ScheduleEntry entry) {
            UrgencyColors colors = UrgencyColors.urgencyColors[App.classesManager.classes[entry.classIndex].urgencyLevel];

            string time = entry.startTime == entry.endTime ? entry.startTime.ToString(@"hh\:mm") : $"{entry.startTime.ToString(@"hh\:mm")} - {entry.endTime.ToString(@"hh\:mm")}";

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
                    }
                }
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => EntryClicked(entry.classIndex);
            frame.GestureRecognizers.Add(tapGestureRecognizer);

            scrollViewContents.Children.Add(frame);
        }

        private async void EntryClicked(int classIndex) {
            if (classDetailOpen) return;
            classDetailOpen = true;

            await AnimateFadeOuts();
            root_stackLayout.IsVisible = false;

            ClassDetailPage page = new ClassDetailPage(classIndex, ClassDetailClosed);
            page.TransitionTo();
            App.Current.MainPage.Navigation.PushModalAsync(page, true);
        }

        private void ClassDetailClosed() {
            root_stackLayout.IsVisible = true;
            classDetailOpen = false;
            AnimateFadeIns();
            UpdateScrollViewContents();
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

        private async void Button_Clicked_New(object sender, EventArgs e) {
            currentSchedule = new Schedule("New Schedule");

            await AnimateFadeOuts();

            ScheduleSetupPage page = new ScheduleSetupPage(currentSchedule, DoneAddingNew);
            page.TransitionTo();
            App.Current.MainPage.Navigation.PushModalAsync(page, false);
        }

        private void DoneAddingNew() {
            AnimateFadeIns();
            UpdateScrollViewContents();
            schedulePicker.Items.Add(App.scheduleManager.schedules.Last().name);
            schedulePicker.SelectedIndex = App.scheduleManager.schedules.Count - 1;
        }

        private async void Button_Clicked_Edit(object sender, EventArgs e) {
            await AnimateFadeOuts();

            ScheduleSetupPage page = new ScheduleSetupPage(currentSchedule, DoneEditing, day);
            page.TransitionTo();
            App.Current.MainPage.Navigation.PushModalAsync(page, false);
        }
        private void DoneEditing() {
            AnimateFadeIns();
            schedulePicker.Items[schedulePicker.SelectedIndex] = currentSchedule.name;
            UpdateScrollViewContents();
        }

        private async void Button_Clicked_Remove(object sender, EventArgs e) {
            if (App.scheduleManager.schedules.Count == 0) {
                DisplayAlert("Cannot remove.", "Cannot remove Schedule if there are no other ones.", "Ok");
                return;
            }

            if (await DisplayAlert("Are you sure?", "Removing will delete this schedule permanently.", "Yes", "Cancel")) {
                App.scheduleManager.schedules.Remove(currentSchedule);
                int prevIndex = schedulePicker.SelectedIndex;
                schedulePicker.Items.Clear();
                foreach (var item in App.scheduleManager.schedules) {
                    schedulePicker.Items.Add(item.name);
                }

                if (prevIndex - 1 < 0) {
                    schedulePicker.SelectedIndex = 0;
                } else {
                    schedulePicker.SelectedIndex = prevIndex - 1;
                }
            }
        }

        private void Button_Clicked_PopPanel(object sender, EventArgs e) {
            editBar.IsVisible = !editBar.IsVisible;
            if (editBar.IsVisible) {
                expandButton.Text = "\\/";
            } else {
                expandButton.Text = "/\\";
            }
        }

        private void Button_Clicked_ClassesTab(object sender, EventArgs e) {
            if (classesPage == null) {
                classesPage = new ClassesPage(BackFromClassesPage);
                classesPage.TransitionTo();
            }
            App.Current.MainPage.Navigation.PushModalAsync(classesPage, false);
        }
        private void BackFromClassesPage() {
            UpdateScrollViewContents();
        }

        protected override bool OnBackButtonPressed() {
            return true;
        }
    }
}