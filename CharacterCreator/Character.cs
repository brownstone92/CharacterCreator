using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharacterCreator
{
    public class CharacterType
    {
        public int     id;
        public string  name, desc;

        public CharacterType()
        {
            id = -1;
            name = desc = "";
        }
    }

    public class Character
    {
        public int      id;
        public byte[]   sheet;

        public string   name, desc, player;

        public CharacterType    type;
        public RPG              rpg;
        public Campaign         campaign;
        public CommonInfo       commonInfo;

        public Character()
        {
            id      = -1;
            sheet   = new byte[] { };

            name    = "New Character";
            desc    = "Enter 'Backstory'";
            player  = "";

            type        = new CharacterType();
            rpg         = new RPG();
            campaign    = new Campaign();
            commonInfo  = new CommonInfo();
        }
    }

    public class CharRace
    {
        public int     id;
        public byte[]  pic;

        public string  name, desc;

        public CharRace()
        {
            id  = -1;
            pic = new byte[] { };

            name = "New Race";
            desc ="";
        }
    }

    public class CharClass
    {
        public int     id;
        public byte[]  pic;

        public string  name, desc;

        public CharClass()
        {
            id = -1;
            pic = new byte[] { };

            name = "New Class";
            desc = "";
        }
    }

    public class CharAlignment
    {
        public int     id;
        public string  name, desc;

        public CharAlignment()
        {
            id      = -1;
            name    = "New Alignment";
            desc    = "";
        }
    }

    public class CommonInfo
    {
        public int      id      = -1,
                        level   = 1;

        public string   charSex;

        public CharRace        charRace;
        public CharClass       charClass;
        public CharAlignment   charAlign;

        public CommonInfo()
        {
            id = -1;
            level = 1;

            charSex = "";

            charRace    = new CharRace();
            charClass   = new CharClass();
            charAlign   = new CharAlignment();
        }
    }

    public class StatSystem
    {
        public int     id;
        public string  name, desc, template;

        public StatSystem()
        {
            id = -1;
            name = "New Stat System";
            desc = template = "";
        }
    }

    public class RPG
    {
        public int     id, dieBase;
        public string  name, desc;
        public byte[]  charTemplate;

        public StatSystem statSys;

        public List<CharRace>      races;
        public List<CharClass>     classes;
        public List<CharAlignment> alignments;

        public RPG()
        {
            id      = -1;
            dieBase = 20;

            charTemplate    = new byte[] { };
            statSys         = new StatSystem();

            races       = new List<CharRace>();
            classes     = new List<CharClass>();
            alignments  = new List<CharAlignment>();
        }
    }

    public class Campaign
    {
        public int id;
        public RPG rpg;

        public string name, dm, desc, extraDesc, type;

        public Campaign()
        {
            id = -1;
            rpg = new RPG();

            name = "New Campaign";
            dm = "Unknown";
            desc = "A New Adventure";
            extraDesc = type = "";
        }
    }

    public class Stat
    {
        public string name;
        public int value, id, parent, mod;

        public List<Stat> subStats;

        public Stat()
        {
            name        = "";
            value = mod = 0;
            id          = 1;
            parent      = -1;
            subStats    = new List<Stat>();
        }
    }
}