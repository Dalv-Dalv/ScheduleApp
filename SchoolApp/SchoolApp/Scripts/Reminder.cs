using System;
using System.Collections.Generic;
using System.Text;

public class Reminder {
    public string title, description;
    public int urgencyLevel;

    public Reminder(string title, string description, int urgencyLevel) {
        this.title = title.Trim();
        this.description = description.Trim();
        this.urgencyLevel = urgencyLevel;
    }
}