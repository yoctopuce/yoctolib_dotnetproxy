/*********************************************************************
 *
 *  $Id: yocto_sms_proxy.cs 38520 2019-11-26 23:12:57Z seb $
 *
 *  Implements YSmsProxy, the Proxy API for Sms
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
    //--- (generated code: YSms class start)
    public class YSmsProxy
    {
        private YSms _objref;
        internal YSmsProxy(YSms objref)
        {
             _objref = objref;
        }
        //--- (end of generated code: YSms class start)
        //--- (generated code: YSms definitions)
        //--- (end of generated code: YSms definitions)
        //--- (generated code: YSms implementation)

        /**
         * <summary>
         *   Returns the content of the message.
         * <para>
         * </para>
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   a string with the content of the message.
         * </returns>
         */
        public virtual string get_textData()
        {
            if (_objref == null)
            {
                string msg = "No Sms connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.get_textData();
        }

        /**
         * <summary>
         *   Add a regular text to the SMS.
         * <para>
         *   This function support messages
         *   of more than 160 characters. ISO-latin accented characters
         *   are supported. For messages with special unicode characters such as asian
         *   characters and emoticons, use the  <c>addUnicodeData</c> method.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="val">
         *   the text to be sent in the message
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         */
        public virtual int addText(string val)
        {
            if (_objref == null)
            {
                string msg = "No Sms connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.addText(val);
        }

        /**
         * <summary>
         *   Add a unicode text to the SMS.
         * <para>
         *   This function support messages
         *   of more than 160 characters, using SMS concatenation.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="val">
         *   an array of special unicode characters
         * </param>
         * <returns>
         *   <c>YAPI.SUCCESS</c> when the call succeeds.
         * </returns>
         */
        public virtual int addUnicodeData(int[] val)
        {
            if (_objref == null)
            {
                string msg = "No Sms connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.addUnicodeData(new List<int>(val));
        }

        /**
         * <summary>
         *   Sends the SMS to the recipient.
         * <para>
         *   Messages of more than 160 characters are supported
         *   using SMS concatenation.
         * </para>
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
        public virtual int send()
        {
            if (_objref == null)
            {
                string msg = "No Sms connected";
                throw new YoctoApiProxyException(msg);
            }
            return _objref.send();
        }
    }
    //--- (end of generated code: YSms implementation)
}

