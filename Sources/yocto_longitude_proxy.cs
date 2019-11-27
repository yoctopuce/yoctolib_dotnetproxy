/*********************************************************************
 *
 *  $Id: yocto_longitude_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YLongitudeProxy, the Proxy API for Longitude
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
    //--- (YLongitude class start)
    static public partial class YoctoProxyManager
    {
        public static YLongitudeProxy FindLongitude(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YLongitude func = null;
            YLongitudeProxy res = (YLongitudeProxy)YFunctionProxy.FindSimilarUnknownFunction("YLongitudeProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YLongitudeProxy)YFunctionProxy.FindSimilarKnownFunction("YLongitudeProxy");
                if (res != null) return res;
                func = YLongitude.FirstLongitude();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YLongitudeProxy)func.get_userData();
                    }
                }
            } else {
                func = YLongitude.FindLongitude(name);
                if (func.get_userData() != null) {
                    return (YLongitudeProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YLongitudeProxy(func, name);
            }
            if (func != null) {
                res.linkToHardware(name);
                if(func.isOnline()) res.arrival();
            }
            return res;
        }
    }

/**
 * <summary>
 *   The YLongitude class allows you to read the longitude from Yoctopuce
 *   geolocation sensors, for instance using a Yocto-GPS.
 * <para>
 *   It inherits from the YSensor class the core functions to
 *   read measurements, register callback functions, access the autonomous
 *   datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YLongitudeProxy : YSensorProxy
    {
        //--- (end of YLongitude class start)
        //--- (YLongitude definitions)

        // reference to real YoctoAPI object
        protected new YLongitude _func;
        // property cache
        //--- (end of YLongitude definitions)

        //--- (YLongitude implementation)
        internal YLongitudeProxy(YLongitude hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Longitude " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YLongitude) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YLongitude hwd = YLongitude.FindLongitude(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YLongitude hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Longitude callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YLongitude it = YLongitude.FirstLongitude();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextLongitude();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
        }
    }
    //--- (end of YLongitude implementation)
}

