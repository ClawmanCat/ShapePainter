using ShapePainter.Shapes;
using ShapePainter.Utility;
using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ShapePainter.Utility {
    public static class GroupViewBuilder {
        public static TreeView Make(ICanvasObject basenode, TreeView prev_state) {
            TreeView view = new TreeView { AllowDrop = true };
            List<TreeViewItem> selected_items = new List<TreeViewItem>();


            Action<object, RoutedEventArgs, ICanvasObject, bool> mouse_callback = (object o, RoutedEventArgs e, ICanvasObject obj, bool selected) => {
                MainWindow.instance.OnGroupViewClicked(selected, obj);
                e.Handled = true;
            };


            Func<ICanvasObject, TreeViewItem> converter = null;
            converter = (ICanvasObject obj) => {
                TreeViewItem item = new TreeViewItem();


                // Drag & drop event handlers.
                item.MouseMove += (object s, MouseEventArgs e) => {
                    if (e.LeftButton != MouseButtonState.Pressed) return;

                    DataObject data = new DataObject(DataFormats.Serializable, selected_items);
                    DragDrop.DoDragDrop(view, data, DragDropEffects.Move);
                };


                item.Drop += (object s, DragEventArgs e) => {
                    List<TreeViewItem> moved = (List<TreeViewItem>) e.Data.GetData(DataFormats.Serializable);

                    TreeViewItem dest = e.Source as TreeViewItem;
                    ICanvasObject dest_obj = dest.Tag as ICanvasObject;

                    if (dest_obj == null) return;

                    foreach (TreeViewItem i in moved) {
                        ICanvasObject contained_obj = i.Tag as ICanvasObject;
                        if (contained_obj == null || contained_obj == dest_obj || !(dest_obj is Group)) continue;

                        contained_obj.parent.children.Remove(contained_obj);
                        contained_obj.parent = dest_obj;
                        dest_obj.children.Add(contained_obj);
                    }

                    MainWindow.instance.ForceRebuildGroupView();
                };


                item.Selected   += mouse_callback.Bind(true).Bind(obj).Invoke;
                item.Unselected += mouse_callback.Bind(false).Bind(obj).Invoke;
                item.IsExpanded = IsExpanded(obj, prev_state);
                item.Tag = obj;


                if (IsSelected(obj)) {
                    selected_items.Add(item);
                    item.Background = Brushes.LightGray;
                }

                if (obj is Shape) {
                    item.Header = ((Shape) obj).shape.GetType().Name;
                } else {
                    item.Header = "[G] " + ((Group) obj).name;

                    item.PreviewMouseLeftButtonDown += (object s, MouseButtonEventArgs e) => {
                        if (e.Source == item && MainWindow.instance.IsKeyPressed(Key.X)) {
                            GroupViewPopup popup = new GroupViewPopup(obj);
                            popup.Show();
                        }
                    };

                    foreach (var child in obj.children) {
                        if (MainWindow.instance.HasCanvasObject(child)) item.Items.Add(converter(child));
                    }
                }


                return item;
            };

            
            view.Items.Add(converter(basenode));
            return view;
        }


        private static bool IsExpanded(ICanvasObject obj, TreeView prev_state) {
            Func<TreeViewItem, bool> search = null; 
            search = (TreeViewItem item) => {
                return item.Tag as ICanvasObject == obj || item.Items.Any(x => search(x as TreeViewItem));
            };

            return MainWindow.instance.GetSelection().Contains(obj) || 
                   obj.children.Any(x => IsExpanded(x, prev_state)) || 
                   (prev_state?.Items.Any(x => search(x as TreeViewItem)) ?? false);
        }


        private static bool IsSelected(ICanvasObject obj) {
            return MainWindow.instance.GetSelection().Contains(obj);
        }


        private static TreeViewItem GetHoveredItem(TreeView view) {
            HitTestResult result = VisualTreeHelper.HitTest(view, Mouse.GetPosition(null));

            return (result == null || !(result.VisualHit is TreeViewItem))
                ? result.VisualHit as TreeViewItem
                : null;
        }
    }
}
