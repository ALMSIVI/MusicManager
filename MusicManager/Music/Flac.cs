using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicManager.Music {
  class Flac : MusicFile {

    public Flac(string musicName, MainWindow windowController)
      : base(musicName, windowController) {
    }

    public override void Play() {
      throw new NotImplementedException();
    }
    public override void Pause() {
      throw new NotImplementedException();
    }
    public override void UpdateTag() {
      throw new NotImplementedException();
    }
  }
}
