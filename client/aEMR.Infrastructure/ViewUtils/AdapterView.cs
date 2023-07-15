using Caliburn.Micro;

namespace aEMR.Infrastructure.ViewUtils
{
    public class AdapterView<T> : PropertyChangedBase, IDataPresenter
    {
        public AdapterView(T data)
        {
            DataView = data;
        }

        public T DataView { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private object _firstDisplay;
        public object FirstDisplay
        {
            get { return _firstDisplay; }
            set
            {
                _firstDisplay = value;
                NotifyOfPropertyChange(() => FirstDisplay);
            }
        }

        private object _secondDisplay;
        public object SecondDisplay
        {
            get { return _secondDisplay; }
            set
            {
                _secondDisplay = value;
                NotifyOfPropertyChange(() => SecondDisplay);
            }
        }

        private object _thirdDisplay;
        public object ThirdDisplay
        {
            get { return _thirdDisplay; }
            set
            {
                _thirdDisplay = value;
                NotifyOfPropertyChange(() => ThirdDisplay);
            }
        }

        private bool _canRemove;
        public bool CanRemove
        {
            get { return _canRemove; }
            set
            {
                _canRemove = value;
                NotifyOfPropertyChange(() => CanRemove);
            }
        }
        private bool _canVisibily;
        public bool CanVisibily
        {
            get { return _canVisibily; }
            set
            {
                _canVisibily = value;
                NotifyOfPropertyChange(() => CanVisibily);
            }
        }


        private bool _changeBackground;
        public bool ChangeBackground
        {
            get { return _changeBackground; }
            set
            {
                _changeBackground = value;
                NotifyOfPropertyChange(() => ChangeBackground);
            }
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;

            if (obj is AdapterView<T> && !(null == DataView))
            {
                isEqual = this.DataView.Equals(((AdapterView<T>)obj).DataView);
            }

            return isEqual;
        }

        public override int GetHashCode()
        {
            return DataView.GetHashCode();
        }
    }
}
