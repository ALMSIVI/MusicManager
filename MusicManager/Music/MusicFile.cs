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
    public string Filename { get; protected set; }
    protected File tagFile;
    public bool ReadOnly { get; private set; }
    protected MainWindow controller;
    #endregion

    #region Tag
    /// <remarks>
    /// Auto property is enabled for tags, but when passing information,
    /// instead of directly using them, two methods called PassInfo() and
    /// SaveInfo(), are used to better format these data for the UI.
    /// </remarks>
    public string Title { get; protected set; }
    public string Album { get; protected set; }
    public string Artist { get; protected set; }
    public uint TrackNo { get; protected set; }
    public string Genre { get; protected set; }
    public uint Year { get; protected set; }
    public string Comment { get; protected set; }
    #endregion

    #region Stream
    protected string encoding;
    protected int channel;
    protected int sampleRate;
    protected int bits;
    protected int bitRate;
    protected TimeSpan length;
    protected uint gain;
    #endregion

    #region Other Info
    public string Format { get; protected set; }
    public string Length {
      get {
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
    }
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
        ["title"] = Title,
        ["album"] = Album,
        ["artist"] = Artist,
        ["trackNo"] = TrackNo.ToString(),
        ["genre"] = Genre,
        ["year"] = Year.ToString(),
        ["comment"] = Comment,

        ["filename"] = Filename,
        ["encoding"] = encoding,
        ["channel"] = channel.ToString(),
        ["sampleRate"] = sampleRate.ToString() + " Hz",
        ["bits"] = bits.ToString() + " Bits",
        ["bitRate"] = bitRate.ToString() + " Kbps",
        ["length"] = Length
      };
    }

    public virtual void SaveInfo(Dictionary<string, string> newInfo) {
      /// <remarks>
      /// Note that TrackNo is not set. This is because for some music with tag
      /// TrackCount, the info passed in will not be a parsable string.
      /// </remarks>
      Title = newInfo["title"];
      Album = newInfo["album"];
      Artist = newInfo["artist"];
      Genre = newInfo["genre"];
      Year = UInt32.Parse(newInfo["year"]);
      Comment = newInfo["comment"];
    }

    #endregion

    #region Override methods
    public override string ToString() {
      // TODO: Read format from settings
      return Title;
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
