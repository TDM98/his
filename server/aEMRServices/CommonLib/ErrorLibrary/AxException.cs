using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.Diagnostics;
using System.Reflection;
using Service.Core.HelperClasses;
using AxLogging;

namespace ErrorLibrary
{
    [DataContract]
    public class AxException
    {
        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string MethodName { get; set; }
        [DataMember]
        public string ModuleName { get; set; }
        [DataMember]
        public AxException InnerException { get; set; }

        private string _ErrorCode;
        [DataMember]
        public string ErrorCode 
        {
            get
            {
                return _ErrorCode;
            }
            set
            {
                _ErrorCode = value;
                _ErrorMessage = Resources.ErrorStrings.GetString(_ErrorCode,"");
            }
        }

        private string _ErrorMessage;
        [DataMember]
        public string ErrorMessage 
        { 
            get
            {
                return _ErrorMessage;
            }
            private set
            {
                _ErrorMessage = value;
            }
        }

        private AxAssemblyInfo _AssemblyInfo;
        public AxAssemblyInfo AssemblyInfo
        {
            get
            {
                return _AssemblyInfo;
            }
        }
        public string WrappedErrorMessage { get; set; }
        private MethodBase _ParentMethod;
        /// <summary>
        /// Method tao ra doi tuong AxException nay.
        /// </summary>
        public MethodBase ParentMethod
        {
            get
            {
                return _ParentMethod;
            }
            private set
            {
                _ParentMethod = value;
                if (_ParentMethod != null)
                {
                    _AssemblyInfo = AxAssemblyHelper.GetAssemblyCustomInfo(_ParentMethod.Module.Assembly);

                    this.MethodName = _ParentMethod.Name;
                    this.ClassName = _ParentMethod.DeclaringType.FullName;
                    this.ModuleName = _AssemblyInfo.Product;
                }
                else
                {
                    _AssemblyInfo = null;

                    this.MethodName = "";
                    this.ClassName = "";
                }
            }
        }

        private List<StackFrame> _StackFrames;
        public List<StackFrame> StackFrames
        {
            get
            {
                return _StackFrames;
            }
        }
        
        public AxException()
        {
            _StackFrames = new List<StackFrame>();
        }
        public AxException(Exception ex):this()
        {
            WrappedErrorMessage = ex.ToString();
            StackTrace st = new StackTrace(ex, true);
            
            _StackFrames.AddRange(st.GetFrames());
        }
        public AxException(MethodBase parent):this()
        {
            ParentMethod = parent;
        }
        public AxException(Exception ex, MethodBase parent):this(ex)
        {
            ParentMethod = parent;
        }
        public AxException(Exception ex, MethodBase parent,string errCode)
            : this(ex,parent)
        {
            this.ErrorCode = errCode;
        }
        public AxException(Exception ex, string errCode)
            : this(ex)
        {
            this.ErrorCode = errCode;
#if SILVERLIGHT
            //Do nothing
#else
            //Get the method create this object.
            MethodBase thisMethod = MethodBase.GetCurrentMethod();
            StackTrace stackTrace = new StackTrace();
            MethodBase tempMethod;
            for (int i=1;i < stackTrace.FrameCount;i++)
            {
                tempMethod = stackTrace.GetFrame(i).GetMethod();
                if(tempMethod.DeclaringType != thisMethod.DeclaringType)
                {
                    ParentMethod = tempMethod;
                    break;
                }
            }
#endif
        }

        public override string ToString()
        {
            StringBuilder sBuilder = new StringBuilder();

            sBuilder.AppendFormat("{0}Error Message: {1}", Environment.NewLine, this.ErrorMessage);
            sBuilder.AppendFormat("{0}Wrapped Error Message: {1}", Environment.NewLine, this.WrappedErrorMessage);
            sBuilder.AppendFormat("{0}Assembly '{1}'", Environment.NewLine, this.AssemblyInfo.AssemblyName);
            sBuilder.AppendFormat("{0}Module '{1}'", Environment.NewLine, this.AssemblyInfo.Product);
            sBuilder.AppendFormat("{0}Method '{0}'", Environment.NewLine, this.MethodName);

            //Trace:
            if (this.StackFrames.Count > 0)
            {
                sBuilder.AppendFormat("{0}Stack Trace:", Environment.NewLine);
                foreach (StackFrame f in this.StackFrames)
                {
                    sBuilder.AppendFormat("{0}\tFile Name: {1}", Environment.NewLine, f.GetFileName());
                    sBuilder.AppendFormat("{0}\tLine Number: {1}", Environment.NewLine, f.GetFileLineNumber());
                    sBuilder.AppendFormat("{0}\tMethod Name: {1}", Environment.NewLine, f.GetMethod().Name);
                    sBuilder.AppendFormat("{0}\tAssembly Name: {1}", Environment.NewLine, f.GetMethod().Module.Assembly.FullName);
                }
            }

            return sBuilder.ToString();
        }

        public static AxException CatchExceptionAndLogMessage(Exception ex,string newErrorCode,object ndc_User = null)
        {
            AxException axErr = new AxException(ex, newErrorCode);
            AxLogger.Instance.LogDebug(axErr, ndc_User);

            return axErr;
        }
    }

    [DataContract]
    public class AxResponse
    {
        [DataMember]
        public AxException Exception { get; set; }
        public AxResponse()
        {
            this.Exception = null;
        }
        public AxResponse(AxException ex)
        {
            this.Exception = ex;
        }
        [DataMember]
        public object Result { get; set; }
    }
}
