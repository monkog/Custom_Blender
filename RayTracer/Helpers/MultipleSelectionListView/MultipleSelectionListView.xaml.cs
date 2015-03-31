using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace RayTracer.Helpers.MultipleSelectionListView
{
    /// <summary>
    /// Interaction logic for MultipleSelectionListView.xaml
    /// http://blogs.microsoft.co.il/miziel/2014/05/02/wpf-binding-listbox-selecteditems-attached-property-vs-style/
    /// </summary>
    public partial class MultipleSelectionListView
    {
        #region SelectedItems

        private static ListBox list;
        private static bool _isRegisteredSelectionChanged = false;

        ///
        /// SelectedItems Attached Dependency Property
        ///
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IList)
            , typeof(MultipleSelectionListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
            , OnSelectedItemsChanged));

        public static IList GetSelectedItems(DependencyObject d)
        {
            return (IList)d.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject d, IList value)
        {
            d.SetValue(SelectedItemsProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_isRegisteredSelectionChanged)
            {
                var listBox = (ListBox)d;
                list = listBox;
                listBox.SelectionChanged += listBox_SelectionChanged;
                _isRegisteredSelectionChanged = true;
            }
        }

        private static void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get list box's selected items.
            IEnumerable listBoxSelectedItems = list.SelectedItems;
            //Get list from model
            IList modelSelectedItems = GetSelectedItems(list);

            //Update the model
            modelSelectedItems.Clear();

            if (list.SelectedItems != null)
            {
                foreach (var item in list.SelectedItems)
                    modelSelectedItems.Add(item);
            }
            SetSelectedItems(list, modelSelectedItems);
        }
        #endregion
    }
}
