using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// A dialog to edit <see cref="Transform"/> objects. Many WPF objects/elements have properties that can use transforms to affect how they appear on screen.
    /// </summary>
    public partial class TransformEditDialog : FlatWindow
    {
        /// <summary>
        /// Create a new TransformEditDialog.
        /// </summary>
        public TransformEditDialog()
        {
            InitializeComponent();
            SetupWindow();
        }

        bool singleEditMode = false;

        void SetupWindow()
        {
            SetValue(TransformListProperty, new SelectableCollection<TransformSelectableControl>());
            TransformList.SelectionChanged += TransformList_SelectionChanged;
            TransformList.CanSelectMultiple = false;

            imgAdd.Source = IconLoader.LoadIcon("Add", ColorScheme);
            imgDelete.Source = IconLoader.LoadIcon("Delete", ColorScheme);
            imgMoveDown.Source = IconLoader.LoadIcon("Down", ColorScheme);
            imgMoveUp.Source = IconLoader.LoadIcon("Up", ColorScheme);
        }

        /// <summary>
        /// Get or set the list of controls that represents each transform to display in the dialog. This is generally used internally, but this can also be used as a reference of what's currently in the dialog.
        /// </summary>
        public SelectableCollection<TransformSelectableControl> TransformList
        {
            get => (SelectableCollection<TransformSelectableControl>)GetValue(TransformListProperty); set => SetValue(TransformListProperty, value);
        }

        /// <summary>
        /// The backing dependency property object for <see cref="TransformList"/>. See the related property for more details.
        /// </summary>
        public static DependencyProperty TransformListProperty
            = DependencyProperty.Register("TransformList", typeof(SelectableCollection<TransformSelectableControl>), typeof(TransformEditDialog),
            new FrameworkPropertyMetadata(new SelectableCollection<TransformSelectableControl>()));

        private void TransformList_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (item is TransformSelectableControl tsc)
                {
                    StoreDataToTransformItem(tsc);
                }
            }

            btnDelete.IsEnabled = TransformList.SelectedItems.Count > 0;
            btnMoveDown.IsEnabled = TransformList.SelectedItems.Count > 0;
            btnMoveUp.IsEnabled = TransformList.SelectedItems.Count > 0;

            LoadSelectedTransform();
        }

        /// <summary>
        /// Import a transform to edit in this TransformEditDialog. This should be called prior to showing the dialog, but additional calls will not reset the dialog.
        /// </summary>
        /// <param name="transform">The transform to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode (where adding more transforms isn't allowed), use <see cref="ImportSingleTransform(Transform)"/>. 
        /// To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(Transform transform)
        {
            if (transform is TransformGroup tg)
            {
                ImportTransforms(tg);
            }
            AddTransform(transform);
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but additional calls will not reset the dialog.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode (where adding more transforms isn't allowed), use <see cref="ImportSingleTransform(Transform)"/>. 
        /// To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(TransformCollection transforms)
        {
            foreach (Transform item in transforms)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                    continue;
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but additional calls will not reset the dialog.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode (where adding more transforms isn't allowed), use <see cref="ImportSingleTransform(Transform)"/>. 
        /// To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(TransformGroup transforms)
        {
            foreach (Transform item in transforms.Children)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                    continue;
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but additional calls will not reset the dialog.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode (where adding more transforms isn't allowed), use <see cref="ImportSingleTransform(Transform)"/>. 
        /// To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(IEnumerable<Transform> transforms)
        {
            foreach (Transform item in transforms)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                    continue;
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a single transform for editing; this activates Single Edit Mode for the TransformEditDialog, which only allows this transform to be edited; no adding or deleting.
        /// This should be called prior to showing the dialog, but additional calls will not reset the dialog.
        /// </summary>
        /// <param name="t">The one transform to edit.</param>
        /// <remarks>If the Transform <paramref name="t"/> is a <see cref="TransformGroup"/>, use <see cref="ImportTransforms(TransformGroup)"/> instead.</remarks>
        public void ImportSingleTransform(Transform t)
        {
            if (t is TransformGroup tg)
            {
                // this singlie transform is actually a TransformGroup
                // which can hold multiple transforms
                // thus, the standard format should apply instead
                ImportTransforms(tg);
            }
            else
            {
                var tsc = AddTransform(t);
                singleEditMode = true;
                btnAdd.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnMoveDown.Visibility = Visibility.Collapsed;
                btnMoveUp.Visibility = Visibility.Collapsed;
                tsc.IsSelected = true;
            }
        }

        /// <summary>
        /// Get all of the transforms listed in this dialog, combined together into a collection.
        /// </summary>
        /// <returns>A TransformCollection that contains each transform listed in this dialog.</returns>
        public TransformCollection ExportTransformCollection()
        {
            TransformCollection transforms = new TransformCollection();
            StoreDataToSelectedTransform();
            foreach (TransformSelectableControl item in TransformList)
            {
                transforms.Add(item.TransformValue);
            }

            return transforms;
        }

        /// <summary>
        /// Get the value of the transform object being edited in this dialog.
        /// </summary>
        /// <param name="returnIdentityIfBlank">If no transforms, set if an identity matrix transform should be returned, or an empty TransformGroup</param>
        /// <remarks>
        /// If this dialog is in Single Edit Mode, the transform object returned should match the type of the object passed in before via <see cref="ImportSingleTransform(Transform)"/>.
        /// Otherwise, this will return a <see cref="TransformGroup"/> that contains all listed transforms in the dialog (or if only a single transform is in the dialog, just that transform).
        /// </remarks>
        public Transform ExportSingleTransform(bool returnIdentityIfBlank = true)
        {
            if (singleEditMode)
            {
                if (TransformList.Count >= 1)
                {
                    StoreDataToSelectedTransform();
                    return TransformList[0].TransformValue;
                }
                else
                {
                    return returnIdentityIfBlank ? (Transform)new MatrixTransform(Matrix.Identity) : new TransformGroup();
                }
            }
            else
            {
                if (TransformList.Count == 1)
                {
                    StoreDataToSelectedTransform();
                    return TransformList[0].TransformValue;
                }
                else if (TransformList.Count == 0)
                {
                    // this "unnecessary cast" is needed for .NET Framework
                    return returnIdentityIfBlank ? (Transform)new MatrixTransform(Matrix.Identity) : new TransformGroup();
                }
                else
                {
                    return new TransformGroup() { Children = ExportTransformCollection() };
                }
            }
        }

        TransformSelectableControl AddTransform(Transform t)
        {
            TransformSelectableControl control = new TransformSelectableControl(t);
            TransformList.Add(control);
            return control;
        }

        void LoadSelectedTransform()
        {
            var selItems = TransformList.SelectedItems;
            if (selItems.Count > 0)
            {
                var selItem = selItems[0];

                Transform selTransform = selItem.TransformValue;

                if (selTransform is RotateTransform r)
                {
                    // load rotation controls
                    DisplayTransformControl(grdRotate);
                    LoadInRotateTransform(r);
                }
                else if (selTransform is ScaleTransform s)
                {
                    // load scale controls
                    DisplayTransformControl(grdScale);
                    LoadInScaleTransform(s);
                }
                else if (selTransform is TranslateTransform u)
                {
                    // load translation controls
                    DisplayTransformControl(grdTranslate);
                    LoadInTranslateTransform(u);
                }
                else if (selTransform is SkewTransform k)
                {
                    // load skew controls
                    DisplayTransformControl(grdSkew);
                    LoadInSkewTransform(k);
                }
                else if (selTransform is MatrixTransform m)
                {
                    // load matrix controls
                    DisplayTransformControl(grdMatrix);
                    LoadInMatrixTransform(m);
                }
                else if (selTransform is TransformGroup)
                {
                    // oh dear
                    DisplayTransformControl(null);
                }
            }

        }

        void StoreDataToSelectedTransform()
        {
            if (TransformList.SelectedItems.Count > 0)
            {
                var selControl = TransformList.SelectedItems[0];
                Transform value = GetValuesFromActiveTransform();
                selControl.UpdateValue(value);
            }
            else
            {
                // nothing is selected, so nothing will be stored
            }
        }

        void StoreDataToTransformItem(TransformSelectableControl tsc)
        {
            if (TransformList.SelectedItems.Count > 0)
            {
                Transform value = GetValuesFromActiveTransform();
                tsc.UpdateValue(value);
            }
            else
            {
                // nothing is selected, so nothing will be stored
            }
        }

        #region Transform Editors
#if NETCOREAPP
        void DisplayTransformControl(Grid? transformControl)
#else
        void DisplayTransformControl(Grid transformControl)
#endif
        {
            if (transformControl == null)
            {
                // select nothing
                grdRotate.Visibility = Visibility.Collapsed;
                grdScale.Visibility = Visibility.Collapsed;
                grdTranslate.Visibility = Visibility.Collapsed;
                grdSkew.Visibility = Visibility.Collapsed;
                grdMatrix.Visibility = Visibility.Collapsed;

                txtGridEmpty.Visibility = Visibility.Visible;
            }
            else if (grdHolder.Children.IndexOf(transformControl) == -1)
            {
                throw new ArgumentException("The inputted element isn't actually present in the transform editor collection", nameof(transformControl));
            }
            else
            {
#if NETCOREAPP
                foreach (UIElement? item in grdHolder.Children)
#else
                foreach (UIElement item in grdHolder.Children)
#endif
                {
                    if (item == null) continue;
                    item.Visibility = item == transformControl ? Visibility.Visible : Visibility.Collapsed;
                }
                // LET'S DO IT
                //grdRotate.Visibility = (transformControl == grdRotate) ? Visibility.Visible : Visibility.Collapsed;
                //grdScale.Visibility = (transformControl == grdScale) ? Visibility.Visible : Visibility.Collapsed;
                //grdSkew.Visibility = (transformControl == grdSkew) ? Visibility.Visible : Visibility.Collapsed;
                //grdTranslate.Visibility = (transformControl == grdTranslate) ? Visibility.Visible : Visibility.Collapsed;
                //grdMatrix.Visibility = (transformControl == grdMatrix) ? Visibility.Visible : Visibility.Collapsed;

                //txtGridEmpty.Visibility = Visibility.Collapsed;
            }
        }

        Transform GetValuesFromActiveTransform()
        {
            UIElement visibleElement = txtGridEmpty;
#if NETCOREAPP
            foreach (UIElement? item in grdHolder.Children)
#else
            foreach (UIElement item in grdHolder.Children)
#endif
            {
                if (item == null) continue;
                if (item.Visibility == Visibility.Visible)
                {
                    visibleElement = item;
                }
            }

            if (visibleElement == txtGridEmpty) { return new MatrixTransform(Matrix.Identity); }
            else if (visibleElement == grdMatrix) { return GetValuesMatrixTransform(); }
            else if (visibleElement == grdRotate) { return GetValuesRotateTransform(); }
            else if (visibleElement == grdScale) { return GetValuesScaleTransform(); }
            else if (visibleElement == grdSkew) { return GetValuesSkewTransform(); }
            else if (visibleElement == grdTranslate) { return GetValuesTranslateTransform(); }
            else { return new MatrixTransform(Matrix.Identity); }
        }

        void LoadInRotateTransform(RotateTransform r)
        {
            nudRotateAngle.Value = r.Angle;
            nudRotateCenterX.Value = r.CenterX;
            nudRotateCenterY.Value = r.CenterY;
        }

        RotateTransform GetValuesRotateTransform()
        {
            return new RotateTransform(nudRotateAngle.Value, nudRotateCenterX.Value, nudRotateCenterY.Value);
        }

        void LoadInScaleTransform(ScaleTransform s)
        {
            nudScaleX.Value = s.ScaleX;
            nudScaleY.Value = s.ScaleY;
            nudScaleCenterX.Value = s.CenterX;
            nudScaleCenterY.Value = s.CenterY;
        }

        ScaleTransform GetValuesScaleTransform()
        {
            return new ScaleTransform(nudScaleX.Value, nudScaleY.Value, nudScaleCenterX.Value, nudScaleCenterY.Value);
        }

        void LoadInTranslateTransform(TranslateTransform t)
        {
            nudTranslateX.Value = t.X;
            nudTranslateY.Value = t.Y;
        }

        TranslateTransform GetValuesTranslateTransform()
        {
            return new TranslateTransform(nudTranslateX.Value, nudTranslateY.Value);
        }

        void LoadInSkewTransform(SkewTransform s)
        {
            nudSkewX.Value = s.AngleX;
            nudSkewY.Value = s.AngleY;
            nudSkewCenterX.Value = s.CenterX;
            nudSkewCenterY.Value = s.CenterY;
        }

        SkewTransform GetValuesSkewTransform()
        {
            return new SkewTransform(nudSkewX.Value, nudSkewY.Value, nudSkewCenterX.Value, nudSkewCenterY.Value);
        }

        void LoadInMatrixTransform(MatrixTransform m)
        {
            nudMatrix11.Value = m.Matrix.M11;
            nudMatrix12.Value = m.Matrix.M12;
            nudMatrix21.Value = m.Matrix.M21;
            nudMatrix22.Value = m.Matrix.M22;
            nudMatrixOffsetX.Value = m.Matrix.OffsetX;
            nudMatrixOffsetY.Value = m.Matrix.OffsetY;
        }

        MatrixTransform GetValuesMatrixTransform()
        {
            return new MatrixTransform(nudMatrix11.Value, nudMatrix12.Value, nudMatrix21.Value, nudMatrix22.Value,
                                            nudMatrixOffsetX.Value, nudMatrixOffsetY.Value);
        }

        #endregion

        #region Add menu
        private void mnuRotateAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new RotateTransform());
            TransformList.Select(tsc);
        }

        private void mnuSkewAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new SkewTransform());
            TransformList.Select(tsc);
        }

        private void mnuScaleAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new ScaleTransform(1, 1));
            TransformList.Select(tsc);
        }

        private void mnuTranslateAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new TranslateTransform());
            TransformList.Select(tsc);
        }

        private void mnuMatrixAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new MatrixTransform(Matrix.Identity));
            TransformList.Select(tsc);
        }
        #endregion

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            selTransformList.MoveItemUp(TransformList.IndexOf(TransformList.SelectedItems[0]));
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            selTransformList.MoveItemDown(TransformList.IndexOf(TransformList.SelectedItems[0]));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TransformList.SelectedItems.Count > 0)
            {
                TransformList.Remove(TransformList.SelectedItems[0]);
                DisplayTransformControl(null);
            }
        }

        /// <summary>
        /// Get or set the result state of the dialog - <c>true</c> if OK (and good to utilize updated variables), or <c>false</c> to cancel and discard changes.
        /// </summary>
        public new bool DialogResult { get; set; } = false;

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

        #region Editor Reset Buttons / Matrix Additions Menu
        private void btnRotateReset_Click(object sender, RoutedEventArgs e)
        {
            nudRotateCenterX.Value = 0;
            nudRotateCenterY.Value = 0;
        }

        private void btnScaleReset_Click(object sender, RoutedEventArgs e)
        {
            nudScaleCenterX.Value = 0;
            nudScaleCenterY.Value = 0;
        }

        private void btnScaleValReset_Click(object sender, RoutedEventArgs e)
        {
            nudScaleX.Value = 1;
            nudScaleY.Value = 1;
        }

        private void btnSkewReset_Click(object sender, RoutedEventArgs e)
        {
            nudSkewCenterX.Value = 0;
            nudSkewCenterY.Value = 0;
        }

        private void btnSkewValReset_Click(object sender, RoutedEventArgs e)
        {
            nudSkewX.Value = 0;
            nudSkewY.Value = 0;
        }

        private void btnTranslateReset_Click(object sender, RoutedEventArgs e)
        {
            nudTranslateX.Value = 0;
            nudTranslateY.Value = 0;
        }

        private void mnuMatrixRotateAppend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<RotateTransform>((v, mt) =>
            {
                mt.Matrix.RotateAt(v.Angle, v.CenterX, v.CenterY);
            });
        }

        private void mnuMatrixScaleAppend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<ScaleTransform>((v, mt) =>
            {
                mt.Matrix.ScaleAt(v.ScaleX, v.ScaleY, v.CenterX, v.CenterY);
            });
        }

        private void mnuMatrixTranslateAppend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<TranslateTransform>((v, mt) =>
            {
                mt.Matrix.Translate(v.X, v.Y);
            });
        }

        private void mnuMatrixRotatePrepend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<RotateTransform>((v, mt) =>
            {
                mt.Matrix.RotateAtPrepend(v.Angle, v.CenterX, v.CenterY);
            });
        }

        private void mnuMatrixScalePrepend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<ScaleTransform>((v, mt) =>
            {
                mt.Matrix.ScaleAtPrepend(v.ScaleX, v.ScaleY, v.CenterX, v.CenterY);
            });
        }

        private void mnuMatrixTranslatePrepend_Click(object sender, RoutedEventArgs e)
        {
            RunMatrixModify<TranslateTransform>((v, mt) =>
            {
                mt.Matrix.TranslatePrepend(v.X, v.Y);
            });

        }

        void RunMatrixModify<T>(Action<T, MatrixTransform> applyToMatrix) where T : Transform, new()
        {
            T t = new T();
            TransformEditDialog lted = new TransformEditDialog();
            lted.ImportSingleTransform(t);
            lted.ColorScheme = ColorScheme;
            lted.ShowDialog();
            if (lted.DialogResult)
            {
                Transform st = lted.ExportSingleTransform();
                if (st is T stt)
                {
                    MatrixTransform mt = GetValuesMatrixTransform();
                    applyToMatrix(stt, mt);
                    LoadInMatrixTransform(mt);
                }
            }
        }

        private void btnMatrixReset_Click(object sender, RoutedEventArgs e)
        {
            Matrix m = Matrix.Identity;

            nudMatrix11.Value = m.M11;
            nudMatrix12.Value = m.M12;
            nudMatrix21.Value = m.M21;
            nudMatrix22.Value = m.M22;
            nudMatrixOffsetX.Value = m.OffsetX;
            nudMatrixOffsetY.Value = m.OffsetY;
        }
        #endregion

    }

    /// <summary>
    /// A SelectableUserControl that is used for storing an arbitrary <see cref="Transform"/> value, for use in a <see cref="TransformEditDialog"/>.
    /// </summary>
    public class TransformSelectableControl : SelectableItem
    {
        /// <summary>
        /// Create a TransformSelectableControl.
        /// </summary>
        /// <param name="transform">The transform value to store in this control.</param>
        public TransformSelectableControl(Transform transform) : base()
        {
            UpdateValue(transform);
        }

        /// <summary>
        /// Update the appearance of this transform display control to match the transform that it is containing.
        /// </summary>
        /// <param name="transform">the new transform value</param>
        public void UpdateValue(Transform transform)
        {
            TransformValue = transform;

            if (transform is RotateTransform rt)
            {
                Text = "Rotate " + rt.Angle;
            }
            else if (transform is ScaleTransform st)
            {
                Text = $"Scale (X:{st.ScaleX}, Y: {st.ScaleY})";
            }
            else if (transform is TranslateTransform tt)
            {
                Text = $"Translate (X:{tt.X}, Y: {tt.Y})";
            }
            else if (transform is SkewTransform kt)
            {
                Text = $"Skew (X:{kt.AngleX}, Y: {kt.AngleY})";
            }
            else if (transform is MatrixTransform mt)
            {
                if (mt.Matrix.IsIdentity)
                {
                    Text = "Identity (nothing)";
                }
                else
                {
                    Text = "Matrix";
                }
            }
            else if (transform is TransformGroup tg)
            {
                Text = "Transform Group";
            }
        }

        /// <summary>
        /// Get or set the Transform value stored in this TransformSelectableControl.
        /// </summary>
        public Transform TransformValue { get; set; } = new MatrixTransform(Matrix.Identity);

    }
}