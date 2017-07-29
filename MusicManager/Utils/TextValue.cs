using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicManager.Utils {
  public class TextValue {
    private List<string> values;

    public TextValue() {
      values = new List<string>();
    }

    public void Add(string value) {
      if (!values.Contains(value)) {
        values.Add(value);
      }
    }

    public void Delete(string value) {
      values.Remove(value);
    }

    public override string ToString() {
      if (values.Count > 1) {
        return "(Multiple Values)";
      } else if (values.Count == 1) {
        return values.ElementAt(0);
      } else {
        return "";
      }
    }
  }
}
