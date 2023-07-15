using System;
using Newtonsoft.Json;

namespace DataEntities
{
    public class OrderDTO
    {
        public static readonly string WAITING_STATUS = "WAITING";
        public static readonly string CALLING_STATUS = "CALLING";
        public static readonly string MISSING_STATUS = "MISSING";
        public static readonly string DONE_STATUS = "DONE";
        public static readonly string AVAILABLE_RESULT_STATUS = "AVAILABLE_RESULT";
        public static readonly string RECEIVED_RESULT_STATUS = "RECEIVED_RESULT";
        public static readonly string NOT_PAY_YET_STATUS = "NOT_PAY_YET";
        public static readonly string WAITING_FOR_VITAL_SIGNS = "WAITING_FOR_VITAL_SIGNS";
        public static readonly string HIGHT = "HIGHT";
        public static readonly string NORMAL = "NORMAL";
        public static readonly string PUBLISH_ORDER_URI = @"api/publish-order";
        public static readonly string DEFAULT_DATE_FORMAT = "yyyy-MM-dd";
        public static readonly string DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private static readonly string PUT_PATH = @"api/publish-order/order-status/{0}/patient-id/{1}/ref-lo-id/{2}/ref-dept-id/{3}";
        private static readonly string GET_NEXT_WAITING_ORDER_URI = @"api/publish-order/ref-location-id/{0}/order-status/{1}";
        private static readonly string GET_ORDER_BY_PATIENT_CODE_URI = @"api/publish-order/ref-location-id/{0}/patient-code/{1}";
        private static readonly string GET_ORDER_BY_PATIENT_CODE_REF_DEPT_URI = @"api/publish-order/ref-dept-id/{0}/patient-code/{1}";
        public static readonly string GET_IS_ENABLE_CHECKBOX = @"api/publish-order/rooms/isenable_checkbox_priority";
        public static readonly string GET_ORDER_BY_ORDER_NUMBER_DEPT_ID = @"api/publish-order/ref-dept-id/{0}/order-number/{1}";
        public static readonly string GET_ORDER_BY_ORDER_NUMBER_DEPT_EXCLUDE_STATUS = @"api/publish-order/ref-dept-id/{0}/patient-code/{1}/exclude-status/{2}";
        private static readonly string UPDATE_LAST_ORDER_URI = @"api/publish-order/order-status/{0}/patient-id/{1}/ref-dept-id/{2}";

        public static string UpdateLastOrderUrl(string orderStatus, long patientId, long refDeptId)
        {
            if (String.IsNullOrEmpty(orderStatus) || 0 == patientId || 0 == refDeptId)
            {
                return null;
            }
            return String.Format(UPDATE_LAST_ORDER_URI, orderStatus, patientId, refDeptId);
        }

        public static string GetOrderByOrderNumberAndDeptIdAndExcludeStatusUrl(long refDepId, string patientCode, string excludeStatus)
        {
            if (0 == refDepId || String.IsNullOrEmpty(patientCode) || String.IsNullOrEmpty(excludeStatus))
            {
                return null;
            }
            return String.Format(GET_ORDER_BY_ORDER_NUMBER_DEPT_EXCLUDE_STATUS, refDepId, patientCode, excludeStatus);
        }

        public static string GetOrderByOrderNumberAndDeptIdUrl(long refDepId, long orderNumber)
        {
            if (0 == refDepId || 0 == orderNumber)
            {
                return null;
            }
            return String.Format(GET_ORDER_BY_ORDER_NUMBER_DEPT_ID, refDepId, orderNumber);
        }

        public static string GetOrderByPatientCodeAndDeptIdUri(long refDepId, string patientCode)
        {
            if (0 == refDepId || String.IsNullOrEmpty(patientCode))
            {
                return null;
            }
            return String.Format(GET_ORDER_BY_PATIENT_CODE_REF_DEPT_URI, refDepId, patientCode);
        }

