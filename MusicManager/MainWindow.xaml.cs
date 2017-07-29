using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;
using System.IO;
using MusicManager.Music;
using Microsoft.Win32;

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

    /// <summary>
    /// Used to control all tag-related texts in the app.
    /// </summary>
    private List<TextBox> tagBoxes;
    /// <summary>
    /// Used to control all streaming-related texts in the app.
    /// </summary>
    private List<TextBox> streamBoxes;
    /// <summary>
    /// Stores all filenames so that a file cannot be added to the list twice.
    /// </summary>
    private HashSet<string> filenameSet;

    public MainWindow() {
      InitializeComponent();

      // Initialize the options

      tagBoxes = new List<TextBox>(7);
      tagBoxes.Add(txtTitle);
      tagBoxes.Add(txtAlbum);
      tagBoxes.Add(txtArtist);
      tagBoxes.Add(txtTrack);
      tagBoxes.Add(txtGenre);
      tagBoxes.Add(txtYear);
      tagBoxes.Add(txtComment);

      streamBoxes = new List<TextBox>(7);
      streamBoxes.Add(txtEncode);
      streamBoxes.Add(txtChannel);
      streamBoxes.Add(txtFrequency);
      streamBoxes.Add(txtBit);
      streamBoxes.Add(txtRate);
      streamBoxes.Add(txtLength);
      streamBoxes.Add(txtGain);

      filenameSet = new HashSet<string>();
    }

    #region Events
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

        case "btnDelete":
          playlist.Items.Remove(playlist.SelectedItems);
          if (playlist.Items.IsEmpty) {
            playlist.Items.Add(defaultItem);
            SongOptions.IsEnabled = true;
            CleanInfo();
          } else {
            UpdateInfo(playlist.Items.OfType<MusicFile>().ToList());
          }
          break;

        case "btnEdit":
          if (btnEdit.Content.Equals("Edit Info")) {
            foreach (TextBox box in tagBoxes) {
              box.IsEnabled = true;
            }
            btnEdit.Content = "Save Info";
          } else {
            foreach (TextBox box in tagBoxes) {
              box.IsEnabled = false;
            }
            btnEdit.Content = "Edit Info";
          }
          break;
      }
    }

    private void MusicDrop(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
        string[] musicFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

        HandleOpen(musicFiles);
      }
    }

    private void CoverArtDrop(object sender, DragEventArgs e) {

    }

    private void MusicOpen(object sender, RoutedEventArgs e) {
      OpenFileDialog musicDialog = new OpenFileDialog();
      musicDialog.Multiselect = true;
      //TODO: Filter and InitialDirectory, read from config file
      if (musicDialog.ShowDialog() == true) { // opens
        HandleOpen(musicDialog.FileNames);
      }
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

    #region Helper methods
    /// <summary>
    /// Opens each music file by setting up the MusicFile objects.
    /// If thte filename already appears in the HashSet, it means that
    /// </summary>
    /// <param name="filenames">an array of music filenames</param>
    private void HandleOpen(string[] filenames) {
      List<MusicFile> musicList = new List<MusicFile>(filenames.Length);
      foreach (string filename in filenames) {
        /* Check repeated adding */
        if (filenameSet.Contains(filename)) {
          // Logic: find added song in the list, add it to the list to be
          // updated
          foreach (MusicFile musicFile in playlist.Items) {
            if (musicFile.Filename.Equals(filename)) {
              musicFile.UpdateTag();
              musicList.Add(musicFile); // Add it to the update list
              break;
            }
          }

        }
        else {
          filenameSet.Add(filename);
          string extension = System.IO.Path.GetExtension(filename);
          if (extension.Equals(".cue")) {
            // TODO: CUE support (find a library)
          } else {
            if (extension.Equals(".mp3")) {
              musicList.Add(new Mp3(filename, this));
            } else {
              // music.Add(new Flac(filename));
              // TODO: Other formats
            }
          }
        }
      }

      bool canEdit = true;
      if (musicList.Count != 0) {
        foreach (MusicFile file in musicList) {
          playlist.Items.Add(file);
          if (file.ReadOnly == false) {
            canEdit = false;
          }
        }
        playlist.Items.Remove(defaultItem);
      }
      UpdateInfo(musicList);
      if (canEdit) {
        SongOptions.IsEnabled = true;
      }
    }

    private void UpdateInfo(List<MusicFile> music) {
      // Get information from the application
      string[] showTag = new string[8];
      showTag[0] = txtTitle.Text;
      showTag[1] = txtAlbum.Text;
      showTag[2] = txtArtist.Text;
      showTag[3] = txtTrack.Text;
      showTag[4] = txtGenre.Text;
      showTag[5] = txtYear.Text;
      showTag[6] = txtComment.Text;
      showTag[7] = txtLength.Text;

      foreach (MusicFile file in music) {
        string[] tag = file.PassInfo();
        for (int i = 0; i < tag.Length; i++) {
          if (showTag[i] == "") {
            showTag[i] = tag[i];
          } else if (!showTag[i].Equals(tag[i])) {
            showTag[i] = "(Multiple Values)";
          }
        }
      }
      // Display information
      txtTitle.Text = showTag[0];
      txtAlbum.Text = showTag[1];
      txtArtist.Text = showTag[2];
      txtTrack.Text = showTag[3];
      txtGenre.Text = showTag[4];
      txtYear.Text = showTag[5];
      txtComment.Text = showTag[6];
      txtLength.Text = showTag[7];
    }

    private void CleanInfo() {
      foreach (TextBox tagInfo in tagBoxes) {
        tagInfo.Clear();
      }
      foreach (TextBox streamInfo in streamBoxes) {
        streamInfo.Clear();
      }
    }
    #endregion
  }
}