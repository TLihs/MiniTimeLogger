using MiniTimeLogger.Controls;
using MiniTimeLogger.Controls.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Data
{
    public abstract class BaseCategoryObject<T1, T2> : INotifyPropertyChanged
        where T1 : BaseCategoryObject<T1, T2>
        where T2 : BaseCategoryObjectControl<T1, T2>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected static int _nextId = 0;

        private T1 _parent;
        private string _name;
        private string _description;
        private T1 _nextItem;
        private T1 _previousItem;
        private T2 _control;

        public static List<T1> CategoryObjects { get; } = new List<T1>();
        public static List<T1> RemovedObjects { get; } = new List<T1>();
        public static T1 GetCategoryObjectById(int id) => CategoryObjects.First(category => category.Id == id);
        public static string ThisStaticType => typeof(T1).Name;

        public virtual T1 Parent
        {
            get => _parent;
            set
            {
                LogDebug($"{GetType()}::{GetCaller()} - value = {value}");
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Id { get; protected set; }
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value != _name)
                {
                    if (CategoryObjects.FindAll(category => category.Name.Equals(value, StringComparison.OrdinalIgnoreCase)).Count == 0)
                    {
                        _name = value;
                        if (Control != null)
                            Control.LabelText = _name;
                    }
                    else
                        LogWarning($"{GetType()}::[public]{nameof(Name)} - Category with that name already exists.");
                    OnPropertyChanged();
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    LogGenericError(new ArgumentNullException(nameof(Name)));
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (value != null && value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
                else if (value == null)
                {
                    _description = string.Empty;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<CategoryItem> CategoryItems { get; set; } = new ObservableCollection<CategoryItem>();
        public T1 NextItem
        {
            get => _nextItem;
            set
            {
                if (value != _nextItem)
                {
                    LogDebug($"{GetType()}::{GetCaller()} - Setting next item for {this} to {value}");
                    
                    if (value == this)
                        value = null;
                    
                    T1 nextitemold = _nextItem;
                    _nextItem = value;
                    if (nextitemold != null)
                        nextitemold.PreviousItem = _previousItem;
                    if (_nextItem != null)
                    {
                        if (Control != null)
                            Control.NextControl = _nextItem.Control;
                        _nextItem.PreviousItem = (T1)this;
                    }
                    RefreshControlSize();
                    OnPropertyChanged();
                }
            }
        }
        public T1 PreviousItem
        {
            get => _previousItem;
            set
            {
                if (value != _previousItem)
                {
                    LogDebug($"{GetType()}::{GetCaller()} - Setting previous item for {this} to {value}");

                    if (value == this)
                        value = null;

                    T1 previousitemold = _previousItem;
                    _previousItem = value;
                    if (previousitemold != null)
                        previousitemold.NextItem = _nextItem;
                    if (_previousItem != null)
                    {
                        if (Control != null)
                            Control.PreviousControl = _previousItem.Control;
                        _previousItem.NextItem = (T1)this;
                    }
                    RefreshControlSize();
                    OnPropertyChanged();
                }
            }
        }
        public T2 Control
        {
            get => _control;
            set
            {
                LogDebug($"{GetType()}::{GetCaller()} - value = {value}");
                if (value != _control)
                    _control = value;
            }
        }

        public BaseCategoryObject(int id = -1)
        {
            if (id < 0)
                Id = _nextId++;

            _name = string.Empty;
            _description = string.Empty;
        }

        public void AddCategoryItem(CategoryItem categoryItem, CategoryItem nextItem = null)
        {
            if (nextItem == null)
            {
                categoryItem.PreviousItem = CategoryItems.LastOrDefault();
                CategoryItems.Add(categoryItem);
                Control.AddItem(categoryItem);
            }
            else
            {
                categoryItem.NextItem = nextItem;
                CategoryItems.Insert(CategoryItems.IndexOf(nextItem), categoryItem);
                Control.AddItem(categoryItem);
            }
        }

        public void RemoveCategoryItem(CategoryItem categoryItem)
        {
            categoryItem.NextItem = null;
            categoryItem.PreviousItem = null;
            CategoryItems.Remove(categoryItem);
            Control.RemoveItem(categoryItem);
        }

        public virtual void MoveItemToBin()
        {
            foreach (CategoryItem item in CategoryItems)
                item.MoveItemToBin();

            if (CategoryObjects.Contains((T1)this))
            {
                RemovedObjects.Add((T1)this);
                CategoryObjects.Remove((T1)this);
            }
        }

        public virtual void RecoverItemFromBin()
        {
            if (RemovedObjects.Contains((T1)this))
            {
                CategoryObjects.Add((T1)this);
                RemovedObjects.Remove((T1)this);
            }

            foreach (CategoryItem item in CategoryItems)
                item.RecoverItemFromBin();
        }

        public void MoveItem(int index)
        {
            int currentindex = CategoryObjects.IndexOf((T1)this);
            if (currentindex != index)
            {
                CategoryObjects.Insert(index, (T1)this);
                if (index < currentindex)
                    CategoryObjects.RemoveAt(currentindex + 1);
                else
                    CategoryObjects.RemoveAt(currentindex);

                if (index < CategoryObjects.Count)
                    NextItem = CategoryObjects[index + 1];
                if (index > 0)
                    PreviousItem = CategoryObjects[index - 1];
            }
        }

        public void MoveItemUp()
        {
            if (PreviousItem != null)
                MoveItem(PreviousItem.Id);
        }

        public void MoveItemDown()
        {
            if (NextItem != null)
                MoveItem(NextItem.Id);
        }

        public override string ToString()
        {
            return $"{GetType().Name}: (Name: '{_name}'; Description: '{_description}')";
        }

        public void RefreshControlSize()
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}()");

            if (Control != null && typeof(T1) == typeof(CategoryItem))
            {
                LogDebug($"{ThisStaticType}::{GetCaller()}() - Name: {Name}; Width: {Control.ActualWidth}; Height: {Control.ActualHeight} - old");
                double newheight = double.NaN;
                if (CategoryItems.Count > 1)
                    newheight = CategoryItems.Sum(item => item.Control.ActualHeight);

                if (newheight == 0)
                    newheight = double.NaN;

                Control.Height = newheight;
                LogDebug($"{ThisStaticType}::{GetCaller()}() - Name: {Name}; Width: {Control.ActualWidth}; Height: {Control.ActualHeight} - new");
            }
        }
    }
}
