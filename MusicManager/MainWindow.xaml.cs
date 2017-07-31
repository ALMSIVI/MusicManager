using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;

using MusicManager.Music;
using MusicManager.Utils;

namespace MusicManager {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    #region Options 
    /// <summary>
    /// Read Type Priority as specified in the ComboBox. Default to 0.
    /// 0: APEv2 > ID3v2 > ID3v1
    /// 1: ID3v2 > APEv2 > ID3v1
    /// 2: ID3v1 > ID3v2 > APEv2
    /// 3: ID3v1 > APEv2 > ID3v2
    /// </summary>
    public int ReadPriority {
      get {
        return cbxPriority.SelectedIndex;
      }
    }

    /// <summary>
    /// Encoding for ID3v2 tag. Default to 0.
    /// 0: ISO-8859-1, a.k.a., Latin-1
    /// 1: UTF-8
    /// 2: UTF-16
    /// </summary>
    public int Encoding {
      get {
        return cbxType.SelectedIndex;
      }
    }

    /// <summary>
    /// Write type for MP3 files. Default to 3.
    /// 0: ID3v1
		///	1: ID3v2
		///	2: APEv2
		///	3: ID3v1 & ID3v2
		///	4: ID3v1 & APEv2
		/// 5: ID3v2 & APEv2
    /// </summary>
    public int Write {
      get {
        return cbxWrite.SelectedIndex;
      }
    }

    /// <summary>
    /// Data padding when writing file. Default to true.
    /// </summary>
    public bool PaddingData {
      get {
        return (bool)(chkPadding.IsChecked); // this cannot be null
      }
    }
    #endregion

    #region StoredInfo
    // TODO: use ConcurrentDictionary to enable multithreading
    /// <summary>
    /// Stores all filenames so that a file cannot be added to the list twice.
    /// </summary>
    private Dictionary<string, MusicFile> filenames;
    /// <summary>
    /// Stores all the TextBox-TextValue pairs that represent the tag section.
    /// </summary>
    private Dictionary<TextBox, TextValue> tagValues;
    /// <summary>
    /// Stores all the TextBox-TextValue pairs that represent the file section.
    /// </summary>
    private Dictionary<TextBox, TextValue> streamValues;
    #endregion

    #region Constructor
    public MainWindow() {
      InitializeComponent();

      /* Initialize the stored info */
      filenames = new Dictionary<string, MusicFile>();

      tagValues = new Dictionary<TextBox, TextValue> {
        [txtTitle] = new TextValue(),
        [txtAlbum] = new TextValue(),
        [txtArtist] = new TextValue(),
        [txtTrack] = new TextValue(),
        [txtGenre] = new TextValue(),
        [txtYear] = new TextValue(),
        [txtComment] = new TextValue()
      };

      streamValues = new Dictionary<TextBox, TextValue> {
        [txtFilename] = new TextValue(),
        [txtEncode] = new TextValue(),
        [txtChannel] = new TextValue(),
        [txtSampleRate] = new TextValue(),
        [txtBits] = new TextValue(),
        [txtBitRate] = new TextValue(),
        [txtLength] = new TextValue(),
        [txtGain] = new TextValue()
      };
    }
    #endregion

    #region Events
    /// <summary>
    /// Handles a variety of trivial button clicks.
    /// </summary>
    /// <param name="sender">The clicked button</param>
    /// <param name="e">Click arguments</param>
    public void ButtonClicked(object sender, RoutedEventArgs e) {
      Button button = sender as Button;
      switch (button.Name) {
        case "btnConvert":
          Conversion conversionWindow = new Conversion();
          conversionWindow.Show();
          break;

        case "btnMore":
          MoreInfo moreInfoWindow = new MoreInfo();
          moreInfoWindow.Show();
          break;

        case "btnGuess":
          Guess guessWindow = new Guess();
          guessWindow.Show();
          break;

        case "btnEdit":
          if (btnEdit.Content.Equals("Edit Tag")) {
            gridTag.IsEnabled = true;
            btnEdit.Content = "Save Tag";
          } else {
            gridTag.IsEnabled = false;
            btnEdit.Content = "Edit Tag";
          }
          break;
      }
    }

