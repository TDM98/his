using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eHCMS.ControlsLibrary
{
    public class AccordionMenu:Accordion
    {
        public AccordionMenu()
        {
            this.DefaultStyleKey = typeof(AccordionMenu);

        }
        public static DependencyProperty TestProperty = DependencyProperty.Register("Test", typeof(string), typeof(AccordionMenu), new PropertyMetadata(null));
        public string Test
        {
            get
            {
                return "ABCD";
            }
            set
            {
                SetValue(TestProperty, value);
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Accordion baseAccordion = this.GetTemplateChild("BaseAccordion") as Accordion;
            DataTemplate dt = (DataTemplate)baseAccordion.ItemTemplate;
         
               
        }


        public T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child.GetType() == typeof(T))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetChild<T>(child);
                    if (child != null && child.GetType() == typeof(T))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

    }
}
