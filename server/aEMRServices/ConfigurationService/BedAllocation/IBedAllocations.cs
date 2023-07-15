using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ErrorLibrary;
using DataEntities;
/*
 * 20210813 #001 TNHX: Thêm func cập nhật giường
 */
namespace ConfigurationManagerService
{
    [ServiceContract]
    public interface IBedAllocations
    {
        #region RefDepartments
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefDepartments> GetRefDepartments();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> GetDeptLocationTreeView();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewRoomPrices(long DeptLocationID
                                                , long StaffID
                                                , DateTime? EffectiveDate
                                                , Decimal NormalPrice
                                                , Decimal PriceForHIPatient
                                                , Decimal HIAllowedPrice
                                                , string Note
                                                , bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RoomPrices> GetAllRoomPricesByDeptID(long DeptLocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewBedAllocation(int times, long DeptLocationID
                                                   , string BedNumber
                                                   , long MedServiceID
                                                    ,string BAGuid
                                                   , long V_BedLocType
                                                   , bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateBedAllocationMedSer(string BAGuid, long MedServiceID, long V_BedLocType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedAllocation> GetAllBedAllocationByDeptID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefStaffCategory> GetAllRefStaffCategories();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Staff> GetAllStaff(long refStaffCateID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteBedAllocation(long BedAllocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string UpdateBedAllocation(IList<BedAllocation> LstBedAllocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewBedAllocationList(IList<BedAllocation> LstBedAllocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateBedAllocationList(IList<BedAllocation> LstBedAllocation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> GetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedAllocation> GetCountBedAllocByDeptID(long DeptLocationID, int Choice);
        #endregion
        #region bedloctype
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateBedLocType(long BedLocTypeID
                                                , string BedLocTypeName
                                                , string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewBedLocType(string BedLocTypeName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteBedLocType(long BedLocTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedAllocType> GetAllBedLocType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedAllocation> GetAllBedAllocByDeptID(long DeptLocationID, int IsActive,out int Total);
        #endregion
        #region bed patient alloc
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddNewBedPatientAllocs(BedPatientAllocs alloc);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditBedPatientAllocs(BedPatientAllocs alloc, long LoggedStaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteBedPatientAllocs(long BedPatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedPatientAllocs> GetAllBedPatientAllocByDeptID(long DeptLocationID, bool IsReadOnly, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MarkDeleteBedPatientAlloc(long bedPatientID);
        #endregion

    }
}
