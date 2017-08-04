using System;
using System.Collections.Generic;
using TagLib;

namespace MusicManager.Music {
  public class Mp3 : MusicFileWithArt {
    private delegate void LoadInfo();

    // MP3 unique properties
    public uint TrackCount { get; private set; } = 0;

    public Mp3(string musicName, MainWindow windowController) :
      base(musicName, windowController) {
      encoding = "MPEG 1 Layer 3";
      bits = 16; // For MP3 there is no such thing as bits
      Format = "MP3 " + (sampleRate / 1000) + "KHz " + bitRate + "K";
      // TODO: There is a bug with VBR tag.
      UpdateTag();
    }

    /* Three tags for MP3 */
    private void LoadId3V1() {
      Tag tag = tagFile.GetTag(TagTypes.Id3v1);
      if (tag != null) {
        Title = tag.Title == null ? Title : tag.Title;
        Artist = String.Join(",", tag.Performers) == String.Empty ? Artist :
          String.Join(",", tag.Performers);
        Album = tag.Album == null ? Album : tag.Album;
        Year = tag.Year;
        Comment = tag.Comment == null ? Comment : tag.Comment;
        TrackNo = tag.Track;
        Genre = String.Join(",", tag.Genres) == String.Empty ? Genre : 
          String.Join(",", tag.Genres);
      }
    }

    private void LoadId3V2() {
      Tag tag = tagFile.GetTag(TagTypes.Id3v2);
      if (tag != null) {
        Title = tag.Title == null ? Title : tag.Title;
        Artist = String.Join(",", tag.Performers) == String.Empty ? Artist :
          String.Join(",", tag.Performers);
        Album = tag.Album == null ? Album : tag.Album;
        Year = tag.Year;
        Comment = tag.Comment == null ? Comment : tag.Comment;
        TrackNo = tag.Track;
        Genre = String.Join(",", tag.Genres) == String.Empty ? Genre :
          String.Join(",", tag.Genres);

        // MP3 unique properties
        TrackCount = tag.TrackCount;
        CoverArt = tag.Pictures[0];
      }
    }

    private void LoadApeV2() {
      Tag tag = tagFile.GetTag(TagTypes.Ape);
      if (tag != null) {
        Title = tag.Title == null ? Title : tag.Title;
        Artist = String.Join(",", tag.Performers) == String.Empty ? Artist :
          String.Join(",", tag.Performers);
        Album = tag.Album == null ? Album : tag.Album;
        Year = tag.Year;
        Comment = tag.Comment == null ? Comment : tag.Comment;
        TrackNo = tag.Track;
        Genre = String.Join(",", tag.Genres) == String.Empty ? Genre :
          String.Join(",", tag.Genres);
      }
    }

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
      Dictionary<string, string> information = base.PassInfo();
      information["trackNo"] = TrackCount == 0 ? TrackNo.ToString() :
        TrackNo + "/" + TrackCount;
      return information;
    }

    public override void Pause() {
      throw new NotImplementedException();
    }

    public override void Play() {
      throw new NotImplementedException();
    }
  }
}
