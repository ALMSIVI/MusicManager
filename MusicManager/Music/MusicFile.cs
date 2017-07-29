using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using TagLib;

namespace MusicManager.Music {
  public abstract class MusicFile {

    #region Constants
    public const long HR_TO_MILLI = 3600000;
    public const long MIN_TO_MILLI = 60000;
    public const long SEC_TO_MILLI = 1000;
    #endregion

    #region File
    public string Filename { get; }
    protected File tagFile;
    public bool ReadOnly { get; private set; }
    protected MainWindow controller;
    #endregion

    #region Tag
    /* Basic Information, will vary from different format */
    protected string title; // 0
    protected string album; // 1
    protected string artist; // 2
    protected uint trackNo; // 3
    protected string genre; // 4
    protected uint year; // 5
    protected string comment; // 6

    /* Encoding Information */
    protected string encoding; // 7
    protected uint channel; // 8
    protected uint frequency; // 9
    protected uint bit; // 10
    protected uint rate; // 11
    protected long length; // 12: In milliseconds
    protected uint gain; // 13
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor of music
    /// </summary>
    /// <param name="musicName"></param>
    public MusicFile(string musicName, MainWindow windowController) {
      Filename = musicName;
      tagFile = File.Create(musicName);
      ReadOnly = tagFile.Writeable;
      length = tagFile.Length;
      controller = windowController;
    }
    #endregion

    #region Abstract methods
    public abstract void UpdateTag();
    public abstract void Play();
    public abstract void Pause();
    #endregion

    #region Concrete methods
    public string[] PassInfo() {
      string[] tag = new string[8];
      tag[0] = title;
      tag[1] = album;
      tag[2] = artist;
      tag[3] = trackNo.ToString();
      tag[4] = genre;
      tag[5] = year.ToString();
      tag[6] = comment;

      tag[7] = FormatLength();
      return tag;
    }



    /// <summary>
    /// Formats the length to HH:MM:SS.Mil.
    /// </summary>
    /// <returns>
    /// A string that contains the format length.
    /// </returns>
    public string FormatLength() {
      string lengthStr = "";

      long time = length;
      long tempTime;

      // Hour: optional
      tempTime = time / HR_TO_MILLI;
      if (tempTime != 0) { // Has an hour value
        lengthStr = lengthStr + tempTime + ":";
      }
      time = time % HR_TO_MILLI;

      // Min: must have
      tempTime = time / MIN_TO_MILLI;
      lengthStr = lengthStr + tempTime + ":";
      time = time % MIN_TO_MILLI;

      // Sec & Millisec: must have
      tempTime = time / SEC_TO_MILLI;
      lengthStr = lengthStr + tempTime + "." + (time % SEC_TO_MILLI);

      return lengthStr;
    }
    #endregion

    #region Override methods
    public override string ToString() {
      return title;
    }

    public override bool Equals(object obj) {
      return obj is MusicFile;
    }
    #endregion
  }
}
