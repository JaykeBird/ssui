﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidShineUi
{
    /// <summary>
    /// A dialog for the user to select a font, as well as being able to choose a font size and styling if desired.
    /// </summary>
    public partial class FontSelectDialog : FlatWindow
    {
        public FontSelectDialog()
        {
            InitializeComponent();
            AutoCompleteBehavior.SetAutoCompleteItemsSource(txtFont, FontFamiliesByString());
        }

        private FontFamily _fam = new FontFamily("Segoe UI");
        private FontWeight _weight;
        private double _size;
        private FontStyle _style;
        private TextDecorationCollection _dec = new TextDecorationCollection();
        private List<string> _fallbackList = new List<string>();

        public new bool DialogResult { get; set; } = false;

        /// <summary>
        /// If a font fallback sequence is defined in the FontFamily, preserve it even if the user changes the font. If preserved, only the first found font will be changed and the remaining values will be preserved.
        /// </summary>
        public bool PreserveFontFallback { get; set; } = true;

        #region Selection Properties

        public FontFamily SelectedFontFamily
        {
            get
            {
#if NETCOREAPP
                string font = (lstFont.SelectedItem as FontFamily)!.Source;
#else
                string font = (lstFont.SelectedItem as FontFamily).Source;
#endif

                FontFamily ff = new FontFamily(font);
                if (PreserveFontFallback && _fallbackList.Count > 0)
                {
                    ff = new FontFamily(font + ", " + string.Join(", ", _fallbackList));
                }

                return ff;
            }
            set
            {
                FontFamily v = value;
                if (v.Source.Contains(","))
                {
                    // custom font fallback sequence found
                    // enumerate fonts and find first one that works
                    // all remaining ones, put into a fallback list
                    string[] l = v.Source.Split(',');

                    bool found = false;
                    _fallbackList = new List<string>();

                    foreach (string f in l)
                    {
                        if (found)
                        {
                            _fallbackList.Add(f.Trim());
                            continue;
                        }

                        if (lstFont.Items.Contains(new FontFamily(f.Trim())))
                        {
                            found = true;
                            FontFamily ff = new FontFamily(f.Trim());
                            _fam = ff;
                            lstFont.SelectedItem = ff;
                            lstFont.ScrollIntoView(ff);
                        }
                    }
                }
                else
                {
                    _fam = value;
                    lstFont.SelectedItem = value;
                    lstFont.ScrollIntoView(value);
                }
            }
        }

        public FontWeight SelectedFontWeight
        {
            get
            {
                return (FontWeight)lstWeight.SelectedItem;
            }
            set
            {
                _weight = value;
                lstWeight.SelectedItem = value;
            }
        }

        public double SelectedFontSize
        {
            get
            {
                return txtSize.Value;
            }
            set
            {
                _size = value;
                txtSize.Value = value;
            }
        }

        public FontStyle SelectedFontStyle
        {
            get
            {
                if (chkItalic.IsChecked.GetValueOrDefault(false))
                {
                    return FontStyles.Italic;
                }
                else if (chkOblique.IsChecked.GetValueOrDefault(false))
                {
                    return FontStyles.Oblique;
                }
                else
                {
                    return FontStyles.Normal;
                }
            }
            set
            {
                _style = value;

                if (value == FontStyles.Italic)
                {
                    chkItalic.IsChecked = true;
                }
                else if (value == FontStyles.Oblique)
                {
                    chkOblique.IsChecked = true;
                }
                else
                {
                    chkItalic.IsChecked = false;
                    chkOblique.IsChecked = false;
                }
            }
        }

        public TextDecorationCollection SelectedTextDecorations
        {
            get
            {
                TextDecorationCollection tdc = new TextDecorationCollection();

                if (chkUnderline.IsChecked.GetValueOrDefault(false)) tdc.Add(TextDecorations.Underline);
                if (chkBaseline.IsChecked.GetValueOrDefault(false)) tdc.Add(TextDecorations.Baseline);
                if (chkStrike.IsChecked.GetValueOrDefault(false)) tdc.Add(TextDecorations.Strikethrough);
                if (chkOverline.IsChecked.GetValueOrDefault(false)) tdc.Add(TextDecorations.OverLine);

                return tdc;
            }
            set
            {
                _dec = value;

                foreach (TextDecoration item in value)
                {
                    if (TextDecorations.Underline.Contains(item))
                    {
                        chkUnderline.IsChecked = true;
                    }
                    if (TextDecorations.Baseline.Contains(item))
                    {
                        chkBaseline.IsChecked = true;
                    }
                    if (TextDecorations.Strikethrough.Contains(item))
                    {
                        chkStrike.IsChecked = true;
                    }
                    if (TextDecorations.OverLine.Contains(item))
                    {
                        chkOverline.IsChecked = true;
                    }
                }
            }
        }

        #endregion

        #region Visibility Properties

        BooleanToVisibilityConverter btv = new BooleanToVisibilityConverter();

        public bool ShowStyles
        {
            get
            {
                return chkItalic.Visibility == Visibility.Visible;
            }
            set
            {
                chkItalic.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                chkOblique.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);

                if (value)
                {
                    if (ShowDecorations)
                    {
                        lblStyles.Text = "Styles/Decorations:";
                        RealignStylesAndDecorations();
                    }
                    else
                    {
                        lblStyles.Text = "Styles:";
                        RealignStylesOnly();
                    }
                }
                else
                {
                    if (ShowDecorations)
                    {
                        lblStyles.Text = "Decorations:";
                        RealignDecorationsOnly();
                    }
                    else
                    {
                        lblStyles.Text = "";
                        RealignStylesAndDecorations();
                    }
                }
            }
        }

        public bool ShowDecorations
        {
            get
            {
                return chkUnderline.Visibility == Visibility.Visible;
            }
            set
            {
                chkUnderline.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                chkOverline.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                chkStrike.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                chkBaseline.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);

                if (value)
                {
                    if (ShowStyles)
                    {
                        lblStyles.Text = "Styles/Decorations:";
                        RealignStylesAndDecorations();
                    }
                    else
                    {
                        lblStyles.Text = "Decorations:";
                        RealignDecorationsOnly();
                    }
                }
                else
                {
                    if (ShowStyles)
                    {
                        lblStyles.Text = "Styles:";
                        RealignStylesOnly();
                    }
                    else
                    {
                        lblStyles.Text = "";
                        RealignStylesAndDecorations();
                    }
                }
            }
        }

        public bool ShowWeights
        {
            get
            {
                return lstWeight.Visibility == Visibility.Visible;
            }
            set
            {
                lstWeight.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                lblWeight.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
            }
        }

        public bool ShowSizes
        {
            get
            {
                return lstSize.Visibility == Visibility.Visible;
            }
            set
            {
                txtSize.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                lstSize.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
                lblSize.Visibility = (Visibility)btv.Convert(value, typeof(Visibility), null, null);
            }
        }

        void RealignStylesOnly()
        {
            chkItalic.Margin = new Thickness(25, 275, 0, 0);
            chkOblique.Margin = new Thickness(25, 295, 0, 0);
        }

        void RealignDecorationsOnly()
        {
            chkBaseline.Margin = new Thickness(135, 275, 0, 0);
        }

        void RealignStylesAndDecorations()
        {
            chkItalic.Margin = new Thickness(135, 275, 0, 0);
            chkOblique.Margin = new Thickness(135, 295, 0, 0);

            chkBaseline.Margin = new Thickness(135, 315, 0, 0);
        }

        #endregion

        private static List<string> FontFamiliesByString()
        {
            List<string> ls = new List<string>();

            foreach (FontFamily item in Fonts.SystemFontFamilies)
            {
                ls.Add(item.Source);
            }

            return ls;
        }

        public static List<FontWeight> AvailableFontWeights = new List<FontWeight>()
        {
            FontWeights.Thin,
            FontWeights.ExtraLight,
            FontWeights.Light,
            FontWeights.Normal,
            FontWeights.Medium,
            FontWeights.SemiBold,
            FontWeights.Bold,
            FontWeights.ExtraBold,
            FontWeights.Black,
            FontWeights.ExtraBlack,

            /*
            FontWeights.Black,
            FontWeights.Bold,
            //FontWeights.DemiBold,
            FontWeights.ExtraBlack,
            FontWeights.ExtraBold,
            FontWeights.ExtraLight,
            //FontWeights.Heavy,
            FontWeights.Light,
            FontWeights.Medium,
            FontWeights.Normal,
            //FontWeights.Regular,
            FontWeights.SemiBold,
            FontWeights.Thin,
            //FontWeights.UltraBlack,
            //FontWeights.UltraBold,
            //FontWeights.UltraLight */
        };

        bool listBoxUpdate = false;

        private void txtFont_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!listBoxUpdate)
            {
                FontFamily pff = new FontFamily(txtFont.Text);
                if (lstFont.Items.Contains(pff))
                {
                    lstFont.SelectedItem = pff;
                }
            }
        }

        private void lstFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxUpdate = true;

