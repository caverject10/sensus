// Copyright 2014 The Rector & Visitors of the University of Virginia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace Sensus.Anonymization.Anonymizers
{
    /// <summary>
    /// Rounds numeric values to the hundreds place (e.g., 123.1235 becomes 123.124).
    /// </summary>
    public class DoubleRoundingThousandthsAnonymizer : DoubleRoundingAnonymizer
    {
        public override string DisplayText
        {
            get
            {
                return "Round to Thousandths";
            }
        }

        public DoubleRoundingThousandthsAnonymizer()
            : base(3)
        {
        }
    }
}

