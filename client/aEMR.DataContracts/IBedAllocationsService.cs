using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities;
using System.Collections.Generic;


namespace BedAllocationsProxy
{
    [ServiceContract]
    public interface  IBedAllocations
    {
        #region RefDepartments
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartments( AsyncCallback callback, object state);
        IList<RefDepartments> EndGetRefDepartments( IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptLocationByDeptID(long DeptID, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocationByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeptLocationTreeView(AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndGetDeptLocationTreeView(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewRoomPrices(long DeptLocationID
                                                , long StaffID
                                                , DateTime? EffectiveDate
                                                , Decimal NormalPrice
                                                , Decimal PriceForHIPatient
                                                , Decimal HIAllowedPrice
                                                , string Note
                                                , bool IsActive, AsyncCallback callback, object state);
        bool EndAddNewRoomPrices(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRoomPricesByDeptID(long DeptLocationID, AsyncCallback callback, object state);
        List<RoomPrices> EndGetAllRoomPricesByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewBedAllocation(int times, long DeptLocationID
                                                   , string BedNumber
                                                   , long MedServiceID
                                                    , string BAGuid
                                                   , long V_BedLocType
                                                   , bool IsActive, AsyncCallback callback, object state);
        bool EndAddNewBedAllocation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateBedAllocationMedSer(string BAGuid, long MedServiceID, long V_BedLocType, AsyncCallback callback, object state);
        bool EndUpdateBedAllocationMedSer(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllBedAllocationByDeptID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback, object state);
        List<BedAllocation> EndGetAllBedAllocationByDeptID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRefStaffCategories(AsyncCallback callback, object state);
        List<RefStaffCategory> EndGetAllRefStaffCategories(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaff(long refStaffCateID, AsyncCallback callback, object state);
        List<Staff> EndGetAllStaff(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteBedAllocation(long BedAllocationID, AsyncCallback callback, object state);
        bool EndDeleteBedAllocation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateBedAllocation(IList<BedAllocation> LstBedAllocation, AsyncCallback callback, object state);
        string EndUpdateBedAllocation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewBedAllocationList(ObservableCollection<BedAllocation> LstBedAllocation, AsyncCallback callback, object state);
        bool EndAddNewBedAllocationList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateBedAllocationList(ObservableCollection<BedAllocation> LstBedAllocation, AsyncCallback callback, object state);
        bool EndUpdateBedAllocationList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID, AsyncCallback callback, object state);
        List<MedServiceItemPrice> EndGetAllDeptMSItemsByDeptIDSerTypeID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCountBedAllocByDeptID(long DeptLocationID, int Choice, AsyncCallback callback, object state);
        List<BedAllocation> EndGetCountBedAllocByDeptID(IAsyncResult asyncResult);
        #endregion
        #region bedloctype
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateBedLocType(long BedLocTypeID
                                                , string BedLocTypeName
                                                , string Description, AsyncCallback callback, object state);
        bool EndUpdateBedLocType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewBedLocType(string BedLocTypeName, string Description, AsyncCallback callback, object state);
        bool EndAddNewBedLocType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteBedLocType(long BedLocTypeID, AsyncCallback callback, object state);
        bool EndDeleteBedLocType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllBedLocType(AsyncCallback callback, object state);
        List<BedAllocType> EndGetAllBedLocType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllBedAllocByDeptID(long DeptLocationID, int IsActive,  AsyncCallback callback, object state);
        List<BedAllocation> EndGetAllBedAllocByDeptID(out int Total, IAsyncResult asyncResult);
        #endregion
        #region bed patient alloc
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive, AsyncCallback callback, object state);
        bool EndUpdateBedPatientAllocs(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewBedPatientAllocs(BedPatientAllocs alloc, AsyncCallback callback, object state);
        int EndAddNewBedPatientAllocs(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginEditBedPatientAllocs(BedPatientAllocs alloc, long LoggedStaffID, AsyncCallback callback, object state);
        bool EndEditBedPatientAllocs(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteBedPatientAllocs(long BedPatientID, AsyncCallback callback, object state);
        bool EndDeleteBedPatientAllocs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllBedPatientAllocByDeptID(long DeptLocationID, bool IsReadOnly,  AsyncCallback callback, object state);
        List<BedPatientAllocs> EndGetAllBedPatientAllocByDeptID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginMarkDeleteBedPatientAlloc(long bedPatientID,  AsyncCallback callback, object state);
        bool EndMarkDeleteBedPatientAlloc(IAsyncResult asyncResult);
        #endregion

    }
}
