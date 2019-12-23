/*********************************************************************
 *
 *  $Id: yocto_wlanrecord_proxy.cs 38687 2019-12-04 18:22:46Z mvuilleu $
 *
 *  Implements YWlanRecordProxy, the Proxy API for WlanRecord
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
    //--- (generated code: YWlanRecord class start)
    public class YWlanRecordProxy
    {
        private YWlanRecord _objref;
        internal YWlanRecordProxy(YWlanRecord objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YWlanRecord class start)
        //--- (generated code: YWlanRecord definitions)
        //--- (end of generated code: YWlanRecord definitions)
        //--- (generated code: YWlanRecord implementation)

        // property with cached value for instant access
        public string Ssid
        {
            get
            {
                return this.get_ssid();
            }
        }
        /**
         * <summary>
         *   Returns the name of the wireless network (SSID).
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with the name of the wireless network (SSID).
         * </returns>
         */
        public virtual string get_ssid()
        {
            if (_objref == null)
            {
                string msg = "No WlanRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_ssid();
        }

        // property with cached value for instant access
        public int Channel
        {
            get
            {
                return this.get_channel();
            }
        }
        /**
         * <summary>
         *   Returns the 802.11 b/g/n channel number used by this network.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the channel.
         * </returns>
         */
        public virtual int get_channel()
        {
            if (_objref == null)
            {
                string msg = "No WlanRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_channel();
        }

        // property with cached value for instant access
        public string Security
        {
            get
            {
                return this.get_security();
            }
        }
        /**
         * <summary>
         *   Returns the security algorithm used by the wireless network.
         * <para>
         *   If the network implements to security, the value is <c>"OPEN"</c>.
         * </para>
         * </summary>
         * <returns>
         *   a string with the security algorithm.
         * </returns>
         */
        public virtual string get_security()
        {
            if (_objref == null)
            {
                string msg = "No WlanRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_security();
        }

        // property with cached value for instant access
        public int LinkQuality
        {
            get
            {
                return this.get_linkQuality();
            }
        }
        /**
         * <summary>
         *   Returns the quality of the wireless network link, in per cents.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer between 0 and 100 corresponding to the signal quality.
         * </returns>
         */
        public virtual int get_linkQuality()
        {
            if (_objref == null)
            {
                string msg = "No WlanRecord connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_linkQuality();
        }
    }
    //--- (end of generated code: YWlanRecord implementation)
}

