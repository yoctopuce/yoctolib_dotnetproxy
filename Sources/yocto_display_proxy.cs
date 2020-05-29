/*********************************************************************
 *
 *  $Id: yocto_display_proxy.cs 40656 2020-05-25 14:13:34Z mvuilleu $
 *
 *  Implements YDisplayProxy, the Proxy API for Display
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
    //--- (YDisplay class start)
    static public partial class YoctoProxyManager
    {
        public static YDisplayProxy FindDisplay(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YDisplay func = null;
            YDisplayProxy res = (YDisplayProxy)YFunctionProxy.FindSimilarUnknownFunction("YDisplayProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YDisplayProxy)YFunctionProxy.FindSimilarKnownFunction("YDisplayProxy");
                if (res != null) return res;
                func = YDisplay.FirstDisplay();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YDisplayProxy)func.get_userData();
                    }
                }
            } else {
                func = YDisplay.FindDisplay(name);
                if (func.get_userData() != null) {
                    return (YDisplayProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YDisplayProxy(func, name);
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
 *   The <c>YDisplay</c> class allows to drive Yoctopuce displays.
 * <para>
 *   Yoctopuce display interface has been designed to easily
 *   show information and images. The device provides built-in
 *   multi-layer rendering. Layers can be drawn offline, individually,
 *   and freely moved on the display. It can also replay recorded
 *   sequences (animations).
 * </para>
 * <para>
 *   In order to draw on the screen, you should use the
 *   <c>display.get_displayLayer</c> method to retrieve the layer(s) on
 *   which you want to draw, and then use methods defined in
 *   <c>YDisplayLayer</c> to draw on the layers.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YDisplayProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a display for a given identifier.
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
         *   This function does not require that the display is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YDisplay.isOnline()</c> to test if the display is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a display by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the display, for instance
         *   <c>YD128X32.display</c>.
         * </param>
         * <returns>
         *   a <c>YDisplay</c> object allowing you to drive the display.
         * </returns>
         */
        public static YDisplayProxy FindDisplay(string func)
        {
            return YoctoProxyManager.FindDisplay(func);
        }
        //--- (end of YDisplay class start)
        //--- (YDisplay definitions)
        public const int _Enabled_INVALID = 0;
        public const int _Enabled_FALSE = 1;
        public const int _Enabled_TRUE = 2;
        public const string _StartupSeq_INVALID = YAPI.INVALID_STRING;
        public const int _Brightness_INVALID = -1;
        public const int _Orientation_INVALID = 0;
        public const int _Orientation_LEFT = 1;
        public const int _Orientation_UP = 2;
        public const int _Orientation_RIGHT = 3;
        public const int _Orientation_DOWN = 4;
        public const int _DisplayWidth_INVALID = -1;
        public const int _DisplayHeight_INVALID = -1;
        public const int _DisplayType_INVALID = 0;
        public const int _DisplayType_MONO = 1;
        public const int _DisplayType_GRAY = 2;
        public const int _DisplayType_RGB = 3;
        public const int _LayerWidth_INVALID = -1;
        public const int _LayerHeight_INVALID = -1;
        public const int _LayerCount_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YDisplay _func;
        // property cache
        protected string _startupSeq = _StartupSeq_INVALID;
        protected int _brightness = _Brightness_INVALID;
        protected int _orientation = _Orientation_INVALID;
        protected int _displayWidth = _DisplayWidth_INVALID;
        protected int _displayHeight = _DisplayHeight_INVALID;
        protected int _displayType = _DisplayType_INVALID;
        protected int _layerWidth = _LayerWidth_INVALID;
        protected int _layerHeight = _LayerHeight_INVALID;
        protected int _layerCount = _LayerCount_INVALID;
        //--- (end of YDisplay definitions)

        //--- (YDisplay implementation)
        internal YDisplayProxy(YDisplay hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Display " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YDisplay) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YDisplay hwd = YDisplay.FindDisplay(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YDisplay hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Display callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Display available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YDisplay.FindDisplay</c> to obtain an object that can control the
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
            YDisplay it = YDisplay.FirstDisplay();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextDisplay();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _displayWidth = _func.get_displayWidth();
            _displayHeight = _func.get_displayHeight();
            _displayType = _func.get_displayType()+1;
            _layerWidth = _func.get_layerWidth();
            _layerHeight = _func.get_layerHeight();
            _layerCount = _func.get_layerCount();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _startupSeq = _func.get_startupSeq();
            _brightness = _func.get_brightness();
            _orientation = _func.get_orientation()+1;
        }

        /**
         * <summary>
         *   Returns true if the screen is powered, false otherwise.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>display._Enabled_FALSE</c> or <c>display._Enabled_TRUE</c>, according to true if the
         *   screen is powered, false otherwise
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Enabled_INVALID</c>.
         * </para>
         */
        public int get_enabled()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_enabled()+1;
        }

        /**
         * <summary>
         *   Changes the power state of the display.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>display._Enabled_FALSE</c> or <c>display._Enabled_TRUE</c>, according to the power state
         *   of the display
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
        public int set_enabled(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            if (newval == _Enabled_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_enabled(newval-1);
        }

        /**
         * <summary>
         *   Returns the name of the sequence to play when the displayed is powered on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the name of the sequence to play when the displayed is powered on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Startupseq_INVALID</c>.
         * </para>
         */
        public string get_startupSeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.get_startupSeq();
        }

        /**
         * <summary>
         *   Changes the name of the sequence to play when the displayed is powered on.
         * <para>
         *   Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a string corresponding to the name of the sequence to play when the displayed is powered on
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
        public int set_startupSeq(string newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            if (newval == _StartupSeq_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_startupSeq(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Name of the sequence to play when the displayed is powered on.</value>
        public string StartupSeq
        {
            get
            {
                if (_func == null) {
                    return _StartupSeq_INVALID;
                }
                if (_online) {
                    return _startupSeq;
                }
                return _StartupSeq_INVALID;
            }
            set
            {
                setprop_startupSeq(value);
            }
        }

        // private helper for magic property
        private void setprop_startupSeq(string newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _StartupSeq_INVALID) {
                return;
            }
            if (newval == _startupSeq) {
                return;
            }
            _func.set_startupSeq(newval);
            _startupSeq = newval;
        }

        /**
         * <summary>
         *   Returns the luminosity of the  module informative LEDs (from 0 to 100).
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the luminosity of the  module informative LEDs (from 0 to 100)
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Brightness_INVALID</c>.
         * </para>
         */
        public int get_brightness()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_brightness();
            if (res == YAPI.INVALID_INT) {
                res = _Brightness_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the brightness of the display.
         * <para>
         *   The parameter is a value between 0 and
         *   100. Remember to call the <c>saveToFlash()</c> method of the module if the
         *   modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the brightness of the display
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
        public int set_brightness(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            if (newval == _Brightness_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_brightness(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Luminosity of the  module informative LEDs (from 0 to 100).</value>
        public int Brightness
        {
            get
            {
                if (_func == null) {
                    return _Brightness_INVALID;
                }
                if (_online) {
                    return _brightness;
                }
                return _Brightness_INVALID;
            }
            set
            {
                setprop_brightness(value);
            }
        }

        // private helper for magic property
        private void setprop_brightness(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Brightness_INVALID) {
                return;
            }
            if (newval == _brightness) {
                return;
            }
            _func.set_brightness(newval);
            _brightness = newval;
        }

        /**
         * <summary>
         *   Returns the currently selected display orientation.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>display._Orientation_LEFT</c>, <c>display._Orientation_UP</c>,
         *   <c>display._Orientation_RIGHT</c> and <c>display._Orientation_DOWN</c> corresponding to the
         *   currently selected display orientation
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Orientation_INVALID</c>.
         * </para>
         */
        public int get_orientation()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_orientation()+1;
        }

        /**
         * <summary>
         *   Changes the display orientation.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a value among <c>display._Orientation_LEFT</c>, <c>display._Orientation_UP</c>,
         *   <c>display._Orientation_RIGHT</c> and <c>display._Orientation_DOWN</c> corresponding to the display orientation
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
        public int set_orientation(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            if (newval == _Orientation_INVALID) {
                return YAPI.SUCCESS;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_orientation(newval-1);
        }

        // property with cached value for instant access (configuration)
        /// <value>Currently selected display orientation.</value>
        public int Orientation
        {
            get
            {
                if (_func == null) {
                    return _Orientation_INVALID;
                }
                if (_online) {
                    return _orientation;
                }
                return _Orientation_INVALID;
            }
            set
            {
                setprop_orientation(value);
            }
        }

        // private helper for magic property
        private void setprop_orientation(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Orientation_INVALID) {
                return;
            }
            if (newval == _orientation) {
                return;
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_orientation(newval-1);
            _orientation = newval;
        }

        /**
         * <summary>
         *   Returns the display width, in pixels.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the display width, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Displaywidth_INVALID</c>.
         * </para>
         */
        public int get_displayWidth()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_displayWidth();
            if (res == YAPI.INVALID_INT) {
                res = _DisplayWidth_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Display width, in pixels.</value>
        public int DisplayWidth
        {
            get
            {
                if (_func == null) {
                    return _DisplayWidth_INVALID;
                }
                if (_online) {
                    return _displayWidth;
                }
                return _DisplayWidth_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the display height, in pixels.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the display height, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Displayheight_INVALID</c>.
         * </para>
         */
        public int get_displayHeight()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_displayHeight();
            if (res == YAPI.INVALID_INT) {
                res = _DisplayHeight_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Display height, in pixels.</value>
        public int DisplayHeight
        {
            get
            {
                if (_func == null) {
                    return _DisplayHeight_INVALID;
                }
                if (_online) {
                    return _displayHeight;
                }
                return _DisplayHeight_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the display type: monochrome, gray levels or full color.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a value among <c>display._Displaytype_MONO</c>, <c>display._Displaytype_GRAY</c> and
         *   <c>display._Displaytype_RGB</c> corresponding to the display type: monochrome, gray levels or full color
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Displaytype_INVALID</c>.
         * </para>
         */
        public int get_displayType()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_displayType()+1;
        }

        // property with cached value for instant access (constant value)
        /// <value>Display type: monochrome, gray levels or full color.</value>
        public int DisplayType
        {
            get
            {
                if (_func == null) {
                    return _DisplayType_INVALID;
                }
                if (_online) {
                    return _displayType;
                }
                return _DisplayType_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the width of the layers to draw on, in pixels.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the width of the layers to draw on, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Layerwidth_INVALID</c>.
         * </para>
         */
        public int get_layerWidth()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_layerWidth();
            if (res == YAPI.INVALID_INT) {
                res = _LayerWidth_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Width of the layers to draw on, in pixels.</value>
        public int LayerWidth
        {
            get
            {
                if (_func == null) {
                    return _LayerWidth_INVALID;
                }
                if (_online) {
                    return _layerWidth;
                }
                return _LayerWidth_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the height of the layers to draw on, in pixels.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the height of the layers to draw on, in pixels
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Layerheight_INVALID</c>.
         * </para>
         */
        public int get_layerHeight()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_layerHeight();
            if (res == YAPI.INVALID_INT) {
                res = _LayerHeight_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Height of the layers to draw on, in pixels.</value>
        public int LayerHeight
        {
            get
            {
                if (_func == null) {
                    return _LayerHeight_INVALID;
                }
                if (_online) {
                    return _layerHeight;
                }
                return _LayerHeight_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the number of available layers to draw on.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of available layers to draw on
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>display._Layercount_INVALID</c>.
         * </para>
         */
        public int get_layerCount()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            res = _func.get_layerCount();
            if (res == YAPI.INVALID_INT) {
                res = _LayerCount_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Number of available layers to draw on.</value>
        public int LayerCount
        {
            get
            {
                if (_func == null) {
                    return _LayerCount_INVALID;
                }
                if (_online) {
                    return _layerCount;
                }
                return _LayerCount_INVALID;
            }
        }

        /**
         * <summary>
         *   Clears the display screen and resets all display layers to their default state.
         * <para>
         *   Using this function in a sequence will kill the sequence play-back. Don't use that
         *   function to reset the display at sequence start-up.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int resetAll()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.resetAll();
        }

        /**
         * <summary>
         *   Smoothly changes the brightness of the screen to produce a fade-in or fade-out
         *   effect.
         * <para>
         * </para>
         * </summary>
         * <param name="brightness">
         *   the new screen brightness
         * </param>
         * <param name="duration">
         *   duration of the brightness transition, in milliseconds.
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int fade(int brightness, int duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.fade(brightness, duration);
        }

        /**
         * <summary>
         *   Starts to record all display commands into a sequence, for later replay.
         * <para>
         *   The name used to store the sequence is specified when calling
         *   <c>saveSequence()</c>, once the recording is complete.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int newSequence()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.newSequence();
        }

        /**
         * <summary>
         *   Stops recording display commands and saves the sequence into the specified
         *   file on the display internal memory.
         * <para>
         *   The sequence can be later replayed
         *   using <c>playSequence()</c>.
         * </para>
         * </summary>
         * <param name="sequenceName">
         *   the name of the newly created sequence
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int saveSequence(string sequenceName)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.saveSequence(sequenceName);
        }

        /**
         * <summary>
         *   Replays a display sequence previously recorded using
         *   <c>newSequence()</c> and <c>saveSequence()</c>.
         * <para>
         * </para>
         * </summary>
         * <param name="sequenceName">
         *   the name of the newly created sequence
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int playSequence(string sequenceName)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.playSequence(sequenceName);
        }

        /**
         * <summary>
         *   Waits for a specified delay (in milliseconds) before playing next
         *   commands in current sequence.
         * <para>
         *   This method can be used while
         *   recording a display sequence, to insert a timed wait in the sequence
         *   (without any immediate effect). It can also be used dynamically while
         *   playing a pre-recorded sequence, to suspend or resume the execution of
         *   the sequence. To cancel a delay, call the same method with a zero delay.
         * </para>
         * </summary>
         * <param name="delay_ms">
         *   the duration to wait, in milliseconds
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int pauseSequence(int delay_ms)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.pauseSequence(delay_ms);
        }

        /**
         * <summary>
         *   Stops immediately any ongoing sequence replay.
         * <para>
         *   The display is left as is.
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int stopSequence()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.stopSequence();
        }

        /**
         * <summary>
         *   Uploads an arbitrary file (for instance a GIF file) to the display, to the
         *   specified full path name.
         * <para>
         *   If a file already exists with the same path name,
         *   its content is overwritten.
         * </para>
         * </summary>
         * <param name="pathname">
         *   path and name of the new file to create
         * </param>
         * <param name="content">
         *   binary buffer with the content to set
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int upload(string pathname, byte[] content)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.upload(pathname, content);
        }

        /**
         * <summary>
         *   Copies the whole content of a layer to another layer.
         * <para>
         *   The color and transparency
         *   of all the pixels from the destination layer are set to match the source pixels.
         *   This method only affects the displayed content, but does not change any
         *   property of the layer object.
         *   Note that layer 0 has no transparency support (it is always completely opaque).
         * </para>
         * </summary>
         * <param name="srcLayerId">
         *   the identifier of the source layer (a number in range 0..layerCount-1)
         * </param>
         * <param name="dstLayerId">
         *   the identifier of the destination layer (a number in range 0..layerCount-1)
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int copyLayerContent(int srcLayerId, int dstLayerId)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.copyLayerContent(srcLayerId, dstLayerId);
        }

        /**
         * <summary>
         *   Swaps the whole content of two layers.
         * <para>
         *   The color and transparency of all the pixels from
         *   the two layers are swapped. This method only affects the displayed content, but does
         *   not change any property of the layer objects. In particular, the visibility of each
         *   layer stays unchanged. When used between one hidden layer and a visible layer,
         *   this method makes it possible to easily implement double-buffering.
         *   Note that layer 0 has no transparency support (it is always completely opaque).
         * </para>
         * </summary>
         * <param name="layerIdA">
         *   the first layer (a number in range 0..layerCount-1)
         * </param>
         * <param name="layerIdB">
         *   the second layer (a number in range 0..layerCount-1)
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int swapLayerContent(int layerIdA, int layerIdB)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Display connected");
            }
            return _func.swapLayerContent(layerIdA, layerIdB);
        }
    }
    //--- (end of YDisplay implementation)
}

