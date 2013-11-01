using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* From what I can tell of code, the Screenshot system is meant to share
 * screenshots between all players currently active on the server. This creates
 * a responsibility for this class to two a few things
 *  1) Receive screenshots from remote clients.
 *  2) Handle the screenshots (save to disk, save in memory, etc)
 *  3) Send back to clients on the server for viewing.
 * Currently this is a couple functions on the server, but it seems as though
 * it could be refactored into a Manager and perhaps a Service of some sort.
 * 
 * Functions to reimplement:
 *  1) saveScreenshot
 *  2) sendScreenshotToWatchers
 *  3) 
 */

namespace KMPServer
{
    class ScreenshotManager
    {
        private int m_screenshotMaxSize;

        public ScreenshotManager(int maxSize)
        {
            // TODO: Complete member initialization
            this.m_screenshotMaxSize = maxSize;
        }

        public bool getScreenshot(ServerClient client)
        {

        }
    }
}
