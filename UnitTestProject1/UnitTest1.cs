using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tensor;
using System.Configuration;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var x = Utils.DownloadDefaultTexts(_currentDir);
        }
    }
}
