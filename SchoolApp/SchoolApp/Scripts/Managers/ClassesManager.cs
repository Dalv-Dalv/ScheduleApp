using SchoolApp;
using System;
using System.Collections.Generic;
using System.Text;

public class ClassesManager {
    public List<Class> classes { get; private set; }

    public static Action<int> onClassDeleted;

    public ClassesManager() {
        classes = new List<Class>();
    }
    public ClassesManager(List<Class> classes) {
        this.classes = new List<Class>(classes);
    }

    public void InstantiateClasses(List<Class> classes) {
        this.classes = new List<Class>(classes);
    }

    public void InstantiateClasses(List<string> classNames) {
        classes.Clear();
        foreach (var item in classNames) {
            AddClass(new Class(item));
        }
    }

    public void AddClass(Class c) {
        classes.Add(c);
    }

    public void DeleteClass(Class c) {
        if (!classes.Contains(c)) return;
        onClassDeleted?.Invoke(classes.IndexOf(c));
        classes.Remove(c);
        App.dataManager.SaveClasses();
    }
    public void DeleteAllClasses() {
        App.scheduleManager.HandleClassesCleared();
        classes.Clear();
        App.dataManager.SaveClasses();
    }

    internal void ClearClasses() {
        classes.Clear();
    }
}