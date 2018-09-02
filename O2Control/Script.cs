#if DEBUG
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

public sealed class Program : MyGridProgram
{
#endif
    //=======================================================================
    //////////////////////////BEGIN//////////////////////////////////////////
    //=======================================================================
    public Program()
    {
        // The constructor, called only once every session and
        // always before any other method is called. Use it to
        // initialize your script.

        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        //This makes the program automatically run every 10 ticks.
        //working out to about 6 times per second. 

    }

    public class Sector
    {
        public List<IMyDoor> doors;
        public List<IMyLightingBlock> lights;
        public List<IMyAirVent> airvent;
        public List<IMyTextPanel> textPanels;
        public int count;

    }

    public void SetColorLight (List<IMyLightingBlock>myLightingBlocks, string color)
    {
        Color colorCode = new Color();
        switch (color)
        {
            case "red":
                colorCode.R = 255;
                colorCode.G = 0;
                colorCode.B = 0;
                break;
            case "normal":
                colorCode.R = 255;
                colorCode.G = 255;
                colorCode.B = 255;
                break;
            default:
                colorCode.R = 255;
                colorCode.G = 255;
                colorCode.B = 255;
                break;
        }

        for (int i = 0; i < myLightingBlocks.Count; i++)
        {
            myLightingBlocks[i].Color = colorCode;
        }

    }

    //=======================================================================
    //Escrita em LCD
    //=======================================================================
    public void WriteLcd (List<IMyTextPanel>textPanels, string system, string text)
    {
        string[] dataSplit;
        for (int i = 0; i < textPanels.Count; i++)
        {
            dataSplit = textPanels[i].CustomData.Split(';');
            if ((dataSplit.Length >= 1) && (dataSplit[1] == system))
            {
                textPanels[i].WritePublicText(text);
            }
        }
    }

    public Sector GetSector(string sectorCode)
    {
        string[] dataSplit;
        Sector sector = new Sector();

        var tmpDoors = new List<IMyDoor>();
        GridTerminalSystem.GetBlocksOfType<IMyDoor>(tmpDoors);
        var tmpDoors2 = new List<IMyDoor>();
        GridTerminalSystem.GetBlocksOfType<IMyDoor>(tmpDoors2);
        for (int i = 0; i < tmpDoors.Count; i++)
        {
            if ((tmpDoors[i].CustomData != "") && (tmpDoors[i].CustomData.Contains(";")))
            {
                dataSplit = tmpDoors[i].CustomData.Split(';');
                if ((dataSplit.Length > 0) && (dataSplit[0] != sectorCode))
                    tmpDoors2.RemoveAt(tmpDoors2.FindIndex(x => x.EntityId == tmpDoors[i].EntityId));
            }
            else
                tmpDoors2.RemoveAt(tmpDoors2.FindIndex(x => x.EntityId == tmpDoors[i].EntityId));
        }
        sector.doors = tmpDoors2;

        var tmpLights = new List<IMyLightingBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(tmpLights);
        var tmpLight2 = new List<IMyLightingBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(tmpLight2);
        for (int i = 0; i < tmpLights.Count; i++)
        {
            if ((tmpLights[i].CustomData != "") && (tmpLights[i].CustomData.Contains(";")))
            {
                dataSplit = tmpLights[i].CustomData.Split(';');
                if ((dataSplit.Length > 0) && (dataSplit[0] != sectorCode))
                    tmpLight2.RemoveAt(tmpLight2.FindIndex(x => x.EntityId == tmpLights[i].EntityId));
            }
            else
                tmpLight2.RemoveAt(tmpLight2.FindIndex(x => x.EntityId == tmpLights[i].EntityId));
        }
        sector.lights = tmpLight2;

        var tmpAirVents = new List<IMyAirVent>();
        GridTerminalSystem.GetBlocksOfType<IMyAirVent>(tmpAirVents);
        var tmpAirVents2 = new List<IMyAirVent>();
        GridTerminalSystem.GetBlocksOfType<IMyAirVent>(tmpAirVents2);
        for (int i = 0; i < tmpAirVents.Count; i++)
        {
            if ((tmpAirVents[i].CustomData != "") && (tmpAirVents[i].CustomData.Contains(";")))
            {
                dataSplit = tmpAirVents[i].CustomData.Split(';');
                if ((dataSplit.Length > 0) && (dataSplit[0] != sectorCode))
                    tmpAirVents2.RemoveAt(tmpAirVents2.FindIndex(x => x.EntityId == tmpAirVents[i].EntityId));
            }
            else
                tmpAirVents2.RemoveAt(tmpAirVents2.FindIndex(x => x.EntityId == tmpAirVents[i].EntityId));
        }
        sector.airvent = tmpAirVents2;

        var tmpTextPanels = new List<IMyTextPanel>();
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(tmpTextPanels);
        var tmpTextPanels2 = new List<IMyTextPanel>();
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(tmpTextPanels2);
        for (int i = 0; i < tmpTextPanels.Count; i++)
        {
            if ((tmpTextPanels[i].CustomData != "") && (tmpTextPanels[i].CustomData.Contains(";")))
            {
                dataSplit = tmpTextPanels[i].CustomData.Split(';');
                if ((dataSplit.Length > 0) && (dataSplit[0] != sectorCode))
                    tmpTextPanels2.RemoveAt(tmpTextPanels2.FindIndex(x => x.EntityId == tmpTextPanels[i].EntityId));
            }
            else
                tmpTextPanels2.RemoveAt(tmpTextPanels2.FindIndex(x => x.EntityId == tmpTextPanels[i].EntityId));
        }
        sector.textPanels = tmpTextPanels2;

        return sector;
    }

