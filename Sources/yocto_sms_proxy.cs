/*********************************************************************
 *
 *  $Id: yocto_sms_proxy.cs 43619 2021-01-29 09:14:45Z mvuilleu $
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
    //--- (YSms class start)
    public class YSmsProxy
    {
        private YSms _objref;
        internal YSmsProxy(YSms objref)
        {
             _objref = objref;
        }
        //--- (end of YSms class start)
        //--- (YSms definitions)
        //--- (end of YSms definitions)
        //--- (YSms implementation)

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
         *   <c>0</c> when the call succeeds.
         * </returns>
         */
        public virtual int addText(string val)
        {
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
         *   <c>0</c> when the call succeeds.
         * </returns>
         */
        public virtual int addUnicodeData(int[] val)
        {
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
         *   <c>0</c> when the call succeeds.
         * </returns>
         * <para>
         *   On failure, throws an exception or returns a negative error code.
         * </para>
         */
        public virtual int send()
        {
            return _objref.send();
        }
    }
    //--- (end of YSms implementation)
}

