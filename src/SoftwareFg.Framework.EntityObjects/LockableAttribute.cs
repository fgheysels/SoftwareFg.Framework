using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Laos;
using PostSharp.Extensibility;
using System.Reflection;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    [AttributeUsage (AttributeTargets.Property)]
    public class LockableAttribute : OnMethodInvocationAspect
    {


        private PropertyInfo _property;

        /// <summary>
        /// Retrieves the PropertyInfo for which the given MethodInfo is the setter-method.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="setterMethod"></param>
        /// <returns></returns>
        private PropertyInfo GetPropertyForSetterMethod( object target, MethodInfo setterMethod )
        {
            if( _property == null )
            {
                string searchName = setterMethod.Name;
                if( searchName.StartsWith ("~") )
                {
                    searchName = searchName.Substring (1, searchName.Length - 1);
                }

                PropertyInfo[] props = target.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | ~BindingFlags.DeclaredOnly);

                foreach( PropertyInfo pi in props )
                {
                    if( pi.CanWrite && pi.GetSetMethod ().Name == searchName )
                    {
                        _property = pi;
                        return _property;
                    }
                }
                throw new ArgumentException ("No Property found for the given setter-method " + setterMethod.Name);                
            }

            return _property;
        }

        public override void OnInvocation( MethodInvocationEventArgs eventArgs )
        {
            if( eventArgs.Delegate.Method.ReturnParameter.ParameterType == typeof (void) )
            {
                System.Diagnostics.Debug.WriteLine ("We are in the setter");

                ILockable target = eventArgs.Delegate.Target as ILockable;

                _property = GetPropertyForSetterMethod (target, eventArgs.Delegate.Method);

                if( target.IsLocked (_property.Name) )
                {
                    MethodInfo method = target.GetType ().GetMethod ("OnAttemptToModifyLockedProperty", BindingFlags.Instance | BindingFlags.NonPublic | ~BindingFlags.DeclaredOnly);

                    Action<string, object, object> converted = 
                        (Action<string, object, object>)Delegate.CreateDelegate (typeof (Action<string, object, object>),target, method.Name);

                    converted (_property.Name,
                               _property.GetValue (target, BindingFlags.GetProperty, null, new object[] { }, null),
                               eventArgs.GetArgumentArray ()[0]);
                }
                else
                {
                    base.OnInvocation (eventArgs);
                }

                object value = eventArgs.GetArgumentArray ()[0];

                Console.WriteLine (value + " will be set");

            }
            else
            {
                base.OnInvocation (eventArgs);
            }            
        }



        //public override bool CompileTimeValidate( System.Reflection.MethodBase method )
        //{
        //    if( method.DeclaringType.GetInterface (typeof (ILockable).Name) == null )
        //    {
        //        Message msg = new Message (SeverityType.Fatal, "MustImplementILockable",
        //                                  "Types that support locking must implement ILockable or derive from LockableEntity",
        //                                  method.DeclaringType.Name);

        //        MessageSource.MessageSink.Write (msg);

        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
}
