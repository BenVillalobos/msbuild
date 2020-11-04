// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Microsoft.Build.Tasks.UnitTests
{
    class ValidateChangeWaves_Tests
    {
        private readonly ITestOutputHelper _testOutput;
        public ValidateChangeWaves_Tests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
    }
}
