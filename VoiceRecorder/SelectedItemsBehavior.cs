﻿using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace VoiceRecorder
{
    public static class SelectedItems
    {
        private static readonly DependencyProperty SelectedItemsBehaviorProperty =
            DependencyProperty.RegisterAttached("SelectedItemsBehavior", typeof(SelectedItemsBehavior), typeof(ListBox), null);

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.RegisterAttached("Items", typeof(IList), typeof(SelectedItems), new PropertyMetadata(null, ItemsPropertyChanged));

        public static void SetItems(ListBox listBox, IList list)
        {
            listBox.SetValue(ItemsProperty, list);
        }

        public static IList GetItems(ListBox listBox)
        {
            return listBox.GetValue(ItemsProperty) as IList;
        }

        private static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ListBox;
            if (target != null && e.NewValue != null)
            {
                SelectedItemsBehavior itemsBehavior = GetOrCreateBehavior(target, e.NewValue as IList);
                itemsBehavior.RemoveSelectionChangedHandler();
                target.SelectedItems.Clear();
                var items = e.NewValue as IList;
                if (items != null)
                    foreach (var item in items)
                    {
                        target.SelectedItems.Add(item);
                    }

                itemsBehavior.AddSelectionChangedHandler();
            }
        }

        private static SelectedItemsBehavior GetOrCreateBehavior(ListBox target, IList list)
        {
            var behavior = target.GetValue(SelectedItemsBehaviorProperty) as SelectedItemsBehavior;
            if (behavior == null)
            {
                behavior = new SelectedItemsBehavior(target, list);
                target.SetValue(SelectedItemsBehaviorProperty, behavior);
            }
            else
            {
                behavior.SetNewList(list);
            }

            return behavior;
        }
    }

    public class SelectedItemsBehavior
    {
        private readonly ListBox _listBox;
        private IList _boundList;

        public SelectedItemsBehavior(ListBox listBox, IList boundList)
        {
            _boundList = boundList;
            _listBox = listBox;
            _listBox.Loaded += ListBoxLoaded;
            AddCollectionChangedHandler();
        }

        void SelectedItemsBehaviorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RemoveSelectionChangedHandler();
            _listBox.SelectedItems.Clear();
            foreach (var item in _boundList)
            {
                _listBox.SelectedItems.Add(item);
            }
            AddSelectionChangedHandler();
        }

        void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in _boundList)
            {
                _listBox.SelectedItems.Add(item);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveCollectionChangedHandler();
            _boundList.Clear();

            foreach (var item in _listBox.SelectedItems)
            {
                _boundList.Add(item);
            }
            AddCollectionChangedHandler();
        }

        public void SetNewList(IList boundList)
        {
            RemoveCollectionChangedHandler();
            _boundList = boundList;
            AddCollectionChangedHandler();
        }

        public void AddSelectionChangedHandler()
        {
            _listBox.SelectionChanged += OnSelectionChanged;
        }

        public void RemoveSelectionChangedHandler()
        {
            _listBox.SelectionChanged -= OnSelectionChanged;
        }

        public void AddCollectionChangedHandler()
        {
            if (_boundList == null) return;
            var notifyCollectionChanged = _boundList as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged += SelectedItemsBehaviorCollectionChanged;
        }

        public void RemoveCollectionChangedHandler()
        {
            if (_boundList == null) return;
            var notifyCollectionChanged = _boundList as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged -= SelectedItemsBehaviorCollectionChanged;
        }
    }
}
