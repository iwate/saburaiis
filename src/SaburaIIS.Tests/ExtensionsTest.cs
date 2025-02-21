using System;
using Xunit;
using SaburaIIS.Extensions;

namespace SaburaIIS.Tests
{
    public class ExtensionsTest
    {
        [Theory]
        [InlineData(@"c:\test.txt")]
        public void ToPath(string path)
        {
            var uri = new Uri(path);
            Assert.Equal(path, uri.ToPath());
        }
    }
}
