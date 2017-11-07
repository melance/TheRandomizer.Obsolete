﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.ViewModels
{
    public class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, object> propertyValues = new Dictionary<string, object>();

        /// <summary>
        /// Returns true if the property exists in the PropertyValues dictionary
        /// </summary>
        /// <param name="propertyName">The name of the property to find</param>
        /// <returns>True if the property is in the dictionary; otherwise false</returns>
        protected virtual bool PropertyExists([CallerMemberName] string propertyName = null)
        {
            return propertyValues.ContainsKey(propertyName);
        }

        /// <summary>
        /// Returns the value of this property as stored in the PropertyValues dictionary
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="propertyName">The name of the property to retrieve the value of</param>
        /// <returns>The value of the property or the default value of the type <typeparamref name="T">T</typeparamref></returns>
        protected virtual T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyValues.ContainsKey(propertyName))
            {
                return (T)propertyValues[propertyName];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets the value of the property in the PropertyValues dictionary and raises the PropertyChanged event if necessary
        /// </summary>
        /// <typeparam name="T">The type of the value to store</typeparam>
        /// <param name="value">The value to be stored</param>
        /// <param name="propertyName">The name of the property to set</param>
        protected virtual void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (!propertyValues.ContainsKey(propertyName))
            {
                propertyValues.Add(propertyName, value);
                OnPropertyChanged(propertyName);
            }
            else if (propertyValues[propertyName] == null || !propertyValues[propertyName].Equals(value))
            {
                propertyValues[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChagned event
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