#if NETCOREAPP
            FontFamily ff = (lstFont.SelectedItem as FontFamily)!;
#else
            FontFamily ff = (lstFont.SelectedItem as FontFamily);
#endif
            txtFont.Text = ff.Source;

            //List<string> tf = new List<string>();
            //foreach (Typeface item in ff.GetTypefaces())
            //{
            //    tf.Add(item.FaceNames[System.Windows.Markup.XmlLanguage.GetLanguage("en-us")]);
            //}
            //Console.WriteLine(string.Join(",", tf));

            listBoxUpdate = false;
        }


        private void lstSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSize.SelectedIndex == -1)
            {
                return;
            }

            listBoxUpdate = true;
            txtSize.Value = (int)lstSize.SelectedItem;
            listBoxUpdate = false;
        }

        private void txtSize_ValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (lstSize == null)
            {
                return;
            }

            if (!listBoxUpdate)
            {
                if (Math.Floor(txtSize.Value) == txtSize.Value)
                {
                    if (lstSize.Items.Contains((int)txtSize.Value))
                    {
                        lstSize.SelectedItem = (int)txtSize.Value;
                    }
                    else
                    {
                        lstSize.SelectedIndex = -1;
                    }
                }
                else
                {
                    lstSize.SelectedIndex = -1;
                }
            }
        }

        private void chkOverline_Checked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.Add(TextDecorations.OverLine);
        }

        private void chkStrike_Checked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.Add(TextDecorations.Strikethrough);
        }

        private void chkUnderline_Checked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.Add(TextDecorations.Underline);
        }

        private void chkItalic_Checked(object sender, RoutedEventArgs e)
        {
            chkOblique.IsChecked = false;
            txtPreview.FontStyle = FontStyles.Italic;
        }

        private void chkOblique_Checked(object sender, RoutedEventArgs e)
        {
            chkItalic.IsChecked = false;
            txtPreview.FontStyle = FontStyles.Oblique;
        }

        private void chkBaseline_Checked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.Add(TextDecorations.Baseline);
        }

        private void chkOverline_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.TryRemove(TextDecorations.OverLine, out var tdc);
            txtPreview.TextDecorations = tdc;
        }

        private void chkStrike_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.TryRemove(TextDecorations.Strikethrough, out var tdc);
            txtPreview.TextDecorations = tdc;
        }

        private void chkUnderline_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.TryRemove(TextDecorations.Underline, out var tdc);
            txtPreview.TextDecorations = tdc;
        }

        private void chkItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.FontStyle = chkOblique.IsChecked.GetValueOrDefault(false) ? FontStyles.Oblique : FontStyles.Normal;
        }

        private void chkOblique_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.FontStyle = chkItalic.IsChecked.GetValueOrDefault(false) ? FontStyles.Italic : FontStyles.Normal;
        }

        private void chkBaseline_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPreview.TextDecorations.TryRemove(TextDecorations.Baseline, out var tdc);
            txtPreview.TextDecorations = tdc;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            StringInputDialog sid = new StringInputDialog(ColorScheme, "Set Preview Text", "Set the text to appear in the Preview box.", txtPreview.Text);
            sid.ShowDialog();
            if (sid.DialogResult)
            {
                txtPreview.Text = sid.Value;
            }
        }

    }
}
