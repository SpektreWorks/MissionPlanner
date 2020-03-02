using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class GuidedTracking: UserControl
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CheckBox chk_followcamera;
        private Timer timer1;
        private System.ComponentModel.IContainer components;
        private UdpClient _udpclient;

        public GuidedTracking()
        {
            InitializeComponent();

            try
            {
                log.Info("About to listern on udp 15000");
                // listern on port 15000
                _udpclient = new UdpClient(15000);

                // setup async receive
                _udpclient.BeginReceive(ProcessUDPPacket, _udpclient);

                log.Info("About to start timer");
                timer1.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.Enabled = false;
            }
        }

        private void ProcessUDPPacket(IAsyncResult ar)
        {
            log.Info("got a udp packet");

            var client = ((UdpClient)ar.AsyncState);

            if (client == null || client.Client == null)
                return;

            IPEndPoint e = null;
            // get the data
            byte[] receiveBytes = client.EndReceive(ar, ref e);

            // setup for next packet
            _udpclient.BeginReceive(ProcessUDPPacket, this);

            if (chk_followcamera.Checked)
            {
                Regex swRegex = new Regex(@"(\$SW,([\-0-9\.]+),([\-0-9\.]+),)([0-9a-fA-F]{2})");
         
                string receiveString = Encoding.ASCII.GetString(receiveBytes);

                log.Info("got " + receiveString);

                if (swRegex.IsMatch(receiveString))
                {
                    var match = swRegex.Match(receiveString);

                    var checksum = match.Groups[1].Value.Sum(a => (byte) a) & 0xff;

                    if (checksum.ToString("X2") == match.Groups[4].Value)
                    {
                        PointLatLngAlt gotolocation = new PointLatLngAlt(double.Parse(match.Groups[2].Value),
                            double.Parse(match.Groups[3].Value),
                            Math.Round(MainV2.comPort.MAV.cs.alt / CurrentState.multiplieralt, 0), "Goto");

                        Locationwp gotohere = new Locationwp();

                        gotohere.id = (ushort) MAVLink.MAV_CMD.WAYPOINT;
                        gotohere.alt = (float) (gotolocation.Alt);
                        gotohere.lat = (gotolocation.Lat);
                        gotohere.lng = (gotolocation.Lng);

                        if (MainV2.comPort.BaseStream.IsOpen && MainV2.comPort.giveComport == false)
                        {
                            try
                            {
                                MainV2.comPort.setGuidedModeWP(gotohere, false);
                            }
                            catch
                            {
                            }

                        }
                    }
                    else
                    {
                        log.Error("Bad GuidedTracking checksum " + receiveString + " vs calced " + checksum.ToString("X2"));
                    }
                }
                else
                {
                    log.Error("Bad GuidedTracking data " + receiveString);
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chk_followcamera = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // chk_followcamera
            // 
            this.chk_followcamera.AutoSize = true;
            this.chk_followcamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chk_followcamera.Location = new System.Drawing.Point(0, 0);
            this.chk_followcamera.Name = "chk_followcamera";
            this.chk_followcamera.Size = new System.Drawing.Size(150, 150);
            this.chk_followcamera.TabIndex = 0;
            this.chk_followcamera.Text = "Follow Camera";
            this.chk_followcamera.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // GuidedTracking
            // 
            this.Controls.Add(this.chk_followcamera);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "GuidedTracking";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Enabled = MainV2.comPort.MAV.cs.mode.ToLower() == "guided" ? true : false;
        }
    }
}
