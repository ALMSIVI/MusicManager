using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace MusicManager.Music {
  public abstract class Music {
    /* Basic Information, will vary from different format */
    private string Title; // 0
    private int TrackNo; // 1
    private string Artist; // 2
    private string Album; // 3
    private string Genre; // 4
    private int Year; // 5
    private string Comment; // 6

    /* Encoding Information */
    private string Encoding; // 7
    private int Channel; // 8
    private int Frequency; // 9
    private int Bit; // 10
    private int Rate; // 11
    private long Length; // 12: In milliseconds
    private int Gain; // 13


    /* Methods */
    public abstract void Play();
    public abstract void Pause();
    public virtual string[] LoadInfo() { return new string[10]; }
    public virtual void SaveInfo(string[] info) { }

  }
}
