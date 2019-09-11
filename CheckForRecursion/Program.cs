using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CheckForRecursion
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileToCheck = null;
            if (args != null && args.Count() > 0 && !string.IsNullOrEmpty(args[0]))
            {
                fileToCheck = args[0];
            }
            else
            {
                Console.WriteLine("Please enter path to dll you wanna check:");
                fileToCheck = Console.ReadLine();
            }

            try
            {

                var assembly = ModuleDefinition.ReadModule(fileToCheck);
                var calls =
                    (from type in assembly.Types
                     from caller in type.Methods
                     where caller != null && caller.Body != null
                     from instruction in caller.Body.Instructions
                     where instruction.OpCode == OpCodes.Call
                     let callee = instruction.Operand as MethodReference
                     select new { type, caller, callee }).Distinct();

                var directRecursiveCalls =
                    from call in calls
                    where call.callee == call.caller
                    select call.caller;

                foreach (var method in directRecursiveCalls)
                    Console.WriteLine(method.DeclaringType.Namespace + "." + method.DeclaringType.Name + "." + method.Name + " calls itself");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
