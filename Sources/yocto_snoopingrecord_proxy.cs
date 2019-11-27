/*********************************************************************
 *
 *  $Id: yocto_snoopingrecord_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YSnoopingRecordProxy, the Proxy API for SnoopingRecord
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
    //--- (generated code: YSnoopingRecord class start)
    public class YSnoopingRecordProxy
    {
        private YSnoopingRecord _objref;
        internal YSnoopingRecordProxy(YSnoopingRecord objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YSnoopingRecord class start)
        //--- (generated code: YSnoopingRecord definitions)
        //--- (end of generated code: YSnoopingRecord definitions)
        //--- (generated code: YSnoopingRecord implementation)

        // property with cached value for instant access
        public int Time
        {
            get
            {
                return this.get_time();
            }
        }
        /**
         * <summary>
         *   Returns the elapsed time, in ms, since the beginning of the preceding message.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the elapsed time, in ms, since the beginning of the preceding message.
         * </returns>
         */
        public virtual int get_time()
        {
            if (_objref == null)
            {
                string msg = "No SnoopingRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_time();
        }

        // property with cached value for instant access
        public int Direction
        {
            get
            {
                return this.get_direction();
            }
        }
        /**
         * <summary>
         *   Returns the message direction (RX=0 , TX=1) .
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the message direction (RX=0 , TX=1) .
         * </returns>
         */
        public virtual int get_direction()
        {
            if (_objref == null)
            {
                string msg = "No SnoopingRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_direction();
        }

        // property with cached value for instant access
        public string Message
        {
            get
            {
                return this.get_message();
            }
        }
        /**
         * <summary>
         *   Returns the message content.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   the message content.
         * </returns>
         */
        public virtual string get_message()
        {
            if (_objref == null)
            {
                string msg = "No SnoopingRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_message();
        }
    }
    //--- (end of generated code: YSnoopingRecord implementation)
}

