using System.Collections.Generic;
using Character.Core.Character;
using Character.Core.Character.Look;

namespace Character.Core.Net
{
    public class Account
    {
        public int AccId;
        public short Female;
        public bool Admin;
        public string Name;
        public bool Muted;
        public bool Pin;
        public short Pic;
    }

    public class World
    {
        public string Name;
        public string Message;
        public List<int> ChLoads = new List<int>();
        public short ChannelCount;
        public short Flag;
        public short WId;
    }

    public class StatsEntry
    {
        public string Name;
        public bool Female;
        public List<long> PetIds = new List<long>();
        public Dictionary<MapleStat.Id, short> Stats = new Dictionary<MapleStat.Id, short>();
        public long Exp;
        public int MapId;
        public short Portal;
        public Dictionary<int, short> Rank = new Dictionary<int, short>();
        public Dictionary<int, short> JobRank = new Dictionary<int, short>();
    }


    public class LookEntry
    {
        public bool Female { get; set; }
        public short Skin { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }
        public Dictionary<short, int> Equips { get; set; } = new Dictionary<short, int>();

        public Dictionary<short, int> MaskedEquips { get; set; } = new Dictionary<short, int>();

        public List<int> PetIds { get; } = new List<int>();
    }

    public class CharEntry
    {
        public StatsEntry Stats { get; } = new StatsEntry();
        public LookEntry Look { get; } = new LookEntry();

        public int Id { get; } = 0;
    }
}