        public static string GetOrderByPatientCodeUri(long refLocationId, string patientCode)
        {
            if (0 == refLocationId || String.IsNullOrEmpty(patientCode))
            {
                return null;
            }
            return String.Format(GET_ORDER_BY_PATIENT_CODE_URI, refLocationId, patientCode);
        }

        public static string GetNextWaitingOrderUri(long refLocationId, bool isOutpatientDept)
        {
            if (0 == refLocationId)
            {
                return null;
            }

            string status = isOutpatientDept ? WAITING_FOR_VITAL_SIGNS : WAITING_STATUS;

            return String.Format(GET_NEXT_WAITING_ORDER_URI, refLocationId, status);
        }

        public static string GetIsEnableCheckBox()
        {
            return String.Format(GET_IS_ENABLE_CHECKBOX);
        }

        public OrderDTO()
        { }

        public OrderDTO(string description, string orderStatus, bool hightPriority,
            DateTime patientDOB, long patientId, string patientName,
            long? ptRegDetailId, long? refDeptId, long? refLocationId)
        {
            this.description = description;
            this.orderStatus = String.IsNullOrEmpty(orderStatus) ? WAITING_STATUS : orderStatus;
            this.hightPriority = hightPriority;
            if (null != patientDOB)
            {
                this.patientDOB = patientDOB.ToString(DEFAULT_DATE_FORMAT);
            }
            this.patientId = patientId;
            this.patientName = patientName;
            this.ptRegDetailId = ptRegDetailId;
            this.refDeptId = refDeptId;
            this.refLocationId = refLocationId;
            this.priority = hightPriority ? HIGHT : NORMAL;
        }

        public OrderDTO(string description, string orderStatus, bool hightPriority,
            DateTime patientDOB, long patientId, string patientName, DateTime startedServiceAt,
            long? ptRegDetailId = 0, long? refDeptId = 0, long? refLocationId = 0)
        {
            this.description = description;
            this.orderStatus = String.IsNullOrEmpty(orderStatus) ? WAITING_STATUS : orderStatus;
            this.hightPriority = hightPriority;
            if (null != patientDOB)
            {
                this.patientDOB = patientDOB.ToString(DEFAULT_DATE_FORMAT);
            }
            this.patientId = patientId;
            this.patientName = patientName;
            this.ptRegDetailId = ptRegDetailId;
            this.refDeptId = refDeptId;
            this.refLocationId = refLocationId;
            this.priority = hightPriority ? HIGHT : NORMAL;

            if (null != startedServiceAt)
            {
                this.startedServiceAt = startedServiceAt.ToString(DEFAULT_DATE_TIME_FORMAT);
            }
        }

        public static OrderDTO ToDTO(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<OrderDTO>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this,
                Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public string GetPutPath()
        {
            return String.Format(PUT_PATH, orderStatus, patientId, refLocationId, refDeptId);
        }

        [JsonProperty("id")]
        public long? id { get; set; }
        [JsonProperty("orderNumber")]
        public long? orderNumber { get; set; }
        [JsonProperty("priority")]
        public string priority { get; set; }
        [JsonProperty("patientId")]
        public long? patientId { get; set; }
        [JsonProperty("patientCode")]
        public string patientCode { get; set; }
        [JsonProperty("ptRegDetailId")]
        public long? ptRegDetailId { get; set; }
        [JsonProperty("patientName")]
        public string patientName { get; set; }
        [JsonProperty("patientDOB")]
        public string patientDOB { get; set; }
        [JsonProperty("orderStatus")]
        public string orderStatus { get; set; }
        [JsonProperty("floorNumber")]
        public int? floorNumber { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        public bool hightPriority { get; set; }
        [JsonProperty("refDeptId")]
        public long? refDeptId { get; set; }
        [JsonProperty("refLocationId")]
        public long? refLocationId { get; set; }
        public string startedServiceAt { get; set; }
        public string endedServiceAt { get; set; }
        public string createdAt { get; set; }
        public long? roomId  { get; set; }
    }
}
