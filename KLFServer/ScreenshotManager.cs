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
 *  4) Handle watching of clients? Might want to be refactored.
 * Currently this is a couple functions on the server, but it seems as though
 * it could be refactored into a Manager and perhaps a Service of some sort.
 * 
 * Functions to reimplement:
 *  1) saveScreenshot
 *  2) sendScreenshotToWatchers
 *  3) 
 *  4) SCREEN_WATCH_PLAYER
 *  
 * Seems to (so far) have an inherant dependency on some form of UserManager. Look
 * in to implementing this.
 */

namespace KMPServer
{
    class Screenshot
    {
        public byte[] screenshot;
        public object screenshot_lock;
    }

    class ScreenshotManager
    {
        private int m_screenshotMaxSize;
        private Dictionary<String, Screenshot> m_clientScreenshots;

        public ScreenshotManager(int maxSize)
        {
            // TODO: Complete member initialization
            this.m_screenshotMaxSize = maxSize;
        }

        public bool addClientWatch(ServerClient watcher, ServerClient client)
        {
            return true;
        }

        public bool setScreenshot(ServerClient client, byte[] screenshot)
        {
            if (screenshot == null)
                return false;
            if (screenshot.Length >= m_screenshotMaxSize)
                return false;
            
            String username = client.username;
            if (m_clientScreenshots.ContainsKey(username))
            {
                // If the user already has a screenshot, lock it and
                // replace the old screenshot with the new one.
                Screenshot ss = m_clientScreenshots[username];
                lock (ss.screenshot_lock)
                {
                    ss.screenshot = screenshot;
                }
            }
            else
            {
                Screenshot ss = new Screenshot();
                ss.screenshot = screenshot;
                m_clientScreenshots[username] = ss;
            }

            return true;
        }

        public byte[] getScreenshot(ServerClient client)
        {
            return getScreenshot(client.username);
        }

        public byte[] getScreenshot(String clientName)
        {
            if (clientName == null || m_clientScreenshots.ContainsKey(clientName))
                return null;

            Screenshot ss = m_clientScreenshots[clientName];
            byte[] toReturn = null;
            lock (ss.screenshot_lock)
            {
                // Copy it in to a new array so that we can return this
                // just in case it changes before we transmit.
                toReturn = new byte[ss.screenshot.Length];
                ss.screenshot.CopyTo(toReturn, 0);
            }

            return toReturn;
        }
    }
}
