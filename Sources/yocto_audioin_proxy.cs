/*********************************************************************
 *
 *  $Id: yocto_audioin_proxy.cs 38282 2019-11-21 14:50:25Z seb $
 *
 *  Implements YAudioInProxy, the Proxy API for AudioIn
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
    //--- (YAudioIn class start)
    static public partial class YoctoProxyManager
    {
        public static YAudioInProxy FindAudioIn(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YAudioIn func = null;
            YAudioInProxy res = (YAudioInProxy)YFunctionProxy.FindSimilarUnknownFunction("YAudioInProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YAudioInProxy)YFunctionProxy.FindSimilarKnownFunction("YAudioInProxy");
                if (res != null) return res;
                func = YAudioIn.FirstAudioIn();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YAudioInProxy)func.get_userData();
                    }
                }
            } else {
                func = YAudioIn.FindAudioIn(name);
                if (func.get_userData() != null) {
                    return (YAudioInProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YAudioInProxy(func, name);
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
 *   The YAudioIn class allows you to configure the volume of an audio input.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YAudioInProxy : YFunctionProxy
    {
        //--- (end of YAudioIn class start)
        //--- (YAudioIn definitions)
        public const int _Volume_INVALID = -1;
        public const int _Mute_INVALID = 0;
        public const int _Mute_FALSE = 1;
        public const int _Mute_TRUE = 2;
        public const string _VolumeRange_INVALID = YAPI.INVALID_STRING;
        public const int _Signal_INVALID = YAPI.INVALID_INT;
        public const int _NoSignalFor_INVALID = YAPI.INVALID_INT;

        // reference to real YoctoAPI object
        protected new YAudioIn _func;
        // property cache
        protected int _volume = _Volume_INVALID;
        protected int _mute = _Mute_INVALID;
        protected int _signal = _Signal_INVALID;
        //--- (end of YAudioIn definitions)

        //--- (YAudioIn implementation)
        internal YAudioInProxy(YAudioIn hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("AudioIn " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YAudioIn) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YAudioIn hwd = YAudioIn.FindAudioIn(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YAudioIn hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering AudioIn callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YAudioIn it = YAudioIn.FirstAudioIn();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextAudioIn();
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
            _volume = _func.get_volume();
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _mute = _func.get_mute()+1;
        }

        /**
         * <summary>
         *   Returns audio input gain, in per cents.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to audio input gain, in per cents
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAudioIn.VOLUME_INVALID</c>.
         * </para>
         */
        public int get_volume()
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_volume();
            if (res == YAPI.INVALID_INT) res = _Volume_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes audio input gain, in per cents.
         * <para>
         *   Remember to call the <c>saveToFlash()</c>
         *   method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to audio input gain, in per cents
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
        public int set_volume(int newval)
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Volume_INVALID) return YAPI.SUCCESS;
            return _func.set_volume(newval);
        }


        // property with cached value for instant access (configuration)
        public int Volume
        {
            get
            {
                if (_func == null) return _Volume_INVALID;
                return (_online ? _volume : _Volume_INVALID);
            }
            set
            {
                setprop_volume(value);
            }
        }

        // private helper for magic property
        private void setprop_volume(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Volume_INVALID) return;
            if (newval == _volume) return;
            _func.set_volume(newval);
            _volume = newval;
        }

        /**
         * <summary>
         *   Returns the state of the mute function.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   either <c>YAudioIn.MUTE_FALSE</c> or <c>YAudioIn.MUTE_TRUE</c>, according to the state of the mute function
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAudioIn.MUTE_INVALID</c>.
         * </para>
         */
        public int get_mute()
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.get_mute()+1;
        }

        /**
         * <summary>
         *   Changes the state of the mute function.
         * <para>
         *   Remember to call the matching module
         *   <c>saveToFlash()</c> method to save the setting permanently.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   either <c>YAudioIn.MUTE_FALSE</c> or <c>YAudioIn.MUTE_TRUE</c>, according to the state of the mute function
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
        public int set_mute(int newval)
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _Mute_INVALID) return YAPI.SUCCESS;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            return _func.set_mute(newval-1);
        }


        // property with cached value for instant access (configuration)
        public int Mute
        {
            get
            {
                if (_func == null) return _Mute_INVALID;
                return (_online ? _mute : _Mute_INVALID);
            }
            set
            {
                setprop_mute(value);
            }
        }

        // private helper for magic property
        private void setprop_mute(int newval)
        {
            if (_func == null) return;
            if (!_online) return;
            if (newval == _Mute_INVALID) return;
            if (newval == _mute) return;
            // our enums start at 0 instead of the 'usual' -1 for invalid
            _func.set_mute(newval-1);
            _mute = newval;
        }

        /**
         * <summary>
         *   Returns the supported volume range.
         * <para>
         *   The low value of the
         *   range corresponds to the minimal audible value. To
         *   completely mute the sound, use <c>set_mute()</c>
         *   instead of the <c>set_volume()</c>.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the supported volume range
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAudioIn.VOLUMERANGE_INVALID</c>.
         * </para>
         */
        public string get_volumeRange()
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_volumeRange();
        }

        // property with cached value for instant access (advertised value)
        public int Signal
        {
            get
            {
                if (_func == null) return _Signal_INVALID;
                return (_online ? _signal : _Signal_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _signal);
        }

        /**
         * <summary>
         *   Returns the detected input signal level.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the detected input signal level
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAudioIn.SIGNAL_INVALID</c>.
         * </para>
         */
        public int get_signal()
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_signal();
        }

        /**
         * <summary>
         *   Returns the number of seconds elapsed without detecting a signal.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of seconds elapsed without detecting a signal
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YAudioIn.NOSIGNALFOR_INVALID</c>.
         * </para>
         */
        public int get_noSignalFor()
        {
            if (_func == null)
            {
                string msg = "No AudioIn connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.get_noSignalFor();
        }
    }
    //--- (end of YAudioIn implementation)
}

