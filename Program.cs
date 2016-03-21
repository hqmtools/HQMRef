using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyEditor;

namespace HQMRef
{
    class Program
    {
        const float BLUETEAM_BLUELINE_Z = 22.86f;
        const float REDTEAM_BLUELINE_Z = 38.1f;

        static void Main(string[] args)
        {
            MemoryEditor.Init();            

            float lastPuckZ = HQMVector.Centre.Z;
            HQMVector lastPuckDir = HQMVector.Zero;

            EOffsideState offsideState = EOffsideState.None;
            bool offsideCalled = false;
            
            while(true)
            {
                float curPuckZ = Puck.Position.Z;
                
                //detect red offside
                if(lastPuckZ >= BLUETEAM_BLUELINE_Z && curPuckZ < BLUETEAM_BLUELINE_Z)
                {                    
                    foreach(Player p in PlayerManager.Players)
                    {
                        if(p.Team == HQMTeam.Red && p.Position.Z < BLUETEAM_BLUELINE_Z)
                        {
                            GameInfo.SendChatMessage("OFFSIDE WARNING - RED!");
                            offsideState = EOffsideState.Red;
                        }
                    }
                }

                //detect blue offside
                if(lastPuckZ <= REDTEAM_BLUELINE_Z && curPuckZ > REDTEAM_BLUELINE_Z)
                {
                    foreach(Player p in PlayerManager.Players)
                    {                        
                        if(p.Team == HQMTeam.Blue && p.Position.Z > REDTEAM_BLUELINE_Z)
                        {
                            GameInfo.SendChatMessage("OFFSIDE WARNING - BLUE!");
                            offsideState = EOffsideState.Blue;
                        }
                    }
                }

                
                //if puck is in neutral zone
                if(curPuckZ <= REDTEAM_BLUELINE_Z && curPuckZ >= BLUETEAM_BLUELINE_Z)
                {
                    if(offsideState != EOffsideState.None)
                    {
                        offsideState = EOffsideState.None;
                        GameInfo.SendChatMessage("offside clear!");
                        offsideCalled = false;
                    }
                }
                
                //call red offside
                if(offsideState == EOffsideState.Red)
                {
                    //if red team clears the zone
                    if(!RedTeamInOZone())
                    {
                        offsideState = EOffsideState.None;
                        GameInfo.SendChatMessage("offside clear!");
                        offsideCalled = false;
                    }
                    else
                    {
                        if(TeamTouchedPuck() == HQMTeam.Red && !offsideCalled)
                        {
                            GameInfo.SendChatMessage("OFFSIDE RED!");
                            offsideCalled = true;
                        }
                    }
                }
                
                //call blue offside
                if(offsideState == EOffsideState.Blue)
                {
                    //if blue team clears the zone
                    if(!BlueTeamInOZone())
                    {
                        offsideState = EOffsideState.None;
                        GameInfo.SendChatMessage("offside clear!");
                        offsideCalled = false;
                    }
                    else
                    {
                        if (TeamTouchedPuck() == HQMTeam.Blue && !offsideCalled)
                        {
                            GameInfo.SendChatMessage("OFFSIDE BLUE!");
                            offsideCalled = true;
                        }
                    }
                }

                lastPuckZ = Puck.Position.Z;
                System.Threading.Thread.Sleep(50); //approximates ping
            }
        }

        enum EOffsideState
        {
            None,
            Red,
            Blue
        }

        static bool RedTeamInOZone()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team == HQMTeam.Red && p.Position.Z < BLUETEAM_BLUELINE_Z)
                    return true;
            }
            return false;
        }

        static bool BlueTeamInOZone()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team == HQMTeam.Blue && p.Position.Z > REDTEAM_BLUELINE_Z)
                    return true;
            }
            return false;
        }

        static HQMTeam TeamTouchedPuck()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if ((p.StickPosition - Puck.Position).Magnitude < 0.25f)
                {
                    return p.Team;
                }
            }
            return HQMTeam.NoTeam;
        }
    }
}
