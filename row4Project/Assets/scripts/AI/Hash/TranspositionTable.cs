using UnityEngine;
using System.Collections.Generic;

public class TranspositionTable {
    public int length;
    Dictionary<int, Record> records;
    
    public TranspositionTable (int _length)
    {
        length = _length;
        records = new Dictionary<int, Record>();
    }

    public void SaveRecord (Record record)
    {
        records[record.hashValue % length] = record;
    }

    public Record GetRecord (int hash)
    {
        Record record;
        int key = hash % length;
        if (records.ContainsKey(key))
        {
            record = records[key];
            if (record.hashValue == hash)
            {
                return record;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
