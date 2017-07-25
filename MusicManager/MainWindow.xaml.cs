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
    //TODO: Convert to enums
    /// <summary>
    /// Read Type Priority as specified in the ComboBox. Default to 0.
    /// 0: APEv2 > ID3v2 > ID3v1
    /// 1: ID3v2 > APEv2 > ID3v1
    /// 2: ID3v1 > ID3v2 > APEv2
    /// 3: ID3v1 > APEv2 > ID3v2
    /// </summary>
    public int readPriority = 0;

    /// <summary>
    /// Encoding for ID3v2 tag. Default to 0.
    /// 0: ISO-8859-1, a.k.a., Latin-1
    /// 1: UTF-8
    /// 2: UTF-16
    /// </summary>
    public int encoding = 0;

    /// <summary>
    /// Write type for MP3 files. Default to 3.
    /// 0: ID3v1
		///	1: ID3v2
		///	2: APEv2
		///	3: ID3v1 & ID3v2
		///	4: ID3v1 & APEv2
		/// 5: ID3v2 & APEv2
    /// </summary>
    public int write = 3;
    
    /// <summary>
    /// Data padding when writing file. Default to true.
    /// </summary>
    public bool padding = true;
    #endregion

    public MainWindow() {
      InitializeComponent();
    }

    #region Events
    public void ButtonClicked(object sender, RoutedEventArgs e) {
      Button button = sender as Button;
      switch (button.Name) {
        case "Convert":
          Conversion conversionWindow = new Conversion();
          conversionWindow.Show();
          break;

        case "More":
          MoreInfo moreInfoWindow = new MoreInfo();
          moreInfoWindow.Show();
          break;

        case "Delete":
          Playlist.Items.Remove(Playlist.SelectedItems);
          if (Playlist.Items.IsEmpty) {
            Playlist.Items.Add(DefaultItem);
            SongOptions.IsEnabled = true;
          }
          break;

        case "Edit":
          if (Edit.Content.Equals("Edit Info")) {
            Title.IsEnabled = true;
            Album.IsEnabled = true;
            Artist.IsEnabled = true;
            Track.IsEnabled = true;
            Genre.IsEnabled = true;
            Year.IsEnabled = true;
            Comment.IsEnabled = true;
            Edit.Content = "Save Info";
          } else {
            Title.IsEnabled = false;
            Album.IsEnabled = false;
            Artist.IsEnabled = false;
            Track.IsEnabled = false;
            Genre.IsEnabled = false;
            Year.IsEnabled = false;
            Comment.IsEnabled = false;
            Edit.Content = "Edit Info";
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
        //TODO: load cover art
      }
    }

    /// <summary>
    /// Sets padding to true.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckPadding(object sender, RoutedEventArgs e) {
      padding = true;
    }

    /// <summary>
    /// Sets padding to false.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UncheckPadding(object sender, RoutedEventArgs e) {
      padding = false;
    }

    private void PriorityChanged(object sender, SelectionChangedEventArgs e) {
      ComboBox box = sender as ComboBox;
      readPriority = box.SelectedIndex;
    }

    private void EncodingChanged(object sender, SelectionChangedEventArgs e) {
      ComboBox box = sender as ComboBox;
      encoding = box.SelectedIndex;
    }

    private void WriteChanged(object sender, SelectionChangedEventArgs e) {
      ComboBox box = sender as ComboBox;
      write = box.SelectedIndex;
    }
    #endregion

    #region Helper methods
    private void HandleOpen(string[] filenames) {
      // Add music to the playlist
      foreach (string filename in filenames) {
        string extension = System.IO.Path.GetExtension(filename);
        if (extension.Equals(".cue")) {
        } else {
          MusicFile music = null;
          if (extension.Equals(".mp3")) {
            music = new Mp3(filename, readPriority);
          } else {
            //music = new Flac(filename);
            // TODO: Other formats
          }

          //TODO: Check that music is already in the list
          if (music != null) {
            Playlist.Items.Add(music);
          }
          Playlist.Items.Remove(DefaultItem);
        }
      }

      // Display information
      string[] showTag = new string[8];
      for (int i = 0; i < showTag.Length; i++) {
        showTag[i] = "";
      }
      foreach (MusicFile music in Playlist.Items) {
        string[] tag = music.PassInfo();
        for (int i = 0; i < tag.Length; i++) {
          if (showTag[i] == "") {
            showTag[i] = tag[i];
          } else if (!showTag[i].Equals(tag[i])) {
            showTag[i] = "(Multiple Values)";
          }
        }
      }

      Title.Text = showTag[0];
      Album.Text = showTag[1];
      Artist.Text = showTag[2];
      Track.Text = showTag[3];
      Genre.Text = showTag[4];
      Year.Text = showTag[5];
      Comment.Text = showTag[6];
      Length.Text = showTag[7];

      SongOptions.IsEnabled = true;
    }
    #endregion
  }
}