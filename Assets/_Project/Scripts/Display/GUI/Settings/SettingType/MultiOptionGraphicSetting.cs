using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MultiOptionGraphicSetting<T> : Setting, IMultiOptionGraphicSetting
{
    protected OptionTable<T> OptionTable = new();
    int selectedIndex;

    protected void AddOption(string name, T value)
    {
        OptionTable.Add(name, value);
    }

    public List<string> GetOptionNames()
    {
        return OptionTable.GetNames();
    }

    public int GetIndex()
    {
        return selectedIndex;
    }

    public void SetIndex(int index)
    {
        selectedIndex = index;
    }
    
    protected void SelectOption(T value, int defaultIndex = 0)
    {
        if (TryGetIndex(value, out var index))
        {
            SetIndex(index);
        }
        else
        {
            SetIndex(defaultIndex);
        }
    }
    
    protected void SelectOption(Predicate<T> predicate, int defaultIndex = 0)
    {
        if(TryGetIndex(predicate, out var index))
        {
            SetIndex(index);
        }
        else
        {
            SetIndex(defaultIndex);
        }
    }
    
    protected bool TryGetIndex(T option, out int index)
    {
        return TryGetIndex(entry => entry.Equals(option), out index);
    }
    
    protected bool TryGetIndex(Predicate<T> predicate, out int index)
    {
        index = -1;

        var entries = OptionTable.GetValues();

        for (int i = 0; i < entries.Count; i++)
        {
            if (predicate(entries[i]))
            {
                index = i;
                return true;
            }
        }

        return false;
    }
    
    public T GetSelectedOption() => OptionTable.GetValue(selectedIndex);
}

public class OptionTable<T>
{
    public struct OptionEntry
    {
        public string Name;
        public T Value;

        public OptionEntry(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }

    List<OptionEntry> entries = new();

    public void Add(string name, T value)
    {
        entries.Add(new OptionEntry(name, value));
    }

    public List<string> GetNames() => entries.Select(x => x.Name).ToList();
    public List<T> GetValues() => entries.Select(x => x.Value).ToList();

    public T GetValue(int index) => entries[index].Value;

    public string GetName(int index) => entries[index].Name;
}

public interface IMultiOptionGraphicSetting
{
    List<string> GetOptionNames();
    int GetIndex();
    void SetIndex(int index);
}
