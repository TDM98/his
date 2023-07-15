using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Caliburn.Micro;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;

using aEMR.DataContracts;
using Castle.Core.Logging;
//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using System.Diagnostics;
using System.Reflection;

namespace aEMR.Infrastructure.Extensions
{
    public static class WcfCallExtensions
    {
        static IWindsorContainer Container { get { return IoC.Get<IWindsorContainer>(); } }
        static ILogger Logger { get { return Container.Resolve<ILogger>(); } }

        public static void CallWcfAsync<TProxy, TResult>(this TProxy proxy, Func<TProxy, TResult> method, Action<TResult, object> OnCompleted, Action<FaultException<AxException>> OnError, object state)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            string name = (methodBase.ReflectedType != null ? methodBase.ReflectedType.FullName : "") + "." + methodBase.Name;

            proxy.BeginWcfCall(
                method, async =>
                    {
                        try
                        {
                            var result = async.End();
                            OnCompleted(result, state);
                        }
                        catch (FaultException<AxException> axEx)
                        {
                            //Sau nay show chi tiet hon.
                            // TxD 01/06/2018: Commented OUT the following because aEMR.Infrastructure CANNOT reference ViewContracts 
                            //                 (causing circular reference) to get to NavigationService
                            //var navigationService = IoC.Get<INavigationService>();
                            //navigationService.ShowMessage(axEx.Detail.ErrorMessage);

                            if (OnError != null)
                            {
                                OnError(axEx);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            var axEx = new FaultException<AxException>(new AxException(), new FaultReason("Wcf service error"));

                            // TxD 01/06/2018: Commented OUT the following because aEMR.Infrastructure CANNOT reference ViewContracts 
                            //                 (causing circular reference) to get to NavigationService
                            //var navigationService = IoC.Get<INavigationService>();
                            //navigationService.ShowMessage(axEx.Detail.ErrorMessage);

                            axEx.Detail.ErrorMessage += "  Method:" + name;
                            if (OnError != null)
                            {
                                OnError(axEx);
                            }
                            Logger.Error(ex.ToString());
                        }

                    }
                , state
                );
        }
    }

    public static class TypeConvertUtil
    {
        static IWindsorContainer Container { get { return IoC.Get<IWindsorContainer>(); } }
        static ILogger Logger { get { return Container.Resolve<ILogger>(); } }

        public static object ToNonAnonymousList<T>(this List<T> list, Type t)
        {
            //define system Type representing List of objects of T type:
            Type genericType = typeof(List<>).MakeGenericType(t);

            //create an object instance of defined type:
            object l = Activator.CreateInstance(genericType);

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (T item in list)
            {
                //convert each object of the list into T object by calling extension ToType<T>()
                //Add this object to newly created list:
                addMethod.Invoke(l, new[] { item.ToType(t) });
            }
            //return List of T objects:
            return l;
        }
        public static object ToType<T>(this object obj, T type)
        {
            //create instance of T type object:
            object tmp = Activator.CreateInstance(Type.GetType(type.ToString()));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {
                    //get the value of property and try to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    return null;
                }
            }
            //return the T type object:         
            return tmp;
        }
    }

}
