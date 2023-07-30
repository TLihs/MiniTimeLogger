using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Data
{
    public abstract class BaseCategoryObject<T1, T2> where T1 : BaseCategoryObject<T1, T2> where T2 : CategoryItem, INotifyPropertyChanged
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

        public static List<T1> CategoryObjects { get; } = new List<T1>();
        public static T1 GetCategoryObjectById(int id) => CategoryObjects.First(category => category.Id == id);

        public virtual T1 Parent
        {
            get => _parent;
            set
            {
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
                    _name = value;
                    OnPropertyChanged();
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    LogGenericError(new ArgumentNullException(nameof(Name)));
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
        public ObservableCollection<T2> CategoryItems { get; set; } = new ObservableCollection<T2>();
        public T1 NextItem
        {
            get => _nextItem;
            set
            {
                if (value != _nextItem)
                {
                    if (_nextItem != null)
                        _nextItem.PreviousItem = _previousItem;
                    _nextItem = value;
                    if (_nextItem != null)
                        _nextItem.PreviousItem = (T1)this;
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
                    if (_previousItem != null)
                        _previousItem.NextItem = _nextItem;
                    _previousItem = value;
                    if (_previousItem != null)
                        _previousItem.NextItem = (T1)this;

                    OnPropertyChanged();
                }
            }
        }

        public BaseCategoryObject(int id = -1)
        {
            if (id < 0)
                Id = _nextId++;

            _name = string.Empty;
            _description = string.Empty;
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
    }
}
