// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Utilities;
using System;
using System.Linq;

namespace Microsoft.Build.Tasks
{
    class ValidateChangeWaves : TaskExtension
    {
        public override bool Execute()
        {
            ChangeWaves.ApplyChangeWave();

            switch (ChangeWaves.ConversionState)
            {
                case ChangeWaveConversionState.InvalidFormat:
                    Log.LogWarningFromResources("ChangeWave_InvalidFormat", Traits.Instance.MSBuildDisableFeaturesFromVersion, $"[{String.Join(", ", ChangeWaves.AllWaves.Select(x => x.ToString()))}]");
                    break;
                case ChangeWaveConversionState.OutOfRotation:
                    Log.LogWarningFromResources("ChangeWave_OutOfRotation", Traits.Instance.MSBuildDisableFeaturesFromVersion, $"[{String.Join(", ", ChangeWaves.AllWaves.Select(x => x.ToString()))}]");
                    break;
            }

            return true;
        }
    }
}
