using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestExecute

{
   
    public class OrderedTestAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderedTestAttribute(int order)
        {
            Order = order;
        }
    }

    public class TestStructure
    {
        public Action Test;
    }

    public class OrderedTestFixture
    {
        public IEnumerable<TestCaseData> TestSource
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Dictionary<int, List<MethodInfo>> methods = assembly
                    .GetType(this.GetType().FullName)
                    .GetMethods()
                    //.GetTypes()
                    //.SelectMany(x => x.GetMethods())
                    .Where(y => y.GetCustomAttributes().OfType<OrderedTestAttribute>().Any())
                    .GroupBy(z => z.GetCustomAttribute<OrderedTestAttribute>().Order)
                    .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList());

                foreach (int order in methods.Keys.OrderBy(x => x))
                {
                    foreach (MethodInfo methodInfo in methods[order])
                    {
                        MethodInfo info = methodInfo;
                        yield return new TestCaseData(
                            new TestStructure
                            {
                                Test = () =>
                                {
                                    object classInstance = Activator.CreateInstance(info.DeclaringType, null);
                                    info.Invoke(classInstance, null);
                                }
                            }).SetName(methodInfo.Name);
                    }
                }
            }
        }
    }
}