using System;
using PostSharp.Extensibility;
using PostSharp.Aspects;
using System.Reflection;
using PostSharp.Reflection;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class LockableAttribute : LocationInterceptionAspect
    {

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            // First of all, determine if this property for this specific instance is locked or not.

            var target = args.Instance as ILockable;

            if (target.IsLocked(args.LocationName))
            {

                MethodInfo method = target.GetType().GetMethod("OnAttemptToModifyLockedProperty", BindingFlags.Instance | BindingFlags.NonPublic | ~BindingFlags.DeclaredOnly);

                Action<string, object, object> converted =
                    (Action<string, object, object>)Delegate.CreateDelegate(typeof(Action<string, object, object>), target, method.Name);

                converted(args.Location.Name,
                          args.GetCurrentValue(),
                          args.Value);
            }
            else
            {
                args.ProceedSetValue();
            }
        }
        
        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {
            // Verify at compile-time that the Lockable attribute can only be
            // placed on properties of types that implement ILockable.

            Type theType = locationInfo.DeclaringType;

            Type[] implementedInterfaces = theType.GetInterfaces();

            bool implementsILockable = false;

            foreach (Type t in implementedInterfaces)
            {
                if (t.Name == typeof(ILockable).Name)
                {
                    implementsILockable = true;
                    break;
                }
            }

            if (implementsILockable == false)
            {
                Message msg = new Message(
                    MessageLocation.Of(locationInfo.DeclaringType),
                    SeverityType.Fatal,
                    "MustImplementILockable",
                    "Types that support locking must implement ILockable or derive from LockableEntity",
                    "", "", null);


                MessageSource.MessageSink.Write(msg);

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
