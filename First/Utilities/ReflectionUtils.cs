using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Main
{
    public static class ReflectionUtils
    {
        public static object GetMemberValue(MemberInfo member, object target)
        {
           Utility.ArgumentNotNull(member, nameof(member));
           Utility.ArgumentNotNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(target);
                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException e)
                    {
                        throw new ArgumentException($"MemberInfo '{nameof(member)}' has index parameters {e}");
                    }
                default:
                    throw new ArgumentException($"MemberInfo '{nameof(member)}' is not of type FieldInfo or PropertyInfo");
            }
        }

        public static void SetMemberValue(MemberInfo member, object target, object? value)
        {
            Utility.ArgumentNotNull(member, nameof(member));
            Utility.ArgumentNotNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;
                default:
                    throw new ArgumentException($"MemberInfo '{nameof(member)}' must be of type FieldInfo or PropertyInfo");
            }
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return (o is IDictionary &&
                     o.GetType().IsGenericType &&
                     (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>))
                     || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(ConcurrentDictionary<,>))));
        }

        public static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public static bool IsList(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }
    }
}
