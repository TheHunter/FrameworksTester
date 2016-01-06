using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using ElasticSearchTester.Domain;
using Xunit;
using Xunit.Extensions;

namespace ElasticSearchTester
{
    public class EmitTester
    {
        private readonly ModuleBuilder moduleBuilder;

        public EmitTester()
        {
            var appDomain = AppDomain.CurrentDomain; //Thread.GetDomain();
            var myAsmName = new AssemblyName { Name = "MyDynamicAssembly" };

            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(myAsmName,
                AssemblyBuilderAccess.RunAndCollect);

            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
        }

        private Type MakeNewTypeFrom(Type baseType, string propertyname)
        {
            TypeBuilder typeBuilder = this.moduleBuilder.DefineType(baseType.Name + "Proxy");
            typeBuilder.SetParent(baseType);

            var propertyBuilder = typeBuilder.DefineProperty(propertyname, PropertyAttributes.HasDefault, typeof(string), null);
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            FieldBuilder fieldBuilder = typeBuilder.DefineField(propertyname.ToLower(),
                                                        typeof(string),
                                                        FieldAttributes.Private);

            MethodBuilder getPropertyBuilder =
            typeBuilder.DefineMethod("get_" + propertyname,
                                       getSetAttr,
                                       typeof(string),
                                       Type.EmptyTypes);

            ILGenerator custNameGetIl = getPropertyBuilder.GetILGenerator();

            custNameGetIl.Emit(OpCodes.Ldarg_0);
            custNameGetIl.Emit(OpCodes.Ldfld, fieldBuilder);
            custNameGetIl.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder setPropertyBuilder =
                typeBuilder.DefineMethod("set_" + propertyname,
                                           getSetAttr,
                                           null,
                                           new Type[] { typeof(string) });

            ILGenerator custNameSetIl = setPropertyBuilder.GetILGenerator();

            custNameSetIl.Emit(OpCodes.Ldarg_0);
            custNameSetIl.Emit(OpCodes.Ldarg_1);
            custNameSetIl.Emit(OpCodes.Stfld, fieldBuilder);
            custNameSetIl.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            propertyBuilder.SetGetMethod(getPropertyBuilder);
            propertyBuilder.SetSetMethod(setPropertyBuilder);


            Type retval = typeBuilder.CreateType();

            // Save the assembly so it can be examined with Ildasm.exe,
            // or referenced by a test program.
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }

        [Theory]
        [InlineData("$IdSession")]
        public void GenerateNewProperty(string propertyName)
        {
            Type orig = typeof(Student);
            Type current = this.MakeNewTypeFrom(orig, propertyName);
            Assert.NotNull(current);

            Assert.True(orig.IsAssignableFrom(current));

            PropertyInfo info = current.GetProperty(propertyName);
            Assert.NotNull(info);

            object instance = Activator.CreateInstance(current);
            Assert.NotNull(instance);

            const string value = "myvalue";
            info.SetValue(instance, "myvalue", null);

            Assert.Equal(value, info.GetValue(instance, null));

            Student st = instance as dynamic;
            Assert.NotNull(st);
        }

        [Theory]
        [InlineData("$IdSession")]
        public void GenerateNewProperty2(string propertyName)
        {
            Assert.Throws<TypeLoadException>(() => this.MakeNewTypeFrom(typeof(MySealedClass), propertyName));
        }
    }
}
