/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YPressureProxy, the Proxy API for Pressure
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
    //--- (YPressure class start)
    static public partial class YoctoProxyManager
    {
        public static YPressureProxy FindPressure(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YPressure func = null;
            YPressureProxy res = (YPressureProxy)YFunctionProxy.FindSimilarUnknownFunction("YPressureProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YPressureProxy)YFunctionProxy.FindSimilarKnownFunction("YPressureProxy");
                if (res != null) return res;
                func = YPressure.FirstPressure();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YPressureProxy)func.get_userData();
                    }
                }
            } else {
                func = YPressure.FindPressure(name);
                if (func.get_userData() != null) {
                    return (YPressureProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YPressureProxy(func, name);
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
 *   The <c>YPressure</c> class allows you to read and configure Yoctopuce pressure sensors.
 * <para>
 *   It inherits from <c>YSensor</c> class the core functions to read measures,
 *   to register callback functions, and to access the autonomous datalogger.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YPressureProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a pressure sensor for a given identifier.
         * <para>
         *   The identifier can be specified using several formats:
         * </para>
         * <para>
         * </para>
         * <para>
         *   - FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleSerialNumber.FunctionLogicalName
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionIdentifier
         * </para>
         * <para>
         *   - ModuleLogicalName.FunctionLogicalName
         * </para>
         * <para>
         * </para>
         * <para>
         *   This function does not require that the pressure sensor is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YPressure.isOnline()</c> to test if the pressure sensor is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a pressure sensor by logical name, no error is notified: the first instance
         *   found is returned. The search is performed first by hardware name,
         *   then by logical name.
         * </para>
         * <para>
         *   If a call to this object's is_online() method returns FALSE although
         *   you are certain that the matching device is plugged, make sure that you did
         *   call registerHub() at application initialization time.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="func">
         *   a string that uniquely characterizes the pressure sensor, for instance
         *   <c>YALTIMK2.pressure</c>.
         * </param>
         * <returns>
         *   a <c>YPressure</c> object allowing you to drive the pressure sensor.
         * </returns>
         */
        public static YPressureProxy FindPressure(string func)
        {
            return YoctoProxyManager.FindPressure(func);
        }
        //--- (end of YPressure class start)
        //--- (YPressure definitions)

        // reference to real YoctoAPI object
        protected new YPressure _func;
        // property cache
        //--- (end of YPressure definitions)

        //--- (YPressure implementation)
        internal YPressureProxy(YPressure hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Pressure " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YPressure) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YPressure hwd = YPressure.FindPressure(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YPressure hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Pressure callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Pressure available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YPressure.FindPressure</c> to obtain an object that can control the
         *   corresponding device.
         * </para>
         * </summary>
         * <returns>
         *   an array of strings, each string containing the unique hardwareId
         *   of a device function currently connected.
         * </returns>
         */
        public static new string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YPressure it = YPressure.FirstPressure();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextPressure();
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
    //--- (end of YPressure implementation)
}

