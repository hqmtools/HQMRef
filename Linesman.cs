using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyEditor;

namespace HQMRef
{
    public class Linesman
    {
        const float BLUETEAM_BLUELINE_Z = 22.86f;
        const float REDTEAM_BLUELINE_Z = 38.1f;

        float m_LastPuckZ = Puck.Position.Z;
        HQMVector m_LastPuckDir = HQMVector.Zero;

        EOffsideState m_OffsideState = EOffsideState.None;
        bool m_OffsideCalled = false;

        public void CheckForOffside()
        {
            float curPuckZ = Puck.Position.Z;
            Player[] players = PlayerManager.Players;

            //detect red offside
            if (m_LastPuckZ >= BLUETEAM_BLUELINE_Z && curPuckZ < BLUETEAM_BLUELINE_Z)
            {
                foreach (Player p in players)
                {
                    if (p.Team == HQMTeam.Red && p.Position.Z < BLUETEAM_BLUELINE_Z)
                    {
                        GameInfo.SendChatMessage("OFFSIDE WARNING - RED!");
                        m_OffsideState = EOffsideState.Red;
                    }
                }
            }

            //detect blue offside
            if (m_LastPuckZ <= REDTEAM_BLUELINE_Z && curPuckZ > REDTEAM_BLUELINE_Z)
            {
                foreach (Player p in players)
                {
                    if (p.Team == HQMTeam.Blue && p.Position.Z > REDTEAM_BLUELINE_Z)
                    {
                        GameInfo.SendChatMessage("OFFSIDE WARNING - BLUE!");
                        m_OffsideState = EOffsideState.Blue;
                    }
                }
            }


            //if puck is in neutral zone
            if (curPuckZ <= REDTEAM_BLUELINE_Z && curPuckZ >= BLUETEAM_BLUELINE_Z)
            {
                if (m_OffsideState != EOffsideState.None)
                {
                    m_OffsideState = EOffsideState.None;
                    GameInfo.SendChatMessage("offside clear!");
                    m_OffsideCalled = false;
                }
            }

            //call red offside
            if (m_OffsideState == EOffsideState.Red)
            {
                //if red team clears the zone
                if (!RedTeamInOZone())
                {
                    m_OffsideState = EOffsideState.None;
                    GameInfo.SendChatMessage("offside clear!");
                    m_OffsideCalled = false;
                }
                else
                {
                    if (TeamTouchedPuck() == HQMTeam.Red && !m_OffsideCalled)
                    {
                        GameInfo.SendChatMessage("OFFSIDE RED!");
                        m_OffsideCalled = true;
                    }
                }
            }

            //call blue offside
            if (m_OffsideState == EOffsideState.Blue)
            {
                //if blue team clears the zone
                if (!BlueTeamInOZone())
                {
                    m_OffsideState = EOffsideState.None;
                    GameInfo.SendChatMessage("offside clear!");
                    m_OffsideCalled = false;
                }
                else
                {
                    if (TeamTouchedPuck() == HQMTeam.Blue && !m_OffsideCalled)
                    {
                        GameInfo.SendChatMessage("OFFSIDE BLUE!");
                        m_OffsideCalled = true;
                    }
                }
            }

            m_LastPuckZ = Puck.Position.Z;            
        }

        enum EOffsideState
        {
            None,
            Red,
            Blue
        }

        bool RedTeamInOZone()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team == HQMTeam.Red && p.Position.Z < BLUETEAM_BLUELINE_Z)
                    return true;
            }
            return false;
        }

        bool BlueTeamInOZone()
        {
            foreach (Player p in PlayerManager.Players)
            {
                if (p.Team == HQMTeam.Blue && p.Position.Z > REDTEAM_BLUELINE_Z)
                    return true;
            }
            return false;
        }

        HQMTeam TeamTouchedPuck()
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
