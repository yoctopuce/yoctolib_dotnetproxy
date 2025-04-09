/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YBuzzerProxy, the Proxy API for Buzzer
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
    //--- (YBuzzer class start)
    static public partial class YoctoProxyManager
    {
        public static YBuzzerProxy FindBuzzer(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YBuzzer func = null;
            YBuzzerProxy res = (YBuzzerProxy)YFunctionProxy.FindSimilarUnknownFunction("YBuzzerProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YBuzzerProxy)YFunctionProxy.FindSimilarKnownFunction("YBuzzerProxy");
                if (res != null) return res;
                func = YBuzzer.FirstBuzzer();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YBuzzerProxy)func.get_userData();
                    }
                }
            } else {
                func = YBuzzer.FindBuzzer(name);
                if (func.get_userData() != null) {
                    return (YBuzzerProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YBuzzerProxy(func, name);
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
 *   The <c>YBuzzer</c> class allows you to drive a buzzer.
 * <para>
 *   You can
 *   choose the frequency and the volume at which the buzzer must sound.
 *   You can also pre-program a play sequence.
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YBuzzerProxy : YFunctionProxy
    {
        /**
         * <summary>
         *   Retrieves a buzzer for a given identifier.
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
         *   This function does not require that the buzzer is online at the time
         *   it is invoked. The returned object is nevertheless valid.
         *   Use the method <c>YBuzzer.isOnline()</c> to test if the buzzer is
         *   indeed online at a given time. In case of ambiguity when looking for
         *   a buzzer by logical name, no error is notified: the first instance
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
         *   a string that uniquely characterizes the buzzer, for instance
         *   <c>YBUZZER2.buzzer</c>.
         * </param>
         * <returns>
         *   a <c>YBuzzer</c> object allowing you to drive the buzzer.
         * </returns>
         */
        public static YBuzzerProxy FindBuzzer(string func)
        {
            return YoctoProxyManager.FindBuzzer(func);
        }
        //--- (end of YBuzzer class start)
        //--- (YBuzzer definitions)
        public const double _Frequency_INVALID = Double.NaN;
        public const int _Volume_INVALID = -1;
        public const int _PlaySeqSize_INVALID = -1;
        public const int _PlaySeqMaxSize_INVALID = -1;
        public const int _PlaySeqSignature_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YBuzzer _func;
        // property cache
        protected double _frequency = _Frequency_INVALID;
        protected int _volume = _Volume_INVALID;
        protected int _playSeqMaxSize = _PlaySeqMaxSize_INVALID;
        //--- (end of YBuzzer definitions)

        //--- (YBuzzer implementation)
        internal YBuzzerProxy(YBuzzer hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("Buzzer " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YBuzzer) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YBuzzer hwd = YBuzzer.FindBuzzer(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YBuzzer hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering Buzzer callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        /**
         * <summary>
         *   Enumerates all functions of type Buzzer available on the devices
         *   currently reachable by the library, and returns their unique hardware ID.
         * <para>
         *   Each of these IDs can be provided as argument to the method
         *   <c>YBuzzer.FindBuzzer</c> to obtain an object that can control the
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
            YBuzzer it = YBuzzer.FirstBuzzer();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextBuzzer();
            }
            return res.ToArray();
        }

        protected override void functionArrival()
        {
            base.functionArrival();
            _playSeqMaxSize = _func.get_playSeqMaxSize();
        }

        protected override void moduleConfigHasChanged()
       	{
            base.moduleConfigHasChanged();
            _volume = _func.get_volume();
        }

        /**
         * <summary>
         *   Changes the frequency of the signal sent to the buzzer.
         * <para>
         *   A zero value stops the buzzer.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   a floating point number corresponding to the frequency of the signal sent to the buzzer
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
        public int set_frequency(double newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            if (newval == _Frequency_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_frequency(newval);
        }

        /**
         * <summary>
         *   Returns the  frequency of the signal sent to the buzzer/speaker.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a floating point number corresponding to the  frequency of the signal sent to the buzzer/speaker
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBuzzer.FREQUENCY_INVALID</c>.
         * </para>
         */
        public double get_frequency()
        {
            double res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            res = _func.get_frequency();
            if (res == YAPI.INVALID_DOUBLE) {
                res = Double.NaN;
            }
            return res;
        }

        // property with cached value for instant access (advertised value)
        /// <value>Frequency of the signal sent to the buzzer/speaker.</value>
        public double Frequency
        {
            get
            {
                if (_func == null) {
                    return _Frequency_INVALID;
                }
                if (_online) {
                    return _frequency;
                }
                return _Frequency_INVALID;
            }
            set
            {
                setprop_frequency(value);
            }
        }

        // private helper for magic property
        private void setprop_frequency(double newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Frequency_INVALID) {
                return;
            }
            if (newval == _frequency) {
                return;
            }
            _func.set_frequency(newval);
            _frequency = newval;
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            _frequency = YAPI._atof(value);
        }

        /**
         * <summary>
         *   Returns the volume of the signal sent to the buzzer/speaker.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the volume of the signal sent to the buzzer/speaker
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBuzzer.VOLUME_INVALID</c>.
         * </para>
         */
        public int get_volume()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            res = _func.get_volume();
            if (res == YAPI.INVALID_INT) {
                res = _Volume_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Changes the volume of the signal sent to the buzzer/speaker.
         * <para>
         *   Remember to call the
         *   <c>saveToFlash()</c> method of the module if the modification must be kept.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the volume of the signal sent to the buzzer/speaker
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
        public int set_volume(int newval)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            if (newval == _Volume_INVALID) {
                return YAPI.SUCCESS;
            }
            return _func.set_volume(newval);
        }

        // property with cached value for instant access (configuration)
        /// <value>Volume of the signal sent to the buzzer/speaker.</value>
        public int Volume
        {
            get
            {
                if (_func == null) {
                    return _Volume_INVALID;
                }
                if (_online) {
                    return _volume;
                }
                return _Volume_INVALID;
            }
            set
            {
                setprop_volume(value);
            }
        }

        // private helper for magic property
        private void setprop_volume(int newval)
        {
            if (_func == null) {
                return;
            }
            if (!(_online)) {
                return;
            }
            if (newval == _Volume_INVALID) {
                return;
            }
            if (newval == _volume) {
                return;
            }
            _func.set_volume(newval);
            _volume = newval;
        }

        /**
         * <summary>
         *   Returns the current length of the playing sequence.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the current length of the playing sequence
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQSIZE_INVALID</c>.
         * </para>
         */
        public int get_playSeqSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            res = _func.get_playSeqSize();
            if (res == YAPI.INVALID_INT) {
                res = _PlaySeqSize_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Returns the maximum length of the playing sequence.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the maximum length of the playing sequence
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQMAXSIZE_INVALID</c>.
         * </para>
         */
        public int get_playSeqMaxSize()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            res = _func.get_playSeqMaxSize();
            if (res == YAPI.INVALID_INT) {
                res = _PlaySeqMaxSize_INVALID;
            }
            return res;
        }

        // property with cached value for instant access (constant value)
        /// <value>Maximum length of the playing sequence.</value>
        public int PlaySeqMaxSize
        {
            get
            {
                if (_func == null) {
                    return _PlaySeqMaxSize_INVALID;
                }
                if (_online) {
                    return _playSeqMaxSize;
                }
                return _PlaySeqMaxSize_INVALID;
            }
        }

        /**
         * <summary>
         *   Returns the playing sequence signature.
         * <para>
         *   As playing
         *   sequences cannot be read from the device, this can be used
         *   to detect if a specific playing sequence is already
         *   programmed.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the playing sequence signature
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YBuzzer.PLAYSEQSIGNATURE_INVALID</c>.
         * </para>
         */
        public int get_playSeqSignature()
        {
            int res;
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            res = _func.get_playSeqSignature();
            if (res == YAPI.INVALID_INT) {
                res = _PlaySeqSignature_INVALID;
            }
            return res;
        }

        /**
         * <summary>
         *   Adds a new frequency transition to the playing sequence.
         * <para>
         * </para>
         * </summary>
         * <param name="freq">
         *   desired frequency when the transition is completed, in Hz
         * </param>
         * <param name="msDelay">
         *   duration of the frequency transition, in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addFreqMoveToPlaySeq(int freq, int msDelay)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.addFreqMoveToPlaySeq(freq, msDelay);
        }

        /**
         * <summary>
         *   Adds a pulse to the playing sequence.
         * <para>
         * </para>
         * </summary>
         * <param name="freq">
         *   pulse frequency, in Hz
         * </param>
         * <param name="msDuration">
         *   pulse duration, in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addPulseToPlaySeq(int freq, int msDuration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.addPulseToPlaySeq(freq, msDuration);
        }

        /**
         * <summary>
         *   Adds a new volume transition to the playing sequence.
         * <para>
         *   Frequency stays untouched:
         *   if frequency is at zero, the transition has no effect.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="volume">
         *   desired volume when the transition is completed, as a percentage.
         * </param>
         * <param name="msDuration">
         *   duration of the volume transition, in milliseconds.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addVolMoveToPlaySeq(int volume, int msDuration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.addVolMoveToPlaySeq(volume, msDuration);
        }

        /**
         * <summary>
         *   Adds notes to the playing sequence.
         * <para>
         *   Notes are provided as text words, separated by
         *   spaces. The pitch is specified using the usual letter from A to G. The duration is
         *   specified as the divisor of a whole note: 4 for a fourth, 8 for an eight note, etc.
         *   Some modifiers are supported: <c>#</c> and <c>b</c> to alter a note pitch,
         *   <c>'</c> and <c>,</c> to move to the upper/lower octave, <c>.</c> to enlarge
         *   the note duration.
         * </para>
         * </summary>
         * <param name="notes">
         *   notes to be played, as a text string.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int addNotesToPlaySeq(string notes)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.addNotesToPlaySeq(notes);
        }

        /**
         * <summary>
         *   Starts the preprogrammed playing sequence.
         * <para>
         *   The sequence
         *   runs in loop until it is stopped by stopPlaySeq or an explicit
         *   change. To play the sequence only once, use <c>oncePlaySeq()</c>.
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int startPlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.startPlaySeq();
        }

        /**
         * <summary>
         *   Stops the preprogrammed playing sequence and sets the frequency to zero.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int stopPlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.stopPlaySeq();
        }

        /**
         * <summary>
         *   Resets the preprogrammed playing sequence and sets the frequency to zero.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int resetPlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.resetPlaySeq();
        }

        /**
         * <summary>
         *   Starts the preprogrammed playing sequence and run it once only.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int oncePlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.oncePlaySeq();
        }

        /**
         * <summary>
         *   Saves the preprogrammed playing sequence to flash memory.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int savePlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.savePlaySeq();
        }

        /**
         * <summary>
         *   Reloads the preprogrammed playing sequence from the flash memory.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int reloadPlaySeq()
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.reloadPlaySeq();
        }

        /**
         * <summary>
         *   Activates the buzzer for a short duration.
         * <para>
         * </para>
         * </summary>
         * <param name="frequency">
         *   pulse frequency, in hertz
         * </param>
         * <param name="duration">
         *   pulse duration in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int pulse(int frequency, int duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.pulse(frequency, duration);
        }

        /**
         * <summary>
         *   Makes the buzzer frequency change over a period of time.
         * <para>
         * </para>
         * </summary>
         * <param name="frequency">
         *   frequency to reach, in hertz. A frequency under 25Hz stops the buzzer.
         * </param>
         * <param name="duration">
         *   pulse duration in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int freqMove(int frequency, int duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.freqMove(frequency, duration);
        }

        /**
         * <summary>
         *   Makes the buzzer volume change over a period of time, frequency  stays untouched.
         * <para>
         * </para>
         * </summary>
         * <param name="volume">
         *   volume to reach in %
         * </param>
         * <param name="duration">
         *   change duration in milliseconds
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int volumeMove(int volume, int duration)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.volumeMove(volume, duration);
        }

        /**
         * <summary>
         *   Immediately play a note sequence.
         * <para>
         *   Notes are provided as text words, separated by
         *   spaces. The pitch is specified using the usual letter from A to G. The duration is
         *   specified as the divisor of a whole note: 4 for a fourth, 8 for an eight note, etc.
         *   Some modifiers are supported: <c>#</c> and <c>b</c> to alter a note pitch,
         *   <c>'</c> and <c>,</c> to move to the upper/lower octave, <c>.</c> to enlarge
         *   the note duration.
         * </para>
         * </summary>
         * <param name="notes">
         *   notes to be played, as a text string.
         * </param>
         * <returns>
         *   <c>0</c> if the call succeeds.
         *   On failure, throws an exception or returns a negative error code.
         * </returns>
         */
        public virtual int playNotes(string notes)
        {
            if (_func == null) {
                throw new YoctoApiProxyException("No Buzzer connected");
            }
            return _func.playNotes(notes);
        }
    }
    //--- (end of YBuzzer implementation)
}

