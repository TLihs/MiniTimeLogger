using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MiniTimeLogger.Controls.PropertyControl.Base
{
    public abstract class BasePropertyControlItem<T1, T2> : Grid
        where T2 : Control
    {
        public event EventHandler<T1> ValueChanged;

        public void InvokeValueChanged(T1 newValue)
        {
            ValueChanged.Invoke(this, newValue);
        }

        private T1 _value;
        private string _name;
        private Label _nameLabel;
        protected T2 _valueControl;
        private ColumnDefinition _columnDefinitionLabel;
        private ColumnDefinition _columnDefinitionValue;

        public T1 Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value) && value != null)
                {
                    _value = value;
                    InvokeValueChanged(_value);
                }
            }
        }
        public string PropertyName
        {
            get => _name;
            set
            {
                if (value != _name && !string.IsNullOrWhiteSpace(value))
                {
                    _name = value;
                    _nameLabel.Content = _name;
                }
            }
        }
        public Label PropertyNameControl
        {
            get => _nameLabel;
        }
        public T2 ValueControl
        {
            get => _valueControl;
            set
            {
                if (_valueControl == null && value != null)
                {
                    _valueControl = value;
                    this.Children.Add(_valueControl);
                    Grid.SetColumn(_valueControl, 1);
                    Grid.SetColumnSpan(_valueControl, 1);
                }
            }
        }

        protected BasePropertyControlItem(string name, T1 defaultValue)
        {
            _columnDefinitionLabel = new ColumnDefinition() { Width = new GridLength(double.NaN, GridUnitType.Auto) };
            _columnDefinitionValue = new ColumnDefinition() { Width = new GridLength(double.NaN, GridUnitType.Auto) };

            _nameLabel = new Label();
            _nameLabel.Name = "Label_Name";
            this.Children.Add(_nameLabel);
            Grid.SetColumn(_nameLabel, 0);
            Grid.SetColumnSpan(_nameLabel, 1);

            PropertyName = name;
            Value = defaultValue;
            ValueChanged += OnValueChanged;
        }

        public abstract void OnValueChanged(object sender, T1 e);
    }
}
