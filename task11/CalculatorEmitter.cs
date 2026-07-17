namespace CalculatorEmitter;
using System;
using System.Reflection;
using System.Reflection.Emit;

public static class CalculatorEmitter
{
    public static ICalculator Create()
    {
        var assemblyname = new AssemblyName("DynamicCalculator");
        var assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyname, AssemblyBuilderAccess.Run);
        var modulebuilder = assemblybuilder.DefineDynamicModule("MainModule");
        var typebuilder = modulebuilder.DefineType("Calculator", TypeAttributes.Public | TypeAttributes.Class);

        typebuilder.AddInterfaceImplementation(typeof(ICalculator));

        // Add(int a, int b) => a + b
        EmitMethod(typebuilder, "Add", OpCodes.Add);

        // Minus(int a, int b) => a - b
        EmitMethod(typebuilder, "Minus", OpCodes.Sub);

        // Mul(int a, int b) => a * b
        EmitMethod(typebuilder, "Mul", OpCodes.Mul);

        // Div(int a, int b) => a / b
        EmitMethod(typebuilder, "Div", OpCodes.Div);

        var type = typebuilder.CreateType();
        return (ICalculator)Activator.CreateInstance(type)!;
    }

    private static void EmitMethod(TypeBuilder typebuilder, string name, OpCode opCode)
    {
        var methodbuilder = typebuilder.DefineMethod(name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(int), new[] { typeof(int), typeof(int) });

        var il = methodbuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);   // a
        il.Emit(OpCodes.Ldarg_2);   // b
        il.Emit(opCode);            // +, -, *, /
        il.Emit(OpCodes.Ret);

        // Привязываем к интерфейсу
        var interfacemethod = typeof(ICalculator).GetMethod(name)!;
        typebuilder.DefineMethodOverride(methodbuilder, interfacemethod);
    }
}