/*********************************************************************
 *
 *  $Id: yocto_consolidateddataset_proxy.cs 38899 2019-12-20 17:21:03Z mvuilleu $
 *
 *  Implements YConsolidatedDataSetProxy, the Proxy API for ConsolidatedDataSet
 *
 *  - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 *
 *********************************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Globalization;
using System.Text.RegularExpressions;
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (generated code: YConsolidatedDataSet class start)
    public class YConsolidatedDataSetProxy
    {
        private YConsolidatedDataSet _objref;
        internal YConsolidatedDataSetProxy(YConsolidatedDataSet objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YConsolidatedDataSet class start)
        // YConsolidatedDataSet constructor
        public YConsolidatedDataSetProxy(double startTime, double endTime, YSensorProxy[] sensorList)
        {
            List<YSensor> realsensorlist = new List<YSensor>(sensorList.Length);
            foreach (YSensorProxy sensorProxy in sensorList) {
                realsensorlist.Add(sensorProxy.get_ysensor());
            }
            _objref = new YConsolidatedDataSet(startTime,endTime, realsensorlist);
        }

        //--- (generated code: YConsolidatedDataSet definitions)
        //--- (end of generated code: YConsolidatedDataSet definitions)
        //--- (generated code: YConsolidatedDataSet implementation)

        /**
         * <summary>
         *   Returns an object holding historical data for multiple
         *   sensors, for a specified time interval.
         * <para>
         *   The measures will be retrieved from the data logger, which must have been turned
         *   on at the desired time. The resulting object makes it possible to load progressively
         *   a large set of measures from multiple sensors, consolidating data on the fly
         *   to align records based on measurement timestamps.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="sensorNames">
         *   array of logical names or hardware identifiers of the sensors
         *   for which data must be loaded from their data logger.
         * </param>
         * <param name="startTime">
         *   the start of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without initial limit.
         * </param>
         * <param name="endTime">
         *   the end of the desired measure time interval,
         *   as a Unix timestamp, i.e. the number of seconds since
         *   January 1, 1970 UTC. The special value 0 can be used
         *   to include any measure, without ending limit.
         * </param>
         * <returns>
         *   an instance of <c>YConsolidatedDataSet</c>, providing access to
         *   consolidated historical data. Records can be loaded progressively
         *   using the <c>YConsolidatedDataSet.nextRecord()</c> method.
         * </returns>
         */
        public static YConsolidatedDataSetProxy Init(string[] sensorNames, double startTime, double endTime)
        {
            return new YConsolidatedDataSetProxy(YConsolidatedDataSet.Init(new List<string>(sensorNames), startTime, endTime));
        }

        /**
         * <summary>
         *   Extracts the next data record from the data logger of all sensors linked to this
         *   object.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="datarec">
         *   array of floating point numbers, that will be filled by the
         *   function with the timestamp of the measure in first position,
         *   followed by the measured value in next positions.
         * </param>
         * <returns>
         *   an integer in the range 0 to 100 (percentage of completion),
         *   or a negative error code in case of failure.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int nextRecord(double[] datarec)
        {
            if (_objref == null)
            {
                string msg = "No ConsolidatedDataSet connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.nextRecord(new List<double>(datarec));
        }
    }
    //--- (end of generated code: YConsolidatedDataSet implementation)
}

