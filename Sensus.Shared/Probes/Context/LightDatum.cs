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
using Sensus.Anonymization;
using Sensus.Anonymization.Anonymizers;
using Sensus.Probes.User.Scripts.ProbeTriggerProperties;

namespace Sensus.Probes.Context
{
    public class LightDatum : Datum, ILightDatum
    {
        private double _brightness;

        [DoubleProbeTriggerProperty]
        [Anonymizable(null, new Type[] { typeof(DoubleRoundingTensAnonymizer), typeof(DoubleRoundingHundredsAnonymizer) }, -1)]
        public double Brightness
        {
            get { return _brightness; }
            set { _brightness = value; }
        }

        public override string DisplayDetail
        {
            get { return "Brightness:  " + Math.Round(_brightness, 2); }
        }

        /// <summary>
        /// Gets the string placeholder value, which is the brightness level.
        /// </summary>
        /// <value>The string placeholder value.</value>
        public override object StringPlaceholderValue
        {
            get
            {
                return Math.Round(_brightness, 2);
            }
        }

        /// <summary>
        /// For JSON deserialization.
        /// </summary>
        private LightDatum() { }

        public LightDatum(DateTimeOffset timestamp, double brightness)
            : base(timestamp)
        {
            _brightness = brightness;
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine +
                   "Brightness:  " + _brightness;
        }
    }
}
