using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.IO;

namespace BatteryDischarger
{
    public partial class LegalNoticeWindow : Window
    {
        public LegalNoticeWindow()
        {
            // GUI
            InitializeComponent();

            // Icon https://github.com/AvaloniaUI/Avalonia/issues/4488
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var iconStream = assets.Open(new Uri("avares://BatteryDischarger/Assets/BatteryDischarger.ico"));
            this.Icon = new WindowIcon(iconStream);

            // Text
            var textStream = new StreamReader(assets.Open(new Uri("avares://BatteryDischarger/Assets/LegalNotice.txt")));
            var text = textStream.ReadToEnd();
            tbLegalText.Text = text;
        }
    }
}