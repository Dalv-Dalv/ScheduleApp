using SchoolApp;
using System.Collections.Generic;
using System.Text;

public class Schedule {
    public string name;
    public readonly List<ScheduleEntry>[] entriesByDay = new List<ScheduleEntry>[7];

    public Schedule(string name) {
        this.name = name;
        for (int i = 0; i < 7; i++) {
            entriesByDay[i] = new List<ScheduleEntry>();
        }

        App.scheduleManager.schedules.Add(this);

        ClassesManager.onClassDeleted += HandleClassDeleted;
    }

    public void AddEntry(int dayIndex, ScheduleEntry newEntry) {
        for (int i = 0; i < entriesByDay[dayIndex].Count; i++) {
            if (newEntry.startTime < entriesByDay[dayIndex][i].startTime) {
                entriesByDay[dayIndex].Insert(i, newEntry);
                return;
            }
        }
        entriesByDay[dayIndex].Add(newEntry);
    }
    public void RemoveEntry(int dayIndex, ScheduleEntry entry) {
        if (!entriesByDay[dayIndex].Contains(entry)) return;
        entriesByDay[dayIndex].Remove(entry);
    }

    public void RemoveAllEntries() {
        for (int i = 0; i < entriesByDay.Length; i++) {
            entriesByDay[i].Clear();
        }
    }

    private void HandleClassDeleted(int classIndexToRemove) {
        for (int i = 0; i < entriesByDay.Length; i++) {
            int e = 0;
            while (e < entriesByDay[i].Count) {
                if (entriesByDay[i][e].classIndex == classIndexToRemove) {
                    entriesByDay[i].Remove(entriesByDay[i][e]);
                    continue;
                } else if (entriesByDay[i][e].classIndex > classIndexToRemove) {
                    entriesByDay[i][e].classIndex--;
                }
                e++;
            }
        }
    }

}