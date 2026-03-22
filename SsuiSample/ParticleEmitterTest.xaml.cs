using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SolidShineUi;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for ParticleEmitterTest.xaml
    /// </summary>
    public partial class ParticleEmitterTest : ThemedUserControl
    {
        public ParticleEmitterTest()
        {
            InitializeComponent();

            Background = Colors.White.ToBrush();

            SsuiThemeChanged += control_SsuiThemeChanged;
            Loaded += control_Loaded;
            Unloaded += control_Closing;
        }

        private void control_SsuiThemeChanged(object sender, RoutedEventArgs e)
        {
            Brush b = SsuiTheme.BaseBackground.CloneCurrentValue();
            b.Opacity = 0.5;
            toolbar.Background = b;
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            pl.LoadObject(emitter);
            pl.ShowInheritedProperties = false;
        }

        private void control_Closing(object sender, RoutedEventArgs e)
        {
            emitter.Shutdown();
        }

        bool hasStarted = false;

        void Start()
        {
            if (!hasStarted)
            {
                emitter.Start();
                hasStarted = true;
            }
            else
            {
                emitter.Resume();
            }
        }

        void Pause()
        {
            emitter.Pause();
        }

        void Clear()
        {
            emitter.Clear();
        }

        void ShowHideSettings()
        {
            if (brdrSettings.Visibility == Visibility.Visible)
            {
                brdrSettings.Visibility = Visibility.Collapsed;
                btnSettings.IsSelected = false;
            }
            else
            {
                brdrSettings.Visibility = Visibility.Visible;
                pl.ReloadObject();
                pl.ShowInheritedProperties = false;
                btnSettings.IsSelected = true;
            }
        }

        SsuiAppTheme TryGetSsuiAppTheme()
        {
            if (SsuiTheme is SsuiAppTheme sat)
            {
                // in most cases, it should be this - the inherited SsuiTheme should be an SsuiAppTheme
                return sat;
            }
            else if (Window.GetWindow(this) is ThemedWindow fw)
            {
                // okay, let's try to pull from the parent window if possible, as it should have a SsuiAppTheme as its theme
                return fw.SsuiTheme;
            }
            else
            {
                // okay, I guess we'll just go with the default
                return new SsuiAppTheme();
            }
        }

        void ChangeBackground()
        {
            Color windowCol = Colors.White;
            if (Background is SolidColorBrush scb)
            {
                windowCol = scb.Color;
            }
            ColorPickerDialog cpd = new ColorPickerDialog(windowCol)
            {
                SsuiTheme = TryGetSsuiAppTheme(),
                Owner = Window.GetWindow(this),
            };

            cpd.ShowDialog();
            if (cpd.DialogResult)
            {
                Background = new SolidColorBrush(cpd.SelectedColor);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowHideSettings();
        }

        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
        }
    }
}
