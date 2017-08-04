using System.Collections.Generic;
using TagLib;

namespace MusicManager.Music {
  /// <summary>
  /// Represents a music file that supports cover art.
  /// </summary>
  public abstract class MusicFileWithArt : MusicFile {
    public IPicture CoverArt { get; protected set; }

    public MusicFileWithArt(string filename, MainWindow windowController) :
      base(filename, windowController) { }

    public override Dictionary<string, string> PassInfo() {
      // TODO: after loading ID3v2, modify track count
      // TODO: Add VBR/CBR/ABR after bitrate
      Dictionary<string, string> information = base.PassInfo();
      information["hasCoverArt"] = CoverArt == null ? "No" : "Yes";
      return information;
    }
  }
}
