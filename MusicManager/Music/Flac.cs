using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicManager.Music {
  class Flac : MusicFile {

    public Flac(string musicName) : base(musicName) {
    }

    public override void Play() { }
    public override void Pause() { }
  }
}
