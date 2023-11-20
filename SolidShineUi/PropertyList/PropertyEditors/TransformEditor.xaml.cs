using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// An editor for WPF element transforms, of <see cref="Transform"/> or similar subtypes.
    /// </summary>
    public partial class TransformEditor : UserControl, IPropertyEditor
    {

        // CloneCurrentValue is used a lot to make sure that the internal variable that I have, _

        /// <summary>
        /// Create an TransformEditor.
        /// </summary>
        public TransformEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Transform), typeof(TransformGroup), typeof(TransformCollection),
            typeof(TranslateTransform), typeof(RotateTransform), typeof(SkewTransform), typeof(ScaleTransform), typeof(MatrixTransform)};

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => _writable; set { _writable = value; btnMenu.IsEnabled = value; } }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { /* _parent = value; */ } }

        ColorScheme _cs = new ColorScheme();

        /// <summary>
        /// Set the visual appearance of this control via a ColorScheme.
        /// </summary>
        /// <param name="value">the color scheme to apply</param>
        public void ApplyColorScheme(ColorScheme value)
        {
            _cs = value;
            btnMenu.ColorScheme = value;
            imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", value);
        }

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                ApplyColorScheme(value);
            }
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        Type _dataType = typeof(TransformGroup);
        bool _specificType = false;

        Transform _transform = new MatrixTransform(Matrix.Identity);

        bool _setNull = false;

#if NETCOREAPP
        //ExperimentalPropertyList? _parent = null;

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;
#else
        //ExperimentalPropertyList _parent = null;
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
#endif

        /// <inheritdoc/>
#if NETCOREAPP
        public object? GetValue()
#else
        public object GetValue()
