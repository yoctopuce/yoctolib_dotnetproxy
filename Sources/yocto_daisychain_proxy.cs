/*********************************************************************
 *
 *  $Id: yocto_daisychain_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YDaisyChainProxy, the Proxy API for DaisyChain
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
    //--- (YDaisyChain class start)
    static public partial class YoctoProxyManager
    {
        public static YDaisyChainProxy FindDaisyChain(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YDaisyChain func = null;
            YDaisyChainProxy res = (YDaisyChainProxy)YFunctionProxy.FindSimilarUnknownFunction("YDaisyChainProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YDaisyChainProxy)YFunctionProxy.FindSimilarKnownFunction("YDaisyChainProxy");
                if (res != null) return res;
                func = YDaisyChain.FirstDaisyChain();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YDaisyChainProxy)func.get_userData();
                    }
                }
            } else {
                func = YDaisyChain.FindDaisyChain(name);
                if (func.get_userData() != null) {
                    return (YDaisyChainProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YDaisyChainProxy(func, name);
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
 *   The YDaisyChain interface can be used to verify that devices that
 *   are daisy-chained directly from device to device, without a hub,
 *   are detected properly.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YDaisyChainProxy : YFunctionProxy
    {
        //--- (end of YDaisyChain class start)
        //--- (YDaisyChain definitions)
        public const int _DaisyState_INVALID = 0;
        public const int _DaisyState_READY = 1;
        public const int _DaisyState_IS_CHILD = 2;
        public const int _DaisyState_FIRMWARE_MISMATCH = 3;
        public const int _DaisyState_CHILD_MISSING = 4;
        public const int _DaisyState_CHILD_LOST = 5;
        public const int _ChildCount_INVALID = -1;
        public const int _RequiredChildCount_INVALID = -1;

        // reference to real YoctoAPI object
        protected new YDaisyChain _func;
        // property cache
        protected int _requiredChildCount = _RequiredChildCount_INVALID;
        //--- (end of YDaisyChain definitions)

        //--- (YDaisyChain implementation)
        internal YDaisyChainProxy(YDaisyChain hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("DaisyChain " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YDaisyChain) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YDaisyChain hwd = YDaisyChain.FindDaisyChain(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YDaisyChain hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering DaisyChain callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YDaisyChain it = YDaisyChain.FirstDaisyChain();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextDaisyChain();
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
            _requiredChildCount = _func.get_requiredChildCount();
        }

        /**
         * <summary>
         *   Returns the state of the daisy-link between modules.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YDaisyChain.DAISYSTATE_READY</c>, <c>YDaisyChain.DAISYSTATE_IS_CHILD</c>,
         *   <c>YDaisyChain.DAISYSTATE_FIRMWARE_MISMATCH</c>, <c>YDaisyChain.DAISYSTATE_CHILD_MISSING</c> and
         *   <c>YDaisyChain.DAISYSTATE_CHILD_LOST</c> corresponding to the state of the daisy-link between modules
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDaisyChain.DAISYSTATE_INVALID</c>.
         * </para>
         */
        public int get_daisyState()
        {
            if (_func == null)
            {
                string msg = "No DaisyChain connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_daisyState()+1;
        }

        /**
         * <summary>
         *   Returns the number of child nodes currently detected.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of child nodes currently detected
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDaisyChain.CHILDCOUNT_INVALID</c>.
         * </para>
         */
        public int get_childCount()
        {
            if (_func == null)
            {
                string msg = "No DaisyChain connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_childCount();
            if (res == YAPI.INVALID_INT) res = _ChildCount_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the number of child nodes expected in normal conditions.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of child nodes expected in normal conditions
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YDaisyChain.REQUIREDCHILDCOUNT_INVALID</c>.
         * </para>
         */
        public int get_requiredChildCount()
        {
            if (_func == null)
            {
                string msg = "No DaisyChain connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_requiredChildCount();
            if (res == YAPI.INVALID_INT) res = _RequiredChildCount_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the number of child nodes expected in normal conditions.
         * <para>
         *   If the value is zero, no check is performed. If it is non-zero, the number
         *   child nodes is checked on startup and the status will change to error if
         *   the count does not match. Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of child nodes expected in normal conditions
         * </param>
         * <para>
         * </para>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public int set_requiredChildCount(int newval)
        {
            if (_func == null)
            {
                string msg = "No DaisyChain connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _RequiredChildCount_INVALID) return YAPI.SUCCESS;
            return _func.set_requiredChildCount(newval);
        }


        // property with cached value for instant access (configuration)
        public int RequiredChildCount
        {
            get
            {
                if (_func == null) return _RequiredChildCount_INVALID;
                return (_online ? _requiredChildCount : _RequiredChildCount_INVALID);
            }
            set
            {
                setprop_requiredChildCount(value);
            }
        }

        // private helper for magic property
        private void setprop_requiredChildCount(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _RequiredChildCount_INVALID) return;
            if (newval == _requiredChildCount) return;
            _func.set_requiredChildCount(newval);
            _requiredChildCount = newval;
        }
    }
    //--- (end of YDaisyChain implementation)
}

