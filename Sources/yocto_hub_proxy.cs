/*********************************************************************
 *
 *  $Id: svn_id $
 *
 *  Implements YHubProxy, the Proxy API for Hub
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
    //--- (YHub class start)
    public class YHubProxy
    {
        private YHub _objptr;
        internal YHubProxy(YHub objptr)
        {
             _objptr = objptr;
        }
        //--- (end of YHub class start)
        //--- (YHub definitions)
        //--- (end of YHub definitions)
        //--- (YHub implementation)

        /**
         * <summary>
         *   Returns the URL that has been used first to register this hub.
         * <para>
         * </para>
         * </summary>
         */
        public virtual string get_registeredUrl()
        {
            return _objptr.get_registeredUrl();
        }

        /**
         * <summary>
         *   Returns all known URLs that have been used to register this hub.
         * <para>
         *   URLs are pointing to the same hub when the devices connected
         *   are sharing the same serial number.
         * </para>
         * </summary>
         */
        public virtual string[] get_knownUrls()
        {
            return _objptr.get_knownUrls().ToArray();
        }

        /**
         * <summary>
         *   Returns the URL currently in use to communicate with this hub.
         * <para>
         * </para>
         * </summary>
         */
        public virtual string get_connectionUrl()
        {
            return _objptr.get_connectionUrl();
        }

        /**
         * <summary>
         *   Returns the hub serial number, if the hub was already connected once.
         * <para>
         * </para>
         * </summary>
         */
        public virtual string get_serialNumber()
        {
            return _objptr.get_serialNumber();
        }

        /**
         * <summary>
         *   Tells if this hub is still registered within the API.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if the hub has not been unregistered.
         * </returns>
         */
        public virtual bool isInUse()
        {
            return _objptr.isInUse();
        }

        /**
         * <summary>
         *   Tells if there is an active communication channel with this hub.
         * <para>
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if the hub is currently connected.
         * </returns>
         */
        public virtual bool isOnline()
        {
            return _objptr.isOnline();
        }

        /**
         * <summary>
         *   Tells if write access on this hub is blocked.
         * <para>
         *   Return <c>true</c> if it
         *   is not possible to change attributes on this hub
         * </para>
         * </summary>
         * <returns>
         *   <c>true</c> if it is not possible to change attributes on this hub.
         * </returns>
         */
        public virtual bool isReadOnly()
        {
            return _objptr.isReadOnly();
        }

        /**
         * <summary>
         *   Modifies tthe network connection delay for this hub.
         * <para>
         *   The default value is inherited from <c>ySetNetworkTimeout</c>
         *   at the time when the hub is registered, but it can be updated
         *   afterward for each specific hub if necessary.
         * </para>
         * <para>
         * </para>
         * </summary>
         * <param name="networkMsTimeout">
         *   the network connection delay in milliseconds.
         * @noreturn
         * </param>
         */
        public virtual void set_networkTimeout(int networkMsTimeout)
        {
            _objptr.set_networkTimeout(networkMsTimeout);
        }

        /**
         * <summary>
         *   Returns the network connection delay for this hub.
         * <para>
         *   The default value is inherited from <c>ySetNetworkTimeout</c>
         *   at the time when the hub is registered, but it can be updated
         *   afterward for each specific hub if necessary.
         * </para>
         * </summary>
         * <returns>
         *   the network connection delay in milliseconds.
         * </returns>
         */
        public virtual int get_networkTimeout()
        {
            return _objptr.get_networkTimeout();
        }

        /**
         * <summary>
         *   Returns the numerical error code of the latest error with the hub.
         * <para>
         *   This method is mostly useful when using the Yoctopuce library with
         *   exceptions disabled.
         * </para>
         * </summary>
         * <returns>
         *   a number corresponding to the code of the latest error that occurred while
         *   using the hub object
         * </returns>
         */
        public virtual int get_errorType()
        {
            return _objptr.get_errorType();
        }

        /**
         * <summary>
         *   Returns the error message of the latest error with the hub.
         * <para>
         *   This method is mostly useful when using the Yoctopuce library with
         *   exceptions disabled.
         * </para>
         * </summary>
         * <returns>
         *   a string corresponding to the latest error message that occured while
         *   using the hub object
         * </returns>
         */
        public virtual string get_errorMessage()
        {
            return _objptr.get_errorMessage();
        }

        /**
         * <summary>
         *   Starts the enumeration of hubs currently in use by the API.
         * <para>
         *   Use the method <c>YHub.nextHubInUse()</c> to iterate on the
         *   next hubs.
         * </para>
         * </summary>
         * <returns>
         *   a pointer to a <c>YHub</c> object, corresponding to
         *   the first hub currently in use by the API, or a
         *   <c>null</c> pointer if none has been registered.
         * </returns>
         */
        public static YHubProxy FirstHubInUse()
        {
            return new YHubProxy(YHub.FirstHubInUse());
        }

        /**
         * <summary>
         *   Continues the module enumeration started using <c>YHub.FirstHubInUse()</c>.
         * <para>
         *   Caution: You can't make any assumption about the order of returned hubs.
         * </para>
         * </summary>
         * <returns>
         *   a pointer to a <c>YHub</c> object, corresponding to
         *   the next hub currenlty in use, or a <c>null</c> pointer
         *   if there are no more hubs to enumerate.
         * </returns>
         */
        public virtual YHubProxy nextHubInUse()
        {
            return new YHubProxy(_objptr.nextHubInUse());
        }
    }
    //--- (end of YHub implementation)
}

