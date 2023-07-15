using System;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel; //IEdittable
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-06
 * Contents: patient extension
/*******************************************************************/
#endregion

namespace DataEntities
{
    public partial class MedicalConditionRecord: IEditableObject
    {
        private MedicalConditionRecord copyData;

        #region Implements IEditableObject
        void IEditableObject.BeginEdit()
        {
            copyData = (MedicalConditionRecord)this.MemberwiseClone();
        }

        void IEditableObject.CancelEdit()
        {
            this.MCID = copyData.MCID;
            this.CommonMedRecID = copyData.CommonMedRecID;
            this.MCYesNo = copyData.MCYesNo;
            this.MCTextValue = copyData.MCTextValue;
            this.MCExplainOrNotes = copyData.MCExplainOrNotes;
            //this.RefMedicalCondition = new RefMedicalCondition();
            //this.RefMedicalCondition.MCDescription = copyData.RefMedicalCondition.MCDescription;
        }

        void IEditableObject.EndEdit()
        {
            copyData = new MedicalConditionRecord();
        }
        #endregion

    }
}
