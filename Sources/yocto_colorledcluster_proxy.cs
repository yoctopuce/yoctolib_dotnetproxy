/*********************************************************************
 *
 *  $Id: yocto_colorledcluster_proxy.cs 44921 2021-05-06 08:03:05Z mvuilleu $
 *
 *  Implements YColorLedClusterProxy, the Proxy API for ColorLedCluster
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
    //--- (YColorLedCluster class start)
    static public partial class YoctoProxyManager
    {
        public static YColorLedClusterProxy FindColorLedCluster(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YColorLedCluster func = null;
            YColorLedClusterProxy res = (YColorLedClusterProxy)YFunctionProxy.FindSimilarUnknownFunction("YColorLedClusterProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YColorLedClusterProxy)YFunctionProxy.FindSimilarKnownFunction("YColorLedClusterProxy");
                if (res != null) return res;
                func = YColorLedCluster.FirstColorLedCluster();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YColorLedClusterProxy)func.get_userData();
                    }
                }
            } else {
                func = YColorLedCluster.FindColorLedCluster(name);
                if (func.get_userData() != null) {
                    return (YColorLedClusterProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YColorLedClusterProxy(func, name);
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
 *   The <c>YColorLedCluster</c> class allows you to drive a
 *   color LED cluster.
 * <para>
 *   Unlike the <c>ColorLed</c> class, the <c>YColorLedCluster</c>
 *   class allows to handle several LEDs at once. Color changes can be done using RGB
 *   coordinates as well as HSL coordinates.
 *   The module performs all conversions form RGB to HSL automatically. It is then
 *   self-evident to turn on a LED with a given hue and to progressively vary its
 *   saturation or lightness. If needed, you can find more information on the
 *   difference between RGB and HSL in the section following this one.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YColorLedClusterProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a RGB LED cluster for a given identifier.
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
         *   This function does not require that the RGB LED cluster is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YColorLedCluster.isOnline()</c> to test if the RGB LED cluster is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a RGB LED cluster by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the RGB LED cluster, for instance
         *   <c>YRGBLED2.colorLedCluster</c>.
         * </param>
         * <returns>
         *   a <c>YColorLedCluster</c> object allowing you to drive the RGB LED cluster.
         * </returns>
         */
        public static YColorLedClusterProxy FindColorLedCluster(string func)
        {
            return YoctoProxyManager.FindColorLedCluster(func);
        }
        //--- (end of YColorLedCluster class start)
        //--- (YColorLedCluster definitions)
        public const int _ActiveLedCount_INVALID = -1;
        public const int _LedType_INVALID = 0;
        public const int _LedType_RGB = 1;
        public const int _LedType_RGBW = 2;
        public const int _LedType_WS2811 = 3;
        public const int _MaxLedCount_INVALID = -1;
        public const int _BlinkSeqMaxCount_INVALID = -1;
        public const int _BlinkSeqMaxSize_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YColorLedCluster _func;
        // property cache
        protected int _activeLedCount = _ActiveLedCount_INVALID;
        protected int _ledType = _LedType_INVALID;
        protected int _maxLedCount = _MaxLedCount_INVALID;
        protected int _blinkSeqMaxCount = _BlinkSeqMaxCount_INVALID;
        protected int _blinkSeqMaxSize = _BlinkSeqMaxSize_INVALID;
        //--- (end of YColorLedCluster definitions)

        //--- (YColorLedCluster implementation)
        internal YColorLedClusterProxy(YColorLedCluster hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("ColorLedCluster " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YColorLedCluster) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YColorLedCluster hwd = YColorLedCluster.FindColorLedCluster(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YColorLedCluster hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering ColorLedCluster callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type ColorLedCluster available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YColorLedCluster.FindColorLedCluster</c> to obtain an object that can control the
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
            YColorLedCluster it = YColorLedCluster.FirstColorLedCluster();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextColorLedCluster();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _maxLedCount = _func.get_maxLedCount();
            _blinkSeqMaxCount = _func.get_blinkSeqMaxCount();
            _blinkSeqMaxSize = _func.get_blinkSeqMaxSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _activeLedCount = _func.get_activeLedCount();
            _ledType = _func.get_ledType()+1;
        }

        /**
         * <summary>
         *   Returns the number of LEDs currently handled by the device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of LEDs currently handled by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLedCluster.ACTIVELEDCOUNT_INVALID</c>.
         * </para>
         */
        public int get_activeLedCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            res = _func.get_activeLedCount();
            if (res == YAPI.INVALID_INT) {
                res = _ActiveLedCount_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the number of LEDs currently handled by the device.
         * <para>
         *   Remember to call the matching module
         *   <c>saveToFlash()</c> method to save the setting permanently.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the number of LEDs currently handled by the device
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
        public int set_activeLedCount(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            if (newval == _ActiveLedCount_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_activeLedCount(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Number of LEDs currently handled by the device.</value>
        public int ActiveLedCount
        {
            get
            {
                if (_func == null) {
                    return _ActiveLedCount_INVALID;
                }
                if (_online) {
                    return _activeLedCount;
                }
                return _ActiveLedCount_INVALID;
            }
            set
            {
                setprop_activeLedCount(value);
            }
        }

        // private helper for magic property
        private void setprop_activeLedCount(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _ActiveLedCount_INVALID) {
                return;
            }
            if (newval == _activeLedCount) {
                return;
            }
            _func.set_activeLedCount(newval);
            _activeLedCount = newval;
        }

        /**
         * <summary>
         *   Returns the RGB LED type currently handled by the device.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>YColorLedCluster.LEDTYPE_RGB</c>, <c>YColorLedCluster.LEDTYPE_RGBW</c> and
         *   <c>YColorLedCluster.LEDTYPE_WS2811</c> corresponding to the RGB LED type currently handled by the device
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLedCluster.LEDTYPE_INVALID</c>.
         * </para>
         */
        public int get_ledType()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_ledType()+1;
        }

        /**
         * <summary>
         *   Changes the RGB LED type currently handled by the device.
         * <para>
         *   Remember to call the matching module
         *   <c>saveToFlash()</c> method to save the setting permanently.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>YColorLedCluster.LEDTYPE_RGB</c>, <c>YColorLedCluster.LEDTYPE_RGBW</c> and
         *   <c>YColorLedCluster.LEDTYPE_WS2811</c> corresponding to the RGB LED type currently handled by the device
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
        public int set_ledType(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            if (newval == _LedType_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_ledType(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>RGB LED type currently handled by the device.</value>
        public int LedType
        {
            get
            {
                if (_func == null) {
                    return _LedType_INVALID;
                }
                if (_online) {
                    return _ledType;
                }
                return _LedType_INVALID;
            }
            set
            {
                setprop_ledType(value);
            }
        }

        // private helper for magic property
        private void setprop_ledType(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _LedType_INVALID) {
                return;
            }
            if (newval == _ledType) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_ledType(newval-1);
            _ledType = newval;
        }

        /**
         * <summary>
         *   Returns the maximum number of LEDs that the device can handle.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum number of LEDs that the device can handle
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLedCluster.MAXLEDCOUNT_INVALID</c>.
         * </para>
         */
        public int get_maxLedCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            res = _func.get_maxLedCount();
            if (res == YAPI.INVALID_INT) {
                res = _MaxLedCount_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum number of LEDs that the device can handle.</value>
        public int MaxLedCount
        {
            get
            {
                if (_func == null) {
                    return _MaxLedCount_INVALID;
                }
                if (_online) {
                    return _maxLedCount;
                }
                return _MaxLedCount_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the maximum number of sequences that the device can handle.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum number of sequences that the device can handle
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLedCluster.BLINKSEQMAXCOUNT_INVALID</c>.
         * </para>
         */
        public int get_blinkSeqMaxCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            res = _func.get_blinkSeqMaxCount();
            if (res == YAPI.INVALID_INT) {
                res = _BlinkSeqMaxCount_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum number of sequences that the device can handle.</value>
        public int BlinkSeqMaxCount
        {
            get
            {
                if (_func == null) {
                    return _BlinkSeqMaxCount_INVALID;
                }
                if (_online) {
                    return _blinkSeqMaxCount;
                }
                return _BlinkSeqMaxCount_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the maximum length of sequences.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum length of sequences
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YColorLedCluster.BLINKSEQMAXSIZE_INVALID</c>.
         * </para>
         */
        public int get_blinkSeqMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            res = _func.get_blinkSeqMaxSize();
            if (res == YAPI.INVALID_INT) {
                res = _BlinkSeqMaxSize_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum length of sequences.</value>
        public int BlinkSeqMaxSize
        {
            get
            {
                if (_func == null) {
                    return _BlinkSeqMaxSize_INVALID;
                }
                if (_online) {
                    return _blinkSeqMaxSize;
                }
                return _BlinkSeqMaxSize_INVALID;
            }
        }

        /**
         * <summary>
         *   Changes the current color of consecutive LEDs in the cluster, using a RGB color.
         * <para>
         *   Encoding is done as follows: 0xRRGGBB.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="rgbValue">
         *   new color.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_rgbColor(int ledIndex, int count, int rgbValue)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_rgbColor(ledIndex, count, rgbValue);
        }

        /**
         * <summary>
         *   Changes the  color at device startup of consecutive LEDs in the cluster, using a RGB color.
         * <para>
         *   Encoding is done as follows: 0xRRGGBB. Don't forget to call <c>saveLedsConfigAtPowerOn()</c>
         *   to make sure the modification is saved in the device flash memory.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="rgbValue">
         *   new color.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_rgbColorAtPowerOn(int ledIndex, int count, int rgbValue)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_rgbColorAtPowerOn(ledIndex, count, rgbValue);
        }

        /**
         * <summary>
         *   Changes the  color at device startup of consecutive LEDs in the cluster, using a HSL color.
         * <para>
         *   Encoding is done as follows: 0xHHSSLL. Don't forget to call <c>saveLedsConfigAtPowerOn()</c>
         *   to make sure the modification is saved in the device flash memory.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="hslValue">
         *   new color.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_hslColorAtPowerOn(int ledIndex, int count, int hslValue)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_hslColorAtPowerOn(ledIndex, count, hslValue);
        }

        /**
         * <summary>
         *   Changes the current color of consecutive LEDs in the cluster, using a HSL color.
         * <para>
         *   Encoding is done as follows: 0xHHSSLL.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="hslValue">
         *   new color.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_hslColor(int ledIndex, int count, int hslValue)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_hslColor(ledIndex, count, hslValue);
        }

        /**
         * <summary>
         *   Allows you to modify the current color of a group of adjacent LEDs to another color, in a seamless and
         *   autonomous manner.
         * <para>
         *   The transition is performed in the RGB space.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="rgbValue">
         *   new color (0xRRGGBB).
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int rgb_move(int ledIndex, int count, int rgbValue, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.rgb_move(ledIndex, count, rgbValue, delay);
        }

        /**
         * <summary>
         *   Allows you to modify the current color of a group of adjacent LEDs  to another color, in a seamless and
         *   autonomous manner.
         * <para>
         *   The transition is performed in the HSL space. In HSL, hue is a circular
         *   value (0..360°). There are always two paths to perform the transition: by increasing
         *   or by decreasing the hue. The module selects the shortest transition.
         *   If the difference is exactly 180°, the module selects the transition which increases
         *   the hue.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="hslValue">
         *   new color (0xHHSSLL).
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int hsl_move(int ledIndex, int count, int hslValue, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.hsl_move(ledIndex, count, hslValue, delay);
        }

        /**
         * <summary>
         *   Adds an RGB transition to a sequence.
         * <para>
         *   A sequence is a transition list, which can
         *   be executed in loop by a group of LEDs.  Sequences are persistent and are saved
         *   in the device flash memory as soon as the <c>saveBlinkSeq()</c> method is called.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="rgbValue">
         *   target color (0xRRGGBB)
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int addRgbMoveToBlinkSeq(int seqIndex, int rgbValue, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.addRgbMoveToBlinkSeq(seqIndex, rgbValue, delay);
        }

        /**
         * <summary>
         *   Adds an HSL transition to a sequence.
         * <para>
         *   A sequence is a transition list, which can
         *   be executed in loop by an group of LEDs.  Sequences are persistent and are saved
         *   in the device flash memory as soon as the <c>saveBlinkSeq()</c> method is called.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="hslValue">
         *   target color (0xHHSSLL)
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int addHslMoveToBlinkSeq(int seqIndex, int hslValue, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.addHslMoveToBlinkSeq(seqIndex, hslValue, delay);
        }

        /**
         * <summary>
         *   Adds a mirror ending to a sequence.
         * <para>
         *   When the sequence will reach the end of the last
         *   transition, its running speed will automatically be reversed so that the sequence plays
         *   in the reverse direction, like in a mirror. After the first transition of the sequence
         *   is played at the end of the reverse execution, the sequence starts again in
         *   the initial direction.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int addMirrorToBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.addMirrorToBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Adds to a sequence a jump to another sequence.
         * <para>
         *   When a pixel will reach this jump,
         *   it will be automatically relinked to the new sequence, and will run it starting
         *   from the beginning.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="linkSeqIndex">
         *   index of the sequence to chain.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int addJumpToBlinkSeq(int seqIndex, int linkSeqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.addJumpToBlinkSeq(seqIndex, linkSeqIndex);
        }

        /**
         * <summary>
         *   Adds a to a sequence a hard stop code.
         * <para>
         *   When a pixel will reach this stop code,
         *   instead of restarting the sequence in a loop it will automatically be unlinked
         *   from the sequence.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int addUnlinkToBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.addUnlinkToBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Links adjacent LEDs to a specific sequence.
         * <para>
         *   These LEDs start to execute
         *   the sequence as soon as  startBlinkSeq is called. It is possible to add an offset
         *   in the execution: that way we  can have several groups of LED executing the same
         *   sequence, with a  temporal offset. A LED cannot be linked to more than one sequence.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="offset">
         *   execution offset in ms.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int linkLedToBlinkSeq(int ledIndex, int count, int seqIndex, int offset)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.linkLedToBlinkSeq(ledIndex, count, seqIndex, offset);
        }

        /**
         * <summary>
         *   Links adjacent LEDs to a specific sequence at device power-on.
         * <para>
         *   Don't forget to configure
         *   the sequence auto start flag as well and call <c>saveLedsConfigAtPowerOn()</c>. It is possible to add an offset
         *   in the execution: that way we  can have several groups of LEDs executing the same
         *   sequence, with a  temporal offset. A LED cannot be linked to more than one sequence.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="offset">
         *   execution offset in ms.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int linkLedToBlinkSeqAtPowerOn(int ledIndex, int count, int seqIndex, int offset)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.linkLedToBlinkSeqAtPowerOn(ledIndex, count, seqIndex, offset);
        }

        /**
         * <summary>
         *   Links adjacent LEDs to a specific sequence.
         * <para>
         *   These LED start to execute
         *   the sequence as soon as  startBlinkSeq is called. This function automatically
         *   introduces a shift between LEDs so that the specified number of sequence periods
         *   appears on the group of LEDs (wave effect).
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <param name="seqIndex">
         *   sequence index.
         * </param>
         * <param name="periods">
         *   number of periods to show on LEDs.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int linkLedToPeriodicBlinkSeq(int ledIndex, int count, int seqIndex, int periods)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.linkLedToPeriodicBlinkSeq(ledIndex, count, seqIndex, periods);
        }

        /**
         * <summary>
         *   Unlinks adjacent LEDs from a  sequence.
         * <para>
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first affected LED.
         * </param>
         * <param name="count">
         *   affected LED count.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int unlinkLedFromBlinkSeq(int ledIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.unlinkLedFromBlinkSeq(ledIndex, count);
        }

        /**
         * <summary>
         *   Starts a sequence execution: every LED linked to that sequence starts to
         *   run it in a loop.
         * <para>
         *   Note that a sequence with a zero duration can't be started.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to start.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int startBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.startBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Stops a sequence execution.
         * <para>
         *   If started again, the execution
         *   restarts from the beginning.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to stop.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int stopBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.stopBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Stops a sequence execution and resets its contents.
         * <para>
         *   LEDs linked to this
         *   sequence are not automatically updated anymore.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to reset
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int resetBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.resetBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Configures a sequence to make it start automatically at device
         *   startup.
         * <para>
         *   Note that a sequence with a zero duration can't be started.
         *   Don't forget to call <c>saveBlinkSeq()</c> to make sure the
         *   modification is saved in the device flash memory.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to reset.
         * </param>
         * <param name="autostart">
         *   0 to keep the sequence turned off and 1 to start it automatically.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_blinkSeqStateAtPowerOn(int seqIndex, int autostart)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_blinkSeqStateAtPowerOn(seqIndex, autostart);
        }

        /**
         * <summary>
         *   Changes the execution speed of a sequence.
         * <para>
         *   The natural execution speed is 1000 per
         *   thousand. If you configure a slower speed, you can play the sequence in slow-motion.
         *   If you set a negative speed, you can play the sequence in reverse direction.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to start.
         * </param>
         * <param name="speed">
         *   sequence running speed (-1000...1000).
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_blinkSeqSpeed(int seqIndex, int speed)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_blinkSeqSpeed(seqIndex, speed);
        }

        /**
         * <summary>
         *   Saves the LEDs power-on configuration.
         * <para>
         *   This includes the start-up color or
         *   sequence binding for all LEDs. Warning: if some LEDs are linked to a sequence, the
         *   method <c>saveBlinkSeq()</c> must also be called to save the sequence definition.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int saveLedsConfigAtPowerOn()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.saveLedsConfigAtPowerOn();
        }

        /**
         * <summary>
         *   Saves the definition of a sequence.
         * <para>
         *   Warning: only sequence steps and flags are saved.
         *   to save the LEDs startup bindings, the method <c>saveLedsConfigAtPowerOn()</c>
         *   must be called.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the sequence to start.
         * </param>
         * <returns>
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int saveBlinkSeq(int seqIndex)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.saveBlinkSeq(seqIndex);
        }

        /**
         * <summary>
         *   Sends a binary buffer to the LED RGB buffer, as is.
         * <para>
         *   First three bytes are RGB components for LED specified as parameter, the
         *   next three bytes for the next LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="buff">
         *   the binary buffer to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_rgbColorBuffer(int ledIndex, byte[] buff)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_rgbColorBuffer(ledIndex, buff);
        }

        /**
         * <summary>
         *   Sends 24bit RGB colors (provided as a list of integers) to the LED RGB buffer, as is.
         * <para>
         *   The first number represents the RGB value of the LED specified as parameter, the second
         *   number represents the RGB value of the next LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="rgbList">
         *   a list of 24bit RGB codes, in the form 0xRRGGBB
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_rgbColorArray(int ledIndex, int[] rgbList)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_rgbColorArray(ledIndex, new List<int>(rgbList));
        }

        /**
         * <summary>
         *   Sets up a smooth RGB color transition to the specified pixel-by-pixel list of RGB
         *   color codes.
         * <para>
         *   The first color code represents the target RGB value of the first LED,
         *   the next color code represents the target value of the next LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="rgbList">
         *   a list of target 24bit RGB codes, in the form 0xRRGGBB
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int rgbArrayOfs_move(int ledIndex, int[] rgbList, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.rgbArrayOfs_move(ledIndex, new List<int>(rgbList), delay);
        }

        /**
         * <summary>
         *   Sets up a smooth RGB color transition to the specified pixel-by-pixel list of RGB
         *   color codes.
         * <para>
         *   The first color code represents the target RGB value of the first LED,
         *   the next color code represents the target value of the next LED, etc.
         * </para>
         * </summary>
         * <param name="rgbList">
         *   a list of target 24bit RGB codes, in the form 0xRRGGBB
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int rgbArray_move(int[] rgbList, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.rgbArray_move(new List<int>(rgbList), delay);
        }

        /**
         * <summary>
         *   Sends a binary buffer to the LED HSL buffer, as is.
         * <para>
         *   First three bytes are HSL components for the LED specified as parameter, the
         *   next three bytes for the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="buff">
         *   the binary buffer to send
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_hslColorBuffer(int ledIndex, byte[] buff)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_hslColorBuffer(ledIndex, buff);
        }

        /**
         * <summary>
         *   Sends 24bit HSL colors (provided as a list of integers) to the LED HSL buffer, as is.
         * <para>
         *   The first number represents the HSL value of the LED specified as parameter, the second number represents
         *   the HSL value of the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="hslList">
         *   a list of 24bit HSL codes, in the form 0xHHSSLL
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int set_hslColorArray(int ledIndex, int[] hslList)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.set_hslColorArray(ledIndex, new List<int>(hslList));
        }

        /**
         * <summary>
         *   Sets up a smooth HSL color transition to the specified pixel-by-pixel list of HSL
         *   color codes.
         * <para>
         *   The first color code represents the target HSL value of the first LED,
         *   the second color code represents the target value of the second LED, etc.
         * </para>
         * </summary>
         * <param name="hslList">
         *   a list of target 24bit HSL codes, in the form 0xHHSSLL
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int hslArray_move(int[] hslList, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.hslArray_move(new List<int>(hslList), delay);
        }

        /**
         * <summary>
         *   Sets up a smooth HSL color transition to the specified pixel-by-pixel list of HSL
         *   color codes.
         * <para>
         *   The first color code represents the target HSL value of the first LED,
         *   the second color code represents the target value of the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be updated
         * </param>
         * <param name="hslList">
         *   a list of target 24bit HSL codes, in the form 0xHHSSLL
         * </param>
         * <param name="delay">
         *   transition duration in ms
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int hslArrayOfs_move(int ledIndex, int[] hslList, int delay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.hslArrayOfs_move(ledIndex, new List<int>(hslList), delay);
        }

        /**
         * <summary>
         *   Returns a binary buffer with content from the LED RGB buffer, as is.
         * <para>
         *   First three bytes are RGB components for the first LED in the interval,
         *   the next three bytes for the second LED in the interval, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be returned
         * </param>
         * <param name="count">
         *   number of LEDs which should be returned
         * </param>
         * <returns>
         *   a binary buffer with RGB components of selected LEDs.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty binary buffer.
         * </para>
         */
        public virtual byte[] get_rgbColorBuffer(int ledIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_rgbColorBuffer(ledIndex, count);
        }

        /**
         * <summary>
         *   Returns a list on 24bit RGB color values with the current colors displayed on
         *   the RGB LEDs.
         * <para>
         *   The first number represents the RGB value of the first LED,
         *   the second number represents the RGB value of the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be returned
         * </param>
         * <param name="count">
         *   number of LEDs which should be returned
         * </param>
         * <returns>
         *   a list of 24bit color codes with RGB components of selected LEDs, as 0xRRGGBB.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_rgbColorArray(int ledIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_rgbColorArray(ledIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list on 24bit RGB color values with the RGB LEDs startup colors.
         * <para>
         *   The first number represents the startup RGB value of the first LED,
         *   the second number represents the RGB value of the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED  which should be returned
         * </param>
         * <param name="count">
         *   number of LEDs which should be returned
         * </param>
         * <returns>
         *   a list of 24bit color codes with RGB components of selected LEDs, as 0xRRGGBB.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_rgbColorArrayAtPowerOn(int ledIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_rgbColorArrayAtPowerOn(ledIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list on sequence index for each RGB LED.
         * <para>
         *   The first number represents the
         *   sequence index for the the first LED, the second number represents the sequence
         *   index for the second LED, etc.
         * </para>
         * </summary>
         * <param name="ledIndex">
         *   index of the first LED which should be returned
         * </param>
         * <param name="count">
         *   number of LEDs which should be returned
         * </param>
         * <returns>
         *   a list of integers with sequence index
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_linkedSeqArray(int ledIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_linkedSeqArray(ledIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list on 32 bit signatures for specified blinking sequences.
         * <para>
         *   Since blinking sequences cannot be read from the device, this can be used
         *   to detect if a specific blinking sequence is already programmed.
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the first blinking sequence which should be returned
         * </param>
         * <param name="count">
         *   number of blinking sequences which should be returned
         * </param>
         * <returns>
         *   a list of 32 bit integer signatures
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_blinkSeqSignatures(int seqIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_blinkSeqSignatures(seqIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list of integers with the current speed for specified blinking sequences.
         * <para>
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the first sequence speed which should be returned
         * </param>
         * <param name="count">
         *   number of sequence speeds which should be returned
         * </param>
         * <returns>
         *   a list of integers, 0 for sequences turned off and 1 for sequences running
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_blinkSeqStateSpeed(int seqIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_blinkSeqStateSpeed(seqIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list of integers with the "auto-start at power on" flag state for specified blinking sequences.
         * <para>
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the first blinking sequence which should be returned
         * </param>
         * <param name="count">
         *   number of blinking sequences which should be returned
         * </param>
         * <returns>
         *   a list of integers, 0 for sequences turned off and 1 for sequences running
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_blinkSeqStateAtPowerOn(int seqIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_blinkSeqStateAtPowerOn(seqIndex, count).ToArray();
        }

        /**
         * <summary>
         *   Returns a list of integers with the started state for specified blinking sequences.
         * <para>
         * </para>
         * </summary>
         * <param name="seqIndex">
         *   index of the first blinking sequence which should be returned
         * </param>
         * <param name="count">
         *   number of blinking sequences which should be returned
         * </param>
         * <returns>
         *   a list of integers, 0 for sequences turned off and 1 for sequences running
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty array.
         * </para>
         */
        public virtual int[] get_blinkSeqState(int seqIndex, int count)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No ColorLedCluster connected");
            }
            return _func.get_blinkSeqState(seqIndex, count).ToArray();
        }
    }
    //--- (end of YColorLedCluster implementation)
}

