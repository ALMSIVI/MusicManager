using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicManager {
  /// <summary>
  /// Represents the value to be displayed in a TextBox.
  /// </summary>
  public class TextValue {
    public const string MULTIPLE_VALUES = "(Multiple Values)";

    private Dictionary<string, int> values;

    public TextValue() {
      values = new Dictionary<string, int>();
    }

    public void Add(string value) {
      int frequency;
      if (values.TryGetValue(value, out frequency)) {
        values[value]++;
      } else {
        values[value] = 1;
      }
    }

    public void Delete(string value) {
      if (values[value] == 1) {
        values.Remove(value);
      } else {
        values[value]--;
      }
    }
    
    public void Clear() {
      values.Clear();
    }

    public override string ToString() {
      if (values.Count > 1) {
        return MULTIPLE_VALUES;
      } else if (values.Count == 1) {
        return values.First().Key;
      } else {
        return "";
      }
    }
  }
}
