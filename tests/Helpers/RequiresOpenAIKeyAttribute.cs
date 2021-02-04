using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Tests.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    // ReSharper disable once InconsistentNaming
    public class RequiresOpenAIKeyAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            if(Environment.GetEnvironmentVariable(Constants.ENV_OPENAI_KEY) == null)
                Assert.Ignore($"This test requires a valid OpenAI API key in the {Constants.ENV_OPENAI_KEY} environment variable.");
        }

        public void AfterTest(ITest test) { }

        public ActionTargets Targets { get; }
    }
}