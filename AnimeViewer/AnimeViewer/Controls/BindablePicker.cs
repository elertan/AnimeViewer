using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Xamarin.Forms;

namespace AnimeViewer.Controls
{
    public class BindablePicker : Picker
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker),
                null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(object), typeof(BindablePicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(object), typeof(BindablePicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

        private bool _disableNestedCalls;

        public BindablePicker()
        {
            SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public string DisplayMemberPath { get; set; }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                if (SelectedItem != value)
                {
                    SetValue(SelectedItemProperty, value);
                    InternalSelectedItemChanged();
                }
            }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
                InternalSelectedValueChanged();
            }
        }

        public string SelectedValuePath { get; set; }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        private void InstanceOnItemsSourceChanged(object oldValue, object newValue)
        {
            _disableNestedCalls = true;
            Items.Clear();

            var oldCollectionINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (oldCollectionINotifyCollectionChanged != null)
                oldCollectionINotifyCollectionChanged.CollectionChanged -= ItemsSource_CollectionChanged;

            var newCollectionINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (newCollectionINotifyCollectionChanged != null)
                newCollectionINotifyCollectionChanged.CollectionChanged += ItemsSource_CollectionChanged;

            if (!Equals(newValue, null))
            {
                var hasDisplayMemberPath = !string.IsNullOrWhiteSpace(DisplayMemberPath);

                foreach (var item in (IEnumerable) newValue)
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(DisplayMemberPath);
                        Items.Add(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        Items.Add(item.ToString());
                    }

                SelectedIndex = -1;
                _disableNestedCalls = false;

                if (SelectedItem != null)
                    InternalSelectedItemChanged();
                else if (hasDisplayMemberPath && (SelectedValue != null))
                    InternalSelectedValueChanged();
            }
            else
            {
                _disableNestedCalls = true;
                SelectedIndex = -1;
                SelectedItem = null;
                SelectedValue = null;
                _disableNestedCalls = false;
            }
        }

        private void InternalSelectedItemChanged()
        {
            if (_disableNestedCalls)
                return;

            var selectedIndex = -1;
            object selectedValue = null;
            if (ItemsSource != null)
            {
                var index = 0;
                var hasSelectedValuePath = !string.IsNullOrWhiteSpace(SelectedValuePath);
                foreach (var item in ItemsSource)
                {
                    if ((item != null) && item.Equals(SelectedItem))
                    {
                        selectedIndex = index;
                        if (hasSelectedValuePath)
                        {
                            var type = item.GetType();
                            var prop = type.GetRuntimeProperty(SelectedValuePath);
                            selectedValue = prop.GetValue(item);
                        }
                        break;
                    }
                    index++;
                }
            }
            _disableNestedCalls = true;
            SelectedValue = selectedValue;
            SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        private void InternalSelectedValueChanged()
        {
            if (_disableNestedCalls)
                return;

            if (string.IsNullOrWhiteSpace(SelectedValuePath))
                return;
            var selectedIndex = -1;
            object selectedItem = null;
            var hasSelectedValuePath = !string.IsNullOrWhiteSpace(SelectedValuePath);
            if ((ItemsSource != null) && hasSelectedValuePath)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    if (item != null)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(SelectedValuePath);
                        if (Equals(prop.GetValue(item), SelectedValue))
                        {
                            selectedIndex = index;
                            selectedItem = item;
                            break;
                        }
                    }

                    index++;
                }
            }
            _disableNestedCalls = true;
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var hasDisplayMemberPath = !string.IsNullOrWhiteSpace(DisplayMemberPath);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(DisplayMemberPath);
                        Items.Add(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        Items.Add(item.ToString());
                    }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.NewItems)
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(DisplayMemberPath);
                        Items.Remove(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        Items.Remove(item.ToString());
                    }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems)
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(DisplayMemberPath);
                        Items.Remove(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        var index = Items.IndexOf(item.ToString());
                        if (index > -1)
                            Items[index] = item.ToString();
                    }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Items.Clear();
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                        if (hasDisplayMemberPath)
                        {
                            var type = item.GetType();
                            var prop = type.GetRuntimeProperty(DisplayMemberPath);
                            Items.Remove(prop.GetValue(item).ToString());
                        }
                        else
                        {
                            var index = Items.IndexOf(item.ToString());
                            if (index > -1)
                                Items[index] = item.ToString();
                        }
                }
                else
                {
                    _disableNestedCalls = true;
                    SelectedItem = null;
                    SelectedIndex = -1;
                    SelectedValue = null;
                    _disableNestedCalls = false;
                }
            }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (Equals(newValue, null) && Equals(oldValue, null))
                return;

            var picker = (BindablePicker) bindable;
            picker.InstanceOnItemsSourceChanged(oldValue, newValue);
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_disableNestedCalls)
                return;

            if ((SelectedIndex < 0) || (ItemsSource == null) || !ItemsSource.GetEnumerator().MoveNext())
            {
                _disableNestedCalls = true;
                if (SelectedIndex != -1)
                    SelectedIndex = -1;
                SelectedItem = null;
                SelectedValue = null;
                _disableNestedCalls = false;
                return;
            }

            _disableNestedCalls = true;

            var index = 0;
            var hasSelectedValuePath = !string.IsNullOrWhiteSpace(SelectedValuePath);
            foreach (var item in ItemsSource)
            {
                if (index == SelectedIndex)
                {
                    SelectedItem = item;
                    if (hasSelectedValuePath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(SelectedValuePath);
                        SelectedValue = prop.GetValue(item);
                    }

                    break;
                }
                index++;
            }

            _disableNestedCalls = false;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var boundPicker = (BindablePicker) bindable;
            boundPicker.ItemSelected?.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
            boundPicker.InternalSelectedItemChanged();
        }

        private static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var boundPicker = (BindablePicker) bindable;
            boundPicker.InternalSelectedValueChanged();
        }
    }
}