using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using eHCMSCommon.Utilities;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
/*
 * 20170927 #001 CMN: Added DeadReason
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IEnumListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EnumListingViewModel : Conductor<object>, IEnumListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EnumListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            AddSelectOneItem = true;
            AddSelectedAllItem = false;
            FirstItemAsDefault = true;
        }
        
        public void LoadData()
        {
            var enumList = new ObservableCollection<EnumDescription>();

            Array enumValues = Enum.GetValues(EnumType);

            if(enumValues != null)
            {
                foreach (Enum enumValue in enumValues)
                {
                    bool flag = true;
                    if (ExclusionValues != null && ExclusionValues.Count>0)
                    {
                        foreach(var st in ExclusionValues)
                        {
                            if (enumValue.ToString() == st)
                            {
                                flag = false;
                                break;
                            }
                        }                        
                    }
                    
                    if(!flag)
                    {
                        continue;
                    }
                    var obj = new EnumDescription();
                     obj.Description = Helpers.GetEnumDescription(enumValue);
                     obj.EnumItem = enumValue;
                     obj.EnumValue = enumValue.ToString();

                    enumList.Add(obj);
                }
            }
            if (AddSelectOneItem)
            {
                var enumDescription = new EnumDescription
                                          {
                                              Description = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri), EnumValue = "-1"
                                          };
                enumList.Insert(0, enumDescription);
            }
            if (AddSelectedAllItem)
            {
                var enumDescription = new EnumDescription
                                          {
                                              Description = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa), EnumValue = "-2"
                                          };
                enumList.Insert(0, enumDescription);
            }
            EnumItemList = enumList;

            if (EnumItemList != null && FirstItemAsDefault && EnumItemList.Count > 0)
            {
                SelectedItem = EnumItemList[0];
            }
        }

        private ObservableCollection<EnumDescription> _enumItemList;

        public ObservableCollection<EnumDescription> EnumItemList
        {
            get { return _enumItemList; }
            set
            {
                _enumItemList = value;
                NotifyOfPropertyChange(() => EnumItemList);
            }
        }

        private ObservableCollection<string> _ExclusionValues;
        public ObservableCollection<string> ExclusionValues
        {
            get { return _ExclusionValues; }
            set
            {
                _ExclusionValues = value;
                NotifyOfPropertyChange(() => ExclusionValues);
            }
        }

        private EnumDescription _selectedItem;
        public EnumDescription SelectedItem
        {
            get { return _selectedItem; }
            set 
            {
                if (_selectedItem == value)
                {
                    return;
                }
                _selectedItem = value;
                NotifyOfPropertyChange(()=>SelectedItem);

                if (SelectionChange!=null)
                {
                    SelectionChange(this, null); 
                }

                /*▼====: #001*/
                if (this.EnumType == typeof(AllLookupValues.DischargeCondition) && SelectedItem != null && SelectedItem.EnumItem != null)
                {
                    Globals.EventAggregator.Publish(new DischargeConditionChange { Item = (AllLookupValues.DischargeCondition)SelectedItem.EnumItem });
                }
                /*▲====: #001*/
            }
        }

        public event EventHandler SelectionChange;
        
        private Type _enumType;
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                _enumType = value;
            NotifyOfPropertyChange(() => EnumType);
            }
        }

        public bool AddSelectOneItem { get; set; }

        public bool AddSelectedAllItem { get; set; }


        public void SetSelectedID(string itemID)
        {
            if (EnumItemList != null)
            {
                EnumDescription foundItem = null;
                foreach (var item in EnumItemList)
                {
                    if (item.EnumValue == itemID)
                    {
                        foundItem = item;
                        break;
                    }
                }
                if(foundItem != null)
                {
                    SelectedItem = foundItem;
                }
                else
                {
                    if (FirstItemAsDefault && EnumItemList.Count > 0)
                    {
                        SelectedItem = EnumItemList[0];
                    }
                }
            }
        }

        public bool FirstItemAsDefault { get; set; }
    }
}

