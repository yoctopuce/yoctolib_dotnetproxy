/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YSegmentedDisplayProxy, the Proxy API for SegmentedDisplay
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
    //--- (YSegmentedDisplay class start)
    static public partial class YoctoProxyManager
    {
        public static YSegmentedDisplayProxy FindSegmentedDisplay(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YSegmentedDisplay func = null;
            YSegmentedDisplayProxy res = (YSegmentedDisplayProxy)YFunctionProxy.FindSimilarUnknownFunction("YSegmentedDisplayProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YSegmentedDisplayProxy)YFunctionProxy.FindSimilarKnownFunction("YSegmentedDisplayProxy");
                if (res != null) return res;
                func = YSegmentedDisplay.FirstSegmentedDisplay();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YSegmentedDisplayProxy)func.get_userData();
                    }
                }
            } else {
                func = YSegmentedDisplay.FindSegmentedDisplay(name);
                if (func.get_userData() != null) {
                    return (YSegmentedDisplayProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YSegmentedDisplayProxy(func, name);
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
 *   The <c>SegmentedDisplay</c> class allows you to drive segmented displays.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YSegmentedDisplayProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a segmented display for a given identifier.
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
         *   This function does not require that the segmented display is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YSegmentedDisplay.isOnline()</c> to test if the segmented display is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a segmented display by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the segmented display, for instance
         *   <c>MyDevice.segmentedDisplay</c>.
         * </param>
         * <returns>
         *   a <c>YSegmentedDisplay</c> object allowing you to drive the segmented display.
         * </returns>
         */
        public static YSegmentedDisplayProxy FindSegmentedDisplay(string func)
        {
            return YoctoProxyManager.FindSegmentedDisplay(func);
        }
        //--- (end of YSegmentedDisplay class start)
        //--- (YSegmentedDisplay definitions)
        public const string _DisplayedText_INVALID = YAPI.INVALID_STRING;
        public const int _DisplayMode_INVALID = 0;
        public const int _DisplayMode_DISCONNECTED = 1;
        public const int _DisplayMode_MANUAL = 2;
        public const int _DisplayMode_AUTO1 = 3;
        public const int _DisplayMode_AUTO60 = 4;

        // reference to real YoctoAPI object
        protected new YSegmentedDisplay _func;
        // property cache
        //--- (end of YSegmentedDisplay definitions)

        //--- (YSegmentedDisplay implementation)
        internal YSegmentedDisplayProxy(YSegmentedDisplay hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("SegmentedDisplay " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YSegmentedDisplay) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YSegmentedDisplay hwd = YSegmentedDisplay.FindSegmentedDisplay(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YSegmentedDisplay hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering SegmentedDisplay callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type SegmentedDisplay available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YSegmentedDisplay.FindSegmentedDisplay</c> to obtain an object that can control the
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
            YSegmentedDisplay it = YSegmentedDisplay.FirstSegmentedDisplay();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextSegmentedDisplay();
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
         *   Returns the text currently displayed on the screen.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the text currently displayed on the screen
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YSegmentedDisplay.DISPLAYEDTEXT_INVALID</c>.
         * </para>
         */
        public string get_displayedText()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SegmentedDisplay connected");
            }
            return _func.get_displayedText();
        }

        /**
         * <summary>
         *   Changes the text currently displayed on the screen.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the text currently displayed on the screen
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_displayedText(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No SegmentedDisplay connected");
            }
            if (newval == _DisplayedText_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_displayedText(newval);
        }
    }
    //--- (end of YSegmentedDisplay implementation)
}

