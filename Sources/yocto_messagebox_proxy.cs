/*********************************************************************
 *
 *  $Id: yocto_messagebox_proxy.cs 38514 2019-11-26 16:54:39Z seb $
 *
 *  Implements YMessageBoxProxy, the Proxy API for MessageBox
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
using YoctoLib;


namespace YoctoProxyAPI
{
    //--- (generated code: YMessageBox class start)
    static public partial class YoctoProxyManager
    {
        public static YMessageBoxProxy FindMessageBox(string name)
        {
            // cases to handle:
            // name =""  no matching unknwn
            // name =""  unknown exists
            // name != "" no  matching unknown
            // name !="" unknown exists
            YMessageBox func = null;
            YMessageBoxProxy res = (YMessageBoxProxy)YFunctionProxy.FindSimilarUnknownFunction("YMessageBoxProxy");

            if (name == "") {
                if (res != null) return res;
                res = (YMessageBoxProxy)YFunctionProxy.FindSimilarKnownFunction("YMessageBoxProxy");
                if (res != null) return res;
                func = YMessageBox.FirstMessageBox();
                if (func != null) {
                    name = func.get_hardwareId();
                    if (func.get_userData() != null) {
                        return (YMessageBoxProxy)func.get_userData();
                    }
                }
            } else {
                func = YMessageBox.FindMessageBox(name);
                if (func.get_userData() != null) {
                    return (YMessageBoxProxy)func.get_userData();
                }
            }
            if (res == null) {
                res = new YMessageBoxProxy(func, name);
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
 *   The YMessageBox class provides SMS sending and receiving capability to
 *   GSM-enabled Yoctopuce devices, for instance using a YoctoHub-GSM-2G, a YoctoHub-GSM-3G-EU or a YoctoHub-GSM-3G-NA.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
    public class YMessageBoxProxy : YFunctionProxy
    {
        //--- (end of generated code: YMessageBox class start)
        //--- (generated code: YMessageBox definitions)
        public const int _SlotsInUse_INVALID = -1;
        public const int _SlotsCount_INVALID = -1;
        public const string _SlotsBitmap_INVALID = YAPI.INVALID_STRING;
        public const int _PduSent_INVALID = -1;
        public const int _PduReceived_INVALID = -1;
        public const string _Command_INVALID = YAPI.INVALID_STRING;

        // reference to real YoctoAPI object
        protected new YMessageBox _func;
        // property cache
        protected int _slotsInUse = _SlotsInUse_INVALID;
        //--- (end of generated code: YMessageBox definitions)

        //--- (generated code: YMessageBox implementation)
        internal YMessageBoxProxy(YMessageBox hwd, string instantiationName) : base(hwd, instantiationName)
        {
            InternalStuff.log("MessageBox " + instantiationName + " instantiation");
            base_init(hwd, instantiationName);
        }

        // perform the initial setup that may be done without a YoctoAPI object (hwd can be null)
        internal override void base_init(YFunction hwd, string instantiationName)
        {
            _func = (YMessageBox) hwd;
           	base.base_init(hwd, instantiationName);
        }

        // link the instance to a real YoctoAPI object
        internal override void linkToHardware(string hwdName)
        {
            YMessageBox hwd = YMessageBox.FindMessageBox(hwdName);
            // first redo base_init to update all _func pointers
            base_init(hwd, hwdName);
            // then setup Yocto-API pointers and callbacks
            init(hwd);
        }

        // perform the 2nd stage setup that requires YoctoAPI object
        protected void init(YMessageBox hwd)
        {
            if (hwd == null) return;
            base.init(hwd);
            InternalStuff.log("registering MessageBox callback");
            _func.registerValueCallback(valueChangeCallback);
        }

        public override string[] GetSimilarFunctions()
        {
            List<string> res = new List<string>();
            YMessageBox it = YMessageBox.FirstMessageBox();
            while (it != null)
            {
                res.Add(it.get_hardwareId());
                it = it.nextMessageBox();
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

        // property with cached value for instant access (advertised value)
        public int SlotsInUse
        {
            get
            {
                if (_func == null) return _SlotsInUse_INVALID;
                return (_online ? _slotsInUse : _SlotsInUse_INVALID);
            }
        }

        protected override void valueChangeCallback(YFunction source, string value)
        {
            base.valueChangeCallback(source, value);
            Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture,out _slotsInUse);
        }

        /**
         * <summary>
         *   Returns the number of message storage slots currently in use.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of message storage slots currently in use
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMessageBox.SLOTSINUSE_INVALID</c>.
         * </para>
         */
        public int get_slotsInUse()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_slotsInUse();
            if (res == YAPI.INVALID_INT) res = _SlotsInUse_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the total number of message storage slots on the SIM card.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the total number of message storage slots on the SIM card
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMessageBox.SLOTSCOUNT_INVALID</c>.
         * </para>
         */
        public int get_slotsCount()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_slotsCount();
            if (res == YAPI.INVALID_INT) res = _SlotsCount_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Returns the number of SMS units sent so far.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of SMS units sent so far
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMessageBox.PDUSENT_INVALID</c>.
         * </para>
         */
        public int get_pduSent()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_pduSent();
            if (res == YAPI.INVALID_INT) res = _PduSent_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the value of the outgoing SMS units counter.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the value of the outgoing SMS units counter
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
        public int set_pduSent(int newval)
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PduSent_INVALID) return YAPI.SUCCESS;
            return _func.set_pduSent(newval);
        }


        /**
         * <summary>
         *   Returns the number of SMS units received so far.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   an integer corresponding to the number of SMS units received so far
         * </returns>
         * <para>
         *   On failure, throws an exception or returns <c>YMessageBox.PDURECEIVED_INVALID</c>.
         * </para>
         */
        public int get_pduReceived()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            int res = _func.get_pduReceived();
            if (res == YAPI.INVALID_INT) res = _PduReceived_INVALID;
            return res;
        }

        /**
         * <summary>
         *   Changes the value of the incoming SMS units counter.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="newval">
         *   an integer corresponding to the value of the incoming SMS units counter
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
        public int set_pduReceived(int newval)
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            if (newval == _PduReceived_INVALID) return YAPI.SUCCESS;
            return _func.set_pduReceived(newval);
        }


        /**
         * <summary>
         *   Clear the SMS units counters.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int clearPduCounters()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.clearPduCounters();
        }

        /**
         * <summary>
         *   Sends a regular text SMS, with standard parameters.
         * <para>
         *   This function can send messages
         *   of more than 160 characters, using SMS concatenation. ISO-latin accented characters
         *   are supported. For sending messages with special unicode characters such as asian
         *   characters and emoticons, use <c>newMessage</c> to create a new message and define
         *   the content of using methods <c>addText</c> and <c>addUnicodeData</c>.
         * </para>
         * </summary>
         * <param name="recipient">
         *   a text string with the recipient phone number, either as a
         *   national number, or in international format starting with a plus sign
         * </param>
         * <param name="message">
         *   the text to be sent in the message
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sendTextMessage(string recipient, string message)
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sendTextMessage(recipient, message);
        }

        /**
         * <summary>
         *   Sends a Flash SMS (class 0 message).
         * <para>
         *   Flash messages are displayed on the handset
         *   immediately and are usually not saved on the SIM card. This function can send messages
         *   of more than 160 characters, using SMS concatenation. ISO-latin accented characters
         *   are supported. For sending messages with special unicode characters such as asian
         *   characters and emoticons, use <c>newMessage</c> to create a new message and define
         *   the content of using methods <c>addText</c> et <c>addUnicodeData</c>.
         * </para>
         * </summary>
         * <param name="recipient">
         *   a text string with the recipient phone number, either as a
         *   national number, or in international format starting with a plus sign
         * </param>
         * <param name="message">
         *   the text to be sent in the message
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int sendFlashMessage(string recipient, string message)
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            return _func.sendFlashMessage(recipient, message);
        }

        /**
         * <summary>
         *   Creates a new empty SMS message, to be configured and sent later on.
         * <para>
         * </para>
         * </summary>
         * <param name="recipient">
         *   a text string with the recipient phone number, either as a
         *   national number, or in international format starting with a plus sign
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual YSmsProxy newMessage(string recipient)
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            return new YSmsProxy(_func.newMessage(recipient));
        }

        /**
         * <summary>
         *   Returns the list of messages received and not deleted.
         * <para>
         *   This function
         *   will automatically decode concatenated SMS.
         * </para>
         * </summary>
         * <returns>
         *   an YSms object list.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns an empty list.
         * </para>
         */
        public virtual YSmsProxy[] get_messages()
        {
            if (_func == null)
            {
                string msg = "No MessageBox connected";
                throw new YoctoApiProxyException(msg);
            }
            int i = 0;
            var std_res = _func.get_messages();
            YSmsProxy[] proxy_res = new YSmsProxy[std_res.Count];
            foreach (var record in std_res) {
                proxy_res[i++] = new YSmsProxy(record);
            }
            return proxy_res;
        }
    }
    //--- (end of generated code: YMessageBox implementation)
}

