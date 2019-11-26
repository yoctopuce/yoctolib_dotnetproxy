/*********************************************************************
 *
 *  $Id: yocto_consolidateddataset_proxy.cs 38282 2019-11-21 14:50:25Z seb $
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
        public YConsolidatedDataSetProxy(double startTime, double endTime, List<YSensorProxy> sensorList)
        {
            List<YSensor> realsensorlist = new List<YSensor>(sensorList.Count);
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
         *   Extracts the next data record from the dataLogger of all sensors linked to this
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

