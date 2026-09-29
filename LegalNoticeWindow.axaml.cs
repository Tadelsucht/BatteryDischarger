using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.IO;

namespace BatteryDischarger
{
    // Displays the bundled legal notice in a dedicated Avalonia window.
    public partial class LegalNoticeWindow : Window
    {
        // Loads the bundled notice text and its window icon after the XAML controls are initialized.
        public LegalNoticeWindow()
        {
            // GUI
            InitializeComponent();

            // Icon https://github.com/AvaloniaUI/Avalonia/issues/4488
            var iconStream = AssetLoader.Open(new Uri("avares://BatteryDischarger/Assets/BatteryDischarger.ico"));
            this.Icon = new WindowIcon(iconStream);

            // Text
            var textStream = new StreamReader(AssetLoader.Open(new Uri("avares://BatteryDischarger/Assets/LegalNotice.txt")));
            var text = textStream.ReadToEnd();
            tbLegalText.Text = text;
        }
    }
}