    public void PressurizedEntryControl(string sectorCode)
    {
        double pressure = 0;
        string status = "";
        bool depressurize = false;
        //Depressurizing
        //Pressurized
        //Depressurized
        string[] dataSplit;

        Sector sector = new Sector();
        sector = GetSector(sectorCode);

        string lcdText = "\nSALA\nDE\nDESPRESSURIZACAO\n\n";

        for (int i = 0; i < sector.airvent.Count; i++)
        {
            dataSplit = sector.airvent[i].DetailedInfo.Split('\n');
            dataSplit[2] = dataSplit[2].Replace("Room pressure:", "");
            lcdText += ("Status: " + dataSplit[2]);
            depressurize = sector.airvent[i].Depressurize;
            if (dataSplit[2] == " Not pressurized")
                pressure += 0;
            else
            {
                dataSplit[2] = dataSplit[2].Replace("%", "");
                pressure += Convert.ToDouble(dataSplit[2]);
            }
            status = sector.airvent[i].Status.ToString();
        }

        if (depressurize == false)
        {
            for (int i = 0; i < sector.doors.Count; i++)
            {
                dataSplit = sector.doors[i].CustomData.Split(';');
                if (dataSplit[1] == "pec" & dataSplit[2] == "external")
                {
                    sector.doors[i].ApplyAction("Open_Off");
                }
            }

        }

        if (pressure == 0)
        {
            for (int i = 0; i < sector.doors.Count; i++)
            {
                dataSplit = sector.doors[i].CustomData.Split(';');
                if (dataSplit[1] == "pec" & dataSplit[2] == "external")
                {
                    sector.doors[i].Enabled = true;
                }
                else if (dataSplit[1] == "pec" & dataSplit[2] == "internal")
                {
                    sector.doors[i].Enabled = false;
                }
            }
            SetColorLight(sector.lights, "red");
        }
        else if ((status == "Pressurized") && (pressure > 95))
        {
            for (int i = 0; i < sector.doors.Count; i++)
            {
                dataSplit = sector.doors[i].CustomData.Split(';');
                if (dataSplit[1] == "pec" & dataSplit[2] == "external")
                    sector.doors[i].Enabled = false;
                else if (dataSplit[1] == "pec" & dataSplit[2] == "internal")
                    sector.doors[i].Enabled = true;
            }
            SetColorLight(sector.lights, "normal");
        }
        else if ((status == "Depressurizing") || (status == "Depressurized"))
        {
            for (int i = 0; i < sector.doors.Count; i++)
            {
                dataSplit = sector.doors[i].CustomData.Split(';');
                if (dataSplit[1] == "pec" & dataSplit[2] == "internal")
                {
                    sector.doors[i].Enabled = true;
                    sector.doors[i].ApplyAction("Open_Off");
                }
            }
            SetColorLight(sector.lights, "red");
        }
        WriteLcd(sector.textPanels, "pec", lcdText);
    }

    public void Main()
    {
        
        //=======================================================================
        //Variaveis globais
        //=======================================================================
        string lcdText = "";

        //=======================================================================
        //TESTES
        //=======================================================================
        PressurizedEntryControl("[sector:3]");


        //=======================================================================
        //GERENCIAMENTO DOS TANQUES DE O2
        //=======================================================================
        var oxygenTanks = new List<IMyGasTank>();
        GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxygenTanks);

        double filledLevel;
        double filledTotal = 0;

        lcdText = "----NIVEIS DOS TANQUES O2:----\n\n";
        for (var i = 0; i < oxygenTanks.Count; i++)
        {
            filledLevel = oxygenTanks[i].FilledRatio * 100;
            filledTotal += filledLevel;
            lcdText += oxygenTanks[i].CustomName + ": " + filledLevel.ToString("N2") + "%\n";
        }
        filledTotal = filledTotal / oxygenTanks.Count; 
        lcdText += "\nNIVEL TOTAL: " + filledTotal.ToString("N2") + "%\n\n";


        //=======================================================================
        //GERENCIAMENTO DOS GERADORES DE 02
        //=======================================================================
        var oxygenGenerators = new List<IMyGasGenerator>();
        GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(oxygenGenerators);

        for (var i = 0; i < oxygenGenerators.Count; i++)
        {
            if (filledTotal > 90)
            {
                oxygenGenerators[i].Enabled = false;
            }
            else if (filledTotal < 60)
            {
                oxygenGenerators[i].Enabled = true;
            }
        }

        //=======================================================================
        //LCD
        //=======================================================================
        var screen = GridTerminalSystem.GetBlockWithName("O2Control[LCD_BASE]") as IMyTextPanel;
        screen.WritePublicText(lcdText);
    }

    public void Save()
    {
        // Called when the program needs to save its state. Use
        // this method to save your state to the Storage field
        // or some other means.

        // This method is optional and can be removed if not
        // needed.
    }

    //=======================================================================
    //////////////////////////END////////////////////////////////////////////
    //=======================================================================
    #if DEBUG
    }
#endif