    /// <summary>
    /// Handles a variety of menu item clicks.
    /// </summary>
    /// <param name="sender">The clicked MenuItem</param>
    /// <param name="e">Click arguments</param>
    public void MenuClicked(object sender, RoutedEventArgs e) {
      MenuItem menuItem = sender as MenuItem;
      switch (menuItem.Name) {
        case "menuAbout":
          About aboutWindow = new About();
          aboutWindow.Show();
          break;

        case "menuSettings":
          Settings settingsWindow = new Settings();
          settingsWindow.Show();
          break;
      }
    }

    /// <summary>
    /// Loads the music files that the user has dropped into the playlist.
    /// </summary>
    /// <param name="sender">The playlist as Listbox</param>
    /// <param name="e">Drag arguments</param>
    private void MusicDrop(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
        HandleOpen((string[])e.Data.GetData(DataFormats.FileDrop));
      }
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CoverArtDrop(object sender, DragEventArgs e) {

    }

    /// <summary>
    /// Opens image and sets as cover album.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImageOpen(object sender, RoutedEventArgs e) {
      OpenFileDialog imageDialog = new OpenFileDialog();
      if (imageDialog.ShowDialog() == true) { // opens
        //TODO: load cover art (When implementing ID3V2)
      }
    }
    #endregion

    #region Commands
    /// <summary>
    /// Open command: generates a dialog and handles file open.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="e"></param>
    private void OpenExecuted(object target, ExecutedRoutedEventArgs e) {
      OpenFileDialog musicDialog = new OpenFileDialog();
      musicDialog.Multiselect = true;
      //TODO: Filter and InitialDirectory, read from config file
      if (musicDialog.ShowDialog() == true) { // opens
        HandleOpen(musicDialog.FileNames);
      }
    }

    /// <summary>
    /// Delete command: delete items from the playlist
    /// </summary>
    /// <param name="target"></param>
    /// <param name="e"></param>
    private void DeleteExecuted(object target, ExecutedRoutedEventArgs e) {
      int selectedIndex = playlist.SelectedIndex;
      if (!playlist.SelectedItems.Contains(defaultItem)) {
        while (playlist.SelectedItems.Count != 0) {
          RemoveInfo((MusicFile)playlist.SelectedItem);
          filenames.Remove(((MusicFile)(playlist.SelectedItem)).Filename);
          playlist.Items.Remove(playlist.SelectedItem);
        }

        if (playlist.Items.IsEmpty) {
          playlist.Items.Add(defaultItem);
          SongOptions.IsEnabled = false;
          CleanInfo();
        }
        DisplayInfo();
        playlist.SelectedIndex = selectedIndex >= playlist.Items.Count ? 
          playlist.Items.Count - 1 : selectedIndex;
      }
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// Opens each music file by setting up the MusicFile objects.
    /// If thte filename already appears in the HashSet, it means that
    /// </summary>
    /// <param name="filenames">an array of music filenames</param>
    private void HandleOpen(string[] filenames) {
      playlist.Items.Remove(defaultItem);
      // This is the list to be updated in tag info

      bool canEdit = true;
      /* Parse the filenames */
      foreach (string filename in filenames) {
        /* Check repeated adding */
        MusicFile music;
        if (this.filenames.TryGetValue(filename, out music)) {
          // If repeated, do same as reloading
          // TODO: Make this reloading procedure a method
          RemoveInfo(music);
          music.UpdateTag();
          AddInfo(music);
          if (music.ReadOnly == false) {
            canEdit = false;
          }
        } else { // Not added; create the music first
          string extension = System.IO.Path.GetExtension(filename);
          if (extension.Equals(".cue")) {
            // TODO: CUE support (find a library)
          } else if (extension.Equals(".mp3")) {
            music = new Mp3(filename, this);
            AddInfo(music);
            this.filenames.Add(filename, music);
            playlist.Items.Add(music);
            if (music.ReadOnly == false) {
              canEdit = false;
            }
          } else {
            MessageBox.Show("Unsupported format!");
            // music.Add(new Flac(filename));
            // TODO: new formats
          }
        }
      }

      DisplayInfo();

      if (canEdit) {
        SongOptions.IsEnabled = true;
      } else {
        MessageBox.Show("Unable to edit tags; is the file read only, or have" +
          " you opened the file in another app?");
      }
    }

    private void AddInfo(MusicFile music) {
      Dictionary<string, string> musicTag = music.PassInfo();
      tagValues[txtTitle].Add(musicTag["title"]);
      tagValues[txtAlbum].Add(musicTag["album"]);
      tagValues[txtArtist].Add(musicTag["artist"]);
      tagValues[txtTrack].Add(musicTag["trackNo"]);
      tagValues[txtGenre].Add(musicTag["genre"]);
      tagValues[txtYear].Add(musicTag["year"]);
      tagValues[txtComment].Add(musicTag["comment"]);

      streamValues[txtFilename].Add(musicTag["filename"]);
      streamValues[txtEncode].Add(musicTag["encoding"]);
      streamValues[txtChannel].Add(musicTag["channel"]);
      streamValues[txtSampleRate].Add(musicTag["sampleRate"]);
      streamValues[txtBits].Add(musicTag["bits"]);
      streamValues[txtBitRate].Add(musicTag["bitRate"]);
      streamValues[txtLength].Add(musicTag["length"]);
    }
    
    private void RemoveInfo(MusicFile music) {
      Dictionary<string, string> musicTag = music.PassInfo();
      tagValues[txtTitle].Delete(musicTag["title"]);
      tagValues[txtAlbum].Delete(musicTag["album"]);
      tagValues[txtArtist].Delete(musicTag["artist"]);
      tagValues[txtTrack].Delete(musicTag["trackNo"]);
      tagValues[txtGenre].Delete(musicTag["genre"]);
      tagValues[txtYear].Add(musicTag["year"]);
      tagValues[txtComment].Delete(musicTag["comment"]);

      streamValues[txtFilename].Delete(musicTag["filename"]);
      streamValues[txtEncode].Delete(musicTag["encoding"]);
      streamValues[txtChannel].Delete(musicTag["channel"]);
      streamValues[txtSampleRate].Delete(musicTag["sampleRate"]);
      streamValues[txtBits].Delete(musicTag["bits"]);
      streamValues[txtBitRate].Delete(musicTag["bitRate"]);
      streamValues[txtLength].Delete(musicTag["length"]);
    }

    private void DisplayInfo() {
      foreach (KeyValuePair<TextBox, TextValue> pair in tagValues) {
        pair.Key.Text = pair.Value.ToString();
        if (pair.Value.ToString().Equals(TextValue.MULTIPLE_VALUES)) {
          pair.Key.Foreground = Brushes.Red;
        } else {
          pair.Key.Foreground = Brushes.Black;
        }
      }

      foreach (KeyValuePair<TextBox, TextValue> pair in streamValues) {
        pair.Key.Text = pair.Value.ToString();
        if (pair.Value.ToString().Equals(TextValue.MULTIPLE_VALUES)) {
          pair.Key.Foreground = Brushes.Red;
        } else {
          pair.Key.Foreground = Brushes.Black;
        }
      }
    }

    private void CleanInfo() {
      foreach (TextBox tagInfo in tagValues.Keys) {
        tagInfo.Clear();
      }
      foreach (TextBox streamInfo in streamValues.Keys) {
        streamInfo.Clear();
      }
      foreach (TextValue value in tagValues.Values) {
        value.Clear();
      }
      foreach (TextValue value in streamValues.Values) {
        value.Clear();
      }
    }
    #endregion

  }
}