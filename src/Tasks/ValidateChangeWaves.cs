// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Utilities;
using System;
using System.Linq;

using MSBuildChangeWaves;

namespace Microsoft.Build.Tasks
{
    class ValidateChangeWaves : TaskExtension
    {
        public override bool Execute()
        {
            ChangeWaves.ApplyChangeWave();

            switch (ChangeWaves.ConversionState)
            {
                case ChangeWaves.ChangeWaveConversionState.InvalidFormat:
                    Log.LogWarningWithCodeFromResources("ChangeWave_InvalidFormat", Traits.Instance.MSBuildDisableFeaturesFromVersion, $"[{String.Join(", ", ChangeWaves.AllWaves.Select(x => x.ToString()))}]");
                    break;
                case ChangeWaves.ChangeWaveConversionState.OutOfRotation:
                    Log.LogWarningWithCodeFromResources("ChangeWave_OutOfRotation", Traits.Instance.MSBuildDisableFeaturesFromVersion, $"[{String.Join(", ", ChangeWaves.AllWaves.Select(x => x.ToString()))}]");
                    break;
            }

            return true;
        }
    }
}
