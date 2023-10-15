using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MiniTimeLogger.Data;

using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Controls.Base
{
    public abstract class BaseCategoryItemControl : BaseCategoryObjectControl<CategoryItem, CategoryItemControl>
    {
        public CategoryItemControl ParentControl => CategoryObject?.Parent?.Control;
        public CategoryItemControl FirstChildControl => CategoryObject?.CategoryItems?.FirstOrDefault()?.Control;
        public List<CategoryItemControl> ChildControls => CategoryObject?.CategoryItems?.Select(item => item.Control).ToList();
        public bool IsCurrentlyLogging { get; set; }

        public void RefreshPosition(bool refreshChildItemPosition)
        {
            LogDebug($"{GetType()}::{GetCaller()}()");
            
            // The top position should first be calculated using the previous item. ...
            if (PreviousControl != null)
                SetTop(PreviousControl.GetTop() + PreviousControl.ActualHeight);
            // ... If there is no previous item, then we estimate, that this item is the first child item,
            // which means, that we use the top position of the parent item. ...
            else if (ParentControl != null)
                SetTop(ParentControl.GetTop());
            // ... If there is neither a previous nor a parent item, then we estimate, that this item is
            // the first item of the highest category.
            else
                SetTop(0d);

            // We refresh the position of the following sibling item
            NextControl?.RefreshPosition(refreshChildItemPosition);

            // Refreshing of the position should be triggered by parent to first child
            if (refreshChildItemPosition)
                foreach (CategoryItemControl item in ChildControls)
                    item.RefreshPosition(refreshChildItemPosition);
        }

        public void RefreshSize()
        {
            LogDebug($"{GetType()}::{GetCaller()}()");

            // We refresh the desired size of the control (based on content)
            Measure(new Size(CategoryObject.CategoryParent.Control.ActualWidth, double.PositiveInfinity));
            double desiredheight = DesiredSize.Height;
            double childrenheightsum = ChildControls.Sum(item => item.ActualHeight);
            bool refreshchildsize = childrenheightsum < desiredheight;

            // If the desired height of this item is bigger than the sum of heights of the child item
            // we set the height to the desired value, else we set it to the sum of the desired child
            // item height.
            if (refreshchildsize)
                Height = desiredheight;
            else
                Height = childrenheightsum;

            // We continue to go upwards the item tree, which will continue, until we reached the first
            // item of all items.
            ParentControl?.RefreshSize();

            // We increase the height of the child items going downward the item tree and also
            // refresh the position of each child.
            ChildControls.ForEach(item =>
            {
                if (refreshchildsize)
                {
                    // If the desired height of an item is bigger than the sum of the desired height of it's
                    // child items, then we have to increase the height of each child item proportionally.
                    double childheightoffset = (Height - childrenheightsum) / ChildControls.Count;
                    item.IncreaseHeight(childheightoffset);
                }
                // We don't want to refresh the position of the child items of the child item
                // of this control unneccessarily
                RefreshPosition(false);
            });
        }

        public void IncreaseHeight(double offset)
        {
            LogDebug($"{GetType()}::{GetCaller()}()");

            Height += offset;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (IsCurrentlyLogging)
            {
                CategoryObject.EndTimeLogging();
                IsCurrentlyLogging = false;
            }
            else
            {
                CategoryObject.StartTimeLogging();
                IsCurrentlyLogging = true;
            }

            base.OnMouseDoubleClick(e);
        }
    }
}
