using MiniTimeLogger.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Data
{
    public class Category : BaseCategoryObject<Category, CategoryItem>, INotifyPropertyChanged
    {
        private CategoryControl _control;
        
        public bool IsMainCategory => Parent == null;
        public Category SubCategory { get; private set; }
        public bool HasSubCategory => SubCategory != null;
        public bool IsLastCategory => !HasSubCategory;
        public CategoryControl Control => _control;

        private Category() : base()
        {
            // If this constructor is used, ID will be by default -1 in the constructor of the base class,
            // which generates a new ID for this Category object.

            _control = new CategoryControl();
            _control.LoadCategoryData(this);
        }

        private Category(int id) : base(id)
        {
            // If this constructor is used, the id omitted will be used and no new ID will be generated,
            // except 'id' is smaller than 0 (which shouldn't happen).

            _control = new CategoryControl();
            _control.LoadCategoryData(this);
        }

        public static Category CreateCategory(string name, string description = "")
        {
            LogDebug($"OrderCategory::[static]CreateCategory({name})");

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException("name");
                
                Category category = new Category()
                {
                    Name = name,
                    Description = description
                };

                return category;
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"OrderCategory::[static]{GetCaller()} - Failed to create category.");
                return null;
            }
        }

        private static Category LoadCategory(Category parent, int id, string name, string description = "")
        {
            LogDebug($"OrderCategory::[static]LoadCategory({(parent != null ? parent.Name : "[null]")}, {id}, {name})");

            try
            {
                if (parent != null)
                    if (parent.HasSubCategory)
                        throw new Exception($"'{parent.Name}' already has a subcategory");
                
                Category category = new Category(id)
                {
                    Name = name,
                    Description = description,
                    Parent = parent
                };

                if (id > _nextId)
                    _nextId = id++;

                return category;
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"OrderCategory::[static]{GetCaller()} - Failed to load Category data.");
                return null;
            }
        }

        private static void LoadCategoryDataRecursive(Category parent, XElement categoryElement)
        {
            try
            {
                Category category = LoadCategory(parent,
                    int.Parse(categoryElement.Element("id").Value, System.Globalization.NumberStyles.HexNumber),
                    categoryElement.Element("name").Value,
                    categoryElement.Element("description").Value);
                if (category != null)
                {
                    CategoryObjects.Add(category);

                    if (categoryElement.Element("subcategory") != null)
                        LoadCategoryDataRecursive(category, categoryElement.Element("subcategory"));
                }
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"OrderCategory::[static]{GetCaller()} - Failed to read element: {categoryElement}");
            }
        }

        public void SaveCategoryRecursive(ref XElement parentElement)
        {
            try
            {
                XElement categoryitemelement = new XElement("category");
                categoryitemelement.Add(new XElement("id"), Id.ToString("X"));
                categoryitemelement.Add(new XElement("name"), Name);
                categoryitemelement.Add(new XElement("description"), Description);

                SubCategory.SaveCategoryRecursive(ref categoryitemelement);

                parentElement.Add(categoryitemelement);
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"OrderCategory::[static]{GetCaller()} - Failed to save category {this} to {parentElement}");
            }
        }

        public static void InitializeCategories(XElement categoryList)
        {
            LogDebug("OrderCategory::[static]InitializeCategories({0})", categoryList);

            try
            {
                foreach (XElement categoryElement in categoryList.Elements("maincategory"))
                    LoadCategoryDataRecursive(null, categoryElement);
            }
            catch (Exception ex)
            {
                LogGenericError(ex);
                LogGenericError($"OrderCategory::[static]{GetCaller()} - Failed to initialize categories.");
            }
        }
    }
}
