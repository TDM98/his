using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
/*
 * 20201019 #001 TNHX: Thêm tên đơn vị (lấy từ nơi làm việc) + Thêm trường Extra2: ID + FullName của User đẩy HDDT + Bỏ mặc định email + Fix lỗi return null của ConvertToDefaultValue 
 */
namespace DataEntities
{
    public class RptOutPtTransactionFinalizationDetail
    {
        public long IdxTemp12Code { get; set; }
        public string HITypeDescription { get; set; }
        public decimal TotalPatientPayment { get; set; }
        public bool IsHasVAT { get; set; }
        public double VATPercent { get; set; }
        public long CharityOrgID { get; set; }
    }
    public class VNPTCustomer
    {
        public VNPTCustomer()
        {
        }
        public VNPTCustomer(Patient aPatient, string TaxMemberAddress = null)
        {
            Code = aPatient.PatientCode;
            Name = aPatient.FullName;
            Address = !string.IsNullOrEmpty(aPatient.PatientFullStreetAddress) ? aPatient.PatientFullStreetAddress : (!string.IsNullOrEmpty(TaxMemberAddress) ? TaxMemberAddress : aPatient.PatientStreetAddress + (string.IsNullOrEmpty(aPatient.PatientSurburb) ? "" : ", " + aPatient.PatientSurburb) + (aPatient.WardName != null && aPatient.WardName.WardNameID > 0 ? ", " + aPatient.WardName.WardName : ""));
            Phone = aPatient.PatientPhoneNumber;
            //▼====: #001
            //Email = string.IsNullOrEmpty(aPatient.PatientEmailAddress) ? string.Format("{0}@hospital.com", aPatient.PatientCode) : aPatient.PatientEmailAddress;
            Email = string.IsNullOrEmpty(aPatient.PatientEmailAddress) ? "" : aPatient.PatientEmailAddress;
            //▲====: #001
            CusType = VNPTCusType.Personal;
        }
        public VNPTCustomer(string XmlData)
        {
            if (string.IsNullOrEmpty(XmlData) || XmlData.StartsWith("ERR"))
            {
                return;
            }
            TextReader mTextReader = new StringReader(XmlData);
            XDocument mXDocument = XDocument.Load(mTextReader);
            if (mXDocument.Element("Data") != null)
            {
                XElement mData = mXDocument.Element("Data");
                if (mData.Element("code") != null)
                {
                    Code = mData.Element("code").Value;
                }
                if (mData.Element("name") != null)
                {
                    Name = mData.Element("name").Value;
                }
                if (mData.Element("address") != null)
                {
                    Address = mData.Element("address").Value;
                }
                if (mData.Element("phone") != null)
                {
                    Phone = mData.Element("phone").Value;
                }
                if (mData.Element("taxcode") != null)
                {
                    TaxCode = mData.Element("taxcode").Value;
                }
                if (mData.Element("email") != null)
                {
                    Email = mData.Element("email").Value;
                }
            }
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string TaxCode { get; set; }
        public string Email { get; set; }
        public string BankAccountName { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }
        public string RepresentPerson { get; set; }
        public VNPTCusType CusType { get; set; }
        public string ToXML()
        {
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("Customers",
                    new XElement("Customer",
                        new XElement("Name", Name),
                        new XElement("Code", Code),
                        new XElement("TaxCode", ConvertToDefaultValue(TaxCode)),
                        new XElement("Address", Address),
                        new XElement("BankAccountName", ConvertToDefaultValue(BankAccountName)),
                        new XElement("BankName", ConvertToDefaultValue(BankName)),
                        new XElement("BankNumber", ConvertToDefaultValue(BankNumber)),
                        new XElement("Email", Email),
                        new XElement("Fax", ConvertToDefaultValue(Fax)),
                        new XElement("Phone", ConvertToDefaultValue(Phone)),
                        new XElement("ContactPerson", ConvertToDefaultValue(ContactPerson)),
                        new XElement("RepresentPerson", ConvertToDefaultValue(RepresentPerson)),
                        new XElement("CusType", (int)CusType)
            )));
            return xmlDocument.ToString();
        }
        private object ConvertToDefaultValue(string InputValue)
        {
            if (string.IsNullOrEmpty(InputValue))
            {
                return "";
            }
            return InputValue;
        }
    }
    public enum VNPTCusType : int
    {
        [Description("Cá nhân")]
        Personal = 0,
        [Description("Tổ chức")]
        Organize = 1
    }
    public class VNPTProduct
    {
        public string ProdName { get; set; }
        public string ProdUnit { get; set; }
        public decimal ProdQuantity { get; set; }
        public decimal ProdPrice { get; set; }
        public decimal Total { get; set; }
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
        public int VATRate { get; set; }
    }
    public class VNPTInvoice
    {
        private XElement ConvertProductCollectionToXml(List<VNPTProduct> aProductCollection)
        {
            return new XElement("Products", from item in aProductCollection
                select new XElement("Product",
                    new XElement("Remark", aProductCollection.IndexOf(item) + 1),
                    new XElement("ProdName", item.ProdName),
                    new XElement("ProdUnit", item.ProdUnit),
                    new XElement("ProdQuantity", item.ProdQuantity == 0 ? "" : item.ProdQuantity.ToString()),
                    new XElement("ProdPrice", item.ProdPrice == 0 ? "" : item.ProdPrice.ToString()),
                    new XElement("Total", item.Total),
                    new XElement("VATRate", item.VATRate == 0 ? "-1" : item.VATRate.ToString()),
                    new XElement("VATAmount", item.VATAmount == 0m ? "" : item.VATAmount.ToString()),
                    new XElement("Amount", item.Amount)));
        }
        private List<VNPTProduct> ConvertFinalizationDetailToProductCollection(List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection, out decimal VATRate, out decimal Total, out decimal VATAmount)
        {
            List<VNPTProduct> aProductCollection = new List<VNPTProduct>();
            aProductCollection = aRptOutPtTransactionFinalizationDetailCollection.Select(x => new VNPTProduct
            {
                ProdName = x.HITypeDescription,
                ProdUnit = eHCMSResources.K0771_G1_Lan,
                ProdQuantity = 1,
                ProdPrice = Math.Round(x.IsHasVAT && x.VATPercent > 1 ? x.TotalPatientPayment / Convert.ToDecimal(x.VATPercent) : x.TotalPatientPayment, 0, MidpointRounding.AwayFromZero),
                Total = Math.Round(x.IsHasVAT && x.VATPercent > 1 ? x.TotalPatientPayment / Convert.ToDecimal(x.VATPercent) : x.TotalPatientPayment, 0, MidpointRounding.AwayFromZero),
                Amount = x.TotalPatientPayment,
                VATAmount = x.TotalPatientPayment - Math.Round(x.IsHasVAT && x.VATPercent > 1 ? x.TotalPatientPayment / Convert.ToDecimal(x.VATPercent) : x.TotalPatientPayment, 0, MidpointRounding.AwayFromZero),
                VATRate = (x.IsHasVAT && x.VATPercent > 1 ? Convert.ToInt32((x.VATPercent - 1) * 100) : 0)
            }).ToList();
            VATRate = aRptOutPtTransactionFinalizationDetailCollection.Any(x => x.IsHasVAT && x.VATPercent > 1) ? Convert.ToDecimal(aRptOutPtTransactionFinalizationDetailCollection.First(x => x.IsHasVAT && x.VATPercent > 1).VATPercent - 1) : 0m;
            Total = aProductCollection.Sum(x => x.Total);
            VATAmount = Math.Round(aRptOutPtTransactionFinalizationDetailCollection.Sum(x => x.TotalPatientPayment) - Total, 0, MidpointRounding.AwayFromZero);
            return aProductCollection;
        }

