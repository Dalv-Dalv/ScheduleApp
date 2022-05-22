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
    public partial class ClassesPage : ContentPage, ITransitionablePage {
        Action onBack;
        bool reminderCreatePopupOpen;
        public ClassesPage(Action onBack = null) {
            InitializeComponent();
            this.onBack += onBack;
        }

        public Page GetPage() {
            return this;
        }

        public void TransitionTo() {
            AnimateFadeIns();

            reminderColorPicker.Items.Clear();
            for (int i = 1; i < UrgencyColors.urgencyColors.Length; i++) {
                reminderColorPicker.Items.Add(UrgencyColors.urgencyColors[i].name);
            }
            reminderColorPicker.SelectedIndexChanged += Reminder_colorPicker_SelectedIndexChanged;

            UpdateHomeworkAddOptions();

            UpdateScrollViewContents();
        }

        private void UpdateHomeworkAddOptions() {
            homeworkClassPicker.Items.Clear();
            foreach (var sclass in App.classesManager.classes) {
                homeworkClassPicker.Items.Add(sclass.name);
            }
        }

        private void Reminder_colorPicker_SelectedIndexChanged(object sender, EventArgs e) {
            UrgencyColors colors = UrgencyColors.urgencyColors[reminderColorPicker.SelectedIndex + 1];
            reminderColorPicker_backgroundFrame.BackgroundColor = colors.backgroundColor;
            reminderColorPicker_borderFrame.BackgroundColor = colors.borderColor;
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

        private void UpdateScrollViewContents() {
            scrollViewContents.Children.Clear();
            for (int i = 0; i < App.classesManager.classes.Count; i++) {
                UrgencyColors colors = UrgencyColors.urgencyColors[App.classesManager.classes[i].urgencyLevel];

                Button btn = new Button() {
                    CornerRadius = 20,
                    BorderWidth = 3,
                    BorderColor = colors.borderColor,
                    BackgroundColor = colors.backgroundColor,
                    Text = App.classesManager.classes[i].name,
                    TextTransform = TextTransform.None,
                    TextColor = Color.White,
                    FontSize = 20,
                    Padding = new Thickness(0, 21),
                    Margin = new Thickness(10)
                };
                int crnIndex = i;
                btn.Clicked += (sender, e) => ClassClicked(sender, e, crnIndex);
                scrollViewContents.Children.Add(btn);
            }
        }

        bool classDetailOpen = false;
        private async void ClassClicked(object sender, EventArgs e, int index) {
            if (classDetailOpen) return;
            classDetailOpen = true;

            await AnimateFadeOuts();
            root_stackLayout.IsVisible = false;

            ClassDetailPage page = new ClassDetailPage(index, ClassDetailClosed);
            page.TransitionTo();
            App.Current.MainPage.Navigation.PushModalAsync(page, true);
        }
        void ClassDetailClosed() {
            root_stackLayout.IsVisible = true;
            classDetailOpen = false;
            AnimateFadeIns();
            UpdateScrollViewContents();
        }

        private void Button_Clicked_AddHomework(object sender, EventArgs e) {
            OpenAddHomeworkPanel();
        }
        private async void OpenAddHomeworkPanel() {
            if (reminderCreatePopupOpen) return;
            reminderCreatePopupOpen = true;
            addHomeworkPopupFrame.IsVisible = true;
            await Task.Delay(10);
            reminderColorPicker.SelectedIndex = 0;
            reminderTitleEntry.Text = "Homework";
            reminderDescriptionEditor.Text = "";
        }
        private void CloseAddHomeworkPanel() {
            if (!reminderCreatePopupOpen) return;

            reminderCreatePopupOpen = false;
            addHomeworkPopupFrame.IsVisible = false;
        }
        private void Button_Clicked_ReminderCreate_Back(object sender, EventArgs e) {
            CloseAddHomeworkPanel();
        }
        private void Button_Clicked_ReminderCreate_Done(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(reminderTitleEntry.Text)) {
                DisplayAlert("No Reminder title.", "Please enter a title for your Reminder.", "Ok");
                return;
            }
            if (homeworkClassPicker.SelectedIndex == -1) {
                DisplayAlert("No Class selected.", "Please select a Class to add your Homework to.", "Ok");
                return;
            }

            Reminder reminderToAdd = new Reminder(reminderTitleEntry.Text, reminderDescriptionEditor.Text, reminderColorPicker.SelectedIndex + 1);
            App.classesManager.classes[homeworkClassPicker.SelectedIndex].AddReminder(reminderToAdd);

            UpdateScrollViewContents();

            App.dataManager.SaveClasses();

            CloseAddHomeworkPanel();
        }

        private async void Button_Clicked_Edit(object sender, EventArgs e) {
            if (classDetailOpen) return;

            await AnimateFadeOuts();

            ClassSetupPage page = new ClassSetupPage(DoneEditing);
            page.TransitionTo();
            App.Current.MainPage.Navigation.PushModalAsync(page, false);
        }
        private void DoneEditing() {
            AnimateFadeIns();
            UpdateHomeworkAddOptions();
            UpdateScrollViewContents();
        }

        private void Button_Clicked_SchedulesTab(object sender, EventArgs e) {
            if (classDetailOpen) return;

            onBack?.Invoke();
            App.Current.MainPage.Navigation.PopModalAsync(false);
        }

        private void Button_Clicked_PopPanel(object sender, EventArgs e) {
            editBar.IsVisible = !editBar.IsVisible;
            if (editBar.IsVisible) {
                expandButton.Text = "\\/";
            } else {
                expandButton.Text = "/\\";
            }
        }

        protected override bool OnBackButtonPressed() {
            if (reminderCreatePopupOpen) {
                CloseAddHomeworkPanel();
            }
            return true;
        }
    }
}