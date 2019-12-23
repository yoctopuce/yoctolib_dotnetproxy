/*********************************************************************
 *
 *  $Id: yocto_qt_proxy.cs 38913 2019-12-20 18:59:49Z mvuilleu $
 *
 *  Implements YQtProxy, the Proxy API for Qt
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
using YoctoLib;

namespace YoctoProxyAPI
{
    //--- (generated code: YQt class start)
    static public partial class YoctoProxyManager
    {
        public static YQtProxy FindQt(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YQt func = null;
            YQtProxy res = (YQtProxy)YFunctionProxy.FindSimilarUnknownFunction("YQtProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YQtProxy)YFunctionProxy.FindSimilarKnownFunction("YQtProxy");
                if (res != null) return res;
                func = YQt.FirstQt();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YQtProxy)func.get_userData();
                    }
                }
            } else {
                func = YQt.FindQt(name);
                if (func.get_userData() != null) {
                    return (YQtProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YQtProxy(func, name);
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
 *   The <c>YQt</c> class provides direct access to the 3D attitude estimation
 *   provided by Yoctopuce inertial sensors.
 * <para>
 *   The four instances of <c>YQt</c>
 *   provide direct access to the individual quaternion components representing the
 *   orientation. It is usually not needed to use the <c>YQt</c> class
 *   directly, as the <c>YGyro</c> class provides a more convenient higher-level
 *   interface.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YQtProxy : YSensorProxy
    {
        /**
         * <summary>
         *   Retrieves a quaternion component for a given identifier.
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
         *   This function does not require that the quaternion component is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YQt.isOnline()</c> to test if the quaternion component is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a quaternion component by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the quaternion component, for instance
         *   <c>Y3DMK002.qt1</c>.
         * </param>
         * <returns>
         *   a <c>YQt</c> object allowing you to drive the quaternion component.
         * </returns>
         */
        public static YQtProxy FindQt(string func)
        {
            return YoctoProxyManager.FindQt(func);
        }
        //--- (end of generated code: YQt class start)
        //--- (generated code: YQt definitions)

        // reference to real YoctoAPI object
        protected new YQt _func;
        // property cache
        //--- (end of generated code: YQt definitions)

        //--- (generated code: YQt implementation)
        internal YQtProxy(YQt hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Qt " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YQt) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YQt hwd = YQt.FindQt(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YQt hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Qt callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Qt available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YQt.FindQt</c> to obtain an object that can control the
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
            YQt it = YQt.FirstQt();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextQt();
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
    //--- (end of generated code: YQt implementation)
}

