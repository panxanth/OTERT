using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[Serializable]
public class FilterData {
    public List<FilterRecord> Records { get; set; }
    public FilterData() {
        this.Records = new List<FilterRecord>();
    }
    public void AddRecord(string Operator, string ColumnName, string Value) {
        FilterRecord newRecord = new FilterRecord();
        newRecord.Operator = Operator;
        newRecord.ColumnName = ColumnName;
        newRecord.Value = Value;
        this.Records.Add(newRecord);
    }
    public void RemoveRecord(string Operator, string ColumnName) {
        FilterRecord newRecord = new FilterRecord();
        newRecord.Operator = Operator;
        newRecord.ColumnName = ColumnName;
        newRecord.Value = "";
        this.Records.Remove(newRecord);
    }
}

[Serializable]
public class FilterRecord : IEquatable<FilterRecord> {
    public string Operator { get; set; }
    public string ColumnName { get; set; }
    public string Value { get; set; }
    public bool Equals(FilterRecord other) {
        if (other == null) return false;
        return (this.Operator.Equals(other.Operator) && this.ColumnName.Equals(other.ColumnName));
    }
}