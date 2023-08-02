using MiniTimeLogger.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MiniTimeLogger.Support.ExceptionHandling;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace MiniTimeLogger.Data
{
    public class CategoryItem : BaseCategoryObject<CategoryItem, CategoryItem>, INotifyPropertyChanged
    {
        private string _referenceKey;
        private Category _categoryParent;
        private CategoryItemControl _control;

        public string ReferenceKey
        {
            get => _referenceKey;
            set
            {
                if (_referenceKey != value)
                {
                    _referenceKey = value;
                    OnPropertyChanged();
                }
            }
        }
        public new CategoryItem Parent
        {
            get => base.Parent;
            set
            {
                if (value != base.Parent)
                    base.Parent = value;
            }
        }
        public Category CategoryParent
        {
            get => _categoryParent;
            set
            {
                if (_categoryParent != value)
                {
                    _categoryParent?.CategoryItems.Remove(this);
                    _categoryParent?.Control.StackPanel_CategoryItems.Children.Remove(_control);
                    _categoryParent = value;
                    _categoryParent?.CategoryItems.Add(this);
                    _categoryParent?.Control.StackPanel_CategoryItems.Children.Add(_control);
                    OnPropertyChanged();
                }
            }
        }
        public bool IsMainCategoryItem => Parent == null;
        public bool IsReferenceKeyMissing => ReferenceKey == null;
        public bool IsLastCategoryItem => CategoryParent.IsLastCategory;
        public CategoryItemControl Control => _control;

        private CategoryItem() : base()
        {
            // If this constructor is used, ID will be by default -1 in the constructor of the base class,
            // which generates a new ID for this Category object.

            _control = new CategoryItemControl();
            _control.LoadCategoryItemData(this);
        }

        private CategoryItem(int id) : base(id)
        {
            // If this constructor is used, the id omitted will be used and no new ID will be generated,
            // except 'id' is smaller than 0 (which shouldn't happen).

            _control = new CategoryItemControl();
            _control.LoadCategoryItemData(this);
        }

        public void UnloadCategoryItem()
        {
            if (_control != null)
                _control.UnloadCategoryItemData();
        }

        public static CategoryItem CreateCategoryItem(CategoryItem parent, string name, string description = null)
        {
            LogDebug($"{ThisStaticType}::[static]{GetCaller()}({(parent != null ? parent?.Name : "[null]")}, {name}, {description})");

            try
            {
                Category category;
                if (parent == null)
                    category = (Category)Category.CategoryObjects[0];
                else
                    category = parent.CategoryParent.SubCategory;

                if (category == null)
                    throw new ArgumentNullException(nameof(category));

                CategoryItem item = new CategoryItem()
                {
                    Name = name,
                    Description = description,
                    CategoryParent = category,
                    Parent = parent
                };
                CategoryObjects.Add(item);

                return item;
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"{ThisStaticType}::[static]{GetCaller()}(...) - Failed to load Category data.");
                return null;
            }
        }


        private static CategoryItem LoadCategoryItem(CategoryItem parent, int id, string name, string description = null)
        {
            LogDebug($"{ThisStaticType}::[static]{GetCaller()}({(parent != null ? parent?.Name : "[null]")}, {id}, {name})");

            try
            {
                Category category;
                if (parent == null)
                    category = (Category)Category.CategoryObjects[0];
                else
                    category = parent.CategoryParent.SubCategory;

                CategoryItem categoryitem = new CategoryItem(id)
                {
                    Name = name,
                    Description = description,
                    CategoryParent = category,
                    Parent = parent,
                };

                if (id > _nextId)
                    _nextId = id++;

                return categoryitem;
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"{ThisStaticType}::[static]{GetCaller()} - Failed to load Category data.");
                return null;
            }
        }

        private static void LoadCategoryItemDataRecursive(CategoryItem parent, XElement categoryElement)
        {
            LogDebug($"{ThisStaticType}::[static]{GetCaller()}({(parent != null ? parent?.Name : "[null]")}, {categoryElement})");

            try
            {
                CategoryItem categoryitem = LoadCategoryItem(parent,
                    int.Parse(categoryElement.Element("id").Value, System.Globalization.NumberStyles.HexNumber),
                    categoryElement.Element("name").Value,
                    categoryElement.Element("description").Value);

                if (categoryitem != null)
                {
                    CategoryObjects.Add(categoryitem);

                    if (categoryElement.Element("subitems") != null)
                        foreach (XElement subitem in categoryElement.Element("subitems").Elements("categoryitem"))
                            LoadCategoryItemDataRecursive(categoryitem, subitem);
                }
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"{ThisStaticType}::[static]{GetCaller()} - Failed to read element: {categoryElement}");
            }
        }

        public void SaveCategoryItemRecursive(ref XElement parentElement)
        {
            LogDebug($"{ThisStaticType}::[static]{GetCaller()}({parentElement})");

            try
            {
                XElement categoryitemelement = new XElement("categoryitem");
                categoryitemelement.Add(new XElement("id"), Id.ToString("X"));
                categoryitemelement.Add(new XElement("name"), Name);
                categoryitemelement.Add(new XElement("description"), Description);

                foreach (CategoryItem subitem in CategoryItems)
                    subitem.SaveCategoryItemRecursive(ref categoryitemelement);

                parentElement.Add(categoryitemelement);
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"{ThisStaticType}::[static]{GetCaller()} - Failed to write element {this} to {parentElement}");
            }
        }

        public static void InitializeCategoryItems(XElement itemList)
        {
            LogDebug($"{ThisStaticType}::[static]{GetCaller()}({itemList})");

            try
            {
                foreach (XElement categoryItemElement in itemList.Elements("categoryitems"))
                    LoadCategoryItemDataRecursive(null, categoryItemElement);
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"{ThisStaticType}::[static]{GetCaller()} - Failed to initialize categories.");
            }
        }
    }
}