        // 20200826 TNHX: Bỏ 3 dòng ra khỏi khi tạo mới HDDT do bên kia chỉnh điều kiện
        private string EntityToNewInvoiceXML(VNPTCustomer aCustomer, OutPtTransactionFinalization aTransactionFinalizationObj, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection)
        {
            decimal mVATRate;
            decimal mTotal;
            decimal mVATAmount;
            List<VNPTProduct> aProductCollection = ConvertFinalizationDetailToProductCollection(aRptOutPtTransactionFinalizationDetailCollection, out mVATRate, out mTotal, out mVATAmount);

            XDocument mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("Invoices", new XElement[] {
                    new XElement("Inv", new XElement[] {
                    new XElement("key", aTransactionFinalizationObj.InvoiceKey),
                    new XElement("Invoice", new XElement[] {
                        new XElement("CusCode", aCustomer.Code),
                        new XElement("Buyer", aTransactionFinalizationObj.IsTaxMemberSameWithPatient ? aTransactionFinalizationObj.TaxMemberName : (!aTransactionFinalizationObj.IsOrganization ? aTransactionFinalizationObj.PatientFullName : "")),
                        //▼====: #001
                        new XElement("CusName", aTransactionFinalizationObj.Buyer ??(!aTransactionFinalizationObj.IsTaxMemberSameWithPatient ? aTransactionFinalizationObj.TaxMemberName : "")),
                        //▲====: #001
                        new XElement("CusAddress", aTransactionFinalizationObj.TaxMemberAddress),
                        new XElement("CusPhone", aCustomer.Phone),
                        new XElement("CusTaxCode", aTransactionFinalizationObj.TaxCode),
                        new XElement("PaymentMethod", (aTransactionFinalizationObj.V_PaymentMode == (long)AllLookupValues.PaymentMode.TIEN_MAT ? "TM" : "CK")),
                        new XElement("CusBankNo", aTransactionFinalizationObj.BankAccountNumber),
                        new XElement("CusBankName", aCustomer.BankName),
                        ConvertProductCollectionToXml(aProductCollection),
                        //new XElement("VehicleNo", 0),
                        new XElement("GrossValue", aProductCollection.Where(x => x.VATRate == 0).Sum(x => x.Total)),
                        new XElement("GrossValue0", 0),
                        new XElement("GrossValue5", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.Total)),
                        new XElement("GrossValue8", aProductCollection.Where(x => x.VATRate == 8).Sum(x => x.Total)),
                        new XElement("GrossValue10", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.Total)),
                        new XElement("VatAmount5", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.VATAmount)),
                        new XElement("VatAmount8", aProductCollection.Where(x => x.VATRate == 8).Sum(x => x.VATAmount)),
                        new XElement("VatAmount10", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.VATAmount)),
                        //new XElement("ContNo", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.Amount)),
                        //new XElement("DocNo", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.Amount)),
                        new XElement("Total", mTotal),
                        new XElement("VATRate", mVATRate.ToString("#,#")),
                        new XElement("VATAmount", mVATAmount),
                        new XElement("Amount", mTotal + mVATAmount),
                        new XElement("AmountInWords", EntityGlobals.MoneyToString(mTotal + mVATAmount)),
                        new XElement("Extra", aTransactionFinalizationObj.InvoiceKey),
                        //▼====: #001
                        new XElement("Extra2", aTransactionFinalizationObj.StaffID + " - " + aTransactionFinalizationObj.CreatedStaff.FullName)
                        //▲====: #001
                    })
                })
            }));
            return mXDocument.ToString();
        }
        private string EntityToAdjustInvoiceXML(VNPTCustomer aCustomer, OutPtTransactionFinalization aTransactionFinalizationObj, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection, VNPTUpdateType aUpdateType)
        {
            decimal mVATRate;
            decimal mTotal;
            decimal mVATAmount;
            List<VNPTProduct> aProductCollection = ConvertFinalizationDetailToProductCollection(aRptOutPtTransactionFinalizationDetailCollection, out mVATRate, out mTotal, out mVATAmount);
            
            XDocument mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("AdjustInv", new XElement[] {
                    new XElement("key", aTransactionFinalizationObj.InvoiceKey),
                    new XElement("CusCode", aCustomer.Code),
                    new XElement("Buyer", aTransactionFinalizationObj.IsTaxMemberSameWithPatient ? aTransactionFinalizationObj.TaxMemberName : (!aTransactionFinalizationObj.IsOrganization ? aTransactionFinalizationObj.PatientFullName : "")),
                    //▼====: #001
                    new XElement("CusName", aTransactionFinalizationObj.Buyer ??(!aTransactionFinalizationObj.IsTaxMemberSameWithPatient ? aTransactionFinalizationObj.TaxMemberName : "")),
                    //▲====: #001
                    new XElement("CusAddress", aTransactionFinalizationObj.TaxMemberAddress),
                    new XElement("CusPhone", aCustomer.Phone),
                    new XElement("CusTaxCode", aTransactionFinalizationObj.TaxCode),
                    new XElement("PaymentMethod", (aTransactionFinalizationObj.V_PaymentMode == (long)AllLookupValues.PaymentMode.TIEN_MAT ? "TM" : "CK")),
                    new XElement("CusBankNo", aTransactionFinalizationObj.BankAccountNumber),
                    new XElement("CusBankName", aCustomer.BankName),
                    ConvertProductCollectionToXml(aProductCollection),
                    new XElement("VehicleNo", 0),
                    new XElement("GrossValue", aProductCollection.Where(x => x.VATRate == 0).Sum(x => x.Total)),
                    new XElement("GrossValue0", 0),
                    new XElement("GrossValue5", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.Total)),
                    new XElement("GrossValue10", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.Total)),
                    new XElement("VatAmount5", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.VATAmount)),
                    new XElement("VatAmount10", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.VATAmount)),
                    new XElement("ContNo", aProductCollection.Where(x => x.VATRate == 5).Sum(x => x.Amount)),
                    new XElement("DocNo", aProductCollection.Where(x => x.VATRate == 10).Sum(x => x.Amount)),
                    new XElement("Total", mTotal),
                    new XElement("VATRate", mVATRate.ToString("#,#")),
                    new XElement("VATAmount", mVATAmount),
                    new XElement("Amount", mTotal + mVATAmount),
                    new XElement("AmountInWords", EntityGlobals.MoneyToString(mTotal + mVATAmount)),
                    new XElement("Extra", aTransactionFinalizationObj.InvoiceKey),
                    new XElement("Type", (int)aUpdateType),
                    //▼====: #001
                    new XElement("Extra2", aTransactionFinalizationObj.StaffID + " - " + aTransactionFinalizationObj.CreatedStaff.FullName)
                    //▲====: #001
            }));
            return mXDocument.ToString();
        }
        public string ToXML(VNPTCustomer aCustomer, OutPtTransactionFinalization aTransactionFinalizationObj, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection)
        {
            return EntityToNewInvoiceXML(aCustomer, aTransactionFinalizationObj, aRptOutPtTransactionFinalizationDetailCollection);
        }
        public string ToXML(VNPTCustomer aCustomer, OutPtTransactionFinalization aTransactionFinalizationObj, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection, VNPTUpdateType Type)
        {
            return EntityToAdjustInvoiceXML(aCustomer, aTransactionFinalizationObj, aRptOutPtTransactionFinalizationDetailCollection, Type);
        }
    }
    public class EntityGlobals
    {
        public static string MoneyToString(decimal aMoney, string aMoneyPrefix = "")
        {
            eHCMS.Services.Core.NumberToLetterConverter mConverter = new eHCMS.Services.Core.NumberToLetterConverter();
            System.Globalization.CultureInfo mCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            decimal aTempMoney = 0;
            if (aMoney < 0)
            {
                aTempMoney = 0 - aMoney;
                aMoneyPrefix = string.Format("{0} ", "Âm");
            }
            else
            {
                aTempMoney = aMoney;
                aMoneyPrefix = "";
            }
            return aMoneyPrefix + mConverter.Convert(aTempMoney.ToString(), mCultureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], "Lẻ", "VNĐ");
        }
    }
    public enum VNPTUpdateType
    {
        Grow = 2,
        Reduce = 3,
        AdjInfo = 4
    }
}