/*********************************************************************
 *
 *  $Id: yocto_cellrecord_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YCellRecordProxy, the Proxy API for CellRecord
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
    //--- (generated code: YCellRecord class start)
    public class YCellRecordProxy
    {
        private YCellRecord _objref;
        internal YCellRecordProxy(YCellRecord objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YCellRecord class start)
        //--- (generated code: YCellRecord definitions)
        //--- (end of generated code: YCellRecord definitions)
        //--- (generated code: YCellRecord implementation)

        // property with cached value for instant access
        public string CellOperator
        {
            get
            {
                return this.get_cellOperator();
            }
        }
        /**
         * <summary>
         *   Returns the name of the the cell operator.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with the name of the the cell operator.
         * </returns>
         */
        public virtual string get_cellOperator()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_cellOperator();
        }

        // property with cached value for instant access
        public int MobileCountryCode
        {
            get
            {
                return this.get_mobileCountryCode();
            }
        }
        /**
         * <summary>
         *   Returns the Mobile Country Code (MCC).
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the Mobile Country Code (MCC).
         * </returns>
         */
        public virtual int get_mobileCountryCode()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_mobileCountryCode();
        }

        // property with cached value for instant access
        public int MobileNetworkCode
        {
            get
            {
                return this.get_mobileNetworkCode();
            }
        }
        /**
         * <summary>
         *   Returns the Mobile Network Code (MNC).
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the Mobile Network Code (MNC).
         * </returns>
         */
        public virtual int get_mobileNetworkCode()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_mobileNetworkCode();
        }

        // property with cached value for instant access
        public int LocationAreaCode
        {
            get
            {
                return this.get_locationAreaCode();
            }
        }
        /**
         * <summary>
         *   Returns the Location Area Code (LAC).
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the Location Area Code (LAC).
         * </returns>
         */
        public virtual int get_locationAreaCode()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_locationAreaCode();
        }

        // property with cached value for instant access
        public int CellId
        {
            get
            {
                return this.get_cellId();
            }
        }
        /**
         * <summary>
         *   Returns the Cell Id.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the Cell Id.
         * </returns>
         */
        public virtual int get_cellId()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_cellId();
        }

        // property with cached value for instant access
        public int SignalStrength
        {
            get
            {
                return this.get_signalStrength();
            }
        }
        /**
         * <summary>
         *   Returns the signal strength.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the signal strength.
         * </returns>
         */
        public virtual int get_signalStrength()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_signalStrength();
        }

        // property with cached value for instant access
        public int TimingAdvance
        {
            get
            {
                return this.get_timingAdvance();
            }
        }
        /**
         * <summary>
         *   Returns the Timing Advance (TA).
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the Timing Advance (TA).
         * </returns>
         */
        public virtual int get_timingAdvance()
        {
            if (_objref == null)
            {
                string msg = "No CellRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_timingAdvance();
        }
    }
    //--- (end of generated code: YCellRecord implementation)
}

