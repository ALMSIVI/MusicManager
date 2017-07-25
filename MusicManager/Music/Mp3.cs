using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace MusicManager.Music {
  public class Mp3 : MusicFile {
    private delegate void LoadInfo();

    public Mp3(string musicName, int priority) : base(musicName) {
      /// <see cref="readPriority"/>
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

      loadInfo[priority]();
    }

    /* Three tags for MP3 */
    private void LoadId3V1() {
      Tag tag = tagFile.GetTag(TagTypes.Id3v1);
      title = tag.Title;
      artist = String.Join(",", tag.Performers);
      album = tag.Album;
      year = tag.Year;
      comment = tag.Comment;
      trackNo = tag.Track;
      genre = String.Join(",", tag.Genres);
    }
    private void LoadId3V2() { }
    private void LoadApeV2() { }


    public override void Pause() {
      throw new NotImplementedException();
    }

    public override void Play() {
      throw new NotImplementedException();
    }
  }
}
