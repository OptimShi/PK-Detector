using System;
using System.Collections.Generic;
using WindowsTimer = System.Windows.Forms.Timer;
using System.Xml;
using Microsoft.Win32;
using System.IO;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;


/*
 * Original example created by Mag-nus. 8/19/2011, VVS added by Virindi-Inquisitor.
 * 
 * No license applied, feel free to use as you wish. H4CK TH3 PL4N3T? TR45H1NG 0UR R1GHT5? Y0U D3C1D3!
 * 
 * Notice how I use try/catch on every function that is called or raised by decal (by base events or user initiated events like buttons, etc...).
 * This is very important. Don't crash out your users!
 * 
 * In 2.9.6.4+ Host and Core both have Actions objects in them. They are essentially the same thing.
 * You sould use Host.Actions though so that your code compiles against 2.9.6.0 (even though I reference 2.9.6.5 in this project)
 * 
 * If you add this plugin to decal and then also create another plugin off of this sample, you will need to change the guid in
 * Properties/AssemblyInfo.cs to have both plugins in decal at the same time.
 * 
 * If you have issues compiling, remove the Decal.Adapater and VirindiViewService references and add the ones you have locally.
 * Decal.Adapter should be in C:\Games\Decal 3.0\
 * VirindiViewService should be in C:\Games\VirindiPlugins\VirindiViewService\
*/

namespace PK_Detector
{
    //Attaches events from core
	[WireUpBaseEvents]

    //View (UI) handling
    [MVView("PK_Detector.mainView.xml")]
    [MVWireUpControlEvents]

	// FriendlyName is the name that will show up in the plugins list of the decal agent (the one in windows, not in-game)
	// View is the path to the xml file that contains info on how to draw our in-game plugin. The xml contains the name and icon our plugin shows in-game.
	// The view here is SamplePlugin.mainView.xml because our projects default namespace is SamplePlugin, and the file name is mainView.xml.
	// The other key here is that mainView.xml must be included as an embeded resource. If its not, your plugin will not show up in-game.
    [FriendlyName("PK Detector Example")]
	public class PluginCore : PluginBase
	{
		/// <summary>
		/// This is called when the plugin is started up. This happens only once.
		/// </summary>
        /// 
        bool loginComplete = false;

        protected override void Startup()
		{
			try
			{
				// This initializes our static Globals class with references to the key objects your plugin will use, Host and Core.
				// The OOP way would be to pass Host and Core to your objects, but this is easier.
                Globals.Init("PK_Detector", Host, Core);

                CoreManager.Current.CharacterFilter.LoginComplete += new EventHandler(CharacterFilter_LoginComplete);
                CoreManager.Current.CharacterFilter.Logoff += new EventHandler<LogoffEventArgs>(CharacterFilter_Logoff);

                //Initialize the view.
                MVWireupHelper.WireupStart(this, Host);

            }
			catch (Exception ex) { Util.LogError(ex); }
		}

        /// <summary>
		/// This is called when the plugin is shut down. This happens only once.
		/// </summary>
		protected override void Shutdown()
		{
			try
			{
                //Destroy the view.
                MVWireupHelper.WireupEnd(this);

			}
			catch (Exception ex) { Util.LogError(ex); }
		}

		[BaseEvent("LoginComplete", "CharacterFilter")]
		private void CharacterFilter_LoginComplete(object sender, EventArgs e)
		{
            Util.WriteToChat("PK Detector - OnLoginCompelte");
            try
			{
                // Subscribe to this event to process all CreateObject messages. 
                CoreManager.Current.WorldFilter.CreateObject += new EventHandler<Decal.Adapter.Wrappers.CreateObjectEventArgs>(WorldFilter_CreateObject);
            }
            catch (Exception ex) { Util.LogError(ex); }
		}


 		[BaseEvent("Logoff", "CharacterFilter")]
		private void CharacterFilter_Logoff(object sender, Decal.Adapter.Wrappers.LogoffEventArgs e)
		{
			try
			{
                // Unsubscribe to events here, but know that this event is not gauranteed to happen. I've never seen it not fire though.
                // This is not the proper place to free up resources, but... its the easy way. It's not proper because of above statement.
                CoreManager.Current.WorldFilter.CreateObject -= new EventHandler<Decal.Adapter.Wrappers.CreateObjectEventArgs>(WorldFilter_CreateObject);
			}
			catch (Exception ex) { Util.LogError(ex); }
		}

        [BaseEvent("CreateObject", "WorldFilter")]
        void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e)
        {
            try
            {
                if((e.New.Behavior & 0x20) > 0)
                {
                    // Could possibly wire up the request ID appraisal info
                    Util.WriteToChat("I found a PK named " + e.New.Name);
                }
            }
            catch (Exception ex) { Util.LogError(ex); }
        }
    }
}
