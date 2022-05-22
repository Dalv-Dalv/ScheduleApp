using SchoolApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;


public class DataManager {
    string classesPath, schedulesPath, auxiliaryPath;
    public DataManager() {
        classesPath = Path.Combine(FileSystem.AppDataDirectory, "SSApp_Classes.txt");
        if (!File.Exists(classesPath)) {
            File.WriteAllText(classesPath, "0");
        }

        schedulesPath = Path.Combine(FileSystem.AppDataDirectory, "SSApp_Schedules.txt");
        if (!File.Exists(schedulesPath)) {
            File.WriteAllText(schedulesPath, "0");
        }

        auxiliaryPath = Path.Combine(FileSystem.AppDataDirectory, "SSApp_Auxiliary.txt");
        if (!File.Exists(auxiliaryPath)) {
            File.WriteAllText(auxiliaryPath, "0");
        }
    }
    public void SaveClasses() {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(App.classesManager.classes.Count.ToString());
        for (int i = 0; i < App.classesManager.classes.Count; i++) {
            stringBuilder.AppendLine(App.classesManager.classes[i].name);
            stringBuilder.AppendLine(App.classesManager.classes[i].reminders.Count.ToString());
            for (int r = 0; r < App.classesManager.classes[i].reminders.Count; r++) {
                stringBuilder.AppendLine(App.classesManager.classes[i].reminders[r].title);
                stringBuilder.AppendLine(App.classesManager.classes[i].reminders[r].urgencyLevel.ToString());
                string[] descriptionLines = App.classesManager.classes[i].reminders[r].description.Split('\n');
                stringBuilder.AppendLine(descriptionLines.Length.ToString());
                foreach (var line in descriptionLines) {
                    stringBuilder.AppendLine(line);
                }
            }
            string[] noteLines = App.classesManager.classes[i].notes.Split('\n');
            stringBuilder.AppendLine(noteLines.Length.ToString());
            foreach (var line in noteLines) {
                stringBuilder.AppendLine(line);
            }
        }
        //foreach (Class sclass in App.classesManager.classes) {
        //    stringBuilder.AppendLine(sclass.name);
        //}
        File.WriteAllText(classesPath, stringBuilder.ToString());
    }
    public void LoadClasses() {
        App.classesManager.ClearClasses();
        string debug = File.ReadAllText(classesPath);
        using (StreamReader reader = new StreamReader(classesPath)) {
            int classesCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < classesCount; i++) {
                Class sclass = new Class(reader.ReadLine());
                App.classesManager.AddClass(sclass);
                int remindersCount = int.Parse(reader.ReadLine());
                for (int r = 0; r < remindersCount; r++) {
                    string reminderTitle = reader.ReadLine();
                    int reminderUrgencyLevel = int.Parse(reader.ReadLine());
                    int reminderDescriptionLinesCount = int.Parse(reader.ReadLine());
                    if (reminderDescriptionLinesCount == 0) {
                        sclass.AddReminder(new Reminder(reminderTitle, "", reminderUrgencyLevel));
                        continue;
                    } else {
                        StringBuilder descriptionSB = new StringBuilder();
                        for (int dl = 0; dl < reminderDescriptionLinesCount; dl++) {
                            descriptionSB.AppendLine(reader.ReadLine());
                        }
                        sclass.AddReminder(new Reminder(reminderTitle, descriptionSB.ToString(), reminderUrgencyLevel));
                    }
                }
                int noteLinesCount = int.Parse(reader.ReadLine());
                if (noteLinesCount == 0) continue;
                StringBuilder notesSB = new StringBuilder();
                for (int nl = 0; nl < noteLinesCount; nl++) {
                    notesSB.AppendLine(reader.ReadLine());
                }
                sclass.SetNotes(notesSB.ToString());
            }
        }
    }


    public void SetAuxiliary(int to) {
        File.WriteAllText(auxiliaryPath, to.ToString());
    }
    public short LoadAuxiliary() {
        return Convert.ToInt16(File.ReadAllText(auxiliaryPath));
    }


    public void SaveSchedules() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(App.scheduleManager.schedules.Count.ToString());
        foreach (var schedule in App.scheduleManager.schedules) {
            sb.AppendLine(schedule.name);
            //Loop through days
            for (int i = 0; i < 7; i++) {
                sb.AppendLine(schedule.entriesByDay[i].Count.ToString());
                foreach (var item in schedule.entriesByDay[i]) {
                    sb.AppendLine(item.classIndex.ToString());
                    sb.AppendLine(item.startTime.ToString(@"hh\:mm"));
                    sb.AppendLine(item.endTime.ToString(@"hh\:mm"));
                }
            }
        }
        string test = sb.ToString();
        File.WriteAllText(schedulesPath, sb.ToString());
    }
    public void LoadSchedules() {
        using (StreamReader reader = new StreamReader(schedulesPath)) {
            int schedulesCount = int.Parse(reader.ReadLine());
            for (int si = 0; si < schedulesCount; si++) {
                Schedule newSchedule = new Schedule(reader.ReadLine());
                for (int day = 0; day < 7; day++) {
                    int classesCount = int.Parse(reader.ReadLine());
                    for (int ci = 0; ci < classesCount; ci++) {
                        newSchedule.entriesByDay[day].Add(new ScheduleEntry(
                            int.Parse(reader.ReadLine()),
                            TimeSpan.Parse(reader.ReadLine()),
                            TimeSpan.Parse(reader.ReadLine())
                            ));
                    }
                }
            }
        }
    }
}
