using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace aEMR.Controls
{
    public class AxTextBoxShorthand : TextBox
    {
        public static readonly DependencyProperty ItemSourceExProperty =
            DependencyProperty.Register(
                "ItemSourceEx",
                typeof(Dictionary<string, string>),
                typeof(AxTextBoxShorthand),
                new PropertyMetadata((s, e) =>
                {
                    var mComboBox = s as AxTextBoxShorthand;
                    mComboBox.gPersionalDict = (Dictionary<string, string>)e.NewValue;
                }));
        public Dictionary<string, string> ItemSourceEx
        {
            get { return (Dictionary<string, string>)GetValue(ItemSourceExProperty); }
            set { SetValue(ItemSourceExProperty, value); }
        }
        public Dictionary<string, string> gPersionalDict = new Dictionary<string, string>();
        public AxTextBoxShorthand() : base()
        {
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            try
            {
                var mIndex = this.CaretIndex;
                if ((e.Key == Key.Space || e.Key == Key.OemComma || e.Key == Key.Oem1) && mIndex > 0)
                {
                    string mText = this.Text.Substring(0, mIndex).TrimEnd(' ');
                    int mLastIndex = mText.LastIndexOf(' ');
                    mLastIndex++;
                    string mCharacter = mText.Substring(mLastIndex, mText.Length - mLastIndex);
                    string mOrgCharacter = mText.Substring(mLastIndex, mText.Length - mLastIndex);
                    if (mCharacter.Length > 0)
                    {
                        mCharacter = mCharacter.ToLower();
                    }
                    if (mCharacter != null && mCharacter.Length > 0 && gPersionalDict.ContainsKey(mCharacter))
                    {
                        BindingExpression be = this.GetBindingExpression(TextBox.TextProperty);
                        this.Text = this.Text.Remove(mLastIndex, mCharacter.Length);
                        this.Text = this.Text.Insert(mLastIndex, gPersionalDict[mCharacter]);
                        be.UpdateSource();
                        this.Select(mIndex + (gPersionalDict[mCharacter].Length - mOrgCharacter.Length), 0);
                    }
                }
            }
            catch { }
            base.OnPreviewKeyDown(e);
        }
    }
}