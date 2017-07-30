using System;
using System.Collections.Generic;
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
    protected int channel; // 8
    protected int sampleRate; // 9
    protected int bits; // 10
    protected int bitRate; // 11
    protected TimeSpan length; // 12: In milliseconds
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

      channel = tagFile.Properties.AudioChannels;
      sampleRate = tagFile.Properties.AudioSampleRate;
      bits = tagFile.Properties.BitsPerSample;
      bitRate = tagFile.Properties.AudioBitrate;
      length = tagFile.Properties.Duration;

      controller = windowController;
    }
    #endregion

    #region Abstract methods
    public abstract void UpdateTag();
    public abstract void Play();
    public abstract void Pause();
    #endregion

    #region Concrete methods
    public virtual Dictionary<string, string> PassInfo() {
      return new Dictionary<string, string> {
        ["title"] = title,
        ["album"] = album,
        ["artist"] = artist,
        ["trackNo"] = trackNo.ToString(),
        ["genre"] = genre,
        ["year"] = year.ToString(),
        ["comment"] = comment,

        ["filename"] = Filename,
        ["encoding"] = encoding,
        ["channel"] = channel.ToString(),
        ["sampleRate"] = sampleRate.ToString() + " Hz",
        ["bits"] = bits.ToString() + " Bits",
        ["bitRate"] = bitRate.ToString() + " Kbps",
        ["length"] = FormatLength()
      };
    }

    /// <summary>
    /// Formats the length to HH:MM:SS.Mil.
    /// </summary>
    /// <returns>
    /// A string that contains the format length.
    /// </returns>
    public string FormatLength() {
      if (length.Hours == 0) {
        if (length.Minutes < 10) {
          return length.ToString(@"m\:ss\.fff");
        } else {
          return length.ToString(@"mm\:ss\.fff");
        }
      } else {
        return length.ToString(@"hh\:mm\:ss\.fff");
      }
    }
    #endregion

    #region Override methods
    public override string ToString() {
      // TODO: Read format from settings
      return title;
    }

    public override bool Equals(object obj) {
      if (!(obj is MusicFile)) {
        return false;
      }

      return Filename.Equals(((MusicFile)obj).Filename);
    }
    #endregion
  }
}
