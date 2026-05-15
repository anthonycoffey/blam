using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Blam.Models;

namespace Blam.ViewModels
{
    public class ScriptViewModel : ObservableObject
    {
        private bool _isSelected;
        public Script Script { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ScriptViewModel(Script script)
        {
            Script = script;
        }
    }
}
