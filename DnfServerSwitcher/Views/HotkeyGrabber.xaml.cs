using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DnfServerSwitcher.Models;
namespace DnfServerSwitcher.Views {
    
    public partial class HotkeyGrabber : UserControl {
        
        public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(nameof(Hotkey),
            typeof(Key),
            typeof(HotkeyGrabber),
            new FrameworkPropertyMetadata(Key.None, HotkeyChanged)
            );
        
        public static readonly DependencyProperty HotkeyDisplayTextProperty = DependencyProperty.Register(nameof(HotkeyDisplayText),
            typeof(string),
            typeof(HotkeyGrabber),
            new FrameworkPropertyMetadata("None")
            );
        public static readonly DependencyProperty HotkeyDisplayTooltipProperty = DependencyProperty.Register(nameof(HotkeyDisplayTooltip),
            typeof(string),
            typeof(HotkeyGrabber),
            new FrameworkPropertyMetadata("None")
            );

        public Key Hotkey {
            get => (Key)this.GetValue(HotkeyGrabber.HotkeyProperty);
            set => this.SetValue(HotkeyGrabber.HotkeyProperty, value);
        }

        public string HotkeyDisplayText {
            get => (string)this.GetValue(HotkeyGrabber.HotkeyDisplayTextProperty);
            private set => this.SetValue(HotkeyGrabber.HotkeyDisplayTextProperty, value);
        }
        
        public string HotkeyDisplayTooltip {
            get => (string)this.GetValue(HotkeyGrabber.HotkeyDisplayTooltipProperty);
            private set => this.SetValue(HotkeyGrabber.HotkeyDisplayTooltipProperty, value);
        }
        
        public HotkeyGrabber() {
            InitializeComponent();
        }

        private static void HotkeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is not HotkeyGrabber hg) return;
            hg.HotkeyDisplayText = DnfHotkeyHelper.GetKeyIniString(hg.Hotkey);
            hg.HotkeyDisplayTooltip = DnfHotkeyHelper.GetKeyVirtualKeyName(hg.Hotkey);
        }
        
        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            // do not bind to TAB, instead allow it to perform normal GUI operation
            if (e.Key == Key.Tab) return;
            
            // allow user to control + tab out of the GUI control...
            // if (e.Key == Key.Tab && Keyboard.IsKeyDown(Key.LeftCtrl))
            // return;
            
            e.Handled = true;
        }
        private void UIElement_OnPreviewKeyUp(object sender, KeyEventArgs e) {
            // do not bind to TAB, instead allow it to perform normal GUI operation
            if (e.Key == Key.Tab) return;
            
            // allow user to control + tab out of the GUI control
            // if (e.Key == Key.Tab && Keyboard.IsKeyDown(Key.LeftCtrl))
            //     return;
            
            if (e.SystemKey == Key.F10) {
                // special handling for F10, since it's used by WPF...
                this.Hotkey = e.SystemKey;
            } else {
                this.Hotkey = e.Key;
            }
            e.Handled = true;
        }
    }
}

