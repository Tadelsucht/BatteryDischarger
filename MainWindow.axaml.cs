using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using BatteryDischarger.BatteryRelated;
using BatteryDischarger.DataSource;
using BatteryDischarger.Miscellaneous;
using BatteryDischarger.PlatformSpecificActions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryDischarger
{
    public partial class MainWindow : Window
    {
        private BatteryInfoManager batteryManager;
        private BatteryWaster batteryWaster;
        private object StartStopLock = new object();
        private static Task ControlledDischargeTask = null;

        public MainWindow()
        {
            // Helper
            batteryManager = new BatteryInfoManager();
            batteryWaster = new BatteryWaster();

            // GUI
            InitializeComponent();
            StaticHelperCore.TryCatchIgnore(() => { tbVersion.Text = StaticHelperCore.Version; });

            // Icon https://github.com/AvaloniaUI/Avalonia/issues/4488
            StaticHelperCore.TryCatchIgnore(() =>
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var iconStream = assets.Open(new Uri("avares://BatteryDischarger/Assets/BatteryDischarger.ico"));
                this.Icon = new WindowIcon(iconStream);
            });

            // Generell events
            bStartControlledDischarge.Click += BStartControlledDischarge_Click;
            bStopControlledDischarge.Click += BStopControlledDischarge_Click;
            sTargetBatteryChargeInPercent.PropertyChanged += STargetBatteryChargeInPercent_PropertyChanged;
            tbTargetBatteryChargeInPercent.PropertyChanged += TbTargetBatteryChargeInPercent_PropertyChanged;
            cbAccelerateBatteryDischarge.PropertyChanged += CbAccelerateBatteryDischarge_PropertyChanged;
            cbPreventUnwantedSystemSleepMode.PropertyChanged += CbPreventUnwantedSystemSleepMode_PropertyChanged;
            bLegalNotice.Click += BLegalNotice_Click;

            // EndAction
            List<EndActionEnumNamedContainer> useableEndActions = new List<EndActionEnumNamedContainer>();
            foreach (EndActionEnum entry in PlatformSpecificActionsManager.PlatformSpecificEndActions.GetSupportedEndActions()) useableEndActions.Add(new EndActionEnumNamedContainer(entry));
            cbActionAtTheSelectedBatteryChargeLevel.Items = useableEndActions;
            foreach (var entry in cbActionAtTheSelectedBatteryChargeLevel.Items)
            {
                if (((EndActionEnumNamedContainer)entry).EmbeddedEnum == IniConfiguration.Instance.EndAction)
                {
                    cbActionAtTheSelectedBatteryChargeLevel.SelectedItem = entry;
                    break;
                }
            }
            cbActionAtTheSelectedBatteryChargeLevel.SelectionChanged += CbActionAtTheSelectedBatteryChargeLevel_SelectionChanged;

            // Language
            cbLanguage.Items = LanguageHandler.GetLanguages();
            foreach (var entry in cbLanguage.Items)
            {
                if (entry.ToString().Substring(0, 2) == IniConfiguration.Instance.Language)
                {
                    cbLanguage.SelectedItem = entry;
                    break;
                }
            }
            cbLanguage.SelectionChanged += CbLanguage_SelectionChanged;

            // Load config
            tbTargetBatteryChargeInPercent.Text = IniConfiguration.Instance.TargetBatteryChargeInPercent.ToString();
            cbAccelerateBatteryDischarge.IsChecked = IniConfiguration.Instance.AccelerateBatteryDischarge;
            cbPreventUnwantedSystemSleepMode.IsChecked = IniConfiguration.Instance.PreventUnwantedSystemSleepMode;

            // Battery info
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var x = batteryManager.GetEstimatedChargeRemaining().ToString();
                        Dispatcher.UIThread.Post(() =>
                        {
                            try { tbCurrentBatteryChargeInPercent.Text = x; } catch { }
                        }, DispatcherPriority.MaxValue);
                    }
                    catch { }

                    Thread.Sleep(1000);
                }
            });

            // Autostart?
            if (Program.Autostart)
            {
                BStartControlledDischarge_Click(null, null);
            }
        }

        private void CbPreventUnwantedSystemSleepMode_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (cbPreventUnwantedSystemSleepMode.IsChecked.HasValue) IniConfiguration.Instance.PreventUnwantedSystemSleepMode = cbPreventUnwantedSystemSleepMode.IsChecked.Value;
        }

        private void CbLanguage_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (cbLanguage.SelectedItem is not null)
            {
                var languageCode = cbLanguage.SelectedItem.ToString().Substring(0, 2);
                if (IniConfiguration.Instance.Language != languageCode)
                {
                    IniConfiguration.Instance.Language = languageCode;
                    Thread.Sleep(1000);

                    // Restart
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                    Close();
                }
            }
        }

        private void CbActionAtTheSelectedBatteryChargeLevel_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (cbActionAtTheSelectedBatteryChargeLevel.SelectedItem is not null)
            {
                var endAction = ((EndActionEnumNamedContainer)cbActionAtTheSelectedBatteryChargeLevel.SelectedItem).EmbeddedEnum;
                IniConfiguration.Instance.EndAction = endAction;
            }
        }

        private void BLegalNotice_Click(object? sender, RoutedEventArgs e)
        {
            StaticHelperCore.TryCatchShowErrorMessageBox(() =>
            {
                var window = new LegalNoticeWindow();
                window.ShowDialog(this);
            });
        }

        private void CbAccelerateBatteryDischarge_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if (cbAccelerateBatteryDischarge.IsChecked.HasValue) IniConfiguration.Instance.AccelerateBatteryDischarge = cbAccelerateBatteryDischarge.IsChecked.Value;
        }

        private void BStopControlledDischarge_Click(object? sender, RoutedEventArgs e)
        {
            lock (StartStopLock)
            {
                if (bStopControlledDischarge.IsVisible)
                {
                    // Do
                    StaticHelperCore.TryCatchIgnore(() => { ControlledDischargeTask = null; });
                    StaticHelperCore.TryCatchIgnore(() => { batteryWaster.Stop(); });

                    // Change button
                    ToggleStartStop();

                    // Reset gui
                    tbNumberOfMinutesUntilTheSelectedBatteryLevelIsReached.Text = "-";
                }
            }
        }

        private void ToggleStartStop()
        {
            lock (StartStopLock)
            {
                // Visible
                bStartControlledDischarge.IsVisible = !bStartControlledDischarge.IsVisible;
                bStopControlledDischarge.IsVisible = !bStopControlledDischarge.IsVisible;

                // Enabled
                tbTargetBatteryChargeInPercent.IsEnabled = bStartControlledDischarge.IsVisible;
                sTargetBatteryChargeInPercent.IsEnabled = bStartControlledDischarge.IsVisible;
                cbAccelerateBatteryDischarge.IsEnabled = bStartControlledDischarge.IsVisible;
                cbPreventUnwantedSystemSleepMode.IsEnabled = bStartControlledDischarge.IsVisible;

                // KeepSystemAwakeService
                if (IniConfiguration.Instance.PreventUnwantedSystemSleepMode)
                {
                    StaticHelperCore.TryCatchIgnore(() =>
                    {
                        KeepSystemAwakeService.KeepSystemAwake = bStopControlledDischarge.IsVisible;
                    });
                }
            }
        }

        private void TbTargetBatteryChargeInPercent_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text, out int result))
            {
                if (result >= 0 && result <= 100)
                {
                    sTargetBatteryChargeInPercent.Value = result;
                    IniConfiguration.Instance.TargetBatteryChargeInPercent = result;
                }
            }
            else
            {
                ((TextBox)sender).Text = "0";
            }
        }

        private void BStartControlledDischarge_Click(object? sender, RoutedEventArgs e)
        {
            lock (StartStopLock)
            {
                // If Battery is zero
                StaticHelperCore.TryCatchIgnore(() =>
                {
                    if (batteryManager.GetEstimatedChargeRemaining() == 0)
                    {
                        var messageBox = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(Properties.Resources.Error, Properties.Resources.NoBatteryWasDetected, icon: MessageBox.Avalonia.Enums.Icon.Error);
                        messageBox.ShowDialog(this);
                        return;
                    }
                });

                // Do
                if (bStartControlledDischarge.IsVisible)
                {
                    // Check IsBatteryDischaring
                    try
                    {
                        if (!batteryManager.IsBatteryDischaring())
                        {
                            var messageBox = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(Properties.Resources.Information, Properties.Resources.TheDeviceMayStillBeChargingOrTheBatteryStatusCouldNotBeDetermined, icon: MessageBox.Avalonia.Enums.Icon.Info);
                            messageBox.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticHelperCore.GetErrorMessageBox(ex).ShowDialog(this);
                        return;
                    }

                    // Waste
                    if (cbAccelerateBatteryDischarge.IsChecked == true)
                    {
                        StaticHelperCore.TryCatchShowErrorMessageBox(() =>
                        {
                            batteryWaster.Reset();
                            batteryWaster.StartWasting();
                        });
                    }

                    // Start process which shutdown the pc if battery have been discharged enough
                    ControlledDischargeTask = new Task(() =>
                    {
                        List<int> estimatedTimeRingBuffer = new List<int>();
                        int estimatedTimeRingBufferIndex = 0;
                        const int estimatedTimeRingBufferMax = 128;

                        while (ControlledDischargeTask is not null)
                        {
                            StaticHelperCore.TryCatchIgnore(() =>
                            {
                                if (batteryManager.GetEstimatedChargeRemaining() <= IniConfiguration.Instance.TargetBatteryChargeInPercent)
                                {
                                    PlatformSpecificActionsManager.TryExecuteEndAction(IniConfiguration.Instance.EndAction);
                                    Thread.Sleep(10000);
                                    ControlledDischargeTask = null;
                                    Dispatcher.UIThread.Post(() =>
                                    {
                                        ToggleStartStop();
                                        Close();
                                    }, DispatcherPriority.MaxValue);
                                }
                                Dispatcher.UIThread.Post(() =>
                                {
                                    try
                                    {
                                        var estimatedTime = batteryManager.GetEstimatedTimeLeftUntilGivenPerctageHasBeenReached(IniConfiguration.Instance.TargetBatteryChargeInPercent);
                                        if (estimatedTimeRingBuffer.Count < estimatedTimeRingBufferIndex + 1)
                                        {
                                            estimatedTimeRingBuffer.Add(estimatedTime);
                                        }
                                        else
                                        {
                                            estimatedTimeRingBuffer[estimatedTimeRingBufferIndex] = estimatedTime;
                                        }
                                        estimatedTimeRingBufferIndex++;
                                        if (estimatedTimeRingBufferIndex >= estimatedTimeRingBufferMax) estimatedTimeRingBufferIndex = 0;

                                        var calculatedTime = (int)(estimatedTimeRingBuffer.ToList().Sum() / estimatedTimeRingBuffer.Count);
                                        tbNumberOfMinutesUntilTheSelectedBatteryLevelIsReached.Text = "≈" + calculatedTime.ToString();
                                    }
                                    catch { }
                                }, DispatcherPriority.MaxValue);
                            });

                            Thread.Sleep(1000);
                        }
                    });
                    ControlledDischargeTask.Start();

                    // Change button
                    ToggleStartStop();
                }
            }
        }

        private void STargetBatteryChargeInPercent_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            var value = ((int)((Slider)sender).Value).ToString();
            if (value != tbTargetBatteryChargeInPercent.Text)
            {
                tbTargetBatteryChargeInPercent.Text = value;
            }
        }
    }
}