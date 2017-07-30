using System;
using System.Collections.Generic;
using TagLib;

namespace MusicManager.Music {
  public class Mp3 : MusicFile {
    private delegate void LoadInfo();

    public Mp3(string musicName, MainWindow windowController) :
      base(musicName, windowController) {
      encoding = "MPEG 1 Layer 3";
      bits = 16; // For MP3 there is no such thing as bits
      // TODO: Resolve encoding not fit issue
      // TODO: There is a bug with VBR tag.
      UpdateTag();
    }

    /* Three tags for MP3 */
    private void LoadId3V1() {
      Tag tag = tagFile.GetTag(TagTypes.Id3v1);
      title = tag.Title == null ? String.Empty : tag.Title;
      artist = String.Join(",", tag.Performers);
      album = tag.Album == null ? String.Empty : tag.Album;
      year = tag.Year;
      comment = tag.Comment == null ? String.Empty : tag.Comment;
      trackNo = tag.Track;
      genre = String.Join(",", tag.Genres);
    }
    private void LoadId3V2() { }
    private void LoadApeV2() { }

    public override void UpdateTag() {
      /// <see cref="MainWindow.ReadPriority"/>
      IDictionary<Int32, LoadInfo> loadInfo = new Dictionary<Int32, LoadInfo>();
      loadInfo[0] = delegate {
        LoadApeV2();
        LoadId3V2();
        LoadId3V1();
      };
      loadInfo[1] = delegate {
        LoadId3V2();
        LoadApeV2();
        LoadId3V1();
      };
      loadInfo[2] = delegate {
        LoadId3V1();
        LoadId3V2();
        LoadApeV2();
      };
      loadInfo[3] = delegate {
        LoadId3V1();
        LoadApeV2();
        LoadId3V2();
      };

      loadInfo[controller.ReadPriority]();
    }

    public override Dictionary<string, string> PassInfo() {
      // TODO: after loading ID3v2, modify track count
      // TODO: Add VBR/CBR/ABR after bitrate
      return base.PassInfo();
    }

    public override void Pause() {
      throw new NotImplementedException();
    }

    public override void Play() {
      throw new NotImplementedException();
    }
  }
}
