using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;

namespace aEMR.Controls
{
    public class KeyEnabledComboBox : AxComboBox
    {
        public KeyEnabledComboBox()
            : base()
        {
            SetKeyboardSelection();
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            
            base.PrepareContainerForItemOverride(element, item);

            ComboBoxItem temp = element as ComboBoxItem;
            if (IsKeyboardEnabled && _keyEnabled)
            {
                temp.KeyUp += keyPressSearch;
            }
            else if (IsKeyboardEnabled && !_keyEnabled)
            {
                temp.KeyUp -= keyPressSearch;
            }
        }

        private bool _DoneSetKbSelection = false;
        public bool DoneSetKbSelection
        {
            get
            {
                return _DoneSetKbSelection;
            }
            set
            {
                _DoneSetKbSelection = value;
            }
        }
        private bool _keyEnabled = true;
        string searchStringEnabled = "KeyboardSelectionEnabled";
        
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }
       
        protected string ComboBoxTag
        {
            get
            {
                return this.Tag == null ? "" : this.Tag.ToString();
            }
        }
        protected bool IsKeyboardEnabled 
        {
            get
            {
                return ComboBoxTag.Contains(searchStringEnabled);
            }
        }
        protected KeyEventHandler keyPressSearch;
        public void SetKeyboardSelection()
        {
            #region KeyPressSearch
            keyPressSearch = delegate(object sender, KeyEventArgs e)
            {
                string key = e.Key.ToString();
                System.Diagnostics.Debug.WriteLine("KeyEnabledComboBox::keyPressSearch =======> Key = [{0}] pressed.", key);
                if (key.Length > 1 && (key.StartsWith("D") || key.StartsWith("NumPad")))
                { //remove the D/NumPad prefix to get the digit
                    key = key.Replace("NumPad", "").Replace("D", "");
                }
                else if (key.Length > 1)
                {
                    this.Tag = searchStringEnabled + "||";
                    return;
                }
                string searchHistoryPartsString = this.Tag == null ? searchStringEnabled + "||" : this.Tag.ToString();
                string[] searchHistoryParts = (searchHistoryPartsString.Contains("|")) ? searchHistoryPartsString.Split('|') : new string[0];

                int historyExpiration = 1000; 	//In 1 second, clear the history, 
                //and start new...
                string searchStringHistory = searchHistoryParts.Length == 3 ?
                        searchHistoryParts[1] : "";
                string searchStringTimeStampString = searchHistoryParts.Length == 3 ?
                        searchHistoryParts[2] : "";
                DateTime searchStringTimeStamp;
                string searchString = key;

                if (DateTime.TryParse(searchStringTimeStampString, out searchStringTimeStamp) && DateTime.Now.Subtract(searchStringTimeStamp).TotalMilliseconds < historyExpiration)
                {   //search history is valid and has not yet expired...
                    searchString = searchStringHistory + key;
                }

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].GetType() == typeof(ComboBoxItem))
                    {
                        if (((ComboBoxItem)this.Items[i]).Content.ToString().StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.SelectedIndex = i;
                            this.Tag = searchStringEnabled + "|" + searchString + "|" + DateTime.Now;
                            System.Diagnostics.Debug.WriteLine("KeyEnabledComboBox::keyPressSearch =======> Key = [{0}] pressed TRIGGER 1 SELECTEDINDEX = [{1}].", key, this.SelectedIndex);
                            break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.DisplayMemberPath))
                        {
                            //This item is bound from datasource
                            object context = this.Items[i];
                            string currentText = "";
                            PropertyInfo property = context.GetType().GetProperty(this.DisplayMemberPath);
                            currentText = (string)property.GetValue(context, null);

                            if (!string.IsNullOrEmpty(currentText) && 
                                currentText.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                this.SelectedIndex = i;
                                this.Tag = searchStringEnabled + "|" + searchString + "|" + DateTime.Now;
                                System.Diagnostics.Debug.WriteLine("KeyEnabledComboBox::keyPressSearch =======> Key = [{0}] pressed TRIGGER 2 SELECTEDINDEX = [{1}].", key, this.SelectedIndex);
                                break;
                            }
                        }
                    }
                }
            };
            #endregion

            if (!IsKeyboardEnabled && _keyEnabled)
            {
                this.Tag = searchStringEnabled + "||";

                //Reset the search history on open and close
                this.DropDownOpened += delegate
                {
                    this.Tag = searchStringEnabled + "||";
                };
                this.DropDownClosed += delegate
                {
                    this.Tag = searchStringEnabled + "||";
                };

                //Add handler to parent control, so that we search even 
                //when combobox is closed, yet focused
                this.KeyUp += keyPressSearch;

                int nSetCnt = 0;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].GetType() == typeof(ComboBoxItem))
                    {
                        ((ComboBoxItem)this.Items[i]).KeyUp += keyPressSearch;
                        ++nSetCnt;
                    }
                }
                if (nSetCnt > 0)
                {
                    DoneSetKbSelection = true;
                }
                
            }
            else if (IsKeyboardEnabled && !_keyEnabled)
            {
                //Remove handler
                this.KeyUp -= keyPressSearch;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].GetType() == typeof(ComboBoxItem))
                    {
                        ((ComboBoxItem)this.Items[i]).KeyUp -= keyPressSearch;
                    }
                }
                this.Tag = "";
            }
            else
            {
                //Remove handler
                this.KeyUp -= keyPressSearch;
                this.Tag = "";
            }

            

        }

    }
}
