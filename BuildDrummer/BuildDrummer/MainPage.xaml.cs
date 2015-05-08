using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Build Drummer AudioGraph Demo
// 
// Note: At the build timeframe, there is a known bug in AudioGraph which causes it to lose the attack portion of samples
// on some configurations. This will be fixed in a future build of Windows 10.
//

namespace BuildDrummer
{

    public sealed partial class MainPage : Page
    {
        const int numSamples = 9;

        enum Samples
        {
            BassDrum,
            ClosedHighHat,
            CrashCymbal,
            FloorTom,
            HighTom,
            MidTom,
            Snare,
            OpenHighHat,
            Steve
        }

        AudioGraph _graph;
        AudioFileInputNode[] _fileNodes = new AudioFileInputNode[numSamples];
        AudioDeviceOutputNode _deviceOutput;

        // create the audio graph and output
        private async void InitAudioGraph()
        {
            var settings = new AudioGraphSettings(AudioRenderCategory.Media);
            settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency; // pick lowest latency available to devices in the graph


            // create the audio graph
            _graph = (await AudioGraph.CreateAsync(settings)).Graph;
            if (_graph == null)
            {
                // failed to create audio graph
                MessageDialog dlg = new MessageDialog("Failed to create audio graph");
                await dlg.ShowAsync();
                return;
            }


            // create the output. You could also create file output here to stream to a temp file or similar
            _deviceOutput = (await _graph.CreateDeviceOutputNodeAsync()).DeviceOutputNode;
            if (_deviceOutput == null)
            {
                // failed to create audio output
                MessageDialog dlg = new MessageDialog("Failed to create device output");
                await dlg.ShowAsync();
                return;
            }


            // load all of the samples into graph nodes
            BuildFileNodes();

            // start playback
            _graph.Start();
        }


        // play a file node (a single sample)
        private void PlayFile(Samples sample)
        {
            // closed high hat should always stop an open high hat
            // so we cut off the open high hat if we're playing the closed one
            // Classic drum machines do this by putting both on the same channel
            if (sample == Samples.ClosedHighHat)
                _fileNodes[(int)Samples.OpenHighHat].Stop();

            // Make sure to reset and start up the node
            _fileNodes[(int)sample].Reset();
            _fileNodes[(int)sample].Start();

            BlamePete();
        }





        // load the samples into file nodes. This is very easy with AudioGraph compared to what
        // we had to do with WASAPI and similar.
        private async void BuildFileNodes()
        {
            // Init all the file input nodes
            Uri[] uris = new Uri[numSamples];

            uris[(int)Samples.Steve] = new Uri("ms-appx:///Assets/guggs_switch_gears.wav", UriKind.Absolute);
            uris[(int)Samples.BassDrum] = new Uri("ms-appx:///Assets/Drum-Bass.wav", UriKind.Absolute);
            uris[(int)Samples.ClosedHighHat] = new Uri("ms-appx:///Assets/Drum-Closed-Hi-Hat.wav", UriKind.Absolute);
            uris[(int)Samples.CrashCymbal] = new Uri("ms-appx:///Assets/Drum-Crash-Cymbal.wav", UriKind.Absolute);
            uris[(int)Samples.FloorTom] = new Uri("ms-appx:///Assets/Drum-Floor-Tom.wav", UriKind.Absolute);
            uris[(int)Samples.HighTom] = new Uri("ms-appx:///Assets/Drum-High-Tom.wav", UriKind.Absolute);
            uris[(int)Samples.MidTom] = new Uri("ms-appx:///Assets/Drum-Mid-Tom.wav", UriKind.Absolute);
            uris[(int)Samples.Snare] = new Uri("ms-appx:///Assets/Drum-Snare.wav", UriKind.Absolute);
            uris[(int)Samples.OpenHighHat] = new Uri("ms-appx:///Assets/Drum-Open-Hi-Hat.wav", UriKind.Absolute);

            // create a node for each uri
            for (int i = 0; i < numSamples; i++)
            {
                StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(uris[i]);
                var res = await _graph.CreateFileInputNodeAsync(f);

                if (res.Status != AudioFileNodeCreationStatus.Success)
                {
                    // Give up on this one if the node didn't init properly
                    continue;
                }

                _fileNodes[i] = res.FileInputNode;
                _fileNodes[i].FileCompleted += MainPage_FileCompleted;
                
                // By default, nodes are started. We want to have the nodes start off in the stop state.
                _fileNodes[i].Stop();
                _fileNodes[i].AddOutgoingConnection(_deviceOutput);

            }
        }





        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitAudioGraph();
        }

        private void MainPage_FileCompleted(AudioFileInputNode sender, object args)
        {
            sender.Stop();
        }


        // Show Guggs easter egg
        private bool _isEgged;
        private void BlamePete()
        {
            if (!_isEgged)
            {
                _isEgged = true;

                ((Storyboard)GuggsGroup.Resources["ShowGuggs"]).Begin();
            }
        }


        private void BassDrum_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.BassDrum);
        
        }

        private void TomLow_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.FloorTom);
        }

        private void TomMid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.MidTom);
        }

        private void TomHigh_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.HighTom);
        }

        private void Snare_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.Snare);
        }

        private void CrashCymbal_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.CrashCymbal);
        }

        private void HighHatOpen_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.OpenHighHat);
        }

        private void HighHatClosed_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.ClosedHighHat);
        }

        private void Steve_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PlayFile(Samples.Steve);
        }

    }

}
