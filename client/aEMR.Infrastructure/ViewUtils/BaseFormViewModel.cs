using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.Windsor;
using FluentValidation;

namespace aEMR.Infrastructure.ViewUtils
{

    public abstract class BaseFormViewModel<T> : CommonView<T>
    {
        private T _data;

        protected BaseFormViewModel(IWindsorContainer container) : base(container)
        {
        }

        public T Data
        {
            get { return _data; }
            set
            {
                //if (Equals(value, _data)) return;
                _data = value;
                NotifyOfPropertyChange(() => Data);
            }
        }

        public object CustomData { get; set; }

        public abstract void Edit(T data);
        public abstract void PrintCmd();
        public abstract void ClearCmd();
        public abstract int SaveCmd();
        public abstract void ProcessSaveResult(T data,object obj);
        

        
    }
}
