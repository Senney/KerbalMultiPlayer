using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public Screenshot(byte[] ss)
        {
            this.screenshot = ss;
            this.screenshot_lock = new object();
        }
    }

    class ScreenshotManager
    {
        public const String SCREENSHOT_DIR = "KMPScreenshots";
        private ScreenshotSettings m_settings;
        private Dictionary<String, Screenshot> m_clientScreenshots;

        public ScreenshotManager(ScreenshotSettings ss_settings)
        {
            // TODO: Complete member initialization
            this.m_settings = ss_settings;
            this.m_clientScreenshots = new Dictionary<String, Screenshot>();
        }

        public bool addClientWatch(ServerClient watcher, ServerClient client)
        {
            return true;
        }

        public bool setScreenshot(ServerClient client, byte[] screenshot)
        {
            if (screenshot == null)
                return false;
            if (screenshot.Length >= m_settings.maxNumBytes)
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
                Screenshot ss = new Screenshot(screenshot);
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
            if (clientName == null || !m_clientScreenshots.ContainsKey(clientName))
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

        public Boolean saveScreenshot(ServerClient client)
        {
            byte[] screenshotData = getScreenshot(client);
            if (screenshotData == null) return false;

            if (!Directory.Exists(SCREENSHOT_DIR))
            {
                //Create the screenshot directory
                try
                {
                    if (!Directory.CreateDirectory(SCREENSHOT_DIR).Exists)
                        return false;
                }
                catch (Exception)
                {
                    Log.Error("Unable to save screenshot for " + client.username + ".");
                    return false;
                }
            }

            //Build the filename
            StringBuilder sb = new StringBuilder();
            sb.Append(SCREENSHOT_DIR);
            sb.Append('/');
            sb.Append(KMPCommon.filteredFileName(client.username));
            sb.Append(' ');
            sb.Append(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            sb.Append(".png");

            //Write the screenshot to file
            String filename = sb.ToString();
            if (!File.Exists(filename))
            {
                try
                {
                    //Read description length
                    int description_length = KMPCommon.intFromBytes(screenshotData, 0);

                    //Trim the description bytes from the image
                    byte[] trimmed_bytes = new byte[screenshotData.Length - 4 - description_length];
                    Array.Copy(screenshotData, 4 + description_length, trimmed_bytes, 0, trimmed_bytes.Length);

                    File.WriteAllBytes(filename, trimmed_bytes);
                }
                catch (Exception)
                {
                }
            }

            return true;
        }
    }
}
