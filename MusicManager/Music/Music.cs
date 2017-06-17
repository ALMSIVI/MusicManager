using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using TagLib;

namespace MusicManager.Music {
  public abstract class Music {
    /* Basic Information, will vary from different format */
    private string title; // 0
    private int trackNo; // 1
    private string artist; // 2
    private string album; // 3
    private string genre; // 4
    private int year; // 5
    private string comment; // 6

    /* Encoding Information */
    private string encoding; // 7
    private int channel; // 8
    private int frequency; // 9
    private int bit; // 10
    private int rate; // 11
    private long length; // 12: In milliseconds
    private int gain; // 13


    /* Methods */
    public abstract void Play();
    public abstract void Pause();
    public virtual string[] LoadInfo() { return new string[10]; }
    public virtual void SaveInfo(string[] info) { }

  }
}
