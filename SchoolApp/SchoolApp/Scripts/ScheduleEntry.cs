using System;

public class ScheduleEntry {
    public int classIndex;
    public TimeSpan startTime, endTime;

    public ScheduleEntry(int classIndex, TimeSpan startTime, TimeSpan endTime) {
        this.classIndex = classIndex;
        this.startTime = startTime;
        this.endTime = endTime;
    }
}