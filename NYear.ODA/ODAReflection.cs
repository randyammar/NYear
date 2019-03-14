using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace NYear.ODA
{
    public static class ODAReflection
    {
        private static readonly MethodInfo IsDBNull = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo GetValue = typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });   
        private static readonly MethodInfo GetBoolean = typeof(IDataRecord).GetMethod("GetBoolean", new Type[] { typeof(int) });
        private static readonly MethodInfo GetByte = typeof(IDataRecord).GetMethod("GetByte", new Type[] { typeof(int) }); 
        private static readonly MethodInfo GetChar = typeof(IDataRecord).GetMethod("GetChar", new Type[] { typeof(int) });
        private static readonly MethodInfo GetDateTime = typeof(IDataRecord).GetMethod("GetDateTime", new Type[] { typeof(int) });
        private static readonly MethodInfo GetDecimal = typeof(IDataRecord).GetMethod("GetDecimal", new Type[] { typeof(int) });
        private static readonly MethodInfo GetDouble = typeof(IDataRecord).GetMethod("GetDouble", new Type[] { typeof(int) });
        private static readonly MethodInfo GetFloat = typeof(IDataRecord).GetMethod("GetFloat", new Type[] { typeof(int) });
        private static readonly MethodInfo GetGuid = typeof(IDataRecord).GetMethod("GetGuid", new Type[] { typeof(int) });
        private static readonly MethodInfo GetInt16 = typeof(IDataRecord).GetMethod("GetInt16", new Type[] { typeof(int) });
        private static readonly MethodInfo GetInt32 = typeof(IDataRecord).GetMethod("GetInt32", new Type[] { typeof(int) });
        private static readonly MethodInfo GetInt64 = typeof(IDataRecord).GetMethod("GetInt64", new Type[] { typeof(int) });
        private static readonly MethodInfo GetString = typeof(IDataRecord).GetMethod("GetString", new Type[] { typeof(int) });


        private static readonly MethodInfo GetEnum = typeof(ODADataReader).GetMethod("GetEnum");
        private static readonly MethodInfo GetBytes = typeof(ODADataReader).GetMethod("GetBytes");
        private static readonly MethodInfo GetChars = typeof(ODADataReader).GetMethod("GetChars");
        private static readonly MethodInfo GetSbyte = typeof(ODADataReader).GetMethod("GetSbyte");
        private static readonly MethodInfo GetUInt32 = typeof(ODADataReader).GetMethod("GetUInt32");
        private static readonly MethodInfo GetUInt64 = typeof(ODADataReader).GetMethod("GetUInt64");
        private static readonly MethodInfo GetUInt16 = typeof(ODADataReader).GetMethod("GetUInt16");
        private static readonly MethodInfo GetDateTimeOffset = typeof(ODADataReader).GetMethod("GetDateTimeOffset");
        private static readonly MethodInfo GetDateTimeOffsetDateTime = typeof(ODADataReader).GetMethod("GetDateTimeOffsetDateTime");
        private static readonly MethodInfo GetSingleInt = typeof(ODADataReader).GetMethod("GetSingleInt");
        private static readonly MethodInfo GetSingleFloat = typeof(ODADataReader).GetMethod("GetSingleFloat");
        private static readonly MethodInfo GetSingleLong = typeof(ODADataReader).GetMethod("GetSingleLong"); 
        private static readonly MethodInfo GetTimeSpanInt = typeof(ODADataReader).GetMethod("GetTimeSpanInt");
        private static readonly MethodInfo GetTimeSpanLong = typeof(ODADataReader).GetMethod("GetTimeSpanLong");  
        private static readonly MethodInfo GetStringValue = typeof(ODADataReader).GetMethod("GetStringValue");
        private static readonly MethodInfo GetValueConvert = typeof(ODADataReader).GetMethod("GetValueConvert");


        private static SafeDictionary<string, object> CreatorsCache;
        public static SafeDictionary<Type, Type> DBTypeMapping { get; private set; }

        static ODAReflection()
        {
            CreatorsCache = new SafeDictionary<string, object>();
            DBTypeMapping = new SafeDictionary<Type, Type>();
        }
        public static Func<IDataReader, T> GetCreator<T>(IDataReader Reader)
        {
            object func = null;
            Type classType = typeof(T);
            List<Tuple<int, Type,string>> FieldInfos = GetDataReaderFieldInfo(Reader); 
            StringBuilder sber = new StringBuilder();
            sber.Append(classType.AssemblyQualifiedName);
            for(int c = 0; c <  FieldInfos.Count; c ++)
            {
                sber.Append(FieldInfos[c].Item2.Name);
            }

            if (CreatorsCache.TryGetValue(sber.ToString(), out func))
                return (Func<IDataReader, T>)func;

            
            var ppIndex = new List<ODAPropertyInfo>();
            PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < prptys.Length; i++)
            {
                if (prptys[i].CanWrite)
                {
                    ppIndex.Add(new ODAPropertyInfo(prptys[i]));
                }
            }
            DynamicMethod method;
            if (classType.IsInterface)
                method = new DynamicMethod("Create" + classType.FullName, classType, new Type[] { typeof(IDataReader) }, classType.Module, true);
            else
                method = new DynamicMethod("Create" + classType.FullName, classType, new Type[] { typeof(IDataReader) }, classType, true);
            ILGenerator il = method.GetILGenerator();
            LocalBuilder result = il.DeclareLocal(classType);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, classType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result);
            for (int i = 0; i < ppIndex.Count; i++)
            {
                Tuple<int, Type, string> Field = null; 
                foreach (var fld in FieldInfos)
                {
                    if (fld.Item3 == ppIndex[i].PropertyName.ToUpper())
                    {
                        Field = fld;
                        break;
                    }
                }

                var setter = ppIndex[i].OriginProperty.GetSetMethod(true);
                if (setter != null && Field != null)
                { 
                    if (ppIndex[i].UnderlyingType.IsValueType)
                    {
                        Label lb = il.DefineLabel();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldc_I4, Field.Item1);
                        il.Emit(OpCodes.Callvirt, IsDBNull);
                        il.Emit(OpCodes.Brtrue, lb);
                        il.Emit(OpCodes.Ldloc, result);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldc_I4, Field.Item1);

                        BindReaderMethod(il, ppIndex[i], Field.Item2); 
                        if (setter.IsFinal || !setter.IsVirtual)
                        {
                            il.Emit(OpCodes.Call, setter);
                        }
                        else
                        {
                            il.Emit(OpCodes.Callvirt, setter);
                        }
                        il.MarkLabel(lb);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc, result);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldc_I4, Field.Item1);
                        BindReaderMethod(il, ppIndex[i], Field.Item2);
                        if (setter.IsFinal || !setter.IsVirtual)
                        {
                            il.Emit(OpCodes.Callvirt, setter);
                        }
                        else
                        {
                            il.Emit(OpCodes.Callvirt, setter);
                        }
                    }
                }
            }
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
            object reator = method.CreateDelegate(typeof(Func<IDataReader, T>));
            CreatorsCache.Add(sber.ToString(), reator);
            GC.KeepAlive(reator);
            return (Func<IDataReader, T>)reator;
        }
        private static List<Tuple<int, Type,string>> GetDataReaderFieldInfo(IDataReader Reader)
        {
            List<Tuple<int, Type, string>> keys = new List<Tuple<int, Type, string>>();
            var count = Reader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                Type dbType = Reader.GetFieldType(i);
                Type CsType;
                //////不同数据库类型与CSharp类型对应
                if (!DBTypeMapping.TryGetValue(dbType, out CsType))
                    CsType = dbType;
                keys.Add(new Tuple<int, Type,string>(i, CsType, Reader.GetName(i).ToUpper()));
            }
            return keys;
        }
        private static void BindReaderMethod(ILGenerator il, ODAPropertyInfo PptyInfo,Type FieldType)
        {
            if (PptyInfo.UnderlyingType == FieldType)
            {
                if (PptyInfo.UnderlyingType == typeof(bool))
                {
                    il.Emit(OpCodes.Callvirt, GetBoolean); 
                }
                else if (PptyInfo.UnderlyingType == typeof(byte))
                {
                    il.Emit(OpCodes.Callvirt, GetByte);
                }
                else if (PptyInfo.UnderlyingType == typeof(char))
                {
                    il.Emit(OpCodes.Callvirt, GetChar);
                }
                else if (PptyInfo.UnderlyingType == typeof(DateTime))
                {
                    il.Emit(OpCodes.Callvirt, GetDateTime);
                }
                else if (PptyInfo.UnderlyingType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                }
                else if (PptyInfo.UnderlyingType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                }
                else if (PptyInfo.UnderlyingType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                }
                else if (PptyInfo.UnderlyingType == typeof(Guid))
                {
                    il.Emit(OpCodes.Callvirt, GetGuid);
                }
                else if (PptyInfo.UnderlyingType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                }
                else if (PptyInfo.UnderlyingType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                }
                else if (PptyInfo.UnderlyingType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                }
                else if (PptyInfo.UnderlyingType == typeof(string))
                {
                    il.Emit(OpCodes.Callvirt, GetString);
                }
                else if (PptyInfo.UnderlyingType == typeof(byte[]))
                {
                    il.Emit(OpCodes.Call, GetBytes);
                }
                else if (PptyInfo.UnderlyingType == typeof(char[]))
                {
                    il.Emit(OpCodes.Call, GetChars);
                }
                else if (PptyInfo.UnderlyingType == typeof(sbyte))
                {
                    il.Emit(OpCodes.Call, GetSbyte);
                }
                else if (PptyInfo.UnderlyingType == typeof(uint))
                {
                    il.Emit(OpCodes.Call, GetUInt32);
                }
                else if (PptyInfo.UnderlyingType == typeof(ulong))
                {
                    il.Emit(OpCodes.Call, GetUInt64);
                }
                else if (PptyInfo.UnderlyingType == typeof(ushort))
                {
                    il.Emit(OpCodes.Call, GetUInt16);
                }
                else if (PptyInfo.UnderlyingType == typeof(DateTimeOffset))
                {
                    il.Emit(OpCodes.Call, GetDateTimeOffset);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, GetValue);
                    il.Emit(OpCodes.Castclass, PptyInfo.OriginType);
                } 
            }
            else
            {
                if (PptyInfo.UnderlyingType.IsEnum)
                {
                    il.Emit(OpCodes.Ldtoken, PptyInfo.OriginType);
                    il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null);
                    il.Emit(OpCodes.Call, GetEnum);
                } 
                else if (PptyInfo.UnderlyingType == typeof(sbyte))
                {
                    il.Emit(OpCodes.Call, GetSbyte);
                }
                else if (PptyInfo.UnderlyingType == typeof(uint))
                {
                    il.Emit(OpCodes.Call, GetUInt32);
                }
                else if (PptyInfo.UnderlyingType == typeof(ulong))
                {
                    il.Emit(OpCodes.Call, GetUInt64);
                }
                else if (PptyInfo.UnderlyingType == typeof(ushort))
                {
                    il.Emit(OpCodes.Call, GetUInt16);
                }
                else if (PptyInfo.UnderlyingType == typeof(DateTimeOffset) && FieldType == typeof(DateTime))
                {
                    il.Emit(OpCodes.Call, GetDateTimeOffsetDateTime);
                }

                else if (PptyInfo.UnderlyingType == typeof(Single) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Call, GetSingleInt);
                }
                else if (PptyInfo.UnderlyingType == typeof(Single) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Call, GetSingleFloat);
                }
                else if (PptyInfo.UnderlyingType == typeof(Single) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Call, GetSingleLong);
                }
                else if (PptyInfo.UnderlyingType == typeof(TimeSpan) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Call, GetTimeSpanInt);
                }
                else if (PptyInfo.UnderlyingType == typeof(TimeSpan) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Call, GetTimeSpanLong);
                }


                else if (PptyInfo.UnderlyingType == typeof(decimal) && FieldType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(double) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(decimal) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(float) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(decimal) && FieldType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(short) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(decimal) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(decimal) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                    il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[] { typeof(long) }));
                }



                else if (PptyInfo.UnderlyingType == typeof(double) && FieldType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(decimal) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(double) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(float) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(double) && FieldType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(short) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(double) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(double) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(long) }));
                }



                else if (PptyInfo.UnderlyingType == typeof(float) && FieldType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                    il.Emit(OpCodes.Newobj, typeof(double).GetConstructor(new Type[] { typeof(decimal) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(float) && FieldType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                    il.Emit(OpCodes.Newobj, typeof(float).GetConstructor(new Type[] { typeof(double) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(float) && FieldType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                    il.Emit(OpCodes.Newobj, typeof(float).GetConstructor(new Type[] { typeof(short) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(float) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                    il.Emit(OpCodes.Newobj, typeof(float).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(float) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                    il.Emit(OpCodes.Newobj, typeof(float).GetConstructor(new Type[] { typeof(long) }));
                }

                else if (PptyInfo.UnderlyingType == typeof(short) && FieldType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                    il.Emit(OpCodes.Newobj, typeof(short).GetConstructor(new Type[] { typeof(decimal) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(short) && FieldType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                    il.Emit(OpCodes.Newobj, typeof(short).GetConstructor(new Type[] { typeof(double) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(short) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                    il.Emit(OpCodes.Newobj, typeof(short).GetConstructor(new Type[] { typeof(float) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(short) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                    il.Emit(OpCodes.Newobj, typeof(short).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(short) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                    il.Emit(OpCodes.Newobj, typeof(short).GetConstructor(new Type[] { typeof(long) }));
                }

                else if (PptyInfo.UnderlyingType == typeof(int) && FieldType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                    il.Emit(OpCodes.Newobj, typeof(int).GetConstructor(new Type[] { typeof(decimal) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(int) && FieldType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                    il.Emit(OpCodes.Newobj, typeof(int).GetConstructor(new Type[] { typeof(double) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(int) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                    il.Emit(OpCodes.Newobj, typeof(int).GetConstructor(new Type[] { typeof(float) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(int) && FieldType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                    il.Emit(OpCodes.Newobj, typeof(int).GetConstructor(new Type[] { typeof(short) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(int) && FieldType == typeof(long))
                {
                    il.Emit(OpCodes.Callvirt, GetInt64);
                    il.Emit(OpCodes.Newobj, typeof(int).GetConstructor(new Type[] { typeof(long) }));
                }

                else if (PptyInfo.UnderlyingType == typeof(long) && FieldType == typeof(decimal))
                {
                    il.Emit(OpCodes.Callvirt, GetDecimal);
                    il.Emit(OpCodes.Newobj, typeof(long).GetConstructor(new Type[] { typeof(decimal) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(long) && FieldType == typeof(double))
                {
                    il.Emit(OpCodes.Callvirt, GetDouble);
                    il.Emit(OpCodes.Newobj, typeof(long).GetConstructor(new Type[] { typeof(double) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(long) && FieldType == typeof(float))
                {
                    il.Emit(OpCodes.Callvirt, GetFloat);
                    il.Emit(OpCodes.Newobj, typeof(long).GetConstructor(new Type[] { typeof(float) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(long) && FieldType == typeof(short))
                {
                    il.Emit(OpCodes.Callvirt, GetInt16);
                    il.Emit(OpCodes.Newobj, typeof(long).GetConstructor(new Type[] { typeof(short) }));
                }
                else if (PptyInfo.UnderlyingType == typeof(long) && FieldType == typeof(int))
                {
                    il.Emit(OpCodes.Callvirt, GetInt32);
                    il.Emit(OpCodes.Newobj, typeof(long).GetConstructor(new Type[] { typeof(int) }));
                } 
                else if (PptyInfo.UnderlyingType == typeof(string))
                {
                    il.Emit(OpCodes.Call, GetStringValue);
                }
                else
                {
                    il.Emit(OpCodes.Ldtoken, PptyInfo.OriginType);
                    il.EmitCall(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)), null);
                    il.Emit(OpCodes.Callvirt, GetValueConvert);
                }
            }
            if (PptyInfo.IsNullableTypeProperty)
                il.Emit(OpCodes.Newobj, PptyInfo.OriginType.GetConstructor(new Type[] { PptyInfo.UnderlyingType })); 
        } 
    }
}
 