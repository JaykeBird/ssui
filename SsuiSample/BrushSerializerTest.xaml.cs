using SolidShineUi;
using SolidShineUi.PropertyList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace SsuiSample
{
    /// <summary>
    /// Interaction logic for SerializerTest.xaml
    /// </summary>
    public partial class BrushSerializerTest : ThemedUserControl, IPropertyEditorHost
    {
        public BrushSerializerTest()
        {
            InitializeComponent();

            SsuiThemeApplied += SerializerTest_SsuiThemeApplied;
            Loaded += SerializerTest_Loaded;

            //brushEditor.ApplySsuiTheme(SsuiTheme ?? SsuiThemes.SystemTheme);
            brushEditor.SetHostControl(this);
            brushEditor.LoadValue(new SolidColorBrush(Colors.Red), typeof(Brush));
            brushEditor.ValueChanged += BrushEditor_ValueChanged;
            txtOutputBox.Text = BrushSerializer.Serialize(new SolidColorBrush(Colors.Red));
        }

        private void SerializerTest_Loaded(object sender, RoutedEventArgs e)
        {
            brushEditor.ApplySsuiTheme(TryGetSsuiAppTheme());
        }

        private void SerializerTest_SsuiThemeApplied(object sender, RoutedEventArgs e)
        {
            //brushEditor.ApplySsuiTheme(SsuiTheme ?? SsuiThemes.SystemTheme);
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

        private void BrushEditor_ValueChanged(object sender, EventArgs e)
        {
            if (brushEditor.GetValue() is Brush b)
            {
                txtOutputBox.Text = BrushSerializer.Serialize(b);
            }
        }

        public IPropertyEditor CreateEditorForType(Type propType)
        {
            return null;
        }

        public object GetCurrentlyLoadedObject()
        {
            return this;
        }

        public SsuiAppTheme GetThemeForDialogs()
        {
            return TryGetSsuiAppTheme();
        }

        public Window GetWindow()
        {
            return Window.GetWindow(this);
        }

        private void btnCopy1_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOutputBox.Text);
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                txtInputBox.Text = Clipboard.GetText();
            }
        }

        private void txtInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtInputBox.Text;

            if (string.IsNullOrEmpty(text)) return;

            try
            {
                Brush b = BrushSerializer.DeserializeBrush(text);

                brdr.Background = b;
                txtBrushOutput.Text = XamlSerialize(b);
            }
            catch (FormatException ex)
            {
                brdr.Background = null;
                txtBrushOutput.Text = "<!-- Invalid brush format: " + ex.Message + " -->";
            }
        }

        /// <summary>
        /// Serializes the specified object
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>The object serialized to XAML</returns>
        private string XamlSerialize(object obj)
        {
            // from https://stackoverflow.com/a/4488566/2987285

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(obj, manager);

            return sb.ToString();
        }
    }
}
