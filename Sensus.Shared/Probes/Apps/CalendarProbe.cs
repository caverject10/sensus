﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sensus.UI.UiProperties;
using Syncfusion.SfChart.XForms;

namespace Sensus.Probes.Apps
{
    public abstract class CalendarProbe : PollingProbe
    {
        public override string DisplayName => "Calendar Events";
        public sealed override Type DatumType => typeof(CalendarDatum);

        protected override ChartDataPoint GetChartDataPointFromDatum(Datum datum)
        {
            throw new NotImplementedException();
        }

        protected override ChartAxis GetChartPrimaryAxis()
        {
            throw new NotImplementedException();
        }

        protected override RangeAxisBase GetChartSecondaryAxis()
        {
            throw new NotImplementedException();
        }

        protected override ChartSeries GetChartSeries()
        {
            throw new NotImplementedException();
        }

        protected async override Task<List<Datum>> PollAsync(CancellationToken cancellationToken)
        {
            List<Datum> calendarMetaData = new List<Datum>();

            foreach (Datum calendarDatum in await GetCalendarEventsAsync())
            {
                calendarMetaData.Add(calendarDatum);
            }

            return calendarMetaData;

        }

        protected abstract Task<List<CalendarDatum>> GetCalendarEventsAsync();
    }
}
