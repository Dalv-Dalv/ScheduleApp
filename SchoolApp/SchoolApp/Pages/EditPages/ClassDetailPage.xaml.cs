using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolApp.Pages.EditPages {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassDetailPage : ContentPage, ITransitionablePage {
        //int classIndex;
        Class classEditing = null;
        bool onRemindersTab = true;
        bool reminderCreatePopupOpen = false;
        Action onBack;
        List<Reminder> remindersOnDisplay = new List<Reminder>();
        public ClassDetailPage(int classIndex, Action onBack) {
            InitializeComponent();
            classEditing = App.classesManager.classes[classIndex];
            this.onBack += onBack;
        }

        private void Reminder_colorPicker_SelectedIndexChanged(object sender, EventArgs e) {
            UrgencyColors colors = UrgencyColors.urgencyColors[reminderColorPicker.SelectedIndex + 1];
            reminderColorPicker_backgroundFrame.BackgroundColor = colors.backgroundColor;
            reminderColorPicker_borderFrame.BackgroundColor = colors.borderColor;
        }

        private void UpdateClassCover() {
            int urgencyLevel = classEditing.urgencyLevel;
            classFrame_Background.BackgroundColor = UrgencyColors.urgencyColors[urgencyLevel].backgroundColor;
            classFrame_Border.BackgroundColor = UrgencyColors.urgencyColors[urgencyLevel].borderColor;
            classCloseButton.TextColor = UrgencyColors.urgencyColors[urgencyLevel].textColor;
            classCloseButton.BorderColor = UrgencyColors.urgencyColors[urgencyLevel].borderColor;
        }


        #region ITransitionable
        public void TransitionTo() {
            reminderColorPicker.Items.Clear();
            for (int i = 1; i < UrgencyColors.urgencyColors.Length; i++) {
                reminderColorPicker.Items.Add(UrgencyColors.urgencyColors[i].name);
            }
            reminderColorPicker.SelectedIndexChanged += Reminder_colorPicker_SelectedIndexChanged;
            className.Text = classEditing.name;

            classNoteEditor.Completed += ClassNoteEditor_Completed;
            classNoteEditor.Text = classEditing.notes;

            UpdateScrollViewContents();

            UpdateClassCover();
        }

        private void ClassNoteEditor_Completed(object sender, EventArgs e) {
            classEditing.SetNotes(classNoteEditor.Text);
            App.dataManager.SaveClasses();
            Console.WriteLine();
        }

        public Page GetPage() {
            return this;
        }
        #endregion

        #region NormalButtonEvents
        private void Button_Clicked_Back(object sender, EventArgs e) {
            ProcessBack();
        }
        protected override bool OnBackButtonPressed() {
            if (reminderCreatePopupOpen) {
                CloseReminderCreatePopUp();
                return true;
            }
            ProcessBack();
            return true;
        }
        void ProcessBack() {
            App.Current.MainPage.Navigation.PopModalAsync(true);
            App.dataManager.SaveClasses();
            onBack?.Invoke();
        }

        private void Button_Clicked_RemindersTab(object sender, EventArgs e) {
            if (onRemindersTab) return;

            Color col = reminderButton.BorderColor;
            reminderButton.BorderColor = notesButton.BorderColor;
            notesButton.BorderColor = col;

            createReminderButton.IsVisible = true;
            classNoteEditorFrame.IsVisible = false;

            onRemindersTab = true;

            scrollViewContents.Children.Clear();
            UpdateScrollViewContents();
        }
        private void Button_Clicked_NotesTab(object sender, EventArgs e) {
            if (!onRemindersTab) return;

            Color col = notesButton.BorderColor;
            notesButton.BorderColor = reminderButton.BorderColor;
            reminderButton.BorderColor = col;

            createReminderButton.IsVisible = false;
            classNoteEditorFrame.IsVisible = true;

            onRemindersTab = false;

            scrollViewContents.Children.Clear();
        }
        private void Button_Clicked_Create(object sender, EventArgs e) {
            OpenReminderCreatePopUp();
        }
        #endregion

        #region CreateReminderPopUp
        private async void OpenReminderCreatePopUp() {
            if (reminderCreatePopupOpen) return;
            reminderCreatePopupOpen = true;
            popupFrame.IsVisible = true;
            await Task.Delay(10);
            reminderColorPicker.SelectedIndex = 0;
            reminderTitleEntry.Text = "";
            reminderDescriptionEditor.Text = "";
        }
        private void CloseReminderCreatePopUp() {
            if (!reminderCreatePopupOpen) return;

            reminderCreatePopupOpen = false;
            popupFrame.IsVisible = false;
        }

        private void Button_Clicked_ReminderCreate_Back(object sender, EventArgs e) {
            CloseReminderCreatePopUp();
        }
        private void Button_Clicked_ReminderCreate_Done(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(reminderTitleEntry.Text)) {
                DisplayAlert("No Reminder title.", "Please enter a title for your Reminder.", "Ok");
                return;
            }
            Reminder reminderToAdd = new Reminder(reminderTitleEntry.Text, reminderDescriptionEditor.Text, reminderColorPicker.SelectedIndex + 1);
            classEditing.AddReminder(reminderToAdd);
            AddReminderToScrollView(reminderToAdd);

            App.dataManager.SaveClasses();

            CloseReminderCreatePopUp();
            UpdateClassCover();
        }
        private void UpdateScrollViewContents() {
            remindersOnDisplay.Clear();
            foreach (var item in classEditing.reminders) {
                AddReminderToScrollView(item);
            }
        }
        private void AddReminderToScrollView(Reminder reminder) {
            int indexToAddInto = scrollViewContents.Children.Count;
            for (int i = 0; i < remindersOnDisplay.Count; i++) {
                if (reminder.urgencyLevel >= remindersOnDisplay[i].urgencyLevel) {
                    indexToAddInto = i;
                    break;
                }
            }
            remindersOnDisplay.Insert(indexToAddInto, reminder);

            Button dismissButton = new Button() {
                Text = "Dismiss",
                TextTransform = TextTransform.None,
                CornerRadius = 15,
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Color.Transparent,
                TextColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].textColor,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            };

            Frame reminderFrame;
            if (string.IsNullOrEmpty(reminder.description)) {
                reminderFrame = new Frame() {
                    VerticalOptions = LayoutOptions.Start,
                    BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].borderColor,
                    CornerRadius = 17,
                    Margin = 10,
                    Padding = new Thickness(0, 0, 1, 1),
                    HasShadow = false,
                    Content = new Frame() {
                        BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].backgroundColor,
                        CornerRadius = 15,
                        Margin = 3,
                        Padding = 0,
                        HasShadow = false,
                        Content = new StackLayout() {
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                new Label() {
                                    Text = reminder.title,
                                    TextColor = Color.White,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    FontSize = 17,
                                    FontAttributes = FontAttributes.Bold,
                                    Margin = new Thickness(15,0,0,0),
                                    HorizontalTextAlignment = TextAlignment.Start,
                                    HorizontalOptions = LayoutOptions.StartAndExpand
                                },
                                dismissButton
                            }
                        }
                    }
                };
            } else {
                reminderFrame = new Frame() {
                    VerticalOptions = LayoutOptions.Start,
                    BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].borderColor,
                    CornerRadius = 20,
                    Margin = 10,
                    Padding = new Thickness(0, 0, 1, 1),
                    HasShadow = false,
                    Content = new Frame() {
                        BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].backgroundColor,
                        CornerRadius = 18,
                        Margin = 3,
                        Padding = 0,
                        HasShadow = false,
                        Content = new StackLayout() {
                            Children = {
                                new Frame() {
                                    VerticalOptions = LayoutOptions.Start,
                                    BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].borderColor,
                                    CornerRadius = 17,
                                    Margin = -3,
                                    Padding = new Thickness(0,0,1,1),
                                    HasShadow = false,
                                    Content = new Frame() {
                                        BackgroundColor = UrgencyColors.urgencyColors[reminder.urgencyLevel].backgroundColor,
                                        CornerRadius = 15,
                                        Margin = 3,
                                        Padding = 0,
                                        HasShadow = false,
                                        Content = new StackLayout() {
                                            Orientation = StackOrientation.Horizontal,
                                            Children = {
                                                new Label() {
                                                    Text = reminder.title,
                                                    TextColor = Color.White,
                                                    VerticalTextAlignment = TextAlignment.Center,
                                                    FontSize = 17,
                                                    FontAttributes = FontAttributes.Bold,
                                                    Margin = new Thickness(15,0,0,0),
                                                    HorizontalTextAlignment = TextAlignment.Start,
                                                    HorizontalOptions = LayoutOptions.StartAndExpand
                                                },
                                                dismissButton
                                            }
                                        }
                                    }
                                },
                                new Label() {
                                    Text = reminder.description,
                                    Margin = new Thickness(15,10),
                                    TextColor = Color.White,
                                }
                            }
                        }
                    }
                };
            }

            dismissButton.Clicked += (s, e) => { OnDismissButtonClicked(reminder, reminderFrame); };

            scrollViewContents.Children.Insert(indexToAddInto, reminderFrame);

        }
        private void OnDismissButtonClicked(Reminder reminder, View reminderView) {
            scrollViewContents.Children.Remove(reminderView);
            remindersOnDisplay.Remove(reminder);
            classEditing.RemoveReminder(reminder);
            App.dataManager.SaveClasses();
            UpdateClassCover();
        }
        #endregion
    }
}