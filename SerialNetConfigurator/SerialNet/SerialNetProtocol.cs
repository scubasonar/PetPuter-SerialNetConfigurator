using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydr0Source.PetPuter.SerialNet
{
    //! This is where all of the command etc should be stored
    public class SerialNetProtocol
    {
        public Dictionary<String, int> configValueDict;


        //!< Configuration commands
        public enum CFG_COMMANDS
        {
            //! Networking Parameters
            WPANID = 100,//!< Extended PAN ID
            WCHAN, //!<       Active channel
            WCHMASK,//!<      Channel mask
            WCHPAGE,//!<      Channel page
            WAUTONET,//!<     Automatic networking
            WROLE,//!<        Node role
            GSN,  //!<        Device extended address
            WSRC, //!<        Node short address
            WNWKPANID, //!<   Short (network) PAN ID
            //! Network Management Functions            
            WNWK,//!<         Request for networking status
            WPARENT,//!<
            WCHILDREN,//!<
            WNBSIZE,//!<
            S30,//!<
            WLQI,//!<
            WRSSI,//!<
            //! Security
            WSECON, //!<
            WSECSTATUS,//!<
            WNETKEY,//!<
            WTCADDR,//!<
            //! Data Transmission           
            WPING,//!<
            WSYNCPRD,//!<
            WTIMEOUT,//!<
            WRETRY,//!<
            WWAIT,//!<
            //! Power Management
            WPWR,//!<
            WTXPWR,//!<
        }
            

        public SerialNetProtocol()
        {
            configValueDict = new Dictionary<string,int>();
            configValueDict.Add("+WPANID",(int)CFG_COMMANDS.WPANID);
            configValueDict.Add("+WCHAN", (int)CFG_COMMANDS.WCHAN);
            configValueDict.Add("+WCHMASK", (int)CFG_COMMANDS.WCHMASK);
            configValueDict.Add("+WCHPAGE", (int)CFG_COMMANDS.WCHPAGE);
            configValueDict.Add("+WAUTONET", (int)CFG_COMMANDS.WAUTONET);
            configValueDict.Add("+WROLE", (int)CFG_COMMANDS.WROLE);
            configValueDict.Add("+GSN", (int)CFG_COMMANDS.GSN);
            configValueDict.Add("+WNWKPANID", (int)CFG_COMMANDS.WNWKPANID);
        }

    }
}
