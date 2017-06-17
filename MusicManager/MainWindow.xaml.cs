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

namespace MusicManager {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    /* Main player */
    private WaveOutEvent playController = new WaveOutEvent();

    public MainWindow() {
      InitializeComponent();
    }

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
      }
    }
  }
}
