using System;
using System.Collections.Generic;

public class Class {
    public string name;
    public string notes = "";
    public int urgencyLevel {
        get; private set;
    } = 0;
    public readonly List<Reminder> reminders = new List<Reminder>();

    public Class(string name) {
        this.name = name;
    }
    public Class(string name, List<Reminder> reminders) {
        this.name = name;
        this.reminders = new List<Reminder>(reminders);
    }

    public void SetNotes(string notes) {
        this.notes = notes.Trim();
    }

    public void AddReminder(Reminder reminder) {
        if (reminder.urgencyLevel > urgencyLevel) {
            urgencyLevel = reminder.urgencyLevel;
        }
        reminders.Add(reminder);
    }
    public void RemoveReminder(Reminder reminder) {
        reminders.Remove(reminder);
        if (reminders.Count == 0) {
            urgencyLevel = 0;
            return;
        }
        int min = urgencyLevel;
        foreach (var item in reminders) {
            if (urgencyLevel > item.urgencyLevel) {
                min = item.urgencyLevel;
            } else if (urgencyLevel == item.urgencyLevel) {
                return;
            }
        }
        urgencyLevel = min;
    }
}
