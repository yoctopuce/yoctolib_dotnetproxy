/*********************************************************************
 *
 *  $Id: yocto_oscontrol_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
 *
 *  Implements YOsControlProxy, the Proxy API for OsControl
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
    //--- (YOsControl class start)
    static public partial class YoctoProxyManager
    {
        public static YOsControlProxy FindOsControl(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YOsControl func = null;
            YOsControlProxy res = (YOsControlProxy)YFunctionProxy.FindSimilarUnknownFunction("YOsControlProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YOsControlProxy)YFunctionProxy.FindSimilarKnownFunction("YOsControlProxy");
                if (res != null) return res;
                func = YOsControl.FirstOsControl();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YOsControlProxy)func.get_userData();
                    }
                }
            } else {
                func = YOsControl.FindOsControl(name);
                if (func.get_userData() != null) {
                    return (YOsControlProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YOsControlProxy(func, name);
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
 *   The <c>YOScontrol</c> class provides some control over the operating system running a VirtualHub.
 * <para>
 *   YOsControl is available on VirtualHub software only. This feature must be activated at the VirtualHub
 *   start up with -o option.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YOsControlProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves OS control for a given identifier.
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
         *   This function does not require that the OS control is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YOsControl.isOnline()</c> to test if the OS control is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   OS control by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the OS control, for instance
         *   <c>MyDevice.osControl</c>.
         * </param>
         * <returns>
         *   a <c>YOsControl</c> object allowing you to drive the OS control.
         * </returns>
         */
        public static YOsControlProxy FindOsControl(string func)
        {
            return YoctoProxyManager.FindOsControl(func);
        }
        //--- (end of YOsControl class start)
        //--- (YOsControl definitions)
        public const int _ShutdownCountdown_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YOsControl _func;
        // property cache
        //--- (end of YOsControl definitions)

        //--- (YOsControl implementation)
        internal YOsControlProxy(YOsControl hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("OsControl " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YOsControl) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YOsControl hwd = YOsControl.FindOsControl(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YOsControl hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering OsControl callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type OsControl available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YOsControl.FindOsControl</c> to obtain an object that can control the
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
            YOsControl it = YOsControl.FirstOsControl();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextOsControl();
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

        /**
         * <summary>
         *   Returns the remaining number of seconds before the OS shutdown, or zero when no
         *   shutdown has been scheduled.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the remaining number of seconds before the OS shutdown, or zero when no
         *   shutdown has been scheduled
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YOsControl.SHUTDOWNCOUNTDOWN_INVALID</c>.
         * </para>
         */
        public int get_shutdownCountdown()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No OsControl connected");
            }
            res = _func.get_shutdownCountdown();
            if (res == YAPI.INVALID_INT) {
                res = _ShutdownCountdown_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Schedules an OS shutdown after a given number of seconds.
         * <para>
         * </para>
         * </summary>
         * <param name="secBeforeShutDown">
         *   number of seconds before shutdown
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int shutdown(int secBeforeShutDown)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No OsControl connected");
            }
            return _func.shutdown(secBeforeShutDown);
        }
    }
    //--- (end of YOsControl implementation)
}