#endif
        {
            if (_dataType == typeof(TransformCollection))
            {
                if (_setNull) return null;
                else if (_transform is TransformGroup tg)
                {
                    return tg.CloneCurrentValue().Children;
                }
                else
                {
                    return new TransformCollection() { _transform.CloneCurrentValue() };
                }
            }
            else if (_dataType == typeof(TransformGroup))
            {
                if (_setNull) return null;
                else if (_transform is TransformGroup tg)
                {
                    return tg.CloneCurrentValue();
                }
                else
                {
                    return new TransformGroup() { Children = new TransformCollection() { _transform.CloneCurrentValue() } };
                }
            }
            else
            {
                return _setNull ? null : _transform.CloneCurrentValue();
            }
        }

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            // first, let's assign the property type needed based upon the dataType
            _dataType = type;

            // next, we're going to assign the value
            if (value == null)
            {
                _transform = new TransformGroup();
                _setNull = true;
                mnuSetToNull.IsChecked = true;
            }
            if (value is Transform tt)
            {
                _transform = tt.CloneCurrentValue();
            }
            else if (value is TransformCollection tc)
            {
                _transform = new TransformGroup()
                {
                    Children = tc.CloneCurrentValue(),
                };
            }

            // check if we're only looking for a specific transform type
            if (type == typeof(MatrixTransform) || type == typeof(RotateTransform) || type == typeof(SkewTransform) 
                || type == typeof(ScaleTransform) || type == typeof(TranslateTransform))
            {
                _specificType = true;
            }

            UpdateText();
        }

        /// <summary>
        /// Open the ListEditorDialog, with its contents being the list or collection of this property.
        /// </summary>
        public bool OpenTransformDialog()
        {
            TransformEditDialog ted = new TransformEditDialog();
            ted.Owner = Window.GetWindow(this);
            ted.ColorScheme = _cs;
            if (_specificType)
            {
                ted.ImportSingleTransform(_transform);
            }
            else
            {
                ted.ImportTransforms(new List<Transform>() { _transform });
            }
            ted.ShowDialog();

            if (ted.DialogResult)
            {
                if (_specificType)
                {
                    _transform = ted.ExportSingleTransform(false);
                }
                else
                {
                    TransformCollection tc = ted.ExportTransformCollection();
                    if (tc.Count == 0)
                    {
                        // no transforms
                        _transform = new MatrixTransform(Matrix.Identity);
                    }
                    else if (tc.Count == 1)
                    {
                        // one transform
                        _transform = tc[0];
                    }
                    else
                    {
                        _transform = new TransformGroup() { Children = tc };
                    }
                }
            }

            return ted.DialogResult;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            bool res = OpenTransformDialog();
            if (res)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
                // update appearance
                UpdateText();
            }
        }

        private void mnuSetToNull_Click(object sender, RoutedEventArgs e)
        {
            if (_setNull)
            {
                mnuSetToNull.IsChecked = false;
                _setNull = false;
            }
            else
            {
                mnuSetToNull.IsChecked = true;
                _setNull = true;
            }

            ValueChanged?.Invoke(this, EventArgs.Empty);
            UpdateText();
        }

        void UpdateText()
        {
            Type tt = _transform.GetType();
            if (_setNull)
            {
                txtData.Text = "(null)";
            }
            else if (tt == typeof(TransformGroup))
            {
                TransformGroup tg = (TransformGroup)_transform;
                if (tg.Children.Count > 0)
                {
                    txtData.Text = $"{GROUP}: {tg.Children.Count} items";
                }
                else
                {
                    txtData.Text = $"{GROUP}: no transforms";
                }
            }
            else if (tt == typeof(RotateTransform))
            {
                txtData.Text = $"{ROTATE}: {((RotateTransform)_transform).Angle}";
            }
            else if (tt == typeof(SkewTransform))
            {
                txtData.Text = $"{SKEW}: X {((SkewTransform)_transform).AngleX}, Y {((SkewTransform)_transform).AngleY}";
            }
            else if (tt == typeof(ScaleTransform))
            {
                txtData.Text = $"{SCALE}: {((ScaleTransform)_transform).ScaleX} x {((ScaleTransform)_transform).ScaleY}";
            }
            else if (tt == typeof(TranslateTransform))
            {
                txtData.Text = $"{TRANSLATE}: {((TranslateTransform)_transform).X}, {((TranslateTransform)_transform).Y}";
            }
            else if (tt == typeof(MatrixTransform))
            {
                if (((MatrixTransform)_transform).Value.IsIdentity)
                {
                    txtData.Text = $"{IDENTITY}";
                }
                else
                {
                    txtData.Text = $"{MATRIX}";
                }
            }
            else
            {
                txtData.Text = _transform.Value.ToString();
            }
        }

        // eventually, once I figure out a localization solution, these will be moved to that spot
        /// <summary>UI text for "group"</summary>
        public static string GROUP = "group";
        /// <summary>UI text for "rotate"</summary>
        public static string ROTATE = "rotate";
        /// <summary>UI text for "skew"</summary>
        public static string SKEW = "skew";
        /// <summary>UI text for "scale"</summary>
        public static string SCALE = "scale";
        /// <summary>UI text for "translate"</summary>
        public static string TRANSLATE = "translate";
        /// <summary>UI text for "matrix"</summary>
        public static string MATRIX = "matrix";
        /// <summary>UI text for "default (identity matrix)"</summary>
        public static string IDENTITY = "default (identity matrix)";

        private void mnuReset_Click(object sender, RoutedEventArgs e)
        {
            if (_specificType)
            {
                if (_dataType == typeof(RotateTransform))
                {
                    _transform = new RotateTransform(0);
                }
                else if (_dataType == typeof(TranslateTransform))
                {
                    _transform = new TranslateTransform(0, 0);
                }
                else if (_dataType == typeof(SkewTransform))
                {
                    _transform = new SkewTransform();
                }
                else if (_dataType == typeof(ScaleTransform))
                {
                    _transform = new ScaleTransform();
                }
                else if (_dataType == typeof(MatrixTransform))
                {
                    _transform = new MatrixTransform(Matrix.Identity);
                }
            }
            else
            {
                _transform = new MatrixTransform(Matrix.Identity);
            }

            ValueChanged?.Invoke(this, EventArgs.Empty);
            UpdateText();
        }
    }
}
