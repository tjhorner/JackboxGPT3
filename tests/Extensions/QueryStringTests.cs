using FluentAssertions;
using JackboxGPT3.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
// ReSharper disable NotAccessedField.Local

namespace Tests.Extensions
{
    public class QueryStringTests
    {
        private struct TestStruct
        {
            public string SomeString;
            public bool SomeBool;
            public int SomeInt;
        }

        [Test]
        public void ShouldSerializeToQueryString()
        {
            var ts = new TestStruct
            {
                SomeString = "hello",
                SomeBool = true,
                SomeInt = 69
            };

            var result = ts.AsQueryString();
            result.Should().Be("SomeString=hello&SomeBool=true&SomeInt=69");
        }

        private struct CustomTestStruct
        {
            [JsonProperty("custom_property")]
            public string CustomProperty;
        }

        [Test]
        public void ShouldUseCustomPropertyAttributes()
        {
            var ts = new CustomTestStruct
            {
                CustomProperty = "hello"
            };

            var result = ts.AsQueryString();
            result.Should().Be("custom_property=hello");
        }
    }
}
