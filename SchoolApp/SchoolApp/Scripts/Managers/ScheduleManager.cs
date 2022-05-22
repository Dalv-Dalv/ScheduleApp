using SchoolApp;
using System.Collections.Generic;

public class ScheduleManager {
    public readonly List<Schedule> schedules;

    public ScheduleManager() {
        schedules = new List<Schedule>();
    }

    public void HandleClassesCleared() {
        foreach (var schedule in schedules) {
            schedule.RemoveAllEntries();
        }
        App.dataManager.SaveSchedules();
    }
}
