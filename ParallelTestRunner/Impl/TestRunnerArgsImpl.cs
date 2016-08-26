﻿using ParallelTestRunner.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace ParallelTestRunner.Impl
{
    public class TestRunnerArgsImpl : ITestRunnerArgs
    {
        public TestRunnerArgsImpl()
        {
            Output = "Result.trx";
            ThreadCount = 4;
            PLevel = PLevel.TestClass;
            LoggedVariables = false;
        }

        private bool LoggedVariables { get; set; }

        public string Provider { get; set; }
        
        public IList<string> AssemblyList { get; set; }
        
        public int ThreadCount { get; set; }
        
        public string Root { get; set; }
        
        public string Output { get; set; }

        public PLevel PLevel { get; set; }

        public string GetExecutablePath()
        {
            var environmentVariable = ConfigurationManager.AppSettings.Get(Provider);
            var commonToolPath = Environment.GetEnvironmentVariable(environmentVariable);
            var vstestSubPath = "..\\IDE\\CommonExtensions\\Microsoft\\TestWindow\\vstest.console.exe";

            if (string.IsNullOrEmpty(commonToolPath))
            {
                throw new Exception($"Cannot locate commonToolPath to use. Please check your EnvironmentVariable with value '{environmentVariable}'. These are parsed from the given provider: '{Provider}'");
            }

            if (!LoggedVariables)
            {
                Console.WriteLine($"Running with the following variables: environmentVariable:'{environmentVariable}', commonToolPath:'{commonToolPath}', vstestSubPath:'{vstestSubPath}'");
                LoggedVariables = true;
            }
            var vstestFullPath = Path.Combine(commonToolPath, vstestSubPath);
            return vstestFullPath;
        }

        public bool IsValid()
        {
            if (AssemblyList == null ||
                AssemblyList.Count == 0)
            {
                Console.WriteLine("at least one DLL must be specified: c:\\work\\testassembly.dll");
                return false;
            }

            if (string.IsNullOrEmpty(Provider))
            {
                Console.WriteLine("Provider is required (see config file): provider:VSTEST_2012");
                return false;
            }

            if (ThreadCount <= 0)
            {
                Console.WriteLine("threadcount must be integer greater than 0: threadcount:4");
                return false;
            }

            if (string.IsNullOrEmpty(Root))
            {
                Console.WriteLine("root path is required: root:d:\\work");
                return false;
            }

            if (PLevel == PLevel.None)
            {
                Console.WriteLine("Level which will be considered parallel is required: plevel:testclass");
                return false;
            }

            return true;
        }
    }